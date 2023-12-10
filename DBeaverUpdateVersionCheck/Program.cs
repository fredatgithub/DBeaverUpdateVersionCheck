using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using HtmlAgilityPack;
using Microsoft.Win32;

namespace DBeaverUpdateVersionCheck
{
  internal class Program
  {
    static void Main()
    {
      Action<string> Display = Console.WriteLine;
      Display($"DBeaver update version checker: {GetVersion()}");
      Display("Checking if there is a newer version of DBeaver:");
      Display(string.Empty);
      // checking if DBeaver is installed in C:\Users\userName\AppData\Local\DBeaver
      // now in F:\Users\user1\AppData\Local\DBeaver
      string userName = Environment.UserName;
      // string userNameProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      string appDatafolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      appDatafolder = @"F:\Users\user1\AppData\Local\";
      string execPath = Assembly.GetEntryAssembly().Location;
      var dbeaverInstallation = GetInstallationPath("Dbeaver.exe");

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
        Display(string.Empty);
        Display("Press any key to exit:");
        Console.ReadKey();
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
      Display(string.Empty);
      // checking latest version on https://dbeaver.io/files/ea/
      string dbeaverInternetAddress = "https://dbeaver.io/files/ea/";
      string dbeaverWebSiteContent = GetWebPageContent(dbeaverInternetAddress);

      // "dbeaver-ce-21.3.0-linux.gtk.aarch64-nojdk.tar.gz";
      string searchedPattern = "dbeaver-ce-";
      if (dbeaverWebSiteContent.Contains(searchedPattern))
      {
        //Display($"The pattern {searchedPattern} has been found");
      }
      else
      {
        //Display($"The pattern {searchedPattern} has not been found");
      }

      var webSiteContentSplittedByInferiorSign = dbeaverWebSiteContent.Split('<');
      List<string> webSiteContentWithPattern = new List<string>();
      string win32Version = string.Empty;
      foreach (string line in webSiteContentSplittedByInferiorSign)
      {
        if (line.Contains(searchedPattern))
        {
          // dbeaver-ce-21.3.0-macosx.cocoa.aarch64.tar.gz
          webSiteContentWithPattern.Add(line);
          if (line.Contains("win32"))
          {
            win32Version = line;
          }
        }
      }

      var versionLineSplitted = win32Version.Split('>');
      string firstLine = versionLineSplitted[1];
      var webSiteLatestVersion = firstLine.Substring(11, 6);
      Display($"The latest version found in the web site is {webSiteLatestVersion}");
      Display(string.Empty);
      Version webSiteVersion = new Version($"{webSiteLatestVersion}.0");
      Version installedVersion = new Version($"{dbeaverVersionInstalled}.0");
      if (webSiteVersion > installedVersion)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Display($"You have DBeaver version {dbeaverVersionInstalled}");
        Display($"There is a newer version of DBeaver available which is {webSiteLatestVersion}");
        Display($"The link for the newer version of DBeaver is available at: {dbeaverInternetAddress}");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Display($"There is no newer version of DBeaver available.");
      }

      Console.ForegroundColor = ConsoleColor.White;
      Display(string.Empty);
      Display("Press any key to exit:");
      Console.ReadKey();
    }

    public static string GetInstallationPath(string applicationName)
    {
      //Ordinateur\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\DBeaver (current user)
      // DisplayVersion = 23.3.0
      string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

      using (RegistryKey key = Registry.LocalMachine.OpenSubKey(uninstallKey))
      {
        if (key != null)
        {
          foreach (string subKeyName in key.GetSubKeyNames())
          {
            using (RegistryKey subKey = key.OpenSubKey(subKeyName))
            {
              object displayName = subKey.GetValue("DisplayName");
              object installLocation = subKey.GetValue("InstallLocation");

              if (displayName != null && installLocation != null && displayName.ToString().Contains(applicationName))
              {
                return installLocation.ToString();
              }
            }
          }
        }
      }

      return null;
    }

    private static string GetVersion()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return $"V{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}.{fvi.FilePrivatePart}";
    }

    private static bool SaveWebPageToFile(string fileName, string content)
    {
      bool result;
      try
      {
        using (StreamWriter streamWriter = new StreamWriter(fileName))
        {
          streamWriter.Write(content);
        }

        result = true;
      }
      catch (Exception)
      {
        result = false;
      }

      return result;
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

    private static string ParseHtml(string webContentFilePath)
    {
      string result = string.Empty;
      HtmlDocument htmlDoc = new HtmlDocument();

      // There are various options, set as needed
      htmlDoc.OptionFixNestedTags = true;

      // filePath is a path to a file containing the html
      htmlDoc.Load(webContentFilePath);

      // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

      // ParseErrors is an ArrayList containing any errors from the Load statement
      if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
      {
        // Handle any parse errors as required

      }
      else
      {

        if (htmlDoc.DocumentNode != null)
        {
          HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

          if (bodyNode != null)
          {
            // Do something with bodyNode
          }
        }
      }

      return result;
    }
  }
}
