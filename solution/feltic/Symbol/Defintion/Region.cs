using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum RegionType
    {
        None=0,
        RegionBegin,
        RegionEnd,
        ProcessIf,
        ProcessElseIf,
        ProcessElse,
    }

    public static class RegionKeywords
    {
        public static readonly RegionSymbol[] Array = 
        {
            new RegionSymbol(RegionType.RegionBegin, "#region"),
            new RegionSymbol(RegionType.RegionEnd, "#endregion"),
            new RegionSymbol(RegionType.ProcessIf, "#if"),
            new RegionSymbol(RegionType.ProcessElseIf, "#elseif"),
            new RegionSymbol(RegionType.ProcessElse, "#else"),
        };
    }

    public class RegionSymbol : Symbol
    {
        public RegionSymbol(RegionType Type, string String) : base(String, (int)TokenType.Region, (int)Type)
        { }
    }
}
