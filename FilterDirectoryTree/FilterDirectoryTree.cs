using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Atmosphere.FilterDirectoryTree
{
    public class FilterDirectoryTree
    {
        private static Regex regex = new Regex(@"%(?<key>[^%]+)%", RegexOptions.Compiled);
        private static MatchEvaluator evaluator = delegate(Match match)
        {
            string key = match.Groups["key"].Value;
            
            if (!FilterValues.ContainsKey(key))
            {
                throw new KeyNotFoundException(String.Format("Key: {0}, Input: {1}", key, match.Value));
            }
            
            return FilterValues[key];
        };
        
        public static void Reset()
        {
            FilterValues = new Dictionary<string, string>();
        }
        
        public static string Filter(string input)
        {
            return regex.Replace(input, evaluator);
        }
        
        public static void CopyAndFilterRecursively(DirectoryInfo sourceDir, DirectoryInfo targetDir)
        {
            Console.WriteLine(" Processing dir: " + sourceDir.FullName);

            foreach (FileInfo sourceFile in sourceDir.GetFiles())
            {
                Console.WriteLine("Processing file: " + sourceFile.FullName);

                string targetFileName = Filter(sourceFile.Name);
                
                FileInfo targetFile = new FileInfo(Path.Combine(targetDir.FullName, targetFileName));
                
                using (StreamWriter writer = new StreamWriter(targetFile.FullName))
                    using (StreamReader reader = new StreamReader(sourceFile.FullName))
                {
                    string line = null;
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = Filter(line);
                        
                        writer.WriteLine(line);
                    }
                }
            }
            
            foreach (DirectoryInfo sourceSubDir in sourceDir.GetDirectories())
            {
                string targetSubDirName = Filter(sourceSubDir.Name);
                
                DirectoryInfo targetSubDir = new DirectoryInfo(Path.Combine(targetDir.FullName, targetSubDirName));
                
                if (!targetSubDir.Exists)
                {
                    targetSubDir.Create();
                    targetDir.Refresh();
                }
                
                CopyAndFilterRecursively(sourceSubDir, targetSubDir);
            }
        }
        
        public static void Main(string[] args)
        {
            DirectoryInfo templateDirectory = new DirectoryInfo(args[0]);
            DirectoryInfo monoDirectory = new DirectoryInfo(args[1]);
            
            FilterValues = new Dictionary<string, string>();
            
            for (int i = 2; i < args.Length; i++)
            {
                string[] parts = args[i].Split('=');
                
                FilterValues.Add(parts[0], parts[1]);
            }
            
            CopyAndFilterRecursively(templateDirectory, monoDirectory);
        }
        
        public static Dictionary<string, string> FilterValues { get; set; }
    }
}
