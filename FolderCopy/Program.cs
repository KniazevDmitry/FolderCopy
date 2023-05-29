﻿using System.Security.Cryptography;

namespace FolderCopy;
class Program
{
    static void Main(string[] args)
    {
        string sourceFolder = args[0];
        string targetFolder = args[1];
        string logFilePath = args[2];
        int copyJobInterval;

        if (!Int32.TryParse(args[3], out copyJobInterval))
        {
            Console.WriteLine("Invalid synchronization interval specified.");
            return;
        }




        while (true)
        {
            WriteLog(logFilePath, "Copy Job started");

            //create target folder if it doesn't exist
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                WriteLog(logFilePath, "Folder created");
            }

            CopyFiles(logFilePath, sourceFolder, targetFolder);

            RemoveDeletedFiles(logFilePath, targetFolder, sourceFolder);

            WriteLog(logFilePath, "Copy Job complete");

            Thread.Sleep(copyJobInterval * 60000);
        }
        

    }

    static void CopyFiles(string logFilePath, string sourceFolder, string targetFolder)
    {             
        foreach (string sourceFile in Directory.GetFiles(sourceFolder))
        {
            string sourceFileName = Path.GetFileName(sourceFile);
            string sourceFilePath = Path.Combine(sourceFileName, sourceFile);           
            string targetFilePath = Path.Combine(targetFolder, sourceFileName);
            

            if (File.Exists(targetFilePath))
            {
                if (CompareMD5(sourceFilePath, targetFilePath))
                {
                    continue;
                }
            }
                            
            File.Copy(sourceFile, targetFilePath, true);
            WriteLog(logFilePath, $"File copied: {sourceFilePath}");
            
        }
    }

    //calculate md5 of both files if it already exists in target folder
    static bool CompareMD5(string sourceFileName, string targetFileName)
    {
        using (var md5 = MD5.Create())
        {
            using (var readSourceFile = File.OpenRead(sourceFileName))
            {
                using (var readTargetFile = File.OpenRead(targetFileName))
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
    static void RemoveDeletedFiles(string logFilePath, string targetFolder, string sourceFolder)
    {
        foreach(string targetFile in Directory.GetFiles(targetFolder))
        {
            if (!File.Exists(Path.Combine(sourceFolder, Path.GetFileName(targetFile))))
            {
                File.Delete(targetFile);
                WriteLog(logFilePath, $"File deleted: {targetFile}");
            }
            
        }
    }

    //Display log message in the console and write it into the file
    private static void WriteLog(string logFile, string message)
    {
        Console.WriteLine(message);

        using (StreamWriter log = File.AppendText(logFile))
        {
            log.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
}
