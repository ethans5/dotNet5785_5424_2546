namespace BO;
public class Volunteer
{
    public int Id { get;init; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Mail { get; set; }
    public string? Password { get; set; }
    public string? Address{ get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public jobType Job { get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public distanceType DistanceType { get; set; }
    public int Totaltreated { get; set; }
    public int TotalSelfCancellation { get; set; }
    public int TotalExpired { get; set; }
    public CallInProgress? CallInProgress { get; set; }

}
