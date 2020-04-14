using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using PRP_AutomatedAnalysisScript.Models;

namespace PRP_AutomatedAnalysisScript.Helpers
{
    class IOHelper
    {
        public static List<string> ReadListFromFile(string path)
        {
            // set up for feedback (progress)
            var cursorPosition = Console.CursorTop;

            // set up empty list
            var list = new List<string>();

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                // Get the lenght of the file - used for calculating the progress
                var lenght = sr.BaseStream.Length;

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // reset the cursor possition to the beginning of the current line
                    Console.SetCursorPosition(0, cursorPosition);

                    // add password to list
                    list.Add(line);

                    // display progress
                    var progress = (double)sr.BaseStream.Position / lenght * 100;
                    Console.WriteLine($"Reading File ... ({string.Format("{0:0.00}", progress)}%)");
                }
                sr.Close();
            }

            return list;
        }

        public static void WriteObjectToJson(string path, Object obj)
        {
            // create directory if dose not exist
            var file = new FileInfo(path);
            file.Directory.Create();

            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // write to file
            File.WriteAllText(path, json);
        }

        public static void WriteListToFile(string path, List<string> list)
        {
            // create directory if dose not exist
            var file = new FileInfo(path);
            file.Directory.Create();

            StreamWriter sw = new StreamWriter(file.FullName);

            foreach(var item in list)
            {
                sw.WriteLine(item);
            }

            sw.Close();
        }

        public static void WriteTextToFile(string path, string text)
        {
            // create directory if dose not exist
            var file = new FileInfo(path);
            file.Directory.Create();

            File.WriteAllText(file.FullName, text);
        }
    }
}
