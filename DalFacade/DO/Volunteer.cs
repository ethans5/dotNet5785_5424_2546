namespace DO;

public record Volunteer
(
  

int Id,
string Name,
string Phone,
string Email,
jobType JobType,
Boolean isActive,
distanceType distanceType,
double? MaxDistance = null,
string? Password = null,
string? Address = null,
double? Latitude = null,
double? Longitude = null
)
{
    public Volunteer() : this(0, "", "", "", jobType.Volunteer, true, distanceType.aerial) { }
}
