namespace Dal;
using DO;
using DalApi;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call call)
    {
        if (DataSource.Calls.Exists(c => c.Id == call.Id))
            throw new InvalidOperationException($"Call with the ID : {call.Id} already exists...");
        DataSource.Calls.Add(call);
    }


    public Call? Read(int id)
    {
        if (DataSource.Calls.Exists(c => c.Id == id))
            return DataSource.Calls.Find(c => c.Id == id);
        else
            throw new InvalidOperationException($"Call with the ID : {id} does not exist...");
    }
    public List<Call> ReadAll()
    {
        return DataSource.Calls;
    }


    public void Update(Call call)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == call.Id);
        if (index == -1)
            throw new InvalidOperationException($"Call with the ID : {call.Id} does not exist...");
        DataSource.Calls[index] = call;
    }

    public void Delete(int id)
    {
        if(DataSource.Calls.Any(c => c.Id == id)!)
            DataSource.Calls.RemoveAll(c => c.Id == id);
        else
            throw new InvalidOperationException($"Call with the ID : {id} deos not exist...");

    }
  
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }




 
 

}
