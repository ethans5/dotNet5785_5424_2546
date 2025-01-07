using BlApi;
using BO;
using DO;
using Helpers;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private DalApi.IDal _dal = Factory.Get;

    public async void CreateVolunteer(BO.Volunteer volunteer)
    {

        try
        {
            try
            {
              //Tools.ValidateFieldsFormat(volunteer);
                var coordinateS = await Tools.GetCoordinatesAsync(volunteer.Address!);
                volunteer.Latitude = coordinateS.Latitude;
                volunteer.Longitude = coordinateS.Longitude;
            }
            catch (BO.BlInvalidInputException ex)
            {
                throw new BlInvalidInputException("Invalid input", ex);
            }
            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Mail,
                Password = volunteer.Password,
                Address = volunteer.Address,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                JobType = (DO.jobType)volunteer.Job,
                isActive = volunteer.IsActive,
                MaxDistance = volunteer.MaxDistance,
               
                distanceType = (DO.distanceType)volunteer.DistanceType
            };
            _dal.Volunteer.Create(doVolunteer);
        }
        catch(DO.DalAlreadyExistException ex)
        {
            throw new BlAlreadyExistsException("already exist", ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        BO.Volunteer? volunteer = ReadVolunteer(id);
        if (volunteer is null || volunteer.IsActive || volunteer.CallInProgress != null
            || volunteer.Totaltreated != 0)
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
            _ => volunteerList.OrderBy(v=>v.Id)
        };



    }
    public BO.Volunteer ReadVolunteer(int id)
    {
        try
        {
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(id)!;
            var volunter= Tools.parseDoToBoVolunteer(doVolunteer)  ;
            volunter.CallInProgress = Tools.GetCallInProgress(id);
            return volunter  ;
        }
        catch
        {
            throw new BlDoesNotExistException($"Volunteer with ID: {id} doesn't exist");
        }
    }

    public async void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        var myVolunteer = _dal.Volunteer.Read(id);
        if (myVolunteer!.JobType == DO.jobType.Director || id == volunteer.Id)
        {
            try
            {
                Tools.ValidateFieldsFormat(volunteer);
                var coordinate = await Tools.GetCoordinatesAsync(volunteer.Address!);
                volunteer.Latitude = coordinate.Latitude;
                volunteer.Longitude = coordinate.Longitude;
            }
            catch
            {
                throw new BlInvalidInputException("Invalid input");
            }

            DO.Volunteer doVolunteer = _dal.Volunteer.Read(volunteer.Id)!;
            if(doVolunteer.JobType != (DO.jobType)volunteer.Job && myVolunteer.JobType != DO.jobType.Director)
            {
                throw new BlUnauthorizedException("You are not authorized to change the job of this volunteer");
            }


        }
        else
            throw new BlUnauthorizedException("You are not authorized to update this volunteer");

        DO.Volunteer doVolunteer2 = new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Mail,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            JobType = (DO.jobType)volunteer.Job,
            isActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            distanceType = (DO.distanceType)volunteer.DistanceType
        };

        _dal.Volunteer.Update(doVolunteer2);
    }
}
