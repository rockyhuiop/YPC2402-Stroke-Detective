using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ExpressionExtractor
{
    // Example chatbot reply.
    //private string chatbotReply = "*Attempts to lift left arm but it barely moves* I... I can't... it's so heavy... *right hand twitches but left arm stays limp* Something's... wrong... *voice trembles with concern*";

    // Start is called before the first frame update.
    

    /// <summary>
    /// Extracts messages enclosed in asterisks (*) from a given text.
    /// </summary>
    /// <param name="text">The input text containing expressions.</param>
    /// <returns>A list of strings containing the extracted expressions.</returns>
    public List<string> ExtractExpressions(string text)
    {
        // Define a regular expression pattern to find text that is enclosed in asterisks.
        // The pattern \*(.*?)\* uses a non-greedy match to capture content between asterisks.
        Regex regex = new Regex(@"\*(.*?)\*");
        MatchCollection matches = regex.Matches(text);

        List<string> extractedExpressions = new List<string>();

        foreach (Match match in matches)
        {
            // match.Groups[1] contains the content between the asterisks.
            if (match.Groups.Count > 1)
            {
                extractedExpressions.Add(match.Groups[1].Value.Trim());
            }
        }

        return extractedExpressions;
    }

    public string RemoveExpressionCommands(string input)
    {
        // Use Regex.Replace to remove all occurrences of text enclosed in asterisks.
        // The pattern \*.*?\* matches any substring starting and ending with an asterisk.
        string cleanedText = Regex.Replace(input, @"\*.*?\*", string.Empty);
        
        // Optionally, you can trim extra white spaces that result from the removal.
        return cleanedText.Trim();
    }
}