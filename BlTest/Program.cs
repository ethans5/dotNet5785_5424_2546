
using BO;
using DalTest;
using System;
using System.Globalization;

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    CultureInfo culture = CultureInfo.InvariantCulture;

    static void Main(string[] args)
    {
        try
        {

            int menu;
            do
            {
                Console.WriteLine
              ("Which menu do you want to access ? :  \n" +
               " 0 - Exit the menu \n" +
               " 1 - Volunteers.\n" +
               " 2 - Calls. \n" +
               " 3 - Admin. \n");
                menu = Convert.ToInt32(Console.ReadLine());
                // Switch case to choose which menu to access
                switch (menu)
                {
                    case 0: return;

                    case 1:
                        volunteersOptions();
                        break;

                    case 2:
                        callsOptions();
                        break;

                    case 3:
                        adminOptions();
                        break;

                    case 4:
                        Initialization.Do();
                        break;


                    default:
                        {
                            Console.WriteLine(" the input is incorrect please try again ");
                            Convert.ToInt32(Console.ReadLine());
                            break;
                        }
                }
            } while (menu != 0);
        }



        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred:{ex}");
        }

    }

    private static void adminOptions()
    {
        Console.WriteLine(
            "0. Exit\n" +
            "1. Get Risk Range\n" +
            "2. Get System Clock\n" +
            "3. Initialize Data\n" +
            "4. Reset Data\n" +
            "5. Update Clock\n" +
            "6. Update Risk Range\n"
            );
        int choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
            case 0:
                return;
            case 1:
                Console.WriteLine(s_bl.Admin.GetRiskRange());
                break;
            case 2:
                Console.WriteLine(s_bl.Admin.GetSystemeClock());
                break;
            case 3:
                s_bl.Admin.InitializaData();
                Console.WriteLine("Data initialized successfully");
                break;
            case 4:
                s_bl.Admin.ResetData();
                Console.WriteLine("Data reset successfully");
                break;
            case 5:
                Console.WriteLine("Enter the unit of time:\n" +
                    "0. Minutes\n" +
                    "1. Hours\n" +
                    "2. Days\n" +
                    "3. Months\n" +
                    "4. Years");
                int time = Convert.ToInt32(Console.ReadLine());
                s_bl.Admin.UpdateClock((BO.UnitTime)time);
                Console.WriteLine("Clock updated successfully");
                break;
            case 6:
                Console.WriteLine("Enter the risk range:");
                TimeSpan range = TimeSpan.Parse(Console.ReadLine()!);
                s_bl.Admin.UpdateRiskRange(range);
                Console.WriteLine("Risk range updated successfully");
                break;
            default:
                Console.WriteLine("Invalid input");
                break;


        }

    }

    private static void callsOptions()
    {
        Console.WriteLine(
        "0. Exit\n" +
        "1. Create Call\n" +
        "2. Read a Specific Calls\n" +
        "3. Read All Call\n" +
        "4. Get Call Counts by Status\n" +
        "5. Update Call\n" +
        "6. Delete Call\n" +
        "7. Read All Ended Calls\n" +
        "8. Read All Open Calls\n" +
        "9. Update Call End\n" +
        "10. Update Call Cancel\n" +
        "11. Choice Call");
        int choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
            case 0:
                return;
            case 1:
                CreateCall();
                break;
            case 2:
                Console.WriteLine("Enter Call ID:");
                int id = Convert.ToInt32(Console.ReadLine());
                var call = s_bl.Call.ReadCall(id);
                PrintCall(call);
                break;
            case 3:
                ReadAllCall();
                break;
            case 4:
                var statut = s_bl.Call.GetCallCountsByStatus();
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Status {i}: {statut[i]}");
                }
                break;
            case 5:
                UpdateCall();
                break;
            case 6:
                DeleteCall();
                break;
            case 7:
                ReadAllEndedCalls();
                break;
            case 8:
                ReadAllOpenCalls();
                break;
            case 9:
                UpdateCallEnd();
                break;
            case 10:
                UpdateCallCancel();
                break;
            case 11:
                ChoiceCall();
                break;
            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }
    private static void PrintCall(Call call)
    {
        Console.WriteLine($"ID: {call.Id}");
        Console.WriteLine($"Call Type: {call.CallType}");
        Console.WriteLine($"Description: {call.Description ?? "N/A"}");
        Console.WriteLine($"Latitude: {call.Latitude?.ToString() ?? "N/A"}");
        Console.WriteLine($"Longitude: {call.Longitude?.ToString() ?? "N/A"}");
        Console.WriteLine($"Created: {call.Created:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Max End Treatment: {call.MaxEndTreatment?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
        Console.WriteLine($"Status: {call.Status}");

        // Imprimer la liste de CallAssignInList si elle existe
        if (call.callAssignInLists != null && call.callAssignInLists.Any())
        {
            Console.WriteLine("Call Assignments:");
            foreach (var assignment in call.callAssignInLists)
            {
                PrintCallAssignInList(assignment, "  ");

            }
        }
        else
        {
            Console.WriteLine("No assignments available.");
        }

        Console.WriteLine();
    }
    private static void PrintCallAssignInList(CallAssignInList callAssign, string indent)
    {
        Console.WriteLine($"{indent}Volunteer ID: {callAssign.volounteerId}");
        Console.WriteLine($"{indent}Volunteer Name: {callAssign.volounteerName ?? "N/A"}");
        Console.WriteLine($"{indent} Starting Time: {callAssign.startingTime}");
        Console.WriteLine($"{indent} Ending Time: {callAssign.endingTime?.ToString() ?? "Not ended yet"}");
        Console.WriteLine($"{indent} Type of End Treatment: {callAssign.TypeOfEndTreatment?.ToString() ?? "N/A"}");
        Console.WriteLine();
    }

    private static void ReadAllCall()
    {
        int? filter1 = null;
        while (true)
        {
            Console.WriteLine("Enter filter (optional):");
            string? filter = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter1 = null;
                break;
            }
            else if (Int32.TryParse(filter, out int parsedFilter))
            {
                filter1 = parsedFilter;
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer or leave empty.");
            }
        }

        object? obj = null;
        while (true)
        {
            Console.WriteLine("Enter object (optional):");
            string? objInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(objInput))
            {
                obj = null;
                break;
            }
            else if (Int32.TryParse(objInput, out int parsedObj))
            {
                obj = parsedObj;
                break;
            }
            else
            {
                obj = objInput; // Keep it as string if not convertible to int
                break;
            }
        }

        int? sort1 = null;
        while (true)
        {
            Console.WriteLine("Enter sort (optional):");
            string? sort = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(sort))
            {
                sort1 = null;
                break;
            }
            else if (Int32.TryParse(sort, out int parsedSort))
            {
                sort1 = parsedSort;
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid integer or leave empty.");
            }
        }

        try
        {
            var allCalls = s_bl.Call.ReadAllCalls(
                (BO.CallFields?)filter1,
                obj,
                (BO.CallFields?)sort1
            );

            foreach (var call1 in allCalls)
            {
                PrintCallInList(call1);
            }
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"An error occurred while processing the data: {ex.Message}");
        }
    }
    private static void PrintCallInList(CallInList call)
    {
        Console.WriteLine($"ID: {call.Id?.ToString() ?? "N/A"}");
        Console.WriteLine($"Call ID: {call.callId}");
        Console.WriteLine($"Call Type: {call.callType}");
        Console.WriteLine($"Starting Time: {call.startingTime:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Remaining Time: {call.remainingTime?.ToString(@"hh\:mm\:ss") ?? "N/A"}");
        Console.WriteLine($"Last Volunteer Name: {call.LastVolunteerName ?? "N/A"}");
        Console.WriteLine($"Duration: {call.duration?.ToString(@"hh\:mm\:ss") ?? "N/A"}");
        Console.WriteLine($"Status: {call.Status}");
        Console.WriteLine($"Total Assignment Allocations: {call.TotalAssignmentAllocations}");
        Console.WriteLine();
    }

    private static void ChoiceCall()
    {
        Console.WriteLine("Enter your Id:");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter the ID of the call you want to choose:");
        int idC = Convert.ToInt32(Console.ReadLine());
        try
        {
            s_bl.Call.ChoiceCall(id, idC);
            Console.WriteLine("Call choosen successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to choose call: {ex.Message}");
        }
    }

    private static void UpdateCallCancel()
    {
        Console.WriteLine("Enter your ID:");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter the ID of the call :");
        int idA = Convert.ToInt32(Console.ReadLine());
        try
        {
            s_bl.Call.UpdateCallCancel(id, idA);
            Console.WriteLine("Call updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update call: {ex.Message}");
        }
    }

    private static void UpdateCallEnd()
    {
        Console.WriteLine("Enter your ID:");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter the ID of the assignement :");
        int idA = Convert.ToInt32(Console.ReadLine());
        try
        {
            s_bl.Call.UpdateCallEnd(id, idA);
            Console.WriteLine("Call updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update call: {ex.Message}");
        }
    }

    private static void ReadAllOpenCalls()
    {
        Console.WriteLine("Please enter your id");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Please enter a filter (optional):0-9 type de call");
        string? filter1 = Console.ReadLine();
        int? filter;
        if (string.IsNullOrWhiteSpace(filter1))
        {
            filter = null;
        }
        else
        {
            filter = Convert.ToInt32(filter1);
        }


        Console.WriteLine("Please enter a sort (optional): 0-6 sadé");
        string? sort1 = Console.ReadLine();
        int? sort;
        if (string.IsNullOrWhiteSpace(sort1))
        {
            sort = null;
        }
        else
        {
            sort = Convert.ToInt32(sort1);
        }


        var calls = s_bl.Call.ReadAllOpenCalls(id, (BO.callType?)filter, (BO.OpenCallFields?)sort);
        foreach (var call in calls)
        {
            PrintOpenCallInList(call);
        }
    }
    private static void PrintOpenCallInList(OpenCallInList call)
    {
        Console.WriteLine($"ID: {call.Id}");
        Console.WriteLine($"Call Type: {call.callType}");
        Console.WriteLine($"Description: {call.description ?? "N/A"}");
        Console.WriteLine($"Address: {call.Address}");
        Console.WriteLine($"Created: {call.Created:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Max End Treatment: {call.MaxEndTreatment?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
        Console.WriteLine($"Distance: {call.Distance:F2} km");
        Console.WriteLine();
    }

    private static void ReadAllEndedCalls()
    {
        Console.WriteLine("Please enter your id");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Please enter a filter (optional):");
        int filter;
        while (true)
        {
            string? filter1 = Console.ReadLine();
            if (Int32.TryParse(filter1, out filter))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.\n");
            }
        }
        Console.WriteLine("Please enter a sort (optional):");
        int sort;
        while (true)
        {
            string? sort1 = Console.ReadLine();
            if (Int32.TryParse(sort1, out sort))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.\n");
            }
        }
        var calls = s_bl.Call.ReadAllEndedCalls(id, (BO.callType)filter, (BO.closedCallFields)sort);
        foreach (var call in calls)
        {
            PrintClosedCallInList(call);
        }

    }
    private static void PrintClosedCallInList(ClosedCallInList call)
    {
        Console.WriteLine($"ID: {call.Id}");
        Console.WriteLine($"Call Type: {call.CallType}");
        Console.WriteLine($"Address: {call.Address}");
        Console.WriteLine($"Created: {call.Created:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Start Treatment: {call.StartTreatment:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"End Treatment: {call.EndTreatment?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
        Console.WriteLine($"Type of End Treatment: {call.TypeOfEndTreatment?.ToString() ?? "N/A"}");
        Console.WriteLine();
    }

    private static void DeleteCall()
    {
        Console.WriteLine("Enter the ID of the call you want to delete:");
        int id = Convert.ToInt32(Console.ReadLine());
        try
        {
            s_bl.Call.DeleteCall(id);
            Console.WriteLine("Call deleted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete call: {ex.Message}");
        }
    }

    private static void UpdateCall()
    {
        Console.WriteLine("Enter the ID of the call you want to update:");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter the call type:\n" +
            "0.Buying Food\n" +
            "1.Buying Medicine\n" +
            "2.Buying Clothes\n" +
            "3.Buying Cartoons\n" +
            "4.Packing Food\n" +
            "5.Packing Medicine\n" +
            "6.Packing Clothes\n" +
            "7.Packing Cartoons in the trucks\n" +
            "8.Deliveries\n" +
            "9.Delivries To The Door\n"
            );
        int type;
        while (true)
        {
            string callType = Console.ReadLine()!;
            if (Int32.TryParse(callType, out type) && (type >= 0 && type <= 9))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 9.\n");
            }
        }
        Console.WriteLine("please enter a descrpition");
        string description = Console.ReadLine()!;
        Console.WriteLine("Please enter a latitude");
        double latitude = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Please enter a longitude");
        double longitude = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Please enter a max end treatment");
        DateTime maxEndTreatment = Convert.ToDateTime(Console.ReadLine());
        var call = s_bl.Call.ReadCall(id);

        try
        {
            s_bl.Call.UpdateCall(new BO.Call
            {
                Id = call.Id,
                CallType = (BO.callType)type,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                Created = call.Created,
                MaxEndTreatment = maxEndTreatment,
                Status = call.Status
            });
            Console.WriteLine("Call updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update call: {ex.Message}");
        }
    }

    private static void CreateCall()
    {
        Console.WriteLine("Enter the call type:\n" +
            "0.Buying Food\n" +
            "1.Buying Medicine\n" +
            "2.Buying Clothes\n" +
            "3.Buying Cartoons\n" +
            "4.Packing Food\n" +
            "5.Packing Medicine\n" +
            "6.Packing Clothes\n" +
            "7.Packing Cartoons in the trucks\n" +
            "8.Deliveries\n" +
            "9.Delivries To The Door\n"
            );
        int type;
        while (true)
        {
            string callType = Console.ReadLine()!;
            if (Int32.TryParse(callType, out type) && (type >= 0 && type <= 9))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 9.\n");
            }
        }
        Console.WriteLine("please enter a descrpition");
        string description = Console.ReadLine()!;
        Console.WriteLine("Please enter a latitude");
        double latitude = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Please enter a longitude");
        double longitude = Convert.ToDouble(Console.ReadLine());
        Console.WriteLine("Please enter a max end treatment");
        DateTime maxEndTreatment = Convert.ToDateTime(Console.ReadLine());

        try
        {
            s_bl.Call.CreateCall(new BO.Call
            {
                CallType = (BO.callType)type,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                Created = s_bl.Admin.GetSystemeClock(),
                MaxEndTreatment = maxEndTreatment,
                Status = Status.Open
            });
            Console.WriteLine("Call created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create call: {ex.Message}");
        }
    }

    private static void volunteersOptions()
    {
        Console.WriteLine(
            "0. Exit\n" +
            "1. Log In \n" +
            "2. Create Volunteer\n" +
            "3. Read All Volunteer\n" +
            "4. Read a specific Volunteer\n" +
            "5. Update Volunteer\n" +
            "6. Delete Volunteer");
        int choice = Convert.ToInt32(Console.ReadLine());
        switch (choice)
        {
            case 1:
                Console.WriteLine("Enter your ID:");
                string id = Console.ReadLine()!;
                Int32.TryParse(id, out int myId);
                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine()!;
                try
                {
                    s_bl.Volunteer.LogIn(myId, password);
                    Console.WriteLine("\nLogin successful\n");
                    Console.WriteLine($"your job position is : {s_bl.Volunteer.ReadVolunteer(myId).Job}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Login failed: {ex.Message}");
                }
                break;

            case 2:
                CreateVolunteer();
                break;

            case 3:
                ReadAllVolunteer();
                break;

            case 4:
                readVolunteer();
                break;
            case 5:
                updateVolunteer();
                break;
            case 6:
                DeleteVolunteer();
                break;
            default:
                Console.WriteLine("Invalid input");
                break;
        }
    }

    private static void CreateVolunteer()
    {
        Console.Write("Please enter your ID :\t");
        string id = Console.ReadLine()!;
        Int32.TryParse(id, out int myId);

        Console.Write("Please enter your name :\t");
        string name = Console.ReadLine()!;

        Console.Write("Please enter your phone number :\t");
        string phone = Console.ReadLine()!;

        Console.Write("Please enter your mail :\t");
        string mail = Console.ReadLine()!;

        Console.Write("Please enter your password :\t");
        string password = Console.ReadLine()!;

        Console.Write("Please enter your Address :\t");
        string address = Console.ReadLine()!;

        int myJob;
        while (true)
        {
            Console.WriteLine("Please enter your job type:\n" +
                "0. Volunteer\n" +
                "1. Director");

            string job = Console.ReadLine()!;
            if (Int32.TryParse(job, out myJob) && (myJob == 0 || myJob == 1))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 0 or 1.\n");
            }
        }
        string isActive;
        bool myIsActive;
        do
        {
            Console.WriteLine("Are you currently Active ?\n" +
            "0. No\n" +
            "1. Yes");
            isActive = Console.ReadLine()!;
            myIsActive = isActive == "1";
        } while (isActive != "0" && isActive != "1");

        Console.Write("Please enter the maximum distance you can travel :\t");
        string distance = Console.ReadLine()!;
        Int32.TryParse(distance, out int myDistance);

        string distanceType;
        int myDistanceType;
        do
        {
            Console.WriteLine("Please enter the distance type :\n" +
                "0. Aerial\n" +
                "1. Walking\n" +
                "2. Driving");
            distanceType = Console.ReadLine()!;
            Int32.TryParse(distanceType, out myDistanceType);
        } while (distanceType != "0" && distanceType != "1" && distanceType != "2");



        try
        {
            s_bl.Volunteer.CreateVolunteer(new BO.Volunteer
            {
                Id = myId,
                Name = name,
                Phone = phone,
                Mail = mail,
                Password = password,
                Address = address,
                Job = (BO.jobType)myJob,
                IsActive = myIsActive,
                MaxDistance = myDistance,
                DistanceType = (BO.distanceType)myDistanceType,
                CallInProgress = null
            });
            Console.WriteLine("\nVolunteer created successfully\n");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create volunteer: {ex.Message}");
        }
    }

    private static void ReadAllVolunteer()
    {
        string isActive;
        bool? myIsActive;
        do
        {
            Console.WriteLine("Do you want to see :\n" +
            "0. All\n" +
            "1. Only active volunteers\n" +
            "2. Only inactive volunteers");

            isActive = Console.ReadLine()!;
            myIsActive = isActive switch
            {
                "0" => null,
                "1" => true,
                "2" => false,
                _ => null
            };
        } while (isActive != "0" && isActive != "1" && isActive != "2");

        string sort;
        int mySort;
        do
        {
            Console.WriteLine("Do you want to sort the volunteers?\n" +
            "0. No\n" +
            "1. Yes");

            sort = Console.ReadLine()!;
        } while (!Int32.TryParse(sort, out mySort) || (mySort != 0 && mySort != 1));

        int? mySortField = null;

        if (mySort == 1)
        {
            string sortField;
            do
            {
                Console.WriteLine("Please enter the field you want to sort by:\n" +
                "0. Name\n" +
                "1. TotalTreated\n" +
                "2. TotalSelfCancellation\n" +
                "3. TotalExpired\n" +
                "4. CallType");

                sortField = Console.ReadLine()!;
            } while (!Int32.TryParse(sortField, out var field) || (field < 0 || field > 4));

            mySortField = int.Parse(sortField); // Conversion sûre
        }

        try
        {
            var volunteers = s_bl.Volunteer.ReadAllVolunteers(myIsActive,
                mySortField.HasValue ? (BO.VolunteerSortField)mySortField.Value : null);

            foreach (var volunteer in volunteers)
            {
                PrintVolunteerInList(volunteer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read volunteers: {ex.Message}");
        }
    }
    private static void readVolunteer()
    {
        Console.WriteLine("Enter the ID of the volunteer you want to read:");
        string id = Console.ReadLine()!;
        int.TryParse(id, out int myId);
        try
        {

            var volunteer = s_bl.Volunteer.ReadVolunteer(myId);

            PrintVolunteer(volunteer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read volunteer: {ex.Message}");
        }
    }

    private static void updateVolunteer()
    {
        Console.Write("Please enter your ID :\t");
        string id = Console.ReadLine()!;
        int.TryParse(id, out int myId);

        Console.Write("Please enter the ID of the volunteer you want to update :\t");
        string idToUpdate = Console.ReadLine()!;
        int.TryParse(idToUpdate, out int myIdToUpdate);

        if (myId != myIdToUpdate && s_bl.Volunteer.ReadVolunteer(myId).Job != BO.jobType.Director)
        {
            Console.WriteLine("You can only update your own volunteer details.");
            return;
        }

        Console.Write("Please enter your name :\t");
        string name = Console.ReadLine()!;

        Console.Write("Please enter your phone number :\t");
        string phone = Console.ReadLine()!;

        Console.Write("Please enter your mail :\t");
        string mail = Console.ReadLine()!;

        Console.Write("Please enter your password :\t");
        string password = Console.ReadLine()!;

        Console.Write("Please enter your Address :\t");
        string address = Console.ReadLine()!;

        int myJob;
        while (true)
        {
            Console.WriteLine("Please enter your job type:\n" +
                "0. Volunteer\n" +
                "1. Director");

            string job = Console.ReadLine()!;
            if (Int32.TryParse(job, out myJob) && (myJob == 0 || myJob == 1))
            {
                break; // L'entrée est correcte, on sort de la boucle
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 0 or 1.\n");
            }
        }
        string isActive;
        bool myIsActive;
        do
        {
            Console.WriteLine("Are you currently Active ?\n" +
            "0. No\n" +
            "1. Yes");
            isActive = Console.ReadLine()!;
            myIsActive = isActive == "1";
        } while (isActive != "0" && isActive != "1");

        Console.Write("Please enter the maximum distance you can travel :\t");
        string distance = Console.ReadLine()!;
        Int32.TryParse(distance, out int myDistance);

        string distanceType;
        int myDistanceType;
        do
        {
            Console.WriteLine("Please enter the distance type :\n" +
                "0. Aerial\n" +
                "1. Walking\n" +
                "2. Driving");
            distanceType = Console.ReadLine()!;
            Int32.TryParse(distanceType, out myDistanceType);
        } while (distanceType != "0" && distanceType != "1" && distanceType != "2");

        try
        {
            s_bl.Volunteer.UpdateVolunteer(myId, new BO.Volunteer
            {
                Id = myIdToUpdate,
                Name = name,
                Phone = phone,
                Mail = mail,
                Password = password,
                Address = address,
                Job = (BO.jobType)myJob,
                IsActive = myIsActive,
                MaxDistance = myDistance,
                DistanceType = (BO.distanceType)myDistanceType
            });
            Console.WriteLine("\nVolunteer updated successfully\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update volunteer: {ex.Message}");
        }

    }
    private static void DeleteVolunteer()
    {
        Console.WriteLine("Enter the ID of the volunteer you want to delete:");
        string id = Console.ReadLine()!;
        int.TryParse(id, out int myId);
        try
        {
            s_bl.Volunteer.DeleteVolunteer(myId);
            Console.WriteLine("\nVolunteer deleted successfully\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete volunteer: {ex.Message}");
        }
    }
    private static void PrintVolunteerInList(VolunteerInList volunteer)
    {
        Console.WriteLine("Volunteer Details:");
        Console.WriteLine($"ID: {volunteer.Id}");
        Console.WriteLine($"Name: {volunteer.Name}");
        Console.WriteLine($"Is Active: {volunteer.IsActive}");
        Console.WriteLine($"Total Treated Cases: {volunteer.Totaltreated}");
        Console.WriteLine($"Total Self-Cancellations: {volunteer.TotalSelfCancellation}");
        Console.WriteLine($"Total Expired Cases: {volunteer.TotalExpired}");
        Console.WriteLine($"Call ID: {(volunteer.IdCall.HasValue ? volunteer.IdCall.ToString() : "N/A")}");
        Console.WriteLine($"Call Type: {volunteer.callType}");
        Console.WriteLine();
    }
    private static void PrintVolunteer(Volunteer myVolunteer)
    {
        Console.WriteLine($"ID: {myVolunteer.Id}");
        Console.WriteLine($"Name: {myVolunteer.Name}");
        Console.WriteLine($"Phone: {myVolunteer.Phone}");
        Console.WriteLine($"Mail: {myVolunteer.Mail}");
        //Console.WriteLine($"Password: {myVolunteer.Password ?? "N/A"}");
        Console.WriteLine($"Address: {myVolunteer.Address ?? "N/A"}");
        Console.WriteLine($"Latitude: {myVolunteer.Latitude?.ToString() ?? "N/A"}");
        Console.WriteLine($"Longitude: {myVolunteer.Longitude?.ToString() ?? "N/A"}");
        Console.WriteLine($"Job Type: {myVolunteer.Job}");
        Console.WriteLine($"Is Active: {myVolunteer.IsActive}");
        Console.WriteLine($"Max Distance: {myVolunteer.MaxDistance?.ToString() ?? "N/A"}");
        Console.WriteLine($"Type of Distance: {myVolunteer.DistanceType}");
        Console.WriteLine($"Total Treated: {myVolunteer.Totaltreated}");
        Console.WriteLine($"Total Self-Cancellation: {myVolunteer.TotalSelfCancellation}");
        Console.WriteLine($"Total Expired: {myVolunteer.TotalExpired}");
        Console.WriteLine("Call in progress:");
        if (myVolunteer.CallInProgress != null)
        {

            PrintCallInProgress(myVolunteer.CallInProgress, "    ");
        }
        else
        {
            Console.WriteLine("No call in progress.");
        }

        Console.WriteLine();
    }
    public static void PrintCallInProgress(CallInProgress callInProgress, string indent)
    {

        Console.WriteLine($"{indent}CallId: {callInProgress.CallId}");
        Console.WriteLine($"{indent}CallType: {callInProgress.CallType}");
        Console.WriteLine($"{indent}Description: {callInProgress.Description ?? "No description"}");
        Console.WriteLine($"{indent}Address: {callInProgress.Address}");
        Console.WriteLine($"{indent}Created: {callInProgress.Created}");
        Console.WriteLine($"{indent}MaxEndTreatment: {callInProgress.MaxEndTreatment?.ToString() ?? "Not available"}");
        Console.WriteLine($"{indent}StartTreatment: {callInProgress.StartTreatment}");
        Console.WriteLine($"{indent}Distance: {callInProgress.Distance} km");
        Console.WriteLine($"{indent}Treatment: {callInProgress.Treatment}");
    }

}