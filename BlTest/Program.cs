
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
               " 3 - Admin. \n" );
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

    private static void volunteersOptions()
    {
        throw new NotImplementedException();
    }
}