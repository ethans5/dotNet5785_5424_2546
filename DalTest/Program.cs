using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;

namespace DalTest
{
    internal class Program
    {
        //private static ICall? s_dalCall = new CallImplementation();
        //private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        //private static IConfig? s_dalConfig = new ConfigImplementation();
        static readonly IDal s_dal = new DalList();
        // fucntion to print the menu of each type of object
        private static void printMenu(string typeOfMenu)
        {
            Console.WriteLine
                (typeOfMenu + " menu : \n" +
                 " 0 - Back to the main menu. \n" +
                 " 1 - Create a new " + typeOfMenu + ". \n" +
                 " 2 - Read a specific " + typeOfMenu + " info.\n" +
                 " 3 - Read all the " + typeOfMenu + " info. \n" +
                 " 4 - Update one of the " + typeOfMenu + "\n" +
                 " 5 - Delete one of the " + typeOfMenu + ". \n" +
                 " 6 - Delete all the " + typeOfMenu + ". \n"
                );

        }

/***************************************************************** Volunteer Functions **********************************************************************************/

        private static void printVolunteer(Volunteer myVolunteer)
        {
            Console.WriteLine("ID : " + myVolunteer.Id);
            Console.WriteLine("Name : " + myVolunteer.Name);
            Console.WriteLine("Phone : " + myVolunteer.Phone);
            Console.WriteLine("Email : " + myVolunteer.Email);
            Console.WriteLine("Password : " + myVolunteer.Password);
            Console.WriteLine("Job Type : " + myVolunteer.JobType);
            Console.WriteLine("Is Active : " + myVolunteer.isActive);
            Console.WriteLine("Address : " + myVolunteer.Address);
            Console.WriteLine("Longitude : " + myVolunteer.Longitude);
            Console.WriteLine("Latitude : " + myVolunteer.Latitude);
            Console.WriteLine("Max Distance : " + myVolunteer.MaxDistance);
            Console.WriteLine("Type of Distance : " + myVolunteer.distanceType);

            Console.WriteLine(" ");


        }

        private static void createVolunteer()
        {
            Console.Write("Please enter your Id (digits only): ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Please enter your full Name : ");
            string name = Console.ReadLine()!;

            Console.Write("Please enter your phone number : ");
            string phone = Console.ReadLine()!;

            Console.Write("Please enter your email : ");
            string email = Console.ReadLine()!;

            Console.Write("Please enter your address : ");
            string address = Console.ReadLine()!;

            Console.WriteLine("Please enter the longitude and the latitude of your address :");
            Console.Write("Longitude : ");
            string temp1 = Console.ReadLine()!;
            double longitude = double.Parse(temp1);   
            Console.Write("Latitude : ");
            string temp2 = Console.ReadLine()!;
            double latitude = double.Parse(temp2);

            Console.WriteLine("Please enter the maximal distance you could volunteer : ");
            int distance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("which type of distance ?");
            Console.WriteLine("1 - Aerial");
            Console.WriteLine("2 - Walking");
            Console.WriteLine("3 - Driving");
            int option = 0;
            while (option < 1 || option > 3)
            {
                option = Convert.ToInt32(Console.ReadLine());
            }
            distanceType distanceType = (distanceType)option;

            s_dal.Volunteer.Create(new Volunteer
            {
                Id = id,
                Name = name,
                Phone = phone,
                Email = email,
                JobType = jobType.Volunteer,
                isActive = true,
                distanceType = distanceType,
                Address = address,
                Longitude = longitude,
                Latitude = latitude,
                MaxDistance = distance
            });

        }

        private static void readVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to read : ");
            int id = Convert.ToInt32(Console.ReadLine());
            Volunteer? volunteer = s_dal.Volunteer.Read(id);
            printVolunteer(volunteer!);
        }

