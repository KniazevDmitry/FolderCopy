using System.Diagnostics;
using System.Security.Cryptography;

namespace FolderCopy;
class Program
{
    private static string logFilePath = @"C:\Users\VMDK\Downloads\log.txt";
    
    static void Main(string[] args)
    {
        Console.WriteLine("To set up a copy job use the following format:");
        Console.WriteLine("[Source folder path] [Target folder path]");      

        string sourceFolder = @"C:\Users\VMDK\Downloads\source";
        string targetFolder = @"C:\Users\VMDK\Downloads\Target";
        int copyJobInterval = 30000;

        while (true)
        {
            WriteLog("Copy Job started");

            //create target folder if it doesn't exist
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                WriteLog("Folder created");
            }

            CopyFiles(sourceFolder, targetFolder);

            RemoveDeletedFiles(targetFolder, sourceFolder);

            WriteLog("Copy Job complete");

            Thread.Sleep(copyJobInterval);
        }
        

    }

    static void CopyFiles(string SourceFolder, string TargetFolder)
    {             
        foreach (string sourceFile in Directory.GetFiles(SourceFolder))
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string sourceFilePath = Path.Combine(sourceFileName, sourceFile);           
            string targetFilePath = Path.Combine(TargetFolder, sourceFileName);
            

            if (File.Exists(targetFilePath))
            {
                if (CompareMD5(sourceFilePath, targetFilePath))
                {
                    continue;
                }
            }
                            
            File.Copy(sourceFile, targetFilePath, true);
            WriteLog($"File copied: {sourceFilePath}");
            
        }
    }

    //calculate md5 of both files if it already exists in target folder
    static bool CompareMD5(string SourceFileName, string TargetFileName)
    {
        using (var md5 = MD5.Create())
        {
            using (var readSourceFile = File.OpenRead(SourceFileName))
            {
                using (var readTargetFile = File.OpenRead(TargetFileName))
                {
                    byte[] hashSourceFile = md5.ComputeHash(readSourceFile);
                    byte[] hashTargetFile = md5.ComputeHash(readTargetFile);

                    if (hashSourceFile.SequenceEqual(hashTargetFile))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }


        }
    }

    //compare the lists of files and remove deleted
    static void RemoveDeletedFiles(string TargetFolder, string SourceFolder)
    {
        foreach(string targetFile in Directory.GetFiles(TargetFolder))
        {
            if (!File.Exists(Path.Combine(SourceFolder, Path.GetFileName(targetFile))))
            {
                File.Delete(targetFile);
                WriteLog($"File deleted: {targetFile}");
            }
            
        }
    }

    //Display log message in the console and write it into the file
    private static void WriteLog(string message)
    {
        Console.WriteLine(message);

        using (StreamWriter log = File.AppendText(logFilePath))
        {
            log.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
}
