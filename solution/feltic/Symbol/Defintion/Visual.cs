using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum VisualType
    {
        None=0,
        Block,
        Inline,
        Column,
        Sheet,
        Line,
        Cell,
        Text,
        Break,
        Input,
        Scroll,
    }

    public static class Visuals
    {
        public static VisualSymbol[] Array =
        {
            new VisualSymbol(VisualType.Block, "block"),
            new VisualSymbol(VisualType.Inline, "inline"),
            new VisualSymbol(VisualType.Column, "column"),
            new VisualSymbol(VisualType.Sheet, "sheet"),
            new VisualSymbol(VisualType.Line, "line"),
            new VisualSymbol(VisualType.Cell, "cell"),
            new VisualSymbol(VisualType.Text, "text"),
            new VisualSymbol(VisualType.Input, "input"),
            new VisualSymbol(VisualType.Scroll, "scroll"),
            new VisualSymbol(VisualType.Break, "br"),
        };
    }

    public class VisualSymbol : Symbol
    {
        public VisualSymbol(VisualType Type, string String) : base(String, (int)TokenType.Visual, (int)Type)
        { }
    }
}
