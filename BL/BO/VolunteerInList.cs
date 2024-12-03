namespace BO;

public class VolunteerInList
{
    int Id { get; init; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public int Totaltreated { get; set; }
    public int TotalSelfCancellation { get; set; }
    public int TotalExpired { get; set; }
    public int ? IdCall { get; set; }
    public callType callType { get; set; }


}
