using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Systems.Network
{
	[CreateAssetMenu(menuName = "Project/Network/NetworkConfig", fileName = "NetworkConfig")]
	public class NetworkConfig : ScriptableObject
	{
		[SerializeField] private List<NetworkPath> networks;

		[SerializeField] private NetworkServer _currentServer;

		[SerializeField] private bool _syncWithServer = true;

		public NetworkServer PriorityServer => _currentServer;

		public bool SyncWithServer => _syncWithServer;

		public NetworkPath GetNetworkPath()
		{
			return networks.First(x => x.server == _currentServer);
		}

		public string GetRestApiUri()
		{
			var path = GetNetworkPath();
			return $"{path.protocol.ToUriString()}{path.uri}";
		}
	}
}