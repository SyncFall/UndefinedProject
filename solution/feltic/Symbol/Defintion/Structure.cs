using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum StructureType
    {
        None=0,
        // spaces
        WhiteSpace,
        TabSpace,
        LineSpace,
        // seperators
        Complete,
        Seperator,
        Point,
        PointDouble,
        // blocks
        BlockBegin,
        BlockEnd,
        ClosingBegin,
        ClosingEnd,
        BracketBegin,
        BracketEnd,
    }

    public enum StructureCategory
    {
        None=0,
        Space,
        Seperator,
        Block,
    }

    public static class Structures
    {
        public static readonly StructureSymbol[] Array =
        {
            new StructureSymbol(StructureType.WhiteSpace, StructureCategory.Space, " "),
            new StructureSymbol(StructureType.TabSpace, StructureCategory.Space, "\t"),
            new StructureSymbol(StructureType.LineSpace, StructureCategory.Space, "\n"),
            new StructureSymbol(StructureType.Complete, StructureCategory.Seperator, ";"),
            new StructureSymbol(StructureType.Seperator, StructureCategory.Seperator, ","),
            new StructureSymbol(StructureType.Point, StructureCategory.Seperator, "."),
            new StructureSymbol(StructureType.PointDouble, StructureCategory.Seperator, ":"),
            new StructureSymbol(StructureType.BlockBegin, StructureCategory.Block, "{"),
            new StructureSymbol(StructureType.BlockEnd, StructureCategory.Block, "}"),
            new StructureSymbol(StructureType.ClosingBegin, StructureCategory.Block, "("),
            new StructureSymbol(StructureType.ClosingEnd, StructureCategory.Block, ")"),
            new StructureSymbol(StructureType.BracketBegin, StructureCategory.Block, "["),
            new StructureSymbol(StructureType.BracketEnd, StructureCategory.Block, "]"),
        };
    }

    public class StructureSymbol : Symbol
    {
        public StructureSymbol(StructureType Type, StructureCategory Category, string String) : base(String, (int)TokenType.Structure, (int)Type, (int)Category)
        { }
    }
}
