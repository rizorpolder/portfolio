using System;
using Game.Scripts.UI.Common;
using UnityEngine;

public class HUDChildPanel : BasePanel
{
	[SerializeField] private BasePanel _parent;
	[SerializeField] private bool _alwaysHidden;
	private Action _callback;

	public bool IsAlwaysHidden => _alwaysHidden;
	
	protected override void OnAwakeAction()
	{
		base.OnAwakeAction();
		_status = ElementStatus.Hidden;
	}

	public override void Hide(Action callback = null)
	{
		if (!_parent.IsShown() || _alwaysHidden)
			base.Hide(callback);

		if (!IsActiveOnScene())
		{
			_callback = callback;
			base.Hide(ActionAfterHideInBlockedScene);
		}
	}

	private void ActionAfterHideInBlockedScene()
	{
		_callback?.Invoke();
		_callback = null;
	}

	public bool IsActiveOnScene()
	{
		// if (_tab == null)
		//     return true;
		//
		// return _tab.IsActiveOnScene();

		return true;
	}
}