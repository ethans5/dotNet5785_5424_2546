namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public callType CallType { get; set; }
    public required string Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime StartTreatment { get; set; }
    public DateTime? EndTreatment { get; set; }
    public typeOfEndTreatment? TypeOfEndTreatment { get; set; }
}
