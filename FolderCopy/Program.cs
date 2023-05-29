namespace FolderCopy;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("To set up a copy job use the following format:");
        Console.WriteLine("[Source folder path] [Target folder path]");
        


        string sourceFolder = @"/Users/vmdk/Downloads/Source";
        string targetFolder = @"/Users/vmdk/Downloads/Target";

        SyncFolder(sourceFolder, targetFolder);
        Console.WriteLine("Copy complete");

    }

    static void SyncFolder(string SourceFolder, string TargetFolder)
    {

        if (!Directory.Exists(TargetFolder))
        {
            Directory.CreateDirectory(TargetFolder);
        }

        foreach (string sourceFilePath in Directory.GetFiles(SourceFolder))
        {
            string sourceFileName = Path.GetFileName(sourceFilePath);
            string targetPath = Path.Combine(TargetFolder, sourceFileName);

            File.Copy(sourceFilePath, targetPath, true);
        }
    }
}
