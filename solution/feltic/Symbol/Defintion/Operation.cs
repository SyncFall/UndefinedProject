using feltic.Library;

namespace feltic.Language
{
    public enum OperationType
    {
        None=0,
        // logic and-or
        And,
        Or,
        // logic equal-not
        Equal,
        NotEqual,
        // logic relation-compare
        LessEqual,
        EqualGreater,
        Less,
        Greater,
        // math-assigment
        AddAssigment,
        MinusAssigment,
        DivideAssigment,
        MultiAssigment,
        // assigment
        Assigment,
        // math
        Add,
        Minus,
        Multi,
        Divide,
        Modulo,
        // variable
        Not,
        Increment,
        Decrement,
        // type
        IsType,
        HasType,
        AsType,
        GetType,
    }

    public enum OperationCategory
    {
        None=0,
        LogicAndOr,
        LogicEqualNot,
        LogicRelationCompare,
        Math,
        MathAssigment,
        Assigment,
        Variable,
        Type,
    }

    public static class Operations
    {
        public static readonly OperationSymbol[] Array =
        {
            // logic and-or
            new OperationSymbol("&&", OperationCategory.LogicAndOr, OperationType.And),
            new OperationSymbol("||", OperationCategory.LogicAndOr, OperationType.Or),
            // logic equal-not
            new OperationSymbol("==", OperationCategory.LogicEqualNot, OperationType.Equal),
            new OperationSymbol("!=", OperationCategory.LogicEqualNot, OperationType.NotEqual),
            // logic relation-compare
            new OperationSymbol("<=", OperationCategory.LogicRelationCompare, OperationType.LessEqual),
            new OperationSymbol(">=", OperationCategory.LogicRelationCompare, OperationType.EqualGreater),
            new OperationSymbol("<", OperationCategory.LogicRelationCompare, OperationType.Less),
            new OperationSymbol(">", OperationCategory.LogicRelationCompare, OperationType.Greater),
            // math-assigment
            new OperationSymbol("+=", OperationCategory.MathAssigment, OperationType.AddAssigment),
            new OperationSymbol("-=", OperationCategory.MathAssigment, OperationType.MinusAssigment),
            new OperationSymbol("/=", OperationCategory.MathAssigment, OperationType.DivideAssigment),
            new OperationSymbol("*=", OperationCategory.MathAssigment, OperationType.MultiAssigment),
            // assigment
            new OperationSymbol("=", OperationCategory.Assigment, OperationType.Assigment),
            // variable
            new OperationSymbol("!", OperationCategory.Variable, OperationType.Not),
            new OperationSymbol("++", OperationCategory.Variable, OperationType.Increment),
            new OperationSymbol("--", OperationCategory.Variable, OperationType.Decrement),
            // basic-math
            new OperationSymbol("+", OperationCategory.Math, OperationType.Add),
            new OperationSymbol("-", OperationCategory.Math, OperationType.Minus),
            new OperationSymbol("/", OperationCategory.Math, OperationType.Divide),
            new OperationSymbol("*", OperationCategory.Math, OperationType.Multi),
            new OperationSymbol("%", OperationCategory.Math, OperationType.Modulo),
            // type
            new OperationSymbol("is", OperationCategory.Type, OperationType.IsType),
            new OperationSymbol("has", OperationCategory.Type, OperationType.HasType),
            new OperationSymbol("as", OperationCategory.Type, OperationType.AsType),
            new OperationSymbol("type", OperationCategory.Type, OperationType.GetType)
        };
    }

    public class OperationSymbol : Symbol
    {
        public OperationSymbol(string String, OperationCategory Category, OperationType Type) : base(String, (int)TokenType.Operation, (int)Type, (int)Category)
        { }
    }
}
