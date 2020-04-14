using System;
using System.Collections.Generic;

namespace PRP_AutomatedAnalysisScript.Helpers
{
    class GetAllCombinatiosScript
    {
        public static List<string> GetAllCombinations(string originalPassword, List<(int position, char character)> nonAlphaCharacterList)
        {
            var listOfAllCombinations = new List<string>();
            
            var alphabetRange = GetAlphabetCharactersRange();
            var originalWordSplit = originalPassword.ToCharArray();

            if (nonAlphaCharacterList.Count == 1)
            { //Get combinations for passwords with one none alphabet character
                for (int i = 0; i < alphabetRange.Length; i++)
                {
                    var nonAlphaPosition = nonAlphaCharacterList[0].position;

                    originalWordSplit[nonAlphaPosition] = alphabetRange[i];

                    var newWord = new String(originalWordSplit);

                    listOfAllCombinations.Add(newWord);
                }
            }
            else if (nonAlphaCharacterList.Count == 2)
            { //Get combinations for passwords with two none alpha characters
                for (int i = 0; i < alphabetRange.Length; i++)
                {
                    for (int y = 0; y < alphabetRange.Length; y++)
                    {
                        var nonAlpha1Position = nonAlphaCharacterList[0].position;
                        var nonAlpha2Position = nonAlphaCharacterList[1].position;

                        originalWordSplit[nonAlpha1Position] = alphabetRange[i];
                        originalWordSplit[nonAlpha2Position] = alphabetRange[y];

                        var newWord = new String(originalWordSplit);

                        listOfAllCombinations.Add(newWord);
                    }
                }
            }

            return listOfAllCombinations;
        }

        private static char[] GetAlphabetCharactersRange()
        {
            return "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        }
    }
}
