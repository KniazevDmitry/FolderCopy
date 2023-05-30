using System.Security.Cryptography;

namespace FolderCopy;
class Program
{
    static void Main(string[] args)
    {
        //The program should take exatcly 4 arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Please use this pattern of command line arguments: \nFolderCopy.exe SourceFolder, TargetFolder, SyncTime(in sec)");
        }

        string sourceFolder = "C:\\Users\\VMDK\\Downloads\\source";
        string targetFolder = "C:\\Users\\VMDK\\Downloads\\Target";
        string logFilePath = "C:\\Users\\VMDK\\Downloads\\log.txt";
        int copyJobInterval = 20;              
        
        
        while (true)
        {
            WriteLog(logFilePath, "Copy Job started");

            if (!Directory.Exists(sourceFolder))
            {
                WriteLog(logFilePath, $"Source folder doesn't exist: {args[0]}");
                return;
            }

            //create target folder if it doesn't exist
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                WriteLog(logFilePath, "Folder created");
            }

            CopyFiles(logFilePath, sourceFolder, targetFolder);

            RemoveDeletedFiles(logFilePath, targetFolder, sourceFolder);

            WriteLog(logFilePath, "Copy Job complete");

            Thread.Sleep(copyJobInterval * 1000); //1sec = 1000ms
        }    
    }

    static void CopyFiles(string logFilePath, string sourceFolder, string targetFolder)
    {             
        foreach (string sourceFilePath in Directory.GetFiles(sourceFolder))
        {
            string sourceFileName = Path.GetFileName(sourceFilePath);                     
            string targetFilePath = Path.Combine(targetFolder, sourceFileName);

            if (File.Exists(targetFilePath))
            {
                if (CompareMD5(sourceFilePath, targetFilePath))
                {
                    continue;
                }
            }
                            
            File.Copy(sourceFilePath, targetFilePath, true);
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
