using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI.WindowsSystem
{
	[CreateAssetMenu(menuName = "Project/WindowsConfig", fileName = "WindowsConfig")]
	public class WindowsConfig : ScriptableObject
	{
		[SerializeField] private List<WindowProperties> _windows;
		public List<WindowProperties> Windows => _windows;
	}
}