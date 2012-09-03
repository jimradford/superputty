using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Gui;

namespace SuperPuttyUnitTests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            RunGui(args);
        }

        static void RunConsole()
        {
            string[] my_args = { Assembly.GetExecutingAssembly().Location };

            int returnCode = NUnit.ConsoleRunner.Runner.Main(my_args);

            if (returnCode != 0)
                Console.Beep();

            Console.WriteLine("Complete - Any key to kill");
            Console.ReadLine();
        }

        static void RunGui(string[] args)
        {
            AppEntry.Main(args);
        }
    }
}
