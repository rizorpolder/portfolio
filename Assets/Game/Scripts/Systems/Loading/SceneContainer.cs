using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.Loading.Scene;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;

namespace Game.Scripts.Systems.Loading
{
	public class SceneContainer
	{
		private readonly Dictionary<string, SceneInstanceLogic> _scenes = new();

		private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _activeSceneSources =
			new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

		private DiContainer _container;

		[Inject]
		public SceneContainer(DiContainer container)
		{
			_container = container;
		}

		public SceneInstanceLogic AddScene(AsyncOperationHandle<SceneInstance> sceneSource)
		{
			var sceneInstance = sceneSource.Result;
			var logic = CreateLogic(sceneInstance);
			_scenes.Add(sceneInstance.Scene.name, logic);
			_activeSceneSources.Add(sceneInstance.Scene.name, sceneSource);
			return logic;
		}

		public SceneInstanceLogic RemoveScene(string sceneName)
		{
			if (!HasScene(sceneName))
				return null;

			var logic = _scenes[sceneName];
			_scenes.Remove(sceneName);
			RemoveSource(sceneName);

			return logic;
		}

		private void RemoveSource(string sceneName)
		{
			var source = _activeSceneSources[sceneName];
			Addressables.Release(source);
			_activeSceneSources.Remove(sceneName);
			UnityEngine.Resources.UnloadUnusedAssets();
		}

		public void RemoveAll()
		{
			_scenes.Clear();
			var sources = _activeSceneSources.Values.ToList();
			foreach (var source in sources)
				Addressables.Release(source);

			_activeSceneSources.Clear();
			UnityEngine.Resources.UnloadUnusedAssets();
		}

		private SceneInstanceLogic CreateLogic(SceneInstance instance)
		{
			var sceneName = instance.Scene.name;

			SceneInstanceLogic result = sceneName switch
			{
				SceneNames.CreateCharacter => new InitializeSceneLogic(instance),
				SceneNames.Office => new OfficeSceneLogic(instance),
				SceneNames.TestingRoom => new TestingRoomSceneLogic(instance),
				SceneNames.MentorsRoom => new MentorRoomSceneLogic(instance),
				SceneNames.MeetingRoom => new MeetingRoomSceneLogic(instance),
				SceneNames.Final => new FinalSceneLogic(instance),
				_ => new SceneInstanceLogic(instance)
			};

			_container.Inject(result);
			return result;
		}

		public bool HasScene(string sceneName)
		{
			return _scenes.ContainsKey(sceneName);
		}
	}
}