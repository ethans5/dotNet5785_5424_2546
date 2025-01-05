
using BO;

namespace BlApi;

public interface IVolunteer
{
    public jobType LogIn(int id, string password);
    public void CreateVolunteer(BO.Volunteer volunteer, int? idcall);
    public IEnumerable<BO.VolunteerInList> ReadAllVolunteers(bool? actif,VolunteerSortField? s);
    public BO.Volunteer ReadVolunteer(int id);
    public void UpdateVolunteer(int id ,BO.Volunteer volunteer);
    public void DeleteVolunteer(int id);


}
