using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DBeaverUpdateVersionCheck
{
  internal class Program
  {
    static void Main()
    {
      Action<string> Display = Console.WriteLine;
      Display("Checking if there is a newer version of DBeaver");
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
      string dbeaverReadMeFilePath = Path.Combine(dbeaverInstallationPath, dbeaverReadMeFileName);
      if (File.Exists(dbeaverReadMeFilePath))
      {
        dbeaverIsInstalled = true;
      }
      else
      {
        dbeaverIsInstalled = false;
      }

      if (!dbeaverIsInstalled)
      {
        Display($"DBeaver has not been found in {dbeaverReadMeFilePath}");
        return;
      }

      List<string> readMeFileContent = new List<string>();
      try
      {
        using (StreamReader streamReader = new StreamReader(dbeaverReadMeFilePath))
        {
          while (streamReader.Peek() >= 0)
          {
            readMeFileContent.Add(streamReader.ReadLine());
          }
        }
      }
      catch (Exception)
      {
        Display($"There was an exception while trying to read the {dbeaverReadMeFilePath} file.");
        return;
      }

      // processing the readme file
      string dbeaverVersionInstalled = readMeFileContent[2];
      Display($"I have found DBeaver version {dbeaverVersionInstalled} installed on your computer");

      // checking latest version on https://dbeaver.io/files/ea/
      string dbeaverInternetAddress = "https://dbeaver.io/files/ea/";
      string dbeaverWebSiteContent = GetWebPageContent(dbeaverInternetAddress);
      // Display($"{dbeaverWebSiteContent}");
      List<string> webSiteContent = new List<string>();
      
      
      Display("Press any key to exit:");
      Console.ReadKey();
    }

    private static string GetWebPageContent(string url)
    {
      string result = string.Empty;
      try
      {
        WebRequest request = WebRequest.Create(url);
        WebResponse response = request.GetResponse();
        Stream data = response.GetResponseStream();
        string html = string.Empty;
        using (StreamReader streamReader = new StreamReader(data))
        {
          html = streamReader.ReadToEnd();
        }

        result = html;
      }
      catch (Exception)
      {
        return result;
      }

      return result;
    }
  }
}
