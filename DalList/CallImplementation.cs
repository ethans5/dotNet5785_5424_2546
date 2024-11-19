namespace s_dal;
using DO;
using DalApi;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call call)
    {
        int id= Config.NextCallId;
        Call copy= call with { Id = id };
        DataSource.Calls.Add(copy);
    }


    public Call? Read(int id)
    {
        if (DataSource.Calls.Exists(c => c.Id == id))
            return DataSource.Calls.FirstOrDefault(c => c.Id == id);
        else
            throw new DalDoesNotExistException($"Call with the ID : {id} does not exist...");
    }
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
    //public List<Call> ReadAll()
    //{
    //    return DataSource.Calls;
    //}

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
       return filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);
    }
    public void Update(Call call)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == call.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Call with the ID : {call.Id} does not exist...");
        DataSource.Calls[index] = call;
    }

    public void Delete(int id)
    {
        if(DataSource.Calls.Any(c => c.Id == id)!)
            DataSource.Calls.RemoveAll(c => c.Id == id);
        else
            throw new DalDoesNotExistException($"Call with the ID : {id} deos not exist...");

    }
  
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    
}
