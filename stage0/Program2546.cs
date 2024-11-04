// See https://aka.ms/new-console-template for more information
namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome2546();
            Welcome5424();
            Console.ReadKey();

        }
        static partial void Welcome5424();

        private static void Welcome2546()
        {
            Console.Write("Enter your name:");
            string userName = Console.ReadLine()!;
            Console.WriteLine("{0},welcome to my first console application", userName);
        }
    }
}
