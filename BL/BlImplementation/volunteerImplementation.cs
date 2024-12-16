using BlApi;
using BO;
using DO;
using Helpers;

namespace BlImplementation;

internal class volunteerImplementation : IVolunteer
{
    private DalApi.IDal _dal = Factory.Get;
    static readonly Tools toolsInstance = new Tools();

    public void CreateVolunteer(BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int id)
    {
        BO.Volunteer? volunteer = ReadVolunteer(id);
        if (volunteer is null || volunteer.IsActive)
            throw new BlDeletionImpossible($"Deletion of the Volunteer with ID {id} impossible");
        try
        {
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Volunteer with ID={id} does not exist", ex);
        }
    }

    public BO.jobType LogIn(int id, string password)
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

    public IEnumerable<VolunteerInList> ReadAllVolunteers(bool? actif, VolunteerSortField? sortBy)
    {
        var allAssign = _dal.Assignment.ReadAll();
        var allCalls = _dal.Call.ReadAll();


        var volunteers = actif switch
        {
            null => _dal.Volunteer.ReadAll(),
            true => _dal.Volunteer.ReadAll().Where(v=> v.isActive),
            false => _dal.Volunteer.ReadAll().Where(v=>!v.isActive)
        };

      
        
            var volunteerList =  from volunteer in volunteers
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

        return sortBy switch
        {
            VolunteerSortField.Name => volunteerList.OrderBy(v=>v.Name),
            VolunteerSortField.Totaltreated=>volunteerList.OrderBy(v=> v.Totaltreated),
            VolunteerSortField.TotalSelfCancellation=>volunteerList.OrderBy(v=>v.TotalSelfCancellation),
            VolunteerSortField.TotalExpired=>volunteerList.OrderBy(v=>v.TotalExpired),
            VolunteerSortField.CallType=>volunteerList.OrderBy(v=>v.callType),
            null => volunteerList.OrderBy(v=>v.Id)
        };



    }
    public BO.Volunteer ReadVolunteer(int id)
    {
        try
        {
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(id)!;
            return toolsInstance.parseDoToBo(doVolunteer);
        }
        catch
        {
            throw new BlDoesNotExistException($"Volunteer with ID: {id} doesn't exist");
        }
    }

    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
