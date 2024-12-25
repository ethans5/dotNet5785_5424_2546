
using BO;

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
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

    private static void volunteersOptions()
    {
        Console.WriteLine(
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
                    Console.WriteLine("Login successful");
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
                DistanceType = (BO.distanceType)myDistanceType
            });
            Console.WriteLine("Volunteer created successfully");
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
                mySortField.HasValue ? (BO.VolunteerSortField)mySortField.Value : default);

            foreach (var volunteer in volunteers)
            {
                Console.WriteLine(volunteer);
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
            Console.WriteLine(volunteer);
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
            s_bl.Volunteer.UpdateVolunteer(myIdToUpdate, new BO.Volunteer
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
                DistanceType = (BO.distanceType)myDistanceType
            });
            Console.WriteLine("Volunteer updated successfully");
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
            Console.WriteLine("Volunteer deleted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete volunteer: {ex.Message}");
        }
    }

}