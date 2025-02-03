using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static readonly IDal _dal = Factory.Get; // Stage 4
    internal static ObserverManager Observers = new();

    public static double? latitude;
    public static double? longitude;

   
    public static  BO.Volunteer ParseDoToBoVolunteer(DO.Volunteer volunteer)
    {
        if (volunteer == null)
            throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");
        //var coordinates = Tools.GetCoordinatesAsync(volunteer.Address!);
        //latitude = coordinates.Result.Latitude;
        //longitude = coordinates.Result.Longitude;
        try
        {
            

            BO.Volunteer volunteer1 = new BO.Volunteer
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

            return volunteer1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            throw;
        }
    }

    public static IEnumerable<BO.Volunteer> GetAvailableVolunteers()
    {
        var volunteers = _dal.Volunteer.ReadAll();
        return volunteers.Select(ParseDoToBoVolunteer).Where(v => v.CallInProgress==null);
    }
}
