using DalApi;

namespace Dal;
public class ConfigImplementation : IConfig
{
    public int NextCallId => Config.NextCallId;

    public int NextAssignmentId => Config.NextAssignmentId;

    public DateTime Clock { get => Config.Clock; set => Config.Clock=value; }
    public TimeSpan RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    public void Reset()
    {
        Config.Reset();
    }
}
