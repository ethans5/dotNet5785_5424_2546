using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();
    public static BO.Volunteer parseDoToBoVolunteer(DO.Volunteer volunteer)
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
            Totaltreated = Tools.totalTreated(volunteer.Id),
            TotalSelfCancellation = Tools.totalSelfCancelled(volunteer.Id),
            TotalExpired = Tools.totalExpired(volunteer.Id)
        };
    }


}
