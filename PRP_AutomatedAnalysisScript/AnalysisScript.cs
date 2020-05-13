using System;
using System.Linq;
using System.Collections.Generic;
using PRP_AutomatedAnalysisScript.Helpers;
using PRP_AutomatedAnalysisScript.Models;

namespace PRP_AutomatedAnalysisScript
{
    class AnalysisScript
    {
        public static List<string> InconsistentPasswordsList;
        public static List<InconsistentMatch> InconsistentMatchList;
        public static List<string> ConsistentPasswordsList;
        public static List<ResultEntry> FinalResultList;

        /// <summary>
        /// This is script goes via the PasswordList and generates all combinations of actual words
        /// then checks all combinations with the dictionary
        /// then if a match is found check if password pattern is consistent
        /// then add to result
        /// </summary>
        /// <param name="dictionaryList"></param>
        /// <param name="passwordList"></param>
        /// <returns></returns>
        public static void AutomatedAnalysisScript(List<string> dictionaryList, List<string> passwordList)
        {
            InconsistentPasswordsList = new List<string>();
            InconsistentMatchList = new List<InconsistentMatch>();
            ConsistentPasswordsList = new List<string>();
            FinalResultList = new List<ResultEntry>();

            // progress feedback variables
            var cursorPosition = Console.CursorTop;
            int count = 0;
            int total = passwordList.Count;

            foreach (var word in passwordList)
            {
                // Update progress feedback
                Console.SetCursorPosition(0, cursorPosition);
                count++;
                var progress = (double)(count) / total * 100;
                Console.WriteLine($"Progress ... ({string.Format("{0:0.00}", progress)}%)");

                // if not a false positive, i.e. a good canditate for a replacement pattern -- continue
                var originalWord = word.ToLower();
                var nonAlphaCharacters = GetNonAlphaCharacters(originalWord);

                if(nonAlphaCharacters.Count < 1 && nonAlphaCharacters.Count > 2)
                {
                    // this is not supposed to accessed -- passwords should have already been filtered to have at least one and maximum 2 non alpha characters
                    // However, it is extra precaution
                    // should be removed -- maybe not
                    continue;
                }

                // Get list of all combinations for a password (that contains at least 1 and maximum 2 non-alpha characters)
                var listOfAllCombinations = GetAllCombinatiosScript.GetAllCombinations(originalWord, nonAlphaCharacters);

                // Check agains the dictionary to find if any of the combinations is a valid word
                var matchedList = dictionaryList.Intersect(listOfAllCombinations).ToList();

                // if matches a combination continue -- otherwise exit the loop and go to next password
                if (matchedList.Count < 1)
                {
                    continue;
                }

                // A list for Matched and Consistent passwords
                var goodMatchedList = new List<string>(); 

                // Checking for consitancy in the password
                if (nonAlphaCharacters.Count == 2)
                {
                    var nonAlphaCharacterOne = nonAlphaCharacters[0];
                    var nonAlphaCharacterTwo = nonAlphaCharacters[1];

                    if (nonAlphaCharacterOne.character.Equals(nonAlphaCharacterTwo.character))
                    {
                        foreach(var matched in matchedList)
                        {
                            var alphaCharOfNonAlphaOne = matched.ElementAt(nonAlphaCharacterOne.position);
                            var alphaCharOfNonAlphaTwo = matched.ElementAt(nonAlphaCharacterTwo.position);

                            if (!alphaCharOfNonAlphaOne.Equals(alphaCharOfNonAlphaTwo))
                            {
                                InconsistentPasswordsList.Add(word);

                                var InconsistentMatch = new InconsistentMatch
                                {
                                    Symbol = nonAlphaCharacterOne.character,
                                    AlphaOne = alphaCharOfNonAlphaOne,
                                    AlphaTwo = alphaCharOfNonAlphaTwo,
                                    PasswordMatch = new PasswordMatch { Password = word, Dictionary = matched }
                                };
                                InconsistentMatchList.Add(InconsistentMatch);

                                continue;
                            }

                            goodMatchedList.Add(matched);
                        }
                    }
                    else
                    {
                        goodMatchedList = matchedList;
                    }
                }
                else
                {
                    goodMatchedList = matchedList;
                }

                if(goodMatchedList.Count < 1)
                {
                    continue;
                }

                // If password is consistent --> add to list
                ConsistentPasswordsList.Add(word);

                // Getting the result
                foreach (var nonAlphaCharacter in nonAlphaCharacters)
                {
                    foreach (var matched in goodMatchedList)
                    {
                        // result entry structure ==> Symbol, Alphabet, Count, List of all instances{password and its match in the dictionary}
                        var nonAlphaChar = nonAlphaCharacter.character;
                        var alphabetChar = matched.ElementAt(nonAlphaCharacter.position);
                        var password = word;
                        var dictionaryWord = matched;

                        // Check the results entry if Symbol and Alphabet matches as an existing entry
                        var resultEntry = FinalResultList.FirstOrDefault(e => e.Symbol == nonAlphaChar && e.Alphabet == alphabetChar);

                        if (resultEntry != null) // Result already has an existing entry for current pattern
                        {
                            resultEntry.Count++;
                            resultEntry.Passwords.Add(new PasswordMatch { Password = password, Dictionary = dictionaryWord });
                        }
                        else // Result does not have the {Symbol "nonAlpha", Alpha} as an entry i.e. new replacement pattern --> therefore, add a new result entry
                        {
                            var newResultEntry = new ResultEntry
                            {
                                Symbol = nonAlphaChar,
                                Alphabet = alphabetChar,
                                Count = 1,
                                Passwords = new List<PasswordMatch> { new PasswordMatch { Password = password, Dictionary = dictionaryWord } }
                            };

                            FinalResultList.Add(newResultEntry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the non alphabet characters in a word with its position.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<(int position, char character)> GetNonAlphaCharacters(string word)
        {
            var nonAlphaCharacters = new List<(int position, char character)>();

            var wordSplit = word.ToCharArray();
            for (int i = 0; i < wordSplit.Length; i++)
            {
                char character = wordSplit[i];
                if (!Char.IsLetter(character))
                {
                    var position = (i, character);
                    nonAlphaCharacters.Add(position);
                }
            }

            return nonAlphaCharacters;
        }
    }
}
