using Game.Scripts.UI.Common;
using Zenject;

namespace Game.Scripts.UI.WindowsSystem
{
	public class BaseWindow : BasePanel
	{
		[Inject] private WindowsController _controller;
		
		public void Open()
		{
			_controller.Show(ID);
		}

		public virtual void Close()
		{
			_controller.Hide(ID);
			
		}

		//implementation of behaviour for back button event

		public virtual void OnBackButton()
		{
		}
	}
}