namespace Dal;

// Import necessary namespaces
using DalApi;

// Sealed class implementing the IDal interface for XML-based data access
sealed public class DalXml : IDal
{
    // Properties for each data type implementation
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    // Method to reset the database by clearing all data
    public void ResetDB()
    {
        Call.DeleteAll();       // Clear all Call records
        Volunteer.DeleteAll();  // Clear all Volunteer records
        Assignment.DeleteAll(); // Clear all Assignment records
        Config.Reset();         // Reset configuration settings
    }
}
