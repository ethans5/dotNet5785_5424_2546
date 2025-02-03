namespace s_dal;
using DO;
using DalApi;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements the IAssignment interface to handle CRUD operations for Assignment entities.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new Assignment and assigns a unique ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment assignment)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = assignment with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    /// <summary>
    /// Retrieves an Assignment by its unique ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        if (DataSource.Assignments.Exists(a => a.Id == id))
            return DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        else
            throw new DalDoesNotExistException($"Assignment with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Retrieves an Assignment that matches the specified condition.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all Assignments, optionally filtering them based on a condition.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing Assignment in the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment assignment)
    {
        int index = DataSource.Assignments.FindIndex(a => a.Id == assignment.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Assignment with the ID : {assignment.Id} does not exist...");
        else
            DataSource.Assignments[index] = assignment;
    }

    /// <summary>
    /// Deletes an Assignment by its unique ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (DataSource.Assignments.Exists(a => a.Id == id))
            DataSource.Assignments.RemoveAll(a => a.Id == id);
        else
            throw new DalDoesNotExistException($"Assignment with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Deletes all Assignments from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
}