        private static void readAllVolunteer()
        {
            IEnumerable<Volunteer> volunteers = s_dal.Volunteer.ReadAll();
            if(volunteers != null && volunteers.Any())
            {
                foreach (Volunteer volunteer in volunteers)
                {
                    printVolunteer(volunteer);
                }
            }
            else
                Console.WriteLine("\nThere are no Volunteers\n");
          

        }

        private static void updateVolunteer()
        {
            Console.Write("Please enter the id of the volunteer you want to update :\t");
            int id = Convert.ToInt32(Console.ReadLine());
            Volunteer? volunteer = s_dal.Volunteer.Read(id);
            printVolunteer(volunteer!);

            Console.Write("Please enter the new name :\t");
            string name = Console.ReadLine()!;
            if (string.IsNullOrEmpty(name))
            {
                name = volunteer!.Name;
            }
            Console.Write("Please enter the new phone number :\t");
            string phone = Console.ReadLine()!;
            if (string.IsNullOrEmpty(phone))
            {
                phone = volunteer!.Phone;
            }
            Console.Write("Please enter the new email :\t");
            string email = Console.ReadLine()!;
            if (string.IsNullOrEmpty(email))
            {
                email = volunteer!.Email;
            }

            Console.Write("Please enter a new Password :\t");
            string password = Console.ReadLine()!;
            if (string.IsNullOrEmpty(password))
            {
                password = volunteer!.Password!;
            }

            Console.Write("Please enter the new address :\t");
            string address = Console.ReadLine()!;
            if (string.IsNullOrEmpty(address))
            {
                address = volunteer!.Address!;
            }
            Console.Write("Please enter the new longitude :\t");
            string longitudeInput = Console.ReadLine()!;
            double? longitude = !string.IsNullOrEmpty(longitudeInput) ? Convert.ToDouble(longitudeInput) : volunteer!.Longitude;

            Console.Write("Please enter the new latitude :\t");
            string latitudeInput = Console.ReadLine()!;
            double? latitude = !string.IsNullOrEmpty(latitudeInput) ? Convert.ToDouble(latitudeInput) : volunteer!.Latitude;

            
            Console.Write("Please enter the new maximal distance :\t");
            string distanceInput = Console.ReadLine()!;
            double distance = !string.IsNullOrEmpty(distanceInput) ? Convert.ToInt32(distanceInput) : volunteer!.MaxDistance!.Value;

            Console.Write("Please enter the new distance type :\n");
            Console.WriteLine("1 - Aerial ");
            Console.WriteLine("2 - Walking ");
            Console.WriteLine("3 - Driving ");

            string? distanceTypeInput = Console.ReadLine();
            distanceType distanceType;

            if(string.IsNullOrEmpty(distanceTypeInput))
            {
                distanceType = volunteer!.distanceType ;
            }
            else
            {
                int option = Convert.ToInt32(distanceTypeInput);

                while (option < 1 || option > 3)
                {
                    Console.WriteLine("please enter a digit between 1 and 3 :");
                    option = Convert.ToInt32(Console.ReadLine());
                }

                distanceType = (distanceType)option;
            }
          
          

            DO.Volunteer myVolunteer = new Volunteer(id, name, phone, email, jobType.Volunteer, true, distanceType, distance,password, address, longitude, latitude);

            s_dal.Volunteer.Update(myVolunteer);

            Console.WriteLine(" ");
            printVolunteer(myVolunteer);
        }

        private static void deleteVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to delete : ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dal.Volunteer.Delete(id);
        }

        private static void volunteersOptions()
        {
            int option;

            do
            {
                printMenu("Volunteer");
                option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 0: break;
                    case 1:
                        createVolunteer();
                        break;

                    case 2:
                        readVolunteer();
                        break;

                    case 3:
                        readAllVolunteer();
                        break;

                    case 4:
                        updateVolunteer();
                        break;

                    case 5:
                        deleteVolunteer();
                        break;

                    case 6:
                        s_dal.Volunteer.DeleteAll();
                        break;

                    default:
                        Console.WriteLine("Please enter a digit between 0-6 : ");
                        option = Convert.ToInt32(Console.ReadLine());
                        break;

                }
            } while (option != 0);


        }


