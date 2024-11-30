using DalApi;

namespace Dal;

// Static class to manage the configuration of the system
internal class ConfigImplementation : IConfig
{
    // Property to retrieve the next Call ID
    public int NextCallId => Config.NextCallId;

    // Property to retrieve the next Assignment ID
    public int NextAssignmentId => Config.NextAssignmentId;

    // Property to manage the system's Clock configuration
    public DateTime Clock
    {
        get => Config.Clock;  // Retrieve the current Clock value
        set => Config.Clock = value;  // Update the Clock value
    }

    // Property to manage the system's RiskRange configuration
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;  // Retrieve the current RiskRange value
        set => Config.RiskRange = value;  // Update the RiskRange value
    }

    // Method to reset the configuration of the system
    public void Reset()
    {
        Config.Reset();  // Reset all configuration settings to their default values
    }
}
