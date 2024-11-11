namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer volunteer)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == volunteer.Id))
            throw new Exception($"Volunteer with the ID : {volunteer.Id} already exists...");
        else
            DataSource.Volunteers.Add(volunteer);
    }

    public Volunteer? Read(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            return DataSource.Volunteers.Find(v => v.Id == id);
        else
            throw new Exception($"Volunteer with the ID : {id} does not exist...");
    }


    public List<Volunteer> ReadAll()
    {
        return DataSource.Volunteers;
    }


    public void Update(Volunteer volunteer)
    {
        int index = DataSource.Volunteers.FindIndex(v => v.Id == volunteer.Id);
        if (index == -1)
            throw new Exception($"Volunteer with the ID : {volunteer.Id} does not exist...");
        else
            DataSource.Volunteers[index] = volunteer;
    }


    public void Delete(int id)
    {
        if (DataSource.Volunteers.Exists(v => v.Id == id))
            DataSource.Volunteers.RemoveAll(v => v.Id == id);
        else
            throw new Exception($"Volunteer with the ID : {id} does not exist...");
    }


    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }




}
