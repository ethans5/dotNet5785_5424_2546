
using BO;

namespace BlApi;

public interface IVolunteer
{
    public jobType LogIn(string mail, string password);
    public void CreateVolunteer(BO.Volunteer volunteer);
    public IEnumerable<BO.VolunteerInList> ReadAllVolunteers(bool? actif,VolunteerSortField? s);
    public BO.Volunteer ReadVolunteer(int id);
    public void UpdateVolunteer(int id ,BO.Volunteer volunteer);
    public void DeleteVolunteer(int id);


}
