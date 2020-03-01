using StressFree.Disney.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StressFree.Disney.Application
{
    public interface IWordApplication
    {
        ResponseBoard GetInitialBoard();
        bool ValidateWordInList(string word);
    }
}
