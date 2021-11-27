using System;
using System.Collections.Generic;
using System.IO;
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
      string userName = Environment.UserName;
      string userNameProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      // remove domain if any
      if (userName.Contains("\\"))
      {
        userName = userName.Split('\\')[1];
      }

      string dbeaverInstallationPath = $"{appDatafolder}\\DBeaver";
      string dbeaverReadMeFileName = "readme.txt";
      bool dbeaverIsInstalled = false;
      if (File.Exists(Path.Combine(dbeaverInstallationPath, dbeaverReadMeFileName)))
      {
        dbeaverIsInstalled = true;
      }
      else
      {
        dbeaverIsInstalled = false;
      }

      if (!dbeaverIsInstalled)
      {
        Display($"DBeaver has not been found in {Path.Combine(dbeaverInstallationPath, dbeaverReadMeFileName)}");
        return;
      }



      Display("Press any key to exit:");
      Console.ReadKey();
    }
  }
}
