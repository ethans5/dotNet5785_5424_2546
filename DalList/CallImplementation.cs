namespace s_dal;
using DO;
using DalApi;
using System.Collections.Generic;

/// <summary>
/// Implements the ICall interface to handle CRUD operations for Call entities.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// Creates a new Call and assigns a unique ID.
    /// </summary>
    public void Create(Call call)
    {
        int id = Config.NextCallId;
        Call copy = call with { Id = id };
        DataSource.Calls.Add(copy);
    }

    /// <summary>
    /// Retrieves a Call by its unique ID.
    /// </summary>
    public Call? Read(int id)
    {
        if (DataSource.Calls.Exists(c => c.Id == id))
            return DataSource.Calls.FirstOrDefault(c => c.Id == id);
        else
            throw new DalDoesNotExistException($"Call with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Retrieves a Call that matches the specified condition.
    /// </summary>
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all Calls, optionally filtering them based on a condition.
    /// </summary>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing Call in the data source.
    /// </summary>
    public void Update(Call call)
    {
        int index = DataSource.Calls.FindIndex(c => c.Id == call.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Call with the ID : {call.Id} does not exist...");
        DataSource.Calls[index] = call;
    }

    /// <summary>
    /// Deletes a Call by its unique ID.
    /// </summary>
    public void Delete(int id)
    {
        if (DataSource.Calls.Any(c => c.Id == id))
            DataSource.Calls.RemoveAll(c => c.Id == id);
        else
            throw new DalDoesNotExistException($"Call with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Deletes all Calls from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
}
