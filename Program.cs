// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

/// <summary>
/// description
/// </summary>
namespace MarkovChain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Main test program for the markov chains.
    /// </summary>
    public static class Program
    {
        private const string Help = "-Input: Input filename.\n-Output: Output filename.\n-RegexpMask: regexp mask to use to pase words. Default is \".{1,3}\"\n-TopLines: Read only these lines from file. Default is 0 and wil lread all the file.\n-OutputLines: How many lines or iterations to generate. Default is 120.\n-ngram: Number of n-gram. Default is 3.\n";

        /// <summary>
        /// Main program.
        /// </summary>
        /// <param name="args">command line arguments.</param>
        public static void Main(string[] args)
        {
            Node parent = new Node();
            Stopwatch t = new Stopwatch();
            string inputFile = string.Empty;
            string outputFile = string.Empty;
            string regexpMask = "(...?)";
            int topLines = 0;
            int topOutLines = 10;
            int ngram = 3;

            if (args is null || args.Length == 0)
            {
                Console.WriteLine(Help.Normalize());
                return;
            }

            foreach (string arg in args)
            {
                if (arg.StartsWith("-Input:", StringComparison.InvariantCulture))
                {
                    inputFile = arg.Substring("-Input:".Length);
                }

                if (arg.StartsWith("-Output:", StringComparison.InvariantCulture))
                {
                    outputFile = arg.Substring("-Output:".Length);
                }

                if (arg.StartsWith("-RegexpMask:", StringComparison.InvariantCulture))
                {
                    regexpMask = arg.Substring("-RegexpMask:".Length);
                }

                if (arg.StartsWith("-TopLines:", StringComparison.InvariantCulture))
                {
                    topLines = int.Parse(arg.Substring("-TopLines:".Length), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);
                }

                if (arg.StartsWith("-OutputLines:", StringComparison.InvariantCulture))
                {
                    topOutLines = int.Parse(arg.Substring("-OutputLines:".Length), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);
                }

                if (arg.StartsWith("-ngram:", StringComparison.InvariantCulture))
                {
                    ngram = int.Parse(arg.Substring("-ngram:".Length), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture);
                }
            }

            StreamReader sr = new StreamReader(File.OpenRead(inputFile));
            Regex r = new Regex(regexpMask);
            Console.OutputEncoding = Encoding.Unicode;
            string line;
            int lineNumber = 0;
            t.Start();
            while ((line = sr.ReadLine()) != null)
            {
                string[] strs = r.Matches(line).Cast<Match>().Select(m => m.Value).ToArray();
                parent.AddSequence(strs);
                if ((++lineNumber % 1000) == 0)
                {
                    Console.WriteLine($"Line number : {lineNumber}\t\tTime elapsed: {t.Elapsed.ToString()}");
                }

                if (topLines != 0 && topLines <= lineNumber)
                {
                    break;
                }
            }

            sr.Close();
            sr.Dispose();
            t.Stop();
            Console.WriteLine($"Total unique words:{parent.NodesCount}\n\n\nAuto generated text:");
            string word = string.Empty;
            List<string> wordList = new List<string>();
            for (int j = 0; j < topOutLines; j++)
            {
                word = parent.ReturnSequenceNoSpace(ngram);
                if (!string.IsNullOrEmpty(outputFile))
                {
                    wordList.Add(word);
                }

                Console.WriteLine(word);
            }

            if (!string.IsNullOrEmpty(outputFile))
            {
                FileStream fs = File.Create(outputFile);
                fs.Close();
                fs.Dispose();
                File.WriteAllLines(outputFile, wordList.ToArray<string>());
            }
        }
    }
}
