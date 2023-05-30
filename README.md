# FolderCopy

This is a console program that periodically replicates a specified folder by copying the files from source to target and deleting the removed files.

The program checks for modified files since the last run and only copies them.

Every copied or deleted file is logged.


## Usage

The program requires 4 command line arguments:
{SourceFolder} {TargetFolder} {LogFileLocation} {TimeBetweenRuns(in sec)}

Example:
>FolderCopy.exe "C:\source" "C:\target" "C:\log.txt" 300

