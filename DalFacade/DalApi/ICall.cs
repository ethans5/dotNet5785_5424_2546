namespace DalApi;
using DO;


public interface ICall
{
    void Create(DO.Call call);
    DO.Call? Read(int id);
    List<DO.Call> ReadAll();
    void Update(DO.Call call);
    void Delete(int id);
    void DeleteAll();
}
