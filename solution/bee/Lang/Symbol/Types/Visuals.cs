using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum VisualElementType
    {
        Compose,
        Block,
        Inline,
        Column,
        Sheet,
        Line,
        Cell,
        Text,
        Input,
    }

    public class VisualKeywords
    {
        public static VisualKeywordSymbol[] Array =
        {
            new VisualKeywordSymbol(VisualElementType.Compose, "compose"),
            new VisualKeywordSymbol(VisualElementType.Block, "block"),
            new VisualKeywordSymbol(VisualElementType.Inline, "inline"),
            new VisualKeywordSymbol(VisualElementType.Column, "column"),
            new VisualKeywordSymbol(VisualElementType.Sheet, "sheet"),
            new VisualKeywordSymbol(VisualElementType.Line, "line"),
            new VisualKeywordSymbol(VisualElementType.Cell, "cell"),
            new VisualKeywordSymbol(VisualElementType.Text, "text"),
            new VisualKeywordSymbol(VisualElementType.Input, "input"),
        };
    }

    public class VisualKeywordSymbol
    {
        public readonly VisualElementType Type;
        public readonly string String;

        public VisualKeywordSymbol(VisualElementType Type, string String)
        {
            this.Type = Type;
            this.String = String;
        }
    }
}
