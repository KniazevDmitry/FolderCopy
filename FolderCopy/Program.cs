using System.Security.Cryptography;

namespace FolderCopy;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("To set up a copy job use the following format:");
        Console.WriteLine("[Source folder path] [Target folder path]");      

        string sourceFolder = "/Users/vmdk/Downloads/Source";
        string targetFolder = "/Users/vmdk/Downloads/Target";

        //create target folder if it doesn't exist
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        SyncFolder(sourceFolder, targetFolder);
        Console.WriteLine("Copy job complete");

    }

    static void SyncFolder(string SourceFolder, string TargetFolder)
    {             
        foreach (string sourceFilePath in Directory.GetFiles(SourceFolder))
        {
            string sourceFileName = Path.GetFileName(sourceFilePath);
            string targetPath = Path.Combine(TargetFolder, sourceFileName);

            //calculate md5 of both files if it already exists in target folder
            if (File.Exists(targetPath))
            {
                using (var md5 = MD5.Create())
                {
                    using (var readSourceFile = File.OpenRead(sourceFilePath))
                    {
                        using (var readTargetFile = File.OpenRead(targetPath))
                        {
                            var hashSourceFile = md5.ComputeHash(readSourceFile);
                            var hashTargetFile = md5.ComputeHash(readTargetFile);

                            if (hashSourceFile == hashTargetFile)
                            {
                                continue;
                            }
                        }
                    }
                    

                }
            }

            File.Copy(sourceFilePath, targetPath, true);
        }
    }
}
