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

        //static readonly IDal s_dal = new DalList();
        //static readonly IDal s_dal = new DalXml();
        static readonly IDal s_dal = Factory.Get;
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
       
        // Prints the details of a single Volunteer object to the console.
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
        // Allows the user to create a new Volunteer object by providing details interactively.
        private static void createVolunteer()
        {
            Console.Write("Please enter your Id (digits only): ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Please enter your full Name : ");
            string name = Console.ReadLine()!;

            Console.Write("Please enter your phone number : ");
            string phone = Console.ReadLine()!;

            string password = name + $"{id}";

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
            // Save the new Volunteer object using a data access layer (DAL).

            s_dal.Volunteer.Create(new Volunteer
            {
                Id = id,
                Name = name,
                Phone = phone,
                Email = email,
                Password = password,
                JobType = jobType.Volunteer,
                isActive = true,
                distanceType = distanceType,
                Address = address,
                Longitude = longitude,
                Latitude = latitude,
                MaxDistance = distance
            });

        }
        // Reads and prints the details of a Volunteer by their ID.
        private static void readVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to read : ");
            int id = Convert.ToInt32(Console.ReadLine());
            Volunteer? volunteer = s_dal.Volunteer.Read(id);
            printVolunteer(volunteer!);
        }
        // Prints the details of all Volunteers if they exist.
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

        // Updates the details of an existing Volunteer by ID.
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
            // Update the Volunteer object in the data store.

            s_dal.Volunteer.Update(myVolunteer);

            Console.WriteLine(" ");
            printVolunteer(myVolunteer);
        }
        // Deletes a Volunteer by their ID.
        private static void deleteVolunteer()
        {
            Console.WriteLine("Please enter the id of the volunteer you want to delete : ");
            int id = Convert.ToInt32(Console.ReadLine());
            s_dal.Volunteer.Delete(id);
        }
        // Provides a menu-driven interface for managing volunteers.

        private static void volunteersOptions()
        {
            int option;

            do
            {
                printMenu("Volunteer");
                option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 0: break;// Exit
                    case 1:
                        createVolunteer();// Create a new volunteer
                        break;

                    case 2:
                        readVolunteer();// Read volunteer details by ID
                        break;

                    case 3:
                        readAllVolunteer();// Read all volunteers
                        break;

                    case 4:
                        updateVolunteer();// Update a volunteer
                        break;

                    case 5:
                        deleteVolunteer();// Delete a volunteer
                        break;

                    case 6:
                        s_dal.Volunteer.DeleteAll();// Delete all volunteers
                        break;

                    default:
                        Console.WriteLine("Please enter a digit between 0-6 : ");
                        option = Convert.ToInt32(Console.ReadLine());
                        break;

                }
            } while (option != 0);


        }


        /********************************************************************* Call Functions ***********************************************************************************/
        // Prints the details of a single Call object to the console.
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

        // Allows the user to create a new Call object by providing input interactively.
        private static void createCall()
        {
        
            Console.WriteLine("Please enter the type of call: ");

            foreach (var item in Enum.GetValues(typeof(callType)))
            {
                Console.WriteLine($"{(int)item}. {item}");
            }

            int callTypeOption = 0;
            while (callTypeOption < 1 || callTypeOption > 10)
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

            // Creates a new Call object and stores it using the data access layer.
            s_dal.Call.Create(new Call
            {
                
                CallType = callType,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                CallTime = callTime,
                Description = description,
                MaxTime = maxTime
            });
        }

        // Reads and prints the details of a specific Call object by its ID.
        private static void readCall()
        {
            Console.WriteLine("Please enter the id of the call you want to read: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Call? call = s_dal.Call.Read(id);
            printCall(call!);
        }

        // Prints the details of all Call objects if they exist.
        private static void readAllCalls()
        {
            IEnumerable<Call> calls = s_dal.Call.ReadAll();// Retrieves all calls from the data store.
            if (calls != null && calls.Any())
            {
                foreach (Call call in calls)
                {
                    printCall(call);
                }
            }
            else
                Console.WriteLine("\nThere are no Calls\n");
         
        }

        // Allows the user to update an existing Call object by its ID.
        private static void updateCall()
        {
            Console.WriteLine("Please enter the id of the call you want to update: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Call? call = s_dal.Call.Read(id);
            printCall(call!);

            Console.WriteLine("Please enter the new call's type (or press enter to keep the current one):");
            foreach (var item in Enum.GetValues(typeof(callType)))
            {
                Console.WriteLine($"{(int)item}. {item}");
            }
            string callTypeInput = Console.ReadLine()!;
            if(string.IsNullOrEmpty(callTypeInput))
            {
                callTypeInput = call!.CallType.ToString();
            }
            callType callType = (callType)Enum.Parse(typeof(callType), callTypeInput);

            Console.WriteLine("Please enter the new address (or press enter to keep the current one): ");
            string address = Console.ReadLine()!;
            if (string.IsNullOrEmpty(address))
            {
                address = call!.Address;
            }

            Console.WriteLine("Please enter the new longitude (or press enter to keep the current one): ");
            string longitudeInput = Console.ReadLine()!;
            double longitude = string.IsNullOrWhiteSpace(longitudeInput) ? call!.Longitude : Convert.ToDouble(longitudeInput);

            Console.WriteLine("Please enter the new latitude (or press enter to keep the current one): ");
            string latitudeInput = Console.ReadLine()!;
            double latitude = string.IsNullOrWhiteSpace(latitudeInput) ? call!.Latitude : Convert.ToDouble(latitudeInput);


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
            // Creates a new Call object with the updated details.

            DO.Call myCall = new Call(id, callType, address, latitude, longitude, callTime, description, maxTime);
            // Updates the Call object in the data store.
            s_dal.Call.Update(myCall);

            Console.WriteLine(" ");
            printCall(myCall);
        }

        // Deletes a Call object by its ID.
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
        // Print details of an assignment
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

        // Create a new assignment
        private static void createAssignment()
        {
            Console.Write("Please enter the Call ID: ");
            int callId = Convert.ToInt32(Console.ReadLine());
            if (s_dal.Call.Read(callId) == null)// Check if the provided Call ID exists
            {
                Console.WriteLine("The call ID does not exist.");
                return;
            }

            Console.Write("Please enter the Volunteer ID: ");
            int volunteerId = Convert.ToInt32(Console.ReadLine());
            if (s_dal.Volunteer.Read(volunteerId) == null)// Check if the Volunteer ID exists
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
            // Create the new assignment and store it in the database

            s_dal.Assignment.Create(new Assignment
            {
               
                CallId = callId,
                VolunteerId = volunteerId,
                StartTreatment = startTreatment,
                endTreatment = endTreatment,
                typeOfEnd = typeOfEnd
            });
        }

        // Read and display details of a specific assignment

        private static void readAssignment()
        {
            Console.WriteLine("Please enter the ID of the assignment you want to read: ");
            int id = Convert.ToInt32(Console.ReadLine());
            Assignment? assignment = s_dal.Assignment.Read(id);
            printAssignment(assignment!);
        }

        // Read and display details of all assignments
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

        // Update an existing assignment by modifying its details
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
            Initialization.Do();
        }

        /************************************************************************************* Config ***************************************************************************/

        // Configuration sub-menu that offers multiple options to modify system settings
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
                // Switch case to handle the option selected by the user
                switch (option)
                {
                    case 0:
                        break;

                    case 1:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);// Advance by 1 minute
                        break;
                    case 2:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);// Advance by 1 hour
                        break;
                    case 3:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);// Advance by 1 day
                        break;
                    case 4:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMonths(1);// Advance by 1 month
                        break;
                    case 5:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddYears(1);// Advance by 1 year
                        break;
                    case 6:
                        Console.WriteLine($"The current time is {s_dal.Config.Clock}");// Display current system time
                        break;
                    case 7:
                        SetNewConfigValue(); // Call method to set a new configuration value
                        break;

                    case 8:
                        ShowConfigValue();// Call method to show the current configuration value
                        break;

                    case 9:
                        s_dal.Config.Reset();// Reset configuration variables to defaults
                        Console.WriteLine("All configuration variables have been reset to their default values.");
                        break;
                    default:
                        Console.WriteLine("Please enter a digit between 0-5: ");
                        break;
                }
            } while (option!=0);
        }
        // Function to change a specific configuration variable value
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
                    // Prompt user to enter a new system clock value
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
                    // Prompt user to enter a new Risk Range in minutes\
                    Console.Write("Enter the new Risk Range (in minutes): ");
                        if (int.TryParse(Console.ReadLine(), out int minutes))
                        {
                            s_dal.Config.RiskRange = TimeSpan.FromMinutes(minutes);// Update the Risk Range as a TimeSpan
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
        // Function to display the current value of a configuration variable
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

            // Switch case to display the value of the selected configuration variable

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

