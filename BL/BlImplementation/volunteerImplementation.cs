using BlApi;
using BO;
using DO;
using Helpers;

namespace BlImplementation;

internal class volunteerImplementation : IVolunteer
{
    private DalApi.IDal _dal = Factory.Get;
    static readonly Tools toolsInstance = new Tools();

    public void CreateVolunteer(Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public jobType LogIn(int id, string password)
    {
        try
        {
            var volunteer = ReadVolunteer(id);
            if (volunteer.Password == password)
            {
                return volunteer.Job;
            }
            else
            {
                throw new WrongPassword("Wrong password");
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Volunteer does not exist", ex);
        }
    }

    public IEnumerable<VolunteerInList> ReadAllVolunteers(bool? actif, VolunteerSortField? s)
    {
        var allAssign = _dal.Assignment.ReadAll();
        var allCalls = _dal.Call.ReadAll();
        if (actif == null)
        {
            return from volunteer in _dal.Volunteer.ReadAll()
                   let volunteerAssignments = allAssign.Where(a => a.VolunteerId == volunteer.Id)
                   let activeCallId = volunteerAssignments
                             .Where(a => a.endTreatment == null)
                             .Select(a => a.CallId)
                             .FirstOrDefault()
                   let activeCallType = allCalls
                                        .Where(c => c.Id == activeCallId)
                                        .Select(c => (BO.callType)c.CallType)
                                        .FirstOrDefault()
                   select new VolunteerInList
                   {
                       Id = volunteer.Id,
                       Name = volunteer.Name,
                       IsActive = volunteer.isActive,
                       Totaltreated = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.treated),
                       TotalSelfCancellation = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.selfCancellation),
                       TotalExpired = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.Expired),
                       IdCall = activeCallId,
                       callType = activeCallType
                   };
        }
        else if (actif == true)
        {
            return from volunteer in _dal.Volunteer.ReadAll()
                   where volunteer.IsActive == true
                   select new VolunteerInList
                   {
                       Id = volunteer.Id,
                       Name = volunteer.Name,
                       IsActive = volunteer.IsActive,
                       Totaltreated = volunteer.Totaltreated,
                       TotalSelfCancellation = volunteer.TotalSelfCancellation,
                       TotalExpired = volunteer.TotalExpired,
                       IdCall = volunteer.IdCall,
                       callType = volunteer.callType
                   };
        }
        else
        {
            return from volunteer in DataSource.Volunteers
                   where volunteer.IsActive == false
                   select new VolunteerInList
                   {
                       Id = volunteer.Id,
                       Name = volunteer.Name,
                       Phone = volunteer.Phone,
                       Email = volunteer.Email,
                       Job = volunteer.Job,
                       IsActive = volunteer.IsActive,
                       MaxDistance = volunteer.MaxDistance,
                       DistanceType = volunteer.DistanceType,
                       Totaltreated = volunteer.Totaltreated,
                       TotalSelfCancellation = volunteer.TotalSelfCancellation,
                       TotalExpired = volunteer.TotalExpired
                   };
        } 
    public Volunteer ReadVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteer(int id, Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
