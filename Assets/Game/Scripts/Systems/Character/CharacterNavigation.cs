using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Extensions;
using Game.Scripts.Helpers;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts.Systems.Character
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class CharacterNavigation : MonoBehaviour 
	{
		public Action<bool> OnNextPointFinished;

		[SerializeField] private NavMeshAgent _agent;
		[SerializeField] private NavMeshObstacle _obstacle;
		[SerializeField] private Transform _rotationTransfrom;

		public float Height => _agent.height;
		public float Radius => _agent.radius;

		private string _targetAnimationName;

		// для корректного поворота персонажа в изометрии необходимо знать размер ячейки
		private const float HalfCellWith = 1f;
		private const float HalfCellHeight = 0.5f;

		private CharacterAnimator _characterAnimator;

		private MoveCompleteStatus _moveCompleteStatus = MoveCompleteStatus.Success;
		private float _defaultSpeed = 1f;

		private IEnumerator _moveCoroutine = null;
		//private IEnumerator _rotateCoroutine = null; OnlyFor3D

		private float _targetAnimationDistance = -1f;

		private float _targetRotation;
		private Vector3 _currentRotation;
		private bool _isMovement = false;
		private float _defaultAcceleration;
		private bool _isStopped = false;
		private bool _pathBreaked = false;
		private bool _resetStoppingDistance = true;
		private float _stoppingDistance;

		[Inject] CoroutineHelper _coroutineHelper;

		private void Start()
		{
			_agent.updateRotation = false;
			_agent.updateUpAxis = false;
		}

		public void MoveTo(Queue<Vector3> targets, bool instantly, Action<MoveCompleteStatus> action = null)
		{
			_moveCompleteStatus = MoveCompleteStatus.Success;

			if (_moveCoroutine != null)
			{
				_coroutineHelper.StopCoroutine(_moveCoroutine);
				_moveCoroutine = null;
			}

			_defaultAcceleration = _agent.acceleration;

			//_characterAnimator.ToIdle();

			if (instantly)
			{
				WarpCharacter(targets, () => action?.Invoke(MoveCompleteStatus.Success));
				return;
			}

			if (_resetStoppingDistance)
			{
				_agent.stoppingDistance = _stoppingDistance;
			}

			_resetStoppingDistance = true;

			_moveCoroutine = moveCoroutine(targets, action);
			_coroutineHelper.StartCoroutine(_moveCoroutine);
		}

		IEnumerator moveCoroutine(Queue<Vector3> targets, Action<MoveCompleteStatus> action = null)
		{
			_isStopped = false;

			yield return CorrectSwitchToAgent();

			yield return null;

			while (targets.Count > 0)
			{
				yield return moveToPoint(targets.Dequeue(), OnMoveToPointFinished);
				if (_isStopped) yield break;
			}


			PathEnd();
			transform.position = _agent.nextPosition;
			action?.Invoke(_moveCompleteStatus);
			yield return null;
		}

		private void OnMoveToPointFinished(bool pathBreak)
		{
			if (pathBreak)
			{
				_moveCompleteStatus = MoveCompleteStatus.Break;
			}

			OnNextPointFinished?.Invoke(pathBreak);
		}

		IEnumerator moveToPoint(Vector3 targetPosition, Action<bool> OnComplete = null)
		{
			_pathBreaked = false;

			while (!_agent.isOnNavMesh)
				yield return null;

			yield return RecalculateDestination(targetPosition);

			// бывают погрешности при расчётах пути, когда не нужно идти и мы уже стоим на точке
			var distance = Vector3.Distance(transform.position, targetPosition);
			var maxRadiusWithDegree = _agent.stoppingDistance + _agent.stoppingDistance / 4;
			if (distance > maxRadiusWithDegree)
				yield return moveAlgorithm(targetPosition);
			OnComplete?.Invoke(_pathBreaked);
		}

		IEnumerator moveAlgorithm(Vector3 targetPosition)
		{
			bool firstUpdate = false;
			bool pathNotFinished = true;
			bool pathBreaked = false;
			_isMovement = false;
			_targetRotation = _currentRotation.y;
			int stayOnOnePointCounter = 0;
			var avoidancePreset = _agent.obstacleAvoidanceType;

			do
			{
				if (firstUpdate)
					UpdateRotationInMove();

				// если по какой-то причине (баг юнити, агент оказался внутри obstacle) не может двигаться
				stayOnOnePointCounter = transform.position.Equals(_agent.nextPosition) ? stayOnOnePointCounter + 1 : 0;
				if (stayOnOnePointCounter > 20)
					_agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

				transform.position = _agent.nextPosition;

				_characterAnimator.SetVelocity(_agent.velocity.magnitude);
				CheckAnimation();

				yield return null;
				firstUpdate = true;

				if (_agent.isOnNavMesh)
				{
					// GetPathRemainingDistance необходим потому что remainingDistance == infinity
					// во всех случаях, кроме последнего corners
					pathNotFinished = _agent.pathPending ||
					                  (_agent.GetPathRemainingDistance() > _agent.stoppingDistance);
					pathBreaked = _agent.pathStatus != NavMeshPathStatus.PathComplete &&
					              pathNotFinished; // не дошли и путь разорвался

					// прерываем движение если почти пришли и сильно замедлились
					if (_agent.velocity.magnitude > 0.005f)
						_isMovement = true;
					else if (_isMovement && !pathNotFinished)
					{
						_agent.SetDestination(transform.position);
						break;
					}
				}

				bool needRecalculate = false;
				while (!_agent.isOnNavMesh)
				{
					needRecalculate = true;
					yield return null;
				}

				if (needRecalculate)
					yield return RecalculateDestination(targetPosition);
			} while (pathNotFinished && !pathBreaked);

			_agent.obstacleAvoidanceType = avoidancePreset;
			_pathBreaked = pathNotFinished && pathBreaked;
		}

		private void CheckAnimation()
		{
			if (_targetAnimationDistance > 0 && _targetAnimationDistance > _agent.GetPathRemainingDistance())
			{
				_targetAnimationDistance = -1;
				_characterAnimator.SetTrigger(_targetAnimationName);
			}
		}

		IEnumerator RecalculateDestination(Vector3 targetPosition)
		{
			_agent.ResetPath();
			_agent.nextPosition = transform.position;
			_agent.SetDestination(targetPosition);

			// нужен для расчёта пути
			yield return null;
		}

		private void PathEnd()
		{
			_targetAnimationDistance = -1f;
			_characterAnimator.SetVelocity(0f);
			_agent.acceleration = _defaultAcceleration;
			_moveCoroutine = null;

			// включаем obstacle, чтобы другие персонажи видели нас как препятствие и перерасчитывали свои пути
			SwitchToObstacle();
		}

		private void WarpCharacter(Queue<Vector3> targets, Action action = null)
		{
			_characterAnimator.SetVelocity(0f);

			if (targets.Count > 0)
			{
				SwitchToAgent();
				_agent.Warp(targets.Peek());
				SwitchToObstacle();
			}

			action?.Invoke();
		}

		public void AddAnimator(CharacterAnimator animator)
		{
			_characterAnimator = animator;
		}

		public void Init()
		{
			_defaultSpeed = _agent.speed;
		}

		private void UpdateRotationInMove()
		{
			this._characterAnimator.SetDirection(_agent.velocity);
			return;
			//TODO Only for 3D models
			#pragma warning disable 0162
			_targetRotation = GetAngleInDirection(_agent.velocity);
			_currentRotation.y = GetMinRotationToTargetRotation(_targetRotation);

			var originalRotation = Mathf.Lerp(_currentRotation.y,
				_targetRotation,
				Time.deltaTime * _agent.angularSpeed);

			SetRotation(originalRotation);
			#pragma warning restore
		}

		#region rotate

		private void SetRotation(float rotation)
		{
			_currentRotation.y = rotation;
			_rotationTransfrom.transform.localEulerAngles = _currentRotation;
		}

		public float GetAngleInDirection(Vector3 rotationDirection)
		{
			// расчитываем угол в нарравлении относительно вписанного в ячейку элипса
			return rotationDirection.normalized.FindEllipseDegree(HalfCellHeight, HalfCellWith);
		}

		/// <summary>
		/// Высчитываем из какого угла поворота лучше будет делать поворот в целевой угол
		/// Например: для поворота из 280 до 10 минимальным угол поворота будет, если поворачивать из -80 (идентичен 280)
		/// Устраняет неестественное прокручивание персонажа
		/// </summary>
		/// <param name="targetRotation"></param>
		/// <returns></returns>
		private float GetMinRotationToTargetRotation(float targetRotation)
		{
			if (_currentRotation.y - targetRotation > 180)
				return _currentRotation.y - 360;

			if (targetRotation - _currentRotation.y > 180)
				return _currentRotation.y + 360;

			return _currentRotation.y;
		}

		#endregion

		public void SwitchToAgent()
		{
			if (_obstacle.enabled)
				_obstacle.enabled = false;

			_agent.Warp(_agent.transform.position);
			_agent.enabled = true;
		}

		public void SwitchToObstacle()
		{
			if (_agent.enabled)
				_agent.enabled = false;

			_obstacle.enabled = true;
		}

		public IEnumerator CorrectSwitchToAgent()
		{
			if (_obstacle.enabled)
				_obstacle.enabled = false;
			yield return null;
			_agent.Warp(_agent.transform.position);
			_agent.enabled = true;
		}

		public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
		{
			if (!_agent.isOnNavMesh)
				return false;

			return _agent.CalculatePath(targetPosition, path);
		}

		public void Stop()
		{
			if (_moveCoroutine != null)
			{
				_isStopped = true;
				_coroutineHelper.StopCoroutine(_moveCoroutine);
				PathEnd();
			}

			_characterAnimator.ToIdle();
		}

		public void SetAgentStoppingDistance(float distance)
		{
			_agent.stoppingDistance = distance;
			_resetStoppingDistance = false;
		}

		private void OnDestroy()
		{
			if (_moveCoroutine != null)
			{
				_coroutineHelper.StopCoroutine(_moveCoroutine);
				_moveCoroutine = null;
			}
		}
	}
}