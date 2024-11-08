using DalApi;

namespace Dal;
public class ConfigImplementation : IConfig
{
    public int NextCallId => Config.NextCallId;

    public int NextAssignmentId => Config.NextAssignmentId;

    public DateTime Clock { get => Config.Clock; set => Config.Clock=value; }
    public TimeSpan RiskRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Reset()
    {
        Config.Reset();
    }
}
