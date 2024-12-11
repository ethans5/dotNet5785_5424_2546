namespace BO;

public class CallInList
{
    public int? Id { get; init;}
    public int callId { get; set; }
    public callType callType { get; set; }
    public DateTime startingTime { get; set; }
    public TimeSpan? remainingTime { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? duration { get; set; }
    public Status Status { get; set; }
    public int TotalAssignmentAllocations { get; set; }
}
