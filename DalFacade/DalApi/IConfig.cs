namespace DalApi;

public interface IConfig
{
    public int NextCallId { get; }
    public int NextAssignmentId { get; }
    public DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
    public void Reset();
}
