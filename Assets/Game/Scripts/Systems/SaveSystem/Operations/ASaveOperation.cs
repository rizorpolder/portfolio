using Cysharp.Threading.Tasks;

namespace Game.Scripts.Systems.SaveSystem.Operations
{
	public abstract class ASaveOperation
	{
		protected string _data;

		public string Data => _data;
		public string SaveKey => FileName;
		
		protected abstract string FileName { get; }

		protected IDataSerializer _serializer { get; private set; }
		protected IDataSaver _saver { get; private set; }

		public ASaveOperation(IDataSerializer serializer, IDataSaver saver)
		{
			_serializer = serializer;
			_saver = saver;
		}

		public virtual void PrepareData()
		{
			_data = GetData();
		}

		protected abstract string GetData();

		public virtual async UniTask SaveData()
		{
			await _saver.Save(FileName, _data);
		}
	}
}