using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static prj.PublicCommands;
namespace prj
{
    internal static class ImplementationCreateRstCommand
    {
        internal static void DoCreateRspCommand(string[] extentionsOfFiles)
        {
            try
            {
                var path = Directory.GetCurrentDirectory();
                string? rspName = GetRspName();
                FileInfo? rspFile = CreateRspFile(path, rspName);
                AddOptionToRspFile(rspFile?.FullName, $"-o \"{GetOutputPath(path)?.FullName}\"");
                AddOptionToRspFile(rspFile?.FullName, $"-l \"{GetLanguages(extentionsOfFiles)}\"");
                if (GetNote()) AddOptionToRspFile(rspFile?.FullName, "-n");
                if (GetSort()) AddOptionToRspFile(rspFile?.FullName, "-s");
                if (GetRemoveELines()) AddOptionToRspFile(rspFile?.FullName, "-r");
                AddOptionToRspFile(rspFile?.FullName, $"-a \"{GetAuthor(extentionsOfFiles)}\"");
                Console.Write($"Successfull creating a new response file ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(rspFile?.Name);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
            }
        }
        private static string? GetRspName()
        {
            Console.WriteLine("Enter name of the response file");
            string? rsp = Console.ReadLine();
            while (rsp == null || rsp.Contains(' ') || rsp.Contains('\\') || rsp.Contains('/') || rsp.Contains('*') || rsp.Contains('?') || rsp.Contains('<') || rsp.Contains('>') || rsp.Contains('|'))
            {
                Console.WriteLine("Invalid name!! name mustn't contains tha chars: \\ / * ? < > |");
                rsp = Console.ReadLine();
            }
            return rsp;
        }
        static FileInfo? CreateRspFile(string path, string? name)
        {
            FileInfo? file = null;
            try
            {
                file = new(path + "\\" + name + ".rsp");
                File.Create(file.FullName).Close();
            }
            catch (Exception)
            {
                WriteError("ERROR!! Unable to write to directory: " + file?.FullName + "\n");
                throw new Exception("exit program");
            }
            return file;
        }
        static FileInfo GetOutputPath(string path)
        {
            FileInfo? output;
            Console.WriteLine("Enter directory for new file: ");
            string? temp = null;
            while (temp == null)
            {
                temp = Console.ReadLine();
                if (temp != null)
                {
                    try
                    {
                        if (temp[1] == ':')
                            output = new FileInfo(temp);
                        else if (temp.Contains(' ') || temp.Contains('\\') || temp.Contains('/') || temp.Contains('*') ||
                            temp.Contains('?') || temp.Contains('<') || temp.Contains('>') || temp.Contains('|'))
                            throw new NotImplementedException();
                        else
                            output = new FileInfo(path + "\\" + temp);
                    }
                    catch (NotImplementedException)
                    {
                        Console.WriteLine("Invalid name!! name mustn't contains tha chars: \\ / * ? < > | and not conteins spaces");
                        temp = null;
                    }
                    catch (PathTooLongException)
                    {
                        Console.WriteLine("Length of path is too long. Enter again.");
                        temp = null;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("invalid path!! Enter again");
                        temp = null;
                    }
                }
            }
            return new FileInfo(temp);
        }
        static string GetLanguages(string[] extentionsOfFiles)
        {
            string languages = "";
            Console.Write("Would you like to copy all languages of files? (y/n) ");
            if (GetBooleanParameter())
                languages = "all";
            else
            {
                Console.WriteLine("choose languages:");
                for (int i = 1; i < extentionsOfFiles.Length; i++)
                {
                    Console.Write("  include " + extentionsOfFiles[i] + "? (y/n) ");
                    if (GetBooleanParameter())
                        languages += " " + extentionsOfFiles[i];
                }
            }
            return languages;
        }
        static bool GetNote()
        {
            Console.Write("Would you like  to list the code's source as a comment in the file? (y/n) ");
            return GetBooleanParameter();
        }
        static bool GetSort()
        {
            Console.Write("Would you like to sort the files by extention? (Default is by file's name) (y/n) ");
            return GetBooleanParameter();
        }
        static bool GetRemoveELines()
        {
            Console.Write("Would you like to remove empty lines in your files? (y/n) ");
            return GetBooleanParameter();
        }
        static string GetAuthor(string[] extentionsOfFiles)
        {
            Console.Write("Would you like to write in the head of the bundle file the author name? (y/n) ");
            string? res = null;
            if (GetBooleanParameter())
            {
                Console.Write("Enter author's name here:  ");
                res = Console.ReadLine();
            }
            return res ?? "";
        }
        private static void AddOptionToRspFile(string? fileName, string option)
        {
            try
            {
                if (fileName != null)
                    using (StreamWriter sw = new(fileName, true))
                        sw.WriteLine(option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                WriteError("File path is: " + fileName + "\n");
                throw new Exception("exit program");
            }
        }
    }
}
