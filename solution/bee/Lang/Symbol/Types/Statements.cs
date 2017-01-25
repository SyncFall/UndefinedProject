using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public enum StatementType
    {
        // condition-blocks
        If,
        ElseIf,
        Else,
        // loop-block
        While,
        DoWhile,
        For,
        // loop-control
        Continue,
        Break,
        // function-control
        Return,
        // sanity-check
        Sanity,
        // error-control
        Throw,
        // error-processing
        Try,
        Catch,
        Finally,
        // thread-synchronisation
        Sync,
        // expression-statement
        ExpressionStatement,
        // variable declaration/definition
        VariableDeclaration,
        // empty-statement
        NoOperation,
        // inner-block
        InnerBlock,
    }

    public enum StatementKeywordType
    {
        If,
        Else,
        For,
        While,
        Do,
        Continue,
        Break,
        Return,
        Sanity,
        Throw,
        Try,
        Catch,
        Finally,
        Sync,
    }

    public static class StatementKeywords
    {
        public static readonly StatementKeywordSymbol[] Array = new StatementKeywordSymbol[]
        {
            new StatementKeywordSymbol(StatementKeywordType.If, "if"),
            new StatementKeywordSymbol(StatementKeywordType.Else, "else"),
            new StatementKeywordSymbol(StatementKeywordType.For, "for"),
            new StatementKeywordSymbol(StatementKeywordType.While, "while"),
            new StatementKeywordSymbol(StatementKeywordType.Do, "do"),
            new StatementKeywordSymbol(StatementKeywordType.Continue, "continue"),
            new StatementKeywordSymbol(StatementKeywordType.Break, "break"),
            new StatementKeywordSymbol(StatementKeywordType.Return, "return"),
            new StatementKeywordSymbol(StatementKeywordType.Sanity, "sanity"),
            new StatementKeywordSymbol(StatementKeywordType.Throw, "throw"),
            new StatementKeywordSymbol(StatementKeywordType.Try, "try"),
            new StatementKeywordSymbol(StatementKeywordType.Catch, "catch"),
            new StatementKeywordSymbol(StatementKeywordType.Finally, "finally"),
            new StatementKeywordSymbol(StatementKeywordType.Sync, "sync"),
        };
    }

    public class StatementKeywordSymbol
    {
        public readonly StatementKeywordType Type;
        public readonly string String;

        public StatementKeywordSymbol(StatementKeywordType KeywordType, string SymbolString)
        {
            this.Type = KeywordType;
            this.String = SymbolString;
        }
    }
}
