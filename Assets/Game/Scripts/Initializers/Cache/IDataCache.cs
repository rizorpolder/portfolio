
public interface IDataCache<T>
{
	void SetData(T data);
	T GetData();
}