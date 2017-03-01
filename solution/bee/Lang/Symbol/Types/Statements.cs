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
        // condition-block
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
        // declaration/definition
        TypeDeclaration,
        // empty-statement
        NoOperation,
        // inner-block
        InnerBlock,
    }

    public enum StatementKeywordType
    {
        // condition
        If,
        Else,
        // loop
        For,
        While,
        Do,
        // flow
        Continue,
        Break,
        Return,
        // error
        Sanity,
        Throw,
        Try,
        Catch,
        Finally,
        // thread
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
