namespace Dal;
using DalApi;
using s_dal;

/// <summary>
/// Provides an implementation of the IDal interface using in-memory data structures.
/// This serves as a complete data access layer for the application.
/// </summary>
 sealed internal class DalList : IDal
{
    public static DalList Instance { get; } = new DalList();
    private DalList() { }
    /// <summary>
    /// Provides operations for managing Call entities.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Provides operations for managing Volunteer entities.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Provides operations for managing Assignment entities.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Provides access to configuration and system-wide settings.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the entire database by clearing all data and resetting configurations.
    /// </summary>
    public void ResetDB()
    {
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
