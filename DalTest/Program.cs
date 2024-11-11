using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;

namespace DalTest
{
    internal class Program
    {
        private static ICall? s_dalCall = new CallImplementation();
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

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

        /* Volunteer option */

        private static void printVolunteer(Volunteer myVolunteer)
        {
            Console.WriteLine("ID :" + myVolunteer.Id);
            Console.WriteLine("Name :" + myVolunteer.Name);
            Console.WriteLine("Phone :" + myVolunteer.Phone);
            Console.WriteLine("Email :" + myVolunteer.Email);
            Console.WriteLine("Job Type :" + myVolunteer.JobType);
            Console.WriteLine("Is Active :" + myVolunteer.isActive);
            Console.WriteLine("Address :" + myVolunteer.Address);
            Console.WriteLine("Longitude :" + myVolunteer.Longitude);
            Console.WriteLine("Latitude :" + myVolunteer.Latitude);
            Console.WriteLine("Max Distance :" + myVolunteer.MaxDistance);

            Console.WriteLine(" ");


        }

        private static void createVolunteer()
        {
            Console.Write("Please enter your Id (digits only): ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Please enter your full Name : ");
            string name = Console.ReadLine()!;

            Console.WriteLine("Please enter your phone number : ");
            string phone = Console.ReadLine()!;

            Console.Write("Please enter your email : ");
            string email = Console.ReadLine()!;

            Console.Write("Please enter your address : ");
            string address = Console.ReadLine()!;

            Console.WriteLine("Please enter the longitude and the latitude of your address : \n");
            Console.WriteLine("Longitude : ");
            double longitude = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Latitude : ");
            double latitude = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Please enter the maximal distance you could volunteer : ");
            int distance = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("which type of distance ? \n");
            Console.WriteLine("1 - Aerial \n");
            Console.WriteLine("2 - Walking \n");
            Console.WriteLine("3 - Driving \n");
            int option = 0;
            while (option < 1 || option > 3)
            {
                option = Convert.ToInt32(Console.ReadLine());
            }
            distanceType distanceType = (distanceType)option;

            s_dalVolunteer!.Create(new Volunteer
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
            Volunteer? volunteer = s_dalVolunteer!.Read(id);
            printVolunteer(volunteer!);
        }

        private static void readAllVolunteer()
        {
            List<Volunteer> volunteers = s_dalVolunteer!.ReadAll();
            foreach (Volunteer volunteer in volunteers)
            {
                printVolunteer(volunteer);
            }
        }

        private static void updateVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to update : ");
            int id = Convert.ToInt32(Console.ReadLine());
            Volunteer? volunteer = s_dalVolunteer!.Read(id);
            printVolunteer(volunteer!);

            Console.WriteLine("Please enter the new name : ");
            string name = Console.ReadLine()!;
            if (string.IsNullOrEmpty(name))
            {
                name = volunteer!.Name;
            }
            Console.WriteLine("Please enter the new phone number : ");
            string phone = Console.ReadLine()!;
            if (string.IsNullOrEmpty(phone))
            {
                phone = volunteer!.Phone;
            }
            Console.WriteLine("Please enter the new email : ");
            string email = Console.ReadLine()!;
            if (string.IsNullOrEmpty(email))
            {
                email = volunteer!.Email;
            }
            Console.WriteLine("Please enter the new address : ");
            string address = Console.ReadLine()!;
            if (string.IsNullOrEmpty(address))
            {
                address = volunteer!.Address!;
            }
            Console.WriteLine("Please enter the new longitude : ");
            string longitudeInput = Console.ReadLine()!;
            double? longitude = !string.IsNullOrEmpty(longitudeInput) ? Convert.ToDouble(longitudeInput) : volunteer!.Longitude;

            Console.WriteLine("Please enter the new latitude : ");
            string latitudeInput = Console.ReadLine()!;
            double? latitude = !string.IsNullOrEmpty(latitudeInput) ? Convert.ToDouble(latitudeInput) : volunteer!.Latitude;

            
            Console.WriteLine("Please enter the new maximal distance : ");
            string distanceInput = Console.ReadLine()!;
            double distance = !string.IsNullOrEmpty(distanceInput) ? Convert.ToInt32(distanceInput) : volunteer!.MaxDistance!.Value;

            Console.WriteLine("Please enter the new distance type : ");
            Console.WriteLine("1 - Aerial \n");
            Console.WriteLine("2 - Walking \n");
            Console.WriteLine("3 - Driving \n");
            int option = 0;
            while (option < 1 || option > 3)
            {
                option = Convert.ToInt32(Console.ReadLine());
            }
            distanceType distanceType = (distanceType)option;

            s_dalVolunteer!.Update(new Volunteer { Id = id, Name = name, Phone = phone, Email = email, JobType = jobType.Volunteer, isActive = true, distanceType = distanceType, Address = address, Longitude = longitude, Latitude = latitude, MaxDistance = distance });
        }

        private static void deleteVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to delete : ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dalVolunteer!.Delete(id);
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
                        s_dalVolunteer!.DeleteAll();
                        break;

