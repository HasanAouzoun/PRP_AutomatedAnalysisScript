using System;
using System.IO;
using System.Collections.Generic;
using PRP_AutomatedAnalysisScript.Helpers;

namespace PRP_AutomatedAnalysisScript
{
    class Program
    {
        private static List<string> _Dict, _List;
        private static string _DictPath, _ListPath;
        private static string _OutputDirectoryPath;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            // Check if command structure is correct
            // Display information about the script when no argument is provided
            // Display an example if the number of arguments is not the expected number i.e. not 3
            // Otherwise, continue the script
            if (args.Length == 0)
            {
                // Help display
                HelpDisplay();
                return;
            }
            else if( args.Length != 2)
            {
                // display structure of command
                CommandStructureDisplay();
                return;
            }

            // Get Arguments
            var dictionaryFileName = args[0];
            var listFileName = args[1];

            // Check if files does exists (i.e. argument 2 and 3 are valid)
            if (!FilesExist(dictionaryFileName, listFileName))
                return;

            // Finished with checking the command structure -- Start the script
            var startDateTime = DateTime.Now.ToString("dd-MM-yyyy HH-mm tt");
            Console.WriteLine($"Script started at {startDateTime}");
            _OutputDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @$"Output\Analysis Result - {startDateTime}\");

            // Read Both Lists
            ReadLists();

            //Run automated analysis script
            RunAutomatedAnalysisScript();

            Console.WriteLine($"Script finished at {DateTime.Now.ToString("dd-MM-yyyy HH-mm tt")}");
            Console.WriteLine($"The results are found in: \n{_OutputDirectoryPath}");
        }

        private static void HelpDisplay()
        {
            Console.WriteLine("Help");
            // TODO
        }

        private static void CommandStructureDisplay()
        {
            Console.WriteLine("Command structure should contain 2 arguments.");
            Console.WriteLine("Example:");
            Console.WriteLine("\tPRP_AutomatedAnalysisScript.exe dictionary.txt list.txt");
            Console.WriteLine("For more information, run the script without arguments.");
        }

        private static bool FilesExist(string dictionaryFileName, string listFileName)
        {
            var dictPath = Path.Combine(Directory.GetCurrentDirectory(), @$"Files\{dictionaryFileName}");
            var listPath = Path.Combine(Directory.GetCurrentDirectory(), @$"Files\{listFileName}");

            if (!File.Exists(dictPath) || !File.Exists(listPath))
            {
                Console.WriteLine($"Please make sure that both file exists:" +
                    $"\nDictionary file: {dictPath}" +
                    $"\nList file: {listPath}");
                return false;
            }

            _DictPath = dictPath;
            _ListPath = listPath;

            return true;
        }

        private static void ReadLists()
        {
            // Get Dictionary List
            Console.WriteLine("Start reading Dictionary list");
            _Dict = IOHelper.ReadListFromFile(_DictPath);
            Console.WriteLine($"Dictionary contains {_Dict.Count} words");

            // Get password List
            Console.WriteLine("Start reading Password list");
            _List = IOHelper.ReadListFromFile(_ListPath);
            Console.WriteLine($"Password list contains {_List.Count} passwords");
        }

        private static void RunAutomatedAnalysisScript()
        {
            // Get Automated analysis Result
            Console.WriteLine("Automated Analysis has started, it will take a long period of time to finish...");
            AnalysisScript.AutomatedAnalysisScript(_Dict, _List);
            Console.WriteLine("Analysis done.");

            // Sort the result list according to the Symbols
            Console.WriteLine("Sorting the result.");
            AnalysisScript.FinalResultList.Sort((a, b) => a.Symbol.CompareTo(b.Symbol));

            // Write Unconsistent Password Lists to File
            Console.WriteLine("Writing the unconsitent passwords to file");
            var unconsistentListFilePath = Path.Combine(_OutputDirectoryPath, "Unconsistent_Password_List.txt");
            IOHelper.WriteListToFile(unconsistentListFilePath, AnalysisScript.UnconsistentPasswordsList);

            // Write Unconsistent Match Lists to File
            Console.WriteLine("Writing the unconsitent matches to file");
            var unconsistentMatchedFilePath = Path.Combine(_OutputDirectoryPath, "Unconsistent_Match_List.json");
            IOHelper.WriteObjectToJson(unconsistentMatchedFilePath, AnalysisScript.UnconsistentMatchList);

            // Write Unconsistent Password Lists to File
            Console.WriteLine("Writing the consistent passwords to file");
            var consistentListFilePath = Path.Combine(_OutputDirectoryPath, "Consistent_Password_List.txt");
            IOHelper.WriteListToFile(consistentListFilePath, AnalysisScript.ConsistentPasswordsList);

            // Write the result to File
            Console.WriteLine("Writing the final results to file");
            var resultFilePath = Path.Combine(_OutputDirectoryPath, "Final_Analysis_Result.json");
            IOHelper.WriteObjectToJson(resultFilePath, AnalysisScript.FinalResultList);

            // Write discription of result
            WriteOutputDiscription();
        }

        private static void WriteOutputDiscription()
        {
            var discription = $"Automated Analysis for Identifing Replacement Patterns" +
                $"\nScript inputs:" +
                $"\n\tDictionary List contains {_Dict.Count} words" +
                $"\n\tLocation: {_DictPath}" +
                $"\n\tPasswords List contains {_List.Count} passwords" +
                $"\n\tLocation: {_ListPath}" +
                $"\nScript outputs:" +
                $"\n\tUnconsistent_Password_List.txt & Unconsistent_Match_List.json which contains {AnalysisScript.UnconsistentPasswordsList.Count} unconsistent passwords" +
                $"\n\tConsistent_Password_List.txt which contains {AnalysisScript.ConsistentPasswordsList.Count} consistent passwords" +
                $"\n\tFinal_Analysis_Result.json which contains all the replacement patterns found in the password list (the input)";

            var discriptionFilePath = Path.Combine(_OutputDirectoryPath, "_Discription.txt");

            IOHelper.WriteTextToFile(discriptionFilePath, discription);
        }
    }
}
