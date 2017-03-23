using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum StructureType
    {
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
        //ShiftBegin,
        //ShiftEnd,
    }

    public enum StructureGroup
    {
        Space,
        Seperator,
        Block,
    }

    public static class Structures
    {
        public static readonly StructureSymbol[] Array =
        {
            new StructureSymbol(StructureType.WhiteSpace, StructureGroup.Space, " "),
            new StructureSymbol(StructureType.TabSpace, StructureGroup.Space, "\t"),
            new StructureSymbol(StructureType.LineSpace, StructureGroup.Space, "\n"),
            new StructureSymbol(StructureType.Complete, StructureGroup.Seperator, ";"),
            new StructureSymbol(StructureType.Seperator, StructureGroup.Seperator, ","),
            new StructureSymbol(StructureType.Point, StructureGroup.Seperator, "."),
            new StructureSymbol(StructureType.PointDouble, StructureGroup.Seperator, ":"),
            new StructureSymbol(StructureType.BlockBegin, StructureGroup.Block, "{"),
            new StructureSymbol(StructureType.BlockEnd, StructureGroup.Block, "}"),
            new StructureSymbol(StructureType.ClosingBegin, StructureGroup.Block, "("),
            new StructureSymbol(StructureType.ClosingEnd, StructureGroup.Block, ")"),
            new StructureSymbol(StructureType.BracketBegin, StructureGroup.Block, "["),
            new StructureSymbol(StructureType.BracketEnd, StructureGroup.Block, "]"),
            //new StructureSymbol(StructureType.ShiftBegin, StructureGroup.Block, "<"),
            //new StructureSymbol(StructureType.ShiftEnd, StructureGroup.Block, ">"),
        };
    }

    public class StructureSymbol
    {
        public readonly StructureType Type;
        public readonly StructureGroup Group;
        public readonly string String;

        public StructureSymbol(StructureType Type, StructureGroup Group, string SymbolString)
        {
            this.Type = Type;
            this.Group = Group;
            this.String = SymbolString;
        }
    }
}
