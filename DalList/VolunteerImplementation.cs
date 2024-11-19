namespace s_dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer volunteer)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == volunteer.Id))
            throw new DalAlreadyExistException($"Volunteer with the ID : {volunteer.Id} already exists...");
        else
            DataSource.Volunteers.Add(volunteer);
    }

    public Volunteer? Read(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        else
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }


    //public List<Volunteer> ReadAll()
    //{
    //    return DataSource.Volunteers;
    //}

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null
             ? DataSource.Volunteers.Select(item => item)
             : DataSource.Volunteers.Where(filter);
    }
    public void Update(Volunteer volunteer)
    {
        int index = DataSource.Volunteers.FindIndex(v => v.Id == volunteer.Id);
        if (index == -1)
            throw new DalDoesNotExistException($"Volunteer with the ID : {volunteer.Id} does not exist...");
        else
            DataSource.Volunteers[index] = volunteer;
    }


    public void Delete(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            DataSource.Volunteers.RemoveAll(v => v.Id == id);
        else
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");
    }


    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    
}
