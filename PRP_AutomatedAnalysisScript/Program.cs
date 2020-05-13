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
            Console.WriteLine("Password Replacement Pattern - Automated Analysis Script");
            Console.WriteLine("\nInformation: " +
                "\n\tThis script was developed as an automated analysis script " +
                "\n\tto identify replacement patterns in password list. The script" +
                "\n\thas two list as inputs. First input is a dictionary list and" +
                "\n\tthe second is the password list. Both should contain strings" +
                "\n\tthat are separated by line." +
                "\n\tThe script uses an algorithm that checks each password to verify" +
                "\n\tif it is an actual word in the dictionary. Then, it records the" +
                "\n\treplacement identified in each matched password." +
                "\nOutput structure;" +
                "\n\tFinally, it outputs the result to text files. There are several" +
                "\n\toutputs:" +
                
                "\n\t1) Final_Analysis_Result.json: this is the full result that contains" +
                "\n\t\ta list of objects. Each object is a replacement identified. it" +
                "\n\t\thas the (Alphabet) replaced and the (Symbol) that replaced it." +
                "\n\t\tMoreover, it contains a (List) of passwords that each are matched" +
                "\n\t\twith their dictionary word. Finally, the (Count) of the List is added" +
                "\n\t\tto each object." +
                
                "\n\t2) Final_Analysis_Result.csv: this is the same result as no. 1, However," +
                "\n\t\tit contains only the (Alphabet), (Symbol) and (Count)." +
                
                "\n\t3) Consistent_Password_List.txt: is the list of passwords that were" +
                "\n\t\tmatched with at least a word from the dictionary list." +
                
                "\n\t4) Inconsitent_Password_List.txt: is the list of passwords that were" +
                "\n\t\tmatched. However, they were not consistent. i.e. if a password has" +
                "\n\t\tmultiple similar non-alpha characters and they were not substituted" +
                "\n\t\tto the same alphabet character, then this password is not consisten." +
                
                "\nExpected input:" +
                "\n\tTwo lists - a dictionary list and a password list" +
                "\n\tExample: PRP_AutomatedAnalysisScript.exe <DictionaryFileName> <ListFileName>");
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

            // Write Inconsistent Password Lists to File
            Console.WriteLine("Writing the unconsitent passwords to file");
            var inconsistentListFilePath = Path.Combine(_OutputDirectoryPath, "Inconsistent_Password_List.txt");
            IOHelper.WriteListToFile(inconsistentListFilePath, AnalysisScript.InconsistentPasswordsList);

            // Write Inconsistent Match Lists to File
            Console.WriteLine("Writing the unconsitent matches to file");
            var inconsistentMatchedFilePath = Path.Combine(_OutputDirectoryPath, "Inconsistent_Match_List.json");
            IOHelper.WriteObjectToJson(inconsistentMatchedFilePath, AnalysisScript.InconsistentMatchList);

            // Write consistent Password Lists to File
            Console.WriteLine("Writing the consistent passwords to file");
            var consistentListFilePath = Path.Combine(_OutputDirectoryPath, "Consistent_Password_List.txt");
            IOHelper.WriteListToFile(consistentListFilePath, AnalysisScript.ConsistentPasswordsList);

            // Write the result to File
            Console.WriteLine("Writing the final results to file");
            var resultFilePath = Path.Combine(_OutputDirectoryPath, "Final_Analysis_Result.json");
            IOHelper.WriteObjectToJson(resultFilePath, AnalysisScript.FinalResultList);

            // Write the result to File
            Console.WriteLine("Writing the final results to csv");
            var resultCsvFilePath = Path.Combine(_OutputDirectoryPath, "Final_Analysis_Result.csv");
            IOHelper.WriteResultToCsv(resultCsvFilePath, AnalysisScript.FinalResultList);

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
                $"\n\tInconsistent_Password_List.txt & Inconsistent_Match_List.json which contains {AnalysisScript.InconsistentPasswordsList.Count} inconsistent passwords" +
                $"\n\tConsistent_Password_List.txt which contains {AnalysisScript.ConsistentPasswordsList.Count} consistent passwords" +
                $"\n\tFinal_Analysis_Result.json which contains all the replacement patterns found in the password list (the input)";

            var discriptionFilePath = Path.Combine(_OutputDirectoryPath, "_Discription.txt");

            IOHelper.WriteTextToFile(discriptionFilePath, discription);
        }
    }
}
