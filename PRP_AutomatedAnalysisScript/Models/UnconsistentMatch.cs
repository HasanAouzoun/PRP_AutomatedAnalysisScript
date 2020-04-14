using System;
using System.Collections.Generic;
using System.Text;

namespace PRP_AutomatedAnalysisScript.Models
{
    class UnconsistentMatch
    {
        public char Symbol { get; set; }
        public char AlphaOne { get; set; }
        public char AlphaTwo { get; set; }
        public PasswordMatch PasswordMatch { get; set; }
    }
}
