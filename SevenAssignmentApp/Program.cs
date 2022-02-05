using System;
using System.Threading.Tasks;
using SevenAssignmentLibrary.DI;
using SevenAssignmentLibrary.Services;

namespace SevenAssignmentApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DependencyResolver.ConfigureDependency();

            var sevenService = DependencyResolver.GetService<ISevenService>();

            //get user 41
            var user = await sevenService.GetUserByIdAsync(41);
            WriteToConsole(string.Empty, "1.");
            WriteToConsole("User 41 fullname: ", $"{user.First} {user.Last}");

            //get users comma seperated who are 23
            var names = await sevenService.GetCommaSeperatedUsersByAgeAsync(23);
            Console.WriteLine("");
            WriteToConsole(string.Empty,"2.");
            WriteToConsole("Comma Seperated Firstnames who are 23: ", $"{names}");

            //number of genders per age, displayed from youngest to oldest
            var gendersPerAge = await sevenService.GetNumberOfGendersPerAgeAsync();
            Console.WriteLine("");
            WriteToConsole(string.Empty, "3.");
            foreach (var g in gendersPerAge)
            {
                WriteToConsole(string.Empty, g);
            }

            Console.WriteLine("");
            Console.WriteLine("============================");
            Console.ReadLine();
        }


        private static void WriteToConsole(string exercise, string input)
        {
            Console.WriteLine($"{exercise}{input}");
        }
    }
}
