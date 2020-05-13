namespace PRP_AutomatedAnalysisScript.Models
{
    class InconsistentMatch
    {
        public char Symbol { get; set; }
        public char AlphaOne { get; set; }
        public char AlphaTwo { get; set; }
        public PasswordMatch PasswordMatch { get; set; }
    }
}
