using Cysharp.Threading.Tasks;

namespace Game.Scripts.Systems.Initialize
{
	public interface IDataLoader
	{
		UniTask LoadAsync();
	}
}