
namespace BO;
public class CallInProgress
{
    public int Id { get;init; }
    public int CallId { get; set; }
    public callType CallType { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; }
    public DateTime Created { get; set; }
    public DateTime? MaxEndTreatment { get; set; }
    public DateTime StartTreatment { get; set; }
    public double Distance { get; set; }
    public Treatment Treatment { get; set; }


}
