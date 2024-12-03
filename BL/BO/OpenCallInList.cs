namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public callType callType { get; set; }
    public string? description { get; set; }
    public required string Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime? MaxEndTreatment { get; set; }
    public double Distance { get; set; }

}
