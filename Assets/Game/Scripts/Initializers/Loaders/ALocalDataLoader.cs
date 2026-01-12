using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.SaveSystem;
using Zenject;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public abstract class ALocalDataLoader<T> : IDataLoader
	{
		protected readonly IDataCache<T> _cache;

		[Inject] private ISaveSystemCommand _saveSystemCommand;

		public abstract T DefaultData { get; }
		
		protected abstract SaveDataType _loadDataType { get; }

		public ALocalDataLoader(IDataCache<T> cache)
		{
			_cache = cache;
		}

		public virtual async UniTask LoadAsync()
		{
			var data = await LoadLocalPlayerAsync();
			_cache.SetData(data);
		}

		private async UniTask<T> LoadLocalPlayerAsync()
		{
			T defaultData = DefaultData;
			var loadResult = await _saveSystemCommand.LoadData<T>(_loadDataType.ToString());
			if (loadResult.success)
				defaultData = loadResult.data;
			
			return defaultData;
		}
	}
}