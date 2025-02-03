using DalApi;
using System.Runtime.CompilerServices;

namespace s_dal;

/// <summary>
/// Provides an implementation of the IConfig interface,
/// delegating operations to the static Config class.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets the next unique Call ID by accessing the static Config class.
    /// </summary>
    public int NextCallId => Config.NextCallId;

    /// <summary>
    /// Gets the next unique Assignment ID by accessing the static Config class.
    /// </summary>
    public int NextAssignmentId => Config.NextAssignmentId;

    /// <summary>
    /// Gets or sets the system clock value.
    /// Delegates to the static Config class.
    /// </summary>
    public DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the risk range value.
    /// Delegates to the static Config class.
    /// </summary>
    public TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// Resets all configuration values to their initial state.
    /// Delegates to the static Config class.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
    {
        Config.Reset();
    }
}
