namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static IAssignment? s_dalAssignment; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1

    private static void createCall()
    {
        for (int i = 0; i < 30; i++)
        { 
        int id = s_dalConfig!.NextCallId;
        Array values = Enum.GetValues(typeof(callType));
        callType type = (callType)values.GetValue(s_rand.Next(values.Length))!;
        string address = "address" + s_rand.Next(1000);
        double latitude = s_rand.NextDouble() * 180 - 90;
        double longitude = s_rand.NextDouble() * 360 - 180;
        DateTime time = s_dalConfig.Clock;
        s_dalCall!.Create(new Call { Id = id, CallType = type, Address = address, Latitude = latitude, Longitude = longitude, CallTime = time });
    }


    }
    private static void createVolunteer()
    {
        for (int i = 0; i < 30; i++)
        {
            int id = s_rand.Next(200000000, 400000001);

            Array values = Enum.GetValues(typeof(jobType));
            jobType type = (jobType)values.GetValue(s_rand.Next(values.Length))!;

            string name = "name" + s_rand.Next(1000);
            string phone = "phone" + s_rand.Next(1000);
            string email = "email" + s_rand.Next(1000);
            Boolean isActive = s_rand.Next(2) == 0;
            values = Enum.GetValues(typeof(distanceType));
            distanceType distanceType = (distanceType)values.GetValue(s_rand.Next(values.Length))!;
            double? distance = s_rand.NextDouble() * 1000;
            string password = "password" + s_rand.Next(1000);
            string address = "address" + s_rand.Next(1000);
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;
            s_dalVolunteer!.Create(new Volunteer { Id = id, Name = name, Phone = phone, Email = email, JobType = type, isActive = isActive, distanceType = distanceType, distance = distance, Password = password, Address = address, Latitude = latitude, Longitude = longitude });
        }
    }
    private static void createAssignment()
    {
        for (int i = 0; i < 30; i++)
        {
            int id = s_dalConfig!.NextAssignmentId;
            var list = s_dalCall!.ReadAll();
            if (list.Count> 0)
            {
                int callId = list[s_rand.Next(list.Count)].Id;
                var list1 = s_dalVolunteer!.ReadAll();
                if (list1.Count > 0)
                {
                    int volunteerId = list1[s_rand.Next(list1.Count)].Id;
                    DateTime time = s_dalConfig.Clock;
                    s_dalAssignment!.Create(new Assignment { Id = id, CallId = callId, VolunteerId = volunteerId, StartTreatment = time });
                }
            }
           
        }
    }

    private static readonly Random s_rand = new();
}
