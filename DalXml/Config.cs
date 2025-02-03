using System.Runtime.CompilerServices;

namespace Dal;

// Static class to manage the configuration of the system
internal static class Config
{
    // File paths for XML configuration and data files
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";

    // Property to retrieve and increment the next Call ID
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    // Property to retrieve and increment the next Assignment ID
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    // Property to get or set the system clock
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    // Property to get or set the risk range as a time span
    internal static TimeSpan RiskRange
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        get => TimeSpan.FromMinutes(XMLTools.GetConfigIntVal(s_data_config_xml, "RiskRange"));
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "RiskRange", (int)value.TotalMinutes);
    }

    // Method to reset the system configuration to default values
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextAssignmentId = 1;
        NextCallId = 1;
        RiskRange = new TimeSpan(0, 30, 0); // Default risk range is 30 minutes
        Clock = DateTime.Now; // Default clock is the current time
    }
}
