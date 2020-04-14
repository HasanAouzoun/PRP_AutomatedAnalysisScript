using System.Collections.Generic;

namespace PRP_AutomatedAnalysisScript.Models
{
    class ResultEntry
    {
        public char Symbol { get; set; }
        public char Alphabet { get; set; }
        public int Count { get; set; }
        public List<PasswordMatch> Passwords { get; set; }
    }
}
