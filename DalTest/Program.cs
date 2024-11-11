using Dal;
using DalApi;
using DO;

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
            Console.WriteLine("Please enter the new phone number : ");
            string phone = Console.ReadLine()!;
            Console.WriteLine("Please enter the new email : ");
            string email = Console.ReadLine()!;
            Console.WriteLine("Please enter the new address : ");
            string address = Console.ReadLine()!;
            Console.WriteLine("Please enter the new longitude : ");
            double longitude = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Please enter the new latitude : ");
            double latitude = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Please enter the new maximal distance : ");
            int distance = Convert.ToInt32(Console.ReadLine());
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
                            //engineerOptions();
                            break;

                        case 2:
                            //tasksOptions();
                            break;

                        case 3:
                            //DependenciesOptions();
                            break;
                        case 4:
                            //funcInitialization();
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