                    default:
                        Console.WriteLine("Please enter a digit between 0-6 : ");
                        option = Convert.ToInt32(Console.ReadLine());
                        break;

                }
            } while (option != 0);


        }
        /* Call option */
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

            s_dalCall!.Create(new Call
            {
                Id = s_dalConfig!.NextCallId,
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
            Call? call = s_dalCall!.Read(id);
            printCall(call!);
        }

        private static void readAllCalls()
        {
            List<Call> calls = s_dalCall!.ReadAll();
            foreach (Call call in calls)
            {
                printCall(call);
            }
        }

        private static void updateCall()
        {
            Console.WriteLine("Please enter the id of the call you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Call? call = s_dalCall!.Read(id);
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

            s_dalCall!.Update(new Call
            {
                Id = id,
                CallType = call!.CallType,
                Address = address,
                Latitude = latitude!,
                Longitude = longitude!,
                CallTime = callTime,
                Description = description,
                MaxTime = maxTime
            });
        }

        private static void deleteCall()
        {
            Console.WriteLine("Please enter the id of the call you want to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dalCall!.Delete(id);
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
                        s_dalCall!.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-6: ");
                        break;
                }
            } while (option != 0);
        }

        /* Assignment option */
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
            if (s_dalCall!.Read(callId) == null)
            {
                Console.WriteLine("The call ID does not exist.");
                return;
            }

            Console.Write("Please enter the Volunteer ID: ");
            int volunteerId = Convert.ToInt32(Console.ReadLine());
            if (s_dalVolunteer!.Read(volunteerId) == null)
            {
                Console.WriteLine("The volunteer ID does not exist.");
                return;
            }

            Console.Write("Please enter the start treatment time (yyyy-MM-dd HH:mm:ss): ");
            DateTime startTreatment = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Please enter the end treatment time (yyyy-MM-dd HH:mm:ss, optional): ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = DateTime.Parse(endTreatmentInput!);

            Console.WriteLine("Please enter the type of end treatment (1 - Success, 2 - Failure, 3 - Cancelled): ");
            int typeOption = 0;
            while (typeOption < 1 || typeOption > 3)
            {
                typeOption = Convert.ToInt32(Console.ReadLine());
            }
            typeOfEndTreatment typeOfEnd = (typeOfEndTreatment)typeOption;

            s_dalAssignment!.Create(new Assignment
            {
                Id = s_dalConfig!.NextAssignmentId,
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
            Assignment? assignment = s_dalAssignment!.Read(id);
            printAssignment(assignment!);
        }

        private static void readAllAssignments()
        {
            List<Assignment> assignments = s_dalAssignment!.ReadAll();
            foreach (Assignment assignment in assignments)
            {
                printAssignment(assignment);
            }
        }

        private static void updateAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Assignment? assignment = s_dalAssignment!.Read(id);
            printAssignment(assignment!);

            Console.WriteLine("Please enter the new Call ID (or press enter to keep the current one): ");
            string? callIdInput = Console.ReadLine();
            int callId = string.IsNullOrEmpty(callIdInput) ? assignment!.CallId : Convert.ToInt32(callIdInput);

            Console.WriteLine("Please enter the new Volunteer ID (or press enter to keep the current one): ");
            string? volunteerIdInput = Console.ReadLine();
            int volunteerId = string.IsNullOrEmpty(volunteerIdInput) ? assignment!.VolunteerId : Convert.ToInt32(volunteerIdInput);

            Console.WriteLine("Please enter the new start treatment time (yyyy-MM-dd HH:mm:ss, or press enter to keep the current one): ");
            string? startTreatmentInput = Console.ReadLine();
            DateTime startTreatment = string.IsNullOrEmpty(startTreatmentInput) ? assignment!.StartTreatment : DateTime.Parse(startTreatmentInput);

            Console.WriteLine("Please enter the new end treatment time (yyyy-MM-dd HH:mm:ss, or press enter to keep the current one): ");
            string? endTreatmentInput = Console.ReadLine();
            DateTime? endTreatment = string.IsNullOrEmpty(endTreatmentInput) ? assignment!.endTreatment : DateTime.Parse(endTreatmentInput);

            Console.WriteLine("Please enter the new type of end treatment (1 - Success, 2 - Failure, 3 - Cancelled): ");
            int typeOption = 0;
            while (typeOption < 1 || typeOption > 3)
            {
                typeOption = Convert.ToInt32(Console.ReadLine());
            }
            typeOfEndTreatment typeOfEnd = (typeOfEndTreatment)typeOption;

            s_dalAssignment!.Update(new Assignment
            {
                Id = id,
                CallId = callId,
                VolunteerId = volunteerId,
                StartTreatment = startTreatment,
                endTreatment = endTreatment,
                typeOfEnd = typeOfEnd
            });
        }

        private static void deleteAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dalAssignment!.Delete(id);
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
                        s_dalAssignment!.DeleteAll();
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-6: ");
                        break;
                }
            } while (option != 0);
        }

        /* Initialization option */
        private static void funcInitialization()
        {
            Initialization.Do(s_dalAssignment,s_dalCall!,s_dalVolunteer!,s_dalConfig!);
        }

        static void Main(string[] args)
        {
            try
            {
                /*main code*/
                int menu;
                do
                {
                    Console.WriteLine
                  (" Which menu do you want to access ? :  \n" +
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

