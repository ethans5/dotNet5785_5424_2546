namespace DalApi;
using DO;

public interface IVolunteer
{
    void Create(DO.Volunteer volunteer);
    DO.Volunteer? Read(int id);
    List<DO.Volunteer> ReadAll();
    void Update(DO.Volunteer volunteer);
    void Delete(int id);
    void DeleteAll();

}