/********************************************************************* Call Functions ***********************************************************************************/
        private static void printCall(Call myCall)
        {
            Console.WriteLine("ID: " + myCall.Id);
            Console.WriteLine("Call Type: " + myCall.CallType);
            Console.WriteLine("Address: " + myCall.Address);
            Console.WriteLine("Latitude: " + myCall.Latitude);
            Console.WriteLine("Longitude: " + myCall.Longitude);
            Console.WriteLine("Call Time: " + myCall.CallTime);
            Console.WriteLine("Description: " + (myCall.Description ?? "No Description"));
            Console.WriteLine("Max Time: " + (myCall.MaxTime?.ToString() ?? "No Max Time"));
            Console.WriteLine(" ");
        }

        private static void createCall()
        {
        
            Console.WriteLine("Please enter the type of call: ");
            int callTypeOption = 0;
            while (callTypeOption < 1 || callTypeOption > 3)
            {
                callTypeOption = Convert.ToInt32(Console.ReadLine());
            }
            callType callType = (callType)callTypeOption;

            Console.Write("Please enter the call address: ");
            string address = Console.ReadLine()!;

            Console.WriteLine("Please enter the latitude and longitude of the call location: ");
            Console.Write("Longitude: ");
            double longitude = Convert.ToDouble(Console.ReadLine());
            Console.Write("Latitude: ");
            double latitude = Convert.ToDouble(Console.ReadLine());

            Console.Write("Please enter the call time (yyyy-MM-dd HH:mm:ss): ");
            DateTime callTime = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Please enter a description : ");
            string? description = Console.ReadLine();

            Console.Write("Please enter the maximum time for the call (yyyy-MM-dd HH:mm:ss): ");
            DateTime? maxTime = DateTime.Parse(Console.ReadLine()!);

            s_dal.Call.Create(new Call
            {
                Id = s_dal.Config.NextCallId,
                CallType = callType,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                CallTime = callTime,
                Description = description,
                MaxTime = maxTime
            });
        }

        private static void readCall()
        {
            Console.WriteLine("Please enter the id of the call you want to read: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Call? call = s_dal.Call.Read(id);
            printCall(call!);
        }

        private static void readAllCalls()
        {
            IEnumerable<Call> calls = s_dal.Call.ReadAll();
            if(calls != null && calls.Any())
            {
                foreach (Call call in calls)
                {
                    printCall(call);
                }
            }
            else
                Console.WriteLine("\nThere are no Calls\n");
         
        }

        private static void updateCall()
        {
            Console.WriteLine("Please enter the id of the call you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Call? call = s_dal.Call.Read(id);
            printCall(call!);

            Console.WriteLine("Please enter the new address (or press enter to keep the current one): ");
            string address = Console.ReadLine()!;
            if (string.IsNullOrEmpty(address))
            {
                address = call!.Address;
            }

            Console.WriteLine("Please enter the new longitude (or press enter to keep the current one): ");
            double longitude = Convert.ToDouble(Console.ReadLine());
            if (longitude == 0)
            {
                longitude = call!.Longitude;
            }

            Console.WriteLine("Please enter the new latitude (or press enter to keep the current one): ");
            double latitude = Convert.ToDouble(Console.ReadLine());
            if (latitude == 0)
            {
                latitude = call!.Latitude;
            }

            Console.WriteLine("Please enter the new call time (yyyy-MM-dd HH:mm:ss) (or press enter to keep the current one): ");
            string? callTimeInput = Console.ReadLine();
            DateTime callTime = string.IsNullOrEmpty(callTimeInput) ? call!.CallTime : DateTime.Parse(callTimeInput);

            Console.WriteLine("Please enter the new description (optional): ");
            string? description = Console.ReadLine();
            if (string.IsNullOrEmpty(description))
            {
                description = call!.Description;
            }

            Console.WriteLine("Please enter the new maximum time (yyyy-MM-dd HH:mm:ss) (or press enter to keep the current one): ");
            string? maxTimeInput = Console.ReadLine();
            DateTime? maxTime = string.IsNullOrEmpty(maxTimeInput) ? call!.MaxTime : DateTime.Parse(maxTimeInput);

            DO.Call myCall = new Call(id, call!.CallType, address, latitude, longitude, callTime, description, maxTime);

            s_dal.Call.Update(myCall);

            Console.WriteLine(" ");
            printCall(myCall);
        }

        private static void deleteCall()
        {
            Console.WriteLine("Please enter the id of the call you want to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dal.Call.Delete(id);
        }

        private static void callsOptions()
        {
            int option;

            do
            {
                printMenu("Call");
                option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 0: break;
                    case 1:
                        createCall();
                        break;
                    case 2:
                        readCall();
                        break;
                    case 3:
                        readAllCalls();
                        break;
                    case 4:
                        updateCall();
                        break;
                    case 5:
                        deleteCall();
                        break;
                    case 6:
                        s_dal.Call.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-6: ");
                        break;
                }
            } while (option != 0);
        }

/********************************************************************** Assignment Functions ****************************************************************************/
        private static void printAssignment(Assignment assignment)
        {
            Console.WriteLine("ID: " + assignment.Id);
            Console.WriteLine("Call ID: " + assignment.CallId);
            Console.WriteLine("Volunteer ID: " + assignment.VolunteerId);
            Console.WriteLine("Start Treatment: " + assignment.StartTreatment);
            Console.WriteLine("End Treatment: " + (assignment.endTreatment?.ToString() ));
            Console.WriteLine("Type of End Treatment: " + (assignment.typeOfEnd?.ToString()));
            Console.WriteLine(" ");
        }

        private static void createAssignment()
        {
            Console.Write("Please enter the Call ID: ");
            int callId = Convert.ToInt32(Console.ReadLine());
            if (s_dal.Call.Read(callId) == null)
            {
                Console.WriteLine("The call ID does not exist.");
                return;
            }

            Console.Write("Please enter the Volunteer ID: ");
            int volunteerId = Convert.ToInt32(Console.ReadLine());
            if (s_dal.Volunteer.Read(volunteerId) == null)
            {
                Console.WriteLine("The volunteer ID does not exist.");
                return;
            }

            Console.Write("Please enter the start treatment time (yyyy-MM-dd HH:mm:ss): ");
            DateTime startTreatment = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Please enter the end treatment time (yyyy-MM-dd HH:mm:ss, optional): ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = null;

            // Utilisation de TryParse pour éviter les exceptions si l'entrée est vide
            if (!string.IsNullOrEmpty(endTreatmentInput) && DateTime.TryParse(endTreatmentInput, out DateTime parsedEndTreatment))
            {
                endTreatment = parsedEndTreatment;
            }

            Console.WriteLine("Please enter the type of end treatment (1 - Treated, 2 - Self Cancellation, 3 - Director Cancellation, 4 - Expired): ");
            int typeOption = 0;
            while (typeOption < 1 || typeOption > 4)
            {
                typeOption = Convert.ToInt32(Console.ReadLine());
            }
            typeOfEndTreatment typeOfEnd = (typeOfEndTreatment)typeOption;

            s_dal.Assignment.Create(new Assignment
            {
                Id = s_dal.Config.NextAssignmentId,
                CallId = callId,
                VolunteerId = volunteerId,
                StartTreatment = startTreatment,
                endTreatment = endTreatment,
                typeOfEnd = typeOfEnd
            });
        }


        private static void readAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to read: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Assignment? assignment = s_dal.Assignment.Read(id);
            printAssignment(assignment!);
        }

        private static void readAllAssignments()
        {
            IEnumerable<Assignment> assignments = s_dal.Assignment.ReadAll();
            if(assignments != null && assignments.Any())
            {
                foreach (Assignment assignment in assignments)
                {
                    printAssignment(assignment);
                }
            }
            else
                Console.WriteLine("\nThere are no Assignments \n");
         
        }

        private static void updateAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Assignment? assignment = s_dal.Assignment.Read(id);

            printAssignment(assignment!);

            // Mise à jour du Call ID
            Console.Write("Please enter the new Call ID (or press enter to keep the current one): ");
            string? callIdInput = Console.ReadLine();
            int callId = string.IsNullOrEmpty(callIdInput) ? assignment!.CallId : Convert.ToInt32(callIdInput);
            if (string.IsNullOrEmpty(callIdInput))
                Console.WriteLine($"{assignment!.CallId}");

            // Mise à jour du Volunteer ID
            Console.Write("Please enter the new Volunteer ID (or press enter to keep the current one): ");
            string? volunteerIdInput = Console.ReadLine();
            int volunteerId = string.IsNullOrEmpty(volunteerIdInput) ? assignment!.VolunteerId : Convert.ToInt32(volunteerIdInput);
            if (string.IsNullOrEmpty(volunteerIdInput))
                Console.WriteLine($"{assignment!.VolunteerId}");

            // Mise à jour du Start Treatment
            Console.Write("Please enter the new start treatment time (yyyy-MM-dd HH:mm:ss, or press enter to keep the current one): ");
            string? startTreatmentInput = Console.ReadLine();
            DateTime startTreatment = string.IsNullOrEmpty(startTreatmentInput) ? assignment!.StartTreatment : DateTime.Parse(startTreatmentInput);
            if (string.IsNullOrEmpty(startTreatmentInput))
                Console.WriteLine($"{assignment!.StartTreatment}");

            // Mise à jour du End Treatment
            Console.Write("Please enter the new end treatment time (yyyy-MM-dd HH:mm:ss, or press enter to keep the current one): ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = string.IsNullOrEmpty(endTreatmentInput) ? assignment!.endTreatment : DateTime.Parse(endTreatmentInput);
            if (string.IsNullOrEmpty(endTreatmentInput))
                Console.WriteLine($"{assignment!.endTreatment}");

            // Mise à jour du Type of End Treatment
            Console.WriteLine("Please enter the type of end treatment (1 - Treated, 2 - Self Cancellation, 3 - Director Cancellation, 4 - Expired) :");
            string? typeOptionInput = Console.ReadLine();
            typeOfEndTreatment typeOfEnd;

            if (string.IsNullOrEmpty(typeOptionInput))
            {
                Console.WriteLine($"{assignment!.typeOfEnd}");
                
                typeOfEnd = (typeOfEndTreatment)assignment.typeOfEnd!;
            }
            else
            {
                int typeOption = Convert.ToInt32(typeOptionInput);
                while (typeOption < 1 || typeOption > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 4:");
                    typeOption = Convert.ToInt32(Console.ReadLine());
                }
                typeOfEnd = (typeOfEndTreatment)typeOption;
            }

            // Création du nouvel Assignment avec les informations mises à jour
            Assignment updatedAssignment = new Assignment(id, callId, volunteerId, startTreatment, endTreatment, typeOfEnd);

            // Mise à jour de l'Assignment dans la base de données
            s_dal.Assignment.Update(updatedAssignment);

            Console.WriteLine("\nUpdated Assignment:");
            printAssignment(updatedAssignment);
        }

        private static void deleteAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dal.Assignment.Delete(id);
        }

        private static void assignmentsOptions()
        {
            int option;

            do
            {
                printMenu("Assignment");
                option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 0: break;
                    case 1:
                        createAssignment();
                        break;
                    case 2:
                        readAssignment();
                        break;
                    case 3:
                        readAllAssignments();
                        break;
                    case 4:
                        updateAssignment();
                        break;
                    case 5:
                        deleteAssignment();
                        break;
                    case 6:
                        s_dal.Assignment.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-6: ");
                        break;
                }
            } while (option != 0);
        }

