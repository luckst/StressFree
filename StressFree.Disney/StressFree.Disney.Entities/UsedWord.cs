using StressFree.Disney.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StressFree.Disney.Entities
{
    public class UsedWord
    {
        public Direction Direction { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Word { get; set; }
    }
}
