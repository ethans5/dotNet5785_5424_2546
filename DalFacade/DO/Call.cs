namespace DO;

public record Call
(

    int Id,
    callType CallType,
    string Address,
    double Latitude,
    double Longitude,
    DateTime CallTime,
    string? Description = null,
    DateTime? MaxTime = null



)
{
    public Call() : this(0, callType.type1, "", 0, 0, DateTime.Now, "") { }
}