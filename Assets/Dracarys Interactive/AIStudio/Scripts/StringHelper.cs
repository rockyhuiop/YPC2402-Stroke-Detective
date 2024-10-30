using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DracarysInteractive.AIStudio
{
    public static class StringHelper
    {
        public static string[] ExtractStringsInParentheses(string input)
        {
            // Define the regular expression pattern
            string pattern = @"\((.*?)\)";

            // Create a regular expression object
            Regex regex = new Regex(pattern);

            // Find all matches in the input string
            MatchCollection matches = regex.Matches(input);

            // Create an array to store the extracted strings
            string[] extractedStrings = new string[matches.Count];

            // Iterate over the matches and extract the strings
            for (int i = 0; i < matches.Count; i++)
            {
                // Remove the parentheses from the matched string
                string extractedString = matches[i].Groups[1].Value;
                extractedStrings[i] = extractedString;
            }

            return extractedStrings;
        }

        public static string RemoveStringsInParentheses(string input)
        {
            // Define the regular expression pattern
            string pattern = @"\((.*?)\)";

            // Create a regular expression object
            Regex regex = new Regex(pattern);

            // Replace all matches with an empty string
            string result = regex.Replace(input, "");

            return result;
        }

        public static string[] SplitCompletion(string completion)
        {
            List<string> subcompletions = new List<string>();
            string delim = "\\n\\n";
            int i = completion.IndexOf(delim);

            if (i == -1)
            {
                i = completion.IndexOf(delim = "\\n");
            }

            while (i != -1)
            {
                subcompletions.Add(filterSubcompletion(completion.Substring(0, i)));
                completion = completion.Substring(i + delim.Length);
                i = completion.IndexOf(delim);
            }

            subcompletions.Add(filterSubcompletion(completion));

            return subcompletions.ToArray();
        }

        private static string filterSubcompletion(string subcompletion)
        {
            return subcompletion.Replace("\\n", " ").Replace("\\", "");
        }

        public static string Remove(string completion, string regex)
        {
            return Regex.Replace(completion, regex, String.Empty);
        }
    }
}
