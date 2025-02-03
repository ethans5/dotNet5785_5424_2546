namespace s_dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements the IVolunteer interface to manage Volunteer entities in the data source.
/// Provides CRUD operations and supports filtering.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new Volunteer to the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer volunteer)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == volunteer.Id))
            throw new DalAlreadyExistException($"Volunteer with the ID : {volunteer.Id} already exists...");
        else
            DataSource.Volunteers.Add(volunteer);
    }

    /// <summary>
    /// Retrieves a Volunteer by its unique ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        else
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Retrieves the first Volunteer that matches a given condition.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all Volunteers, optionally filtering them based on a condition.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null
             ? DataSource.Volunteers.Select(item => item)
             : DataSource.Volunteers.Where(filter);
    }

    /// <summary>
    /// Updates an existing Volunteer in the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer volunteer)
    {
        int index = DataSource.Volunteers.FindIndex(v => v.Id == volunteer.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Volunteer with the ID : {volunteer.Id} does not exist...");
        else
            DataSource.Volunteers[index] = volunteer;
    }

    /// <summary>
    /// Deletes a Volunteer by its unique ID.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            DataSource.Volunteers.RemoveAll(v => v.Id == id);
        else
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");
    }

    /// <summary>
    /// Deletes all Volunteers from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}
