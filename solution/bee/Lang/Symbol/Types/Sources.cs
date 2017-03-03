using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Lang
{
    public enum RegionType
    {
        RegionBegin,
        RegionEnd,
        ProcessIf,
        ProcessElseIf,
        ProcessElse,
        ProcessEnd,
    }

    public static class RegionKeywords
    {
        public static readonly RegionSymbol[] Array = {
            new RegionSymbol("#region", RegionType.RegionBegin),
            new RegionSymbol("#endregion", RegionType.RegionEnd),
            new RegionSymbol("#if", RegionType.ProcessIf),
            new RegionSymbol("#elseif", RegionType.ProcessElseIf),
            new RegionSymbol("#else", RegionType.ProcessElse),
        };
    }

    public class RegionSymbol
    {
        public readonly RegionType Type;
        public readonly string String;

        public RegionSymbol(string String, RegionType Type)
        {
            this.Type = Type;
            this.String = String;
        }
    }
}
