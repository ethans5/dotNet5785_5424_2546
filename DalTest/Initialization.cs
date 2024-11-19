namespace DalTest;
using DalApi;
using DO;
using System.Threading;

public static class Initialization
{
    //private static IAssignment? s_dalAssignment; //stage 1
    //private static ICall? s_dalCall; //stage 1
    //private static IVolunteer? s_dalVolunteer; //stage 1
    //private static IConfig? s_dalConfig; //stage 1
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();

    /// <summary>
    /// 
    /// </summary>
    private static void createCall()
    {
        string[] addresses = new string[]
        {
            "1 HaTsanhanim Street, Jerusalem", "2 HaPalmach Street, Jerusalem", "3 HaNasi Street, Jerusalem",
            "4 Yad HaRav Nissim Street, Jerusalem", "5 HaRav Herzog Street, Jerusalem", "6 HaMesila Street, Jerusalem",
            "7 HaNavi Street, Jerusalem", "8 HaSadna Street, Jerusalem", "9 HaShoftim Street, Jerusalem",
            "10 HaTurim Street, Jerusalem", "11 HaMelekh David Street, Jerusalem", "12 Derech HaPisga Street, Jerusalem",
            "13 HaRav Uziel Street, Jerusalem", "14 HaAvoda Street, Jerusalem", "15 HaRav Kook Street, Jerusalem",
            "16 HaParsa Street, Jerusalem", "17 HaTikva Street, Jerusalem", "18 HaNevi'im Street, Jerusalem",
            "19 Haneviim Street, Jerusalem", "20 HaYarden Street, Jerusalem", "21 Sderot Levi Eshkol, Jerusalem",
            "22 HaShalom Street, Jerusalem", "23 HaRav Berlin Street, Jerusalem", "24 Sderot Ben Gurion, Jerusalem",
            "25 Sderot HaNasi, Jerusalem", "26 HaZerem Street, Jerusalem", "27 Derech Aharon Shulov, Jerusalem",
            "28 HaOfanim Street, Jerusalem", "29 HaRav Yitzhak Nissim Street, Jerusalem", "30 HaDoar Street, Jerusalem",
            "31 HaRav Elyashiv Street, Jerusalem", "32 HaTamar Street, Jerusalem", "33 HaRav Zini Street, Jerusalem",
            "34 HaRav Goren Street, Jerusalem", "35 HaRav Pincus Street, Jerusalem", "36 HaRav Ovadia Street, Jerusalem",
            "37 HaRav Shapira Street, Jerusalem", "38 HaSharon Street, Jerusalem", "39 Derech Bar Lev, Jerusalem",
            "40 Yad Eliyahu Street, Jerusalem", "41 Derech Betlehem, Jerusalem", "42 HaRav Maimon Street, Jerusalem",
            "43 Keren HaYesod Street, Jerusalem", "44 Emek Refaim Street, Jerusalem", "45 HaRav Chaim Berlin Street, Jerusalem",
            "46 HaRav Shlomo Zalman Street, Jerusalem", "47 HaTzofim Street, Jerusalem", "48 Sderot Golda Meir, Jerusalem",
            "49 HaNegev Street, Jerusalem", "50 HaEtzel Street, Jerusalem"
        };

        // Tableau des latitudes fictives
        double[] latitudes = new double[]
        {
            31.7690, 31.7685, 31.7720, 31.7750, 31.7730, 31.7715, 31.7760, 31.7695, 31.7725, 31.7740,
            31.7700, 31.7718, 31.7699, 31.7765, 31.7748, 31.7736, 31.7752, 31.7727, 31.7688, 31.7697,
            31.7751, 31.7705, 31.7687, 31.7749, 31.7706, 31.7733, 31.7710, 31.7737, 31.7763, 31.7692,
            31.7723, 31.7694, 31.7708, 31.7755, 31.7719, 31.7702, 31.7684, 31.7768, 31.7731, 31.7709,
            31.7729, 31.7743, 31.7757, 31.7686, 31.7701, 31.7761, 31.7716, 31.7754, 31.7721, 31.7703
        };

        // Tableau des longitudes fictives
        double[] longitudes = new double[]
        {
            35.2130, 35.2155, 35.2110, 35.2165, 35.2120, 35.2170, 35.2135, 35.2185, 35.2105, 35.2140,
            35.2150, 35.2102, 35.2163, 35.2149, 35.2124, 35.2182, 35.2145, 35.2139, 35.2128, 35.2181,
            35.2107, 35.2168, 35.2146, 35.2138, 35.2157, 35.2119, 35.2176, 35.2115, 35.2167, 35.2141,
            35.2122, 35.2108, 35.2179, 35.2134, 35.2117, 35.2183, 35.2169, 35.2109, 35.2154, 35.2131,
            35.2175, 35.2116, 35.2136, 35.2152, 35.2184, 35.2121, 35.2151, 35.2172, 35.2118, 35.2162
        };

        for (int i = 0; i < 50; i++)
        {
            int id = s_dal!.Config.NextCallId;
            Array values = Enum.GetValues(typeof(callType));
            callType type = (callType)values.GetValue(s_rand.Next(values.Length))!;
            string address = "address" + s_rand.Next(1000);
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;
            DateTime start = new DateTime(s_dal.Config.Clock.Year - 2, 1, 1); //stage 1
            int range = (s_dal.Config.Clock - start).Days; //stage 1
            DateTime time = start.AddDays(s_rand.Next(range));
            Boolean datelimite = s_rand.Next(2) == 0;
            DateTime? limite = null;
            if (datelimite)
            {
                int range1 = 60;
                int randomDays = s_rand.Next(range1);
                limite = s_dal.Config.Clock.AddDays(randomDays);
            }


            s_dal.Call!.Create(new Call
            {
                
                CallType = type,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                CallTime = time,
                MaxTime = limite
            });
        }


    }
    private static void createVolunteer()
    {
        int[] ids = {
    218114920, 339762723, 358113614, 340485311, 206371723,
    224008181, 263061435, 339366513, 342943374, 360309029,
    232357019, 353009284, 374360423, 392492475, 255402454
};
        string[] names =
        {
            "Bensimon Ruben",
            "Sarfati Ethan",
            "Martin Lucas",
            "Dubois Léa",
            "Lefevre Antoine",
            "Garnier Camille",
            "Morel Julien",
            "Mercier Clara",
            "Dupont Arthur",
            "Laurent Inès",
            "Simon Hugo",
            "Caron Emma",
            "Richard Thomas",
            "Rousseau Alice",
            "Girard Louis"
        };

        string[] mails =
        {
            "rubenaar@g.jct.ac.il",
            "sarfati@g.jct.ac.il",
            "martin.lucas@email.com",
            "dubois.lea@email.com",
            "lefevre.antoine@email.com",
            "garnier.camille@email.com",
            "morel.julien@email.com",
            "mercier.clara@email.com",
            "dupont.arthur@email.com",
            "laurent.ines@email.com",
            "simon.hugo@email.com",
            "caron.emma@email.com",
            "richard.thomas@email.com",
            "rousseau.alice@email.com",
            "girard.louis@email.com",
        };

        string[] phones =
        {
            "0534703587",
            "0584741216",
            "0501234567",
            "0549876543",
            "0522345678",
            "0587654321",
            "0503456789",
            "0548765432",
            "0524567890",
            "0586781234",
            "0505678901",
            "0547890123",
            "0526789012",
            "0581234567",
            "0508901234",
        };

        string[] addresses =
        {
            "48 King George Street, Jerusalem",
            "12 Yafo Street, Jerusalem",
            "25 Ben Yehuda Street, Jerusalem",
            "17 Agron Street, Jerusalem",
            "33 Shlomtzion Hamalka Street, Jerusalem",
            "9 Hillel Street, Jerusalem",
            "56 Emek Refaim Street, Jerusalem",
            "22 Azza Street, Jerusalem",
            "67 Beit Lehem Street, Jerusalem",
            "41 Hanevi'im Street, Jerusalem",
            "10 Ramban Street, Jerusalem",
            "3 Nachlaot Street, Jerusalem",
            "29 Rachel Imenu Street, Jerusalem",
            "15 Derech Hevron Street, Jerusalem",
            "78 Hapalmah Street, Jerusalem",
        };

        double[] longitudes = new double[]
        {
            35.2132, // Longitude pour 48 King George Street, Jerusalem
            35.2137, // Longitude pour 12 Yafo Street, Jerusalem
            35.2110, // Longitude pour 25 Ben Yehuda Street, Jerusalem
            35.2178, // Longitude pour 17 Agron Street, Jerusalem
            35.2129, // Longitude pour 33 Shlomtzion Hamalka Street, Jerusalem
            35.2105, // Longitude pour 9 Hillel Street, Jerusalem
            35.2098, // Longitude pour 56 Emek Refaim Street, Jerusalem
            35.2154, // Longitude pour 22 Azza Street, Jerusalem
            35.2123, // Longitude pour 67 Beit Lehem Street, Jerusalem
            35.2091, // Longitude pour 41 Hanevi'im Street, Jerusalem
            35.2187, // Longitude pour 10 Ramban Street, Jerusalem
            35.2165, // Longitude pour 3 Nachlaot Street, Jerusalem
            35.2140, // Longitude pour 29 Rachel Imenu Street, Jerusalem
            35.2158, // Longitude pour 15 Derech Hevron Street, Jerusalem
            35.2170  // Longitude pour 78 Hapalmah Street, Jerusalem
        };

        double[] latitudes = new double[]
        {
            31.7815, // Latitude pour 48 King George Street, Jerusalem
            31.7780, // Latitude pour 12 Yafo Street, Jerusalem
            31.7712, // Latitude pour 25 Ben Yehuda Street, Jerusalem
            31.7819, // Latitude pour 17 Agron Street, Jerusalem
            31.7797, // Latitude pour 33 Shlomtzion Hamalka Street, Jerusalem
            31.7731, // Latitude pour 9 Hillel Street, Jerusalem
            31.7720, // Latitude pour 56 Emek Refaim Street, Jerusalem
            31.7765, // Latitude pour 22 Azza Street, Jerusalem
            31.7742, // Latitude pour 67 Beit Lehem Street, Jerusalem
            31.7708, // Latitude pour 41 Hanevi'im Street, Jerusalem
            31.7800, // Latitude pour 10 Ramban Street, Jerusalem
            31.7755, // Latitude pour 3 Nachlaot Street, Jerusalem
            31.7735, // Latitude pour 29 Rachel Imenu Street, Jerusalem
            31.7775, // Latitude pour 15 Derech Hevron Street, Jerusalem
            31.7790  // Latitude pour 78 Hapalmah Street, Jerusalem
        };
      

        for (int i = 0; i < 15; i++)
        {

            Array values = Enum.GetValues(typeof(jobType));
            jobType type = (jobType)values.GetValue(s_rand.Next(values.Length))!;
            Boolean isActive = s_rand.Next(2) == 0;
            values = Enum.GetValues(typeof(distanceType));
            distanceType distanceType = (distanceType)values.GetValue(s_rand.Next(values.Length))!;
            double? distance = s_rand.NextDouble() * 1000;
            string password = "password" + s_rand.Next(1000);
            s_dal!.Volunteer!.Create(new Volunteer
            {
                Id = ids[i],
                Name = names[i],
                Phone = phones[i],
                Email = mails[i],
                JobType = type,
                isActive = isActive,
                distanceType = distanceType,
                MaxDistance = distance,
                Password = password,
                Address = addresses[i],
                Latitude = latitudes[i],
                Longitude = longitudes[i]
            });
        }
    }
    private static void createAssignment()
    {
        for (int i = 0; i < 50; i++)
        {
            int id = s_dal!.Config!.NextAssignmentId;
            var callList = s_dal!.Call!.ReadAll().ToList();
            int myCallId = callList[s_rand.Next(callList.Count)].Id;

            var assignmentList = s_dal!.Assignment!.ReadAll();
            if (assignmentList.Any(c => myCallId == c.CallId))
            {
                continue;
            }
            else
            {
                DateTime callStartTime = callList.First(c => c.Id == myCallId).CallTime;
                DateTime? maxEndTime = callList.First(c => c.Id == myCallId).MaxTime;
                var volunteerList = s_dal.Volunteer!.ReadAll().ToList();
                int volunteerId = volunteerList[s_rand.Next(volunteerList.Count)].Id;
                DateTime time;
                typeOfEndTreatment? typeOfEnd = null;
                DateTime? endTime = null;
                double endOffset;
                if (maxEndTime == null)
                {
                    time = callStartTime.AddMinutes(s_rand.Next(10, 60));
                    endOffset = s_rand.Next(10, 600);
                }
                else
                {
                    double startOffset = s_rand.NextDouble() * (maxEndTime!.Value).Subtract(callStartTime).TotalMinutes;
                    time = callStartTime.AddMinutes(startOffset);
                    endOffset= s_rand.Next(10, (int)(maxEndTime.Value.Subtract(time).TotalMinutes));

                }
                if (s_rand.NextDouble() < 0.5)
                {
                    endTime = time.AddMinutes(endOffset);
                    if (endTime <= maxEndTime)
                    {
                        typeOfEnd = typeOfEndTreatment.treated;
                    }
                    else
                    {
                        typeOfEnd = s_rand.Next(2) == 0 ? typeOfEndTreatment.selfCancellation : typeOfEndTreatment.directorCancellation;
                    }

                }
                else if (s_dal.Config!.Clock > maxEndTime)
                {
                    typeOfEnd = typeOfEndTreatment.Expired;
                }
                s_dal.Assignment!.Create(new Assignment { CallId = myCallId, VolunteerId = volunteerId, StartTreatment = time, endTreatment = endTime, typeOfEnd = typeOfEnd });
            }
        }
    }
    public static void Do(IDal dal)
    {
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values");
        s_dal.ResetDB();
        Console.WriteLine("Initializing call list");
        createCall();
        Console.WriteLine("Initializing volunteer list");
        createVolunteer();
        Console.WriteLine("Initializing assignment list");
        createAssignment();

    }
}
