using Cysharp.Threading.Tasks;

namespace Game.Scripts.Initializers.Interfaces
{
	public interface IDataLoader
	{
		UniTask LoadAsync();
	}
}