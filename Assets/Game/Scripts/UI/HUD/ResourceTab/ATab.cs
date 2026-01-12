using System;
using Game.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.ResourceTab
{
	public abstract class ATab: AbstractUIElement
    {
        [SerializeField] protected Transform root;
        [SerializeField] private bool alwaysHidden;
        [SerializeField] private ResourceTabContext context = ResourceTabContext.All;
        [SerializeField] private Animator _animator;
        
        [Inject] protected GlobalUI _globalUI;
        
        protected bool _isActiveOnScene = true;
        private bool IsShown => root.gameObject.activeSelf;

        private LayoutElement _layoutElement;
        
        public abstract void Refresh();
        

        private void OnValidate()
        {
            if (!_animator)
                _animator = GetComponent<Animator>();
            
            if(!_animator)
                Debug.LogWarning($"{name} has no animator!", gameObject);
        }

        protected override void OnAwakeAction()
        {
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }
        
        public override void Show(Action callback = null)
        {
            if (IsShown)
                return;

            if (_layoutElement)
            {
                Destroy(_layoutElement);
            }

            root.gameObject.SetActive(true);
           
            _globalUI.HUD.PlayerPanel.RebuildTopPanelLayout();
        }

        protected override void OnEnableAction()
        {
            base.OnEnableAction();
            
            if(_animator)
                _animator.Play(Animator.StringToHash("Show"));
        }

        private void OnSceneLoaded(Scene prevScene, Scene activeScene)
        {
            ChangeTabContext(activeScene.name);
        }

        protected void ChangeTabContext(string activeSceneName)
        {
            // if (SceneNames.IsMetaScene(activeSceneName))
            // {
            //     _isActiveOnScene = context.HasFlag(ResourceTabContext.Meta);
            // }
            // else if (SceneNames.IsCoreScene(activeSceneName))
            // {
            //     _isActiveOnScene = context.HasFlag(ResourceTabContext.Core);
            // }

            if (!_isActiveOnScene)
            {
                Hide();
                _globalUI.HUD.PlayerPanel.RebuildTopPanelLayout();
            }
            else if(!HasBlockedCondition())
                Show();
        }

        protected virtual bool HasBlockedCondition()
        {
            return alwaysHidden;
        }

        public bool IsActiveOnScene()
        {
            return _isActiveOnScene;
        }
        
        public override void Hide(Action callback = null)
        {
            if (!IsShown)
                return;

            if (_layoutElement == null)
            {
                _layoutElement = gameObject.AddComponent<LayoutElement>();
                _layoutElement.ignoreLayout = true;
            }

            root.gameObject.SetActive(false);
        }
        
        protected override void OnDestroyAction()
        {
            SceneManager.activeSceneChanged -= OnSceneLoaded;
        }
        
    }
}