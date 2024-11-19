namespace s_dal;

/// <summary>
/// Static configuration class for managing unique IDs, system clock, and other global settings.
/// </summary>
internal static class Config
{
    // Starting value for Call IDs.
    internal const int startCallId = 1;

    // Internal field for the next Call ID.
    private static int nextCallId = startCallId;

    /// <summary>
    /// Gets the next unique Call ID and increments the internal counter.
    /// </summary>
    internal static int NextCallId { get => nextCallId++; }

    // Starting value for Assignment IDs.
    internal const int startAssignmentId = 1;

    // Internal field for the next Assignment ID.
    private static int nextAssignmentId = startAssignmentId;

    /// <summary>
    /// Gets the next unique Assignment ID and increments the internal counter.
    /// </summary>
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    /// <summary>
    /// Represents the current system clock.
    /// Can be used for time-based operations.
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

    /// <summary>
    /// Represents the risk range as a time span.
    /// Default is 30 minutes.
    /// </summary>
    internal static TimeSpan RiskRange { get; set; } = new TimeSpan(0, 30, 0);

    /// <summary>
    /// Resets all configuration values to their initial state.
    /// Resets IDs, system clock, and risk range.
    /// </summary>
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskRange = new TimeSpan(0, 30, 0);
    }
}
