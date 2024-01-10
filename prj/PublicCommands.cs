using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace prj
{
    public class PublicCommands
    {
        public static bool GetBooleanParameter()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleKeyInfo cki = Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            return cki.Key.ToString() == "y" || cki.Key.ToString() == "Y";
        }
        public static void WriteError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteWarning(string error)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
