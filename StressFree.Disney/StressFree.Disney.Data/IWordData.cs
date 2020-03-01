using System;
using System.Collections.Generic;
using System.Text;

namespace StressFree.Disney.Data
{
    public interface IWordData
    {
        List<string> GetRandomWords();
        bool ValidateWordInList(string word);
    }
}
