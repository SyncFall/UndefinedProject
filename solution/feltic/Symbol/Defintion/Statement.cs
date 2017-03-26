using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum StatementType
    {
        None=0,
        // condition-block
        If,
        ElseIf,
        Else,
        // loop-block
        For,
        While,
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

    public enum StatementCategory
    {
        None=0,
        KeywordStatement,
        ConditionBlock,
        ExpressionStatement,
        BlockStatement,
        TypeDeclaration,
        NoOperation,
        ForLoop,
    }

    public enum StatementKeywordType
    {
        None=0,
        // condition
        If,
        Else,
        // loop
        For,
        While,
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
        public static readonly StatementSymbol[] Array =
        {
            new StatementSymbol(StatementKeywordType.If, "if"),
            new StatementSymbol(StatementKeywordType.Else, "else"),
            new StatementSymbol(StatementKeywordType.For, "for"),
            new StatementSymbol(StatementKeywordType.While, "while"),
            new StatementSymbol(StatementKeywordType.Continue, "continue"),
            new StatementSymbol(StatementKeywordType.Break, "break"),
            new StatementSymbol(StatementKeywordType.Return, "return"),
            new StatementSymbol(StatementKeywordType.Sanity, "sanity"),
            new StatementSymbol(StatementKeywordType.Throw, "throw"),
            new StatementSymbol(StatementKeywordType.Try, "try"),
            new StatementSymbol(StatementKeywordType.Catch, "catch"),
            new StatementSymbol(StatementKeywordType.Finally, "finally"),
            new StatementSymbol(StatementKeywordType.Sync, "sync"),
        };
    }

    public class StatementSymbol : Symbol
    {
        public StatementSymbol(StatementKeywordType Type, string String) : base(String, (int)TokenType.Statement, (int)Type)
        { }
    }
}
