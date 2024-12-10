
using BO;

namespace BlApi;

public interface IVolunteer
{
    public jobType Login(string mail, string password);
    public IEnumerable<BO.VolunteerInList> GetVolunteers(bool actif,VolunteerSortField s);
    public BO.Volunteer GetVolunteer(int id);
    public void UpdateVolunteer(int id ,BO.Volunteer volunteer);
    public void DeleteVolunteer(int id);
    public void AddVolunteer(BO.Volunteer volunteer);

}
