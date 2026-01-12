using Game.Scripts.Systems.Texts;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class TextsDataCache : IDataCache<TextsRepository>
	{
		private TextsRepository _textsRepository;
		
		void IDataCache<TextsRepository>.SetData(TextsRepository saveData) => _textsRepository = saveData;
		public TextsRepository GetData() => _textsRepository?? new TextsRepository();
	}
}