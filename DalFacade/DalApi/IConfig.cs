namespace DalApi;

public interface IConfig
{
    /// <summary>
    /// Represents the configuration settings for managing system calls and assignments.
    /// Includes properties for tracking unique IDs, system time, and risk range settings.
    /// Provides a method to reset the configuration to its default state.
    /// </summary>
    public int NextCallId { get; }
    public int NextAssignmentId { get; }
    public DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
    public void Reset();
}
