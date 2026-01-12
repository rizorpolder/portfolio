namespace Game.Scripts.Systems.Input
{
	public interface IInputCommand
	{
		public void AddImportantAction(string action);
		public void RemoveImportantAction(string action);
	}
}