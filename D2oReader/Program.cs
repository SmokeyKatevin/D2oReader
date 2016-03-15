using System;
using System.Diagnostics;

namespace D2oReader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                #if DEBUG
                    args = new[] { @"C:\Dofus\app\data\common\Areas.d2o" };
                #endif

                if (args != null)
                {
                    string d2oFilePath;

                    d2oFilePath = args[0];

                    new App(d2oFilePath);

                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine("Press any key to continue . . .");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
             }
        }
    }
}

 