/**************************************************************************** Initialization option *********************************************************************/
        private static void funcInitialization()
        {
            Initialization.Do(s_dal);
        }

/************************************************************************************* Config ***************************************************************************/

        private static void configOptions()
        {
            int option;
            do
            {
                Console.WriteLine("\n--- Configuration Sub-Menu ---");
                Console.WriteLine("0. Exit the sub-menu");
                Console.WriteLine("1. Advance the system clock by one minute");
                Console.WriteLine("2. Advance the system clock by one hour");
                Console.WriteLine("3. Advance the system clock by one day");
                Console.WriteLine("4. Advance the system clock by one month");
                Console.WriteLine("5. Advance the system clock by one year");
                Console.WriteLine("6. Display the current value of the system clock");
                Console.WriteLine("7. Set a new value for a configuration variable");
                Console.WriteLine("8. Display the current value of a configuration variable");
                Console.WriteLine("9. Reset all configuration variables to default values");
                Console.Write("Enter your choice: ");
                option = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("");
                switch (option)
                {
                    case 0:
                        break;

                    case 1:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                        break;
                    case 2:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                        break;
                    case 3:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                        break;
                    case 4:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                        break;
                    case 5:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);
                        break;
                    case 6:
                        Console.WriteLine($"The current time is {s_dal.Config.Clock}");
                        break;
                    case 7:
                        SetNewConfigValue();
                        break;

                    case 8:
                        ShowConfigValue();
                        break;

                    case 9:
                        s_dal.Config.Reset();
                        Console.WriteLine("All configuration variables have been reset to their default values.");
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-5: ");
                        break;
                }
            } while (option!=0);
        }
        private static void SetNewConfigValue()
        {
            Console.WriteLine("Which configuration variable would you like to change?");
            Console.WriteLine("1. System Clock");
            Console.WriteLine("2. Risk Range (in minutes)");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            
                switch (option)
                {
                    case 1:
                        Console.Write("Enter the new system clock value (format: YYYY-MM-DD HH:mm): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime newClock))
                        {
                            s_dal.Config.Clock = newClock;
                            Console.WriteLine($"New system clock set to: {s_dal.Config.Clock}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                        break;

                    case 2:
                        Console.Write("Enter the new Risk Range (in minutes): ");
                        if (int.TryParse(Console.ReadLine(), out int minutes))
                        {
                            s_dal.Config.RiskRange = TimeSpan.FromMinutes(minutes);
                            Console.WriteLine($"New Risk Range set to: {s_dal.Config.RiskRange}");
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid number.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
           


        }
        private static void ShowConfigValue()
        {
            Console.WriteLine("Which configuration variable would you like to display?");
            Console.WriteLine("1. System Clock");
            Console.WriteLine("2. Risk Range");
         

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            
                switch (option)
                {
                    case 1:
                        Console.WriteLine($"System Clock: {s_dal.Config.Clock}");
                        break;

                    case 2:
                        Console.WriteLine($"Risk Range: {s_dal.Config.RiskRange}");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
           

        }

/****************************************************************************** All data *******************************************************************************/

        private static void printAllData()
        {
            Console.WriteLine("Volunteers :");
            readAllVolunteer();
            Console.WriteLine("Calls :");
            readAllCalls();
            Console.WriteLine("Assignments :");
            readAllAssignments() ;
        }

        private static void deleteAllData()
        {
            s_dal.Volunteer.DeleteAll();
            s_dal.Call.DeleteAll();
            s_dal.Assignment.DeleteAll();

            Console.WriteLine("All the data has been deleted \n");
        }

/************************************************************************ MAIN CODE ************************************************************************************/
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
                   " 3 - Assignments. \n" +
                   " 4 - Initialization. \n" +
                   " 5 - See all the data. \n" +
                   " 6 - Config. \n" +
                   " 7 - Reset all data. \n");
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
                            assignmentsOptions();
                            break;
                        case 4:
                            funcInitialization();
                            break;

                        case 5:
                            printAllData();
                            break;

                        case 6:
                            configOptions();
                            break;

                        case 7:
                            deleteAllData();
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
    }
}

