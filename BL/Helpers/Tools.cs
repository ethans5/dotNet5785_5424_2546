using DalApi;
namespace Helpers;

internal class Tools
{
    public IDal _dal = Factory.Get;

    public int totalTreated(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.treated);
    }

    public int totalSelfCancelled(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.selfCancellation);
    }

    public int totalExpired(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.Expired);
    }


    public BO.Volunteer parseDoToBo(DO.Volunteer volunteer)
    {
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Mail = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Job = (BO.jobType)volunteer.JobType,
            IsActive = volunteer.isActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (BO.distanceType)volunteer.distanceType,
            Totaltreated = totalTreated(volunteer.Id),
            TotalSelfCancellation = totalSelfCancelled(volunteer.Id),
            TotalExpired = totalExpired(volunteer.Id)
        };
    }
}
