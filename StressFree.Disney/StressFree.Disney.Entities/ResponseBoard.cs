using System;
using System.Collections.Generic;
using System.Text;

namespace StressFree.Disney.Entities
{
    public class ResponseBoard
    {
        public char[,] WordsLetters { get; set; }
        public List<string> Words { get; set; }
        public int MaxSize { get; set; }
        public List<UsedWord> UsedWords { get; set; }
        public List<string> IntersectionLetters { get; set; }
    }
}
