using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

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
      List<string> webSiteStartsWithDBeaver = new List<string>();
      //string htmlFileName = "file1.html";
      //SaveWebPageToFile(htmlFileName, dbeaverWebSiteContent);
      //var tmp = ParseHtml(htmlFileName);
      string updateWebLine = "dbeaver-ce-21.3.0-linux.gtk.aarch64-nojdk.tar.gz";
      string searchedPattern = "dbeaver-ce-";
      if (dbeaverWebSiteContent.Contains(searchedPattern))
      {
        //Display($"The pattern {searchedPattern} has been found");
      }
      else
      {
        //Display($"The pattern {searchedPattern} has not been found");
      }

      var webSiteContentSplitted = dbeaverWebSiteContent.Split(new[] { '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in webSiteContentSplitted)
      {
        if(line.Contains(searchedPattern))
        {
          // dbeaver-ce-21.3.0-macosx.cocoa.aarch64.tar.gz
          webSiteStartsWithDBeaver.Add(line);
          //Display(line);
        }
      }

      string firstLine = webSiteStartsWithDBeaver[0];
      var hrefStrings = firstLine.Split('>');
      string firstVersionInstance = hrefStrings[16];
      var lineSplitted = firstVersionInstance.Split('-');
      string webSiteLatestVersion = lineSplitted[2];
      Display($"The latest version found in the web site is {webSiteLatestVersion}");
      if (webSiteLatestVersion == dbeaverVersionInstalled)
      {
        Display($"There is no newer version of DBeaver available.");
      }
      else
      {
        Display($"You have DBeaver version {dbeaverVersionInstalled}");
        Display($"There is a newer version of DBeaver available which is {webSiteLatestVersion}");
      }

      Display("Press any key to exit:");
      Console.ReadKey();
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
