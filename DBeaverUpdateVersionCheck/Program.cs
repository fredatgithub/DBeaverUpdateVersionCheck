using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBeaverUpdateVersionCheck
{
  internal class Program
  {
    static void Main()
    {
      Action<string> Display = Console.WriteLine;
      Display("Checking if there is a newer version of DBeaver, please wait ...");
      Display(string.Empty);
      // checking if DBeaver is installed in C:\Users\userName\AppData\Local\DBeaver

      Display("Press any key to exit:");
      Console.ReadKey();
    }
  }
}
