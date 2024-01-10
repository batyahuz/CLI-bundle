using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using static prj.PublicCommands;
namespace prj
{
    internal static class ImplementationBundleCommand
    {
        internal static void DoBundleCommand(string languages, string[] extentionsOfFiles, FileInfo output, bool note, bool sort, bool removeElines, string author = "")
        {
            try
            {
                CheckValidationLanguageOption(languages, extentionsOfFiles);
                string path = Directory.GetCurrentDirectory();
                if (!CreateFile(output, path))
                    return;
                var files = FilesOfLanguages(languages, extentionsOfFiles, path);
                if (author != null && author != "")
                    AddLineToFile("//name of author: " + author, output);
                if (files == null)
                {
                    AddLineToFile("//Not Found files to copy", output);
                    WriteWarning("WARNING!! Not Found files to copy\n");
                    return;
                }
                if (removeElines)
                    RemoveEmptyLinesInFiles(files);
                if (sort)
                    SortFiles((name) => name.Split('.').ToList().Last(), files);
                else
                    SortFiles((name) => name.Split('.')[0], files);
                int counterFiles = 0;
                files.ForEach(f =>
                {
                    AddLineToFile("//**************************************************************", output);
                    if (note)
                        AddLineToFile("// " + f.FullName.Substring(path.Length + 1), output);
                    CopyFileToFile(f, output);
                    counterFiles++;
                });
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(counterFiles);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" files copied successfully to ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(output.Name);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
            }
        }

        static void CheckValidationLanguageOption(string languages, string[] extentionsOfFiles)
        {
            if (languages != "all")
                languages.Split(' ').ToList().ForEach(l =>
                {
                    if (l != "" && l != null && !extentionsOfFiles.Contains(l))
                    {
                        string message = $"Argument '{languages}' not recognized. Must be\r\n        'all'\r\nor one or more of:";
                        extentionsOfFiles.ToList().ForEach(e => message += $"\r\n        '{e}'");
                        WriteError(message + "\n");
                        throw new Exception();
                    }
                });
        }
        static bool CreateFile(FileInfo fileInfo, string path)
        {
            try
            {
                if (fileInfo.FullName[1] != ':')
                    fileInfo = new FileInfo(path + "\\" + fileInfo.FullName);
                try
                {
                    FileStream f = File.OpenRead(fileInfo.FullName);
                    f.Close();
                    WriteWarning("WARNING!! destination file is already exist. Do you want to override it? (y/n) ");
                    if (!GetBooleanParameter())
                    {
                        Console.WriteLine("exit program...");
                        return false;
                    }
                }
                catch
                {
                }
                File.Create(fileInfo.FullName).Close();
                Console.Write("Successfull creating a new file ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(fileInfo?.FullName);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (DirectoryNotFoundException)
            {
                WriteError($"Error: File path {fileInfo.FullName} is invalid.\n");
                return false;
            }
            catch (System.Security.SecurityException)
            {
                WriteError($"ERROR! You mustn't write to directory {fileInfo.FullName} due to the security.\n");

            }
            catch (UnauthorizedAccessException)
            {
                WriteError("ERROR!! The output is not a file to write to.\n");
                return false;
            }
            return true;
        }
        static List<FileInfo>? FilesOfLanguages(string lang, string[] extentionsOfFiles, string path)
        {
            List<FileInfo> files = new List<FileInfo>();
            try
            {
                Directory.GetFiles(path + "/", "", SearchOption.AllDirectories).ToList()
                    .ForEach(filePath => files.Add(new FileInfo(filePath)));
            }
            catch (UnauthorizedAccessException)
            {
                WriteError("ERROR! You don't have a permission to write to directory.\n");
                throw new Exception("exit program");
            }
            catch (Exception)
            {
                WriteError("ERROR! Directory of command is invalid\n");
                throw new Exception("exit program");
            }
            if (files == null)
                return null;
            string[] languages;
            if (lang != "all")
                languages = lang.Split(" ");
            else
                languages = extentionsOfFiles;
            files = files.FindAll(f =>
                !f.FullName.Contains("\\debug\\") && !f.FullName.Contains("\\git\\") &&
                !f.FullName.Contains("\\bin\\") && !f.FullName.Contains("\\.vs\\") &&
                !f.FullName.Contains("\\obj\\") && !f.FullName.Contains("\\venv\\") &&
                !f.FullName.Contains("\\.idea\\") &&
                languages.Contains(f.FullName.Split('.').ToList().Last())
                );
            return files;
        }
        static void RemoveEmptyLinesInFiles(List<FileInfo> files)
        {
            files.ForEach(f =>
            {
                try
                {
                    string? line;
                    List<string> lines = new List<string>();
                    using (StreamReader sr = new StreamReader(f.FullName))
                    {
                        lines.Clear();
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Length != 0)
                                lines.Add(line);
                        }
                    }
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(f.FullName))
                            lines.ForEach(l => sw.WriteLine(l));
                    }
                    catch (Exception)
                    {
                        WriteError("ERROR!! Error in removing empty lines in a file. file path was changed or removed.\n");
                        WriteError("File path is: " + f.Name + "\n");
                        throw new Exception("exit program");
                    }
                }
                catch (Exception)
                {
                    WriteError("ERROR!! Error in reading a file. file path was changed or removed.\n");
                    WriteError("File path is: " + f.FullName + "\n");
                    throw new Exception("exit program");
                }
            });
        }
        static void SortFiles(Func<string, string> split, List<FileInfo> files)
        {
            files.Sort((f1, f2) => split(f1.Name).CompareTo(split(f2.Name)));
        }
        static void AddLineToFile(string sourceLine, FileInfo destinationFile)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(destinationFile.FullName, true))
                    sw.WriteLine(sourceLine);
            }
            catch (Exception e)
            {
                WriteError("ERROR!! Error in coping to file. file path was changed or removed.\n");
                WriteError("File path is: " + destinationFile.FullName + "\n");
                WriteError(e.Message + "\n");
                throw new Exception("exit program");
            }
        }
        static void CopyFileToFile(FileInfo source, FileInfo destination)
        {
            try
            {
                string? line;
                using (StreamReader sr = new StreamReader(source.FullName))
                {
                    while ((line = sr.ReadLine()) != null)
                        AddLineToFile(line, destination);
                }
            }
            catch (Exception e)
            {
                WriteError("ERROR!! Error in coping to file. file path was changed or removed.\n");
                WriteError("File path is: " + destination.FullName + "\n");
                WriteError(e.Message + "\n");
                throw new Exception("exit program");
            }
        }
    }
}
