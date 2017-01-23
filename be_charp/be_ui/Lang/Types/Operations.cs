using Bee.Library;

namespace Bee.Language
{
    public enum OperationType
    {
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
        // basic-math
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

    public enum OperationGroup
    {
        LogicAndOr,
        LogicEqualNot,
        LogicRelationCompare,
        Math,
        MathAssigment,
        Assigment,
        Variable,
        Type,
    }

    public class Operations
    {
        public static readonly OperationSymbol[] Array = new OperationSymbol[]
        {
            // logic and-or
            new OperationSymbol("&&", OperationGroup.LogicAndOr, OperationType.And),
            new OperationSymbol("||", OperationGroup.LogicAndOr, OperationType.Or),
            // logic equal-not
            new OperationSymbol("==", OperationGroup.LogicEqualNot, OperationType.Equal),
            new OperationSymbol("!=", OperationGroup.LogicEqualNot, OperationType.NotEqual),
            // logic relation-compare
            new OperationSymbol("<=", OperationGroup.LogicRelationCompare, OperationType.LessEqual),
            new OperationSymbol(">=", OperationGroup.LogicRelationCompare, OperationType.EqualGreater),
            new OperationSymbol("<", OperationGroup.LogicRelationCompare, OperationType.Less),
            new OperationSymbol(">", OperationGroup.LogicRelationCompare, OperationType.Greater),
            // math-assigment
            new OperationSymbol("+=", OperationGroup.MathAssigment, OperationType.AddAssigment),
            new OperationSymbol("-=", OperationGroup.MathAssigment, OperationType.MinusAssigment),
            new OperationSymbol("/=", OperationGroup.MathAssigment, OperationType.DivideAssigment),
            new OperationSymbol("*=", OperationGroup.MathAssigment, OperationType.MultiAssigment),
            // assigment
            new OperationSymbol("=", OperationGroup.Assigment, OperationType.Assigment),
            // variable
            new OperationSymbol("!", OperationGroup.Variable, OperationType.Not),
            new OperationSymbol("++", OperationGroup.Variable, OperationType.Increment),
            new OperationSymbol("--", OperationGroup.Variable, OperationType.Decrement),
            // basic-math
            new OperationSymbol("+", OperationGroup.Math, OperationType.Add),
            new OperationSymbol("-", OperationGroup.Math, OperationType.Minus),
            new OperationSymbol("/", OperationGroup.Math, OperationType.Divide),
            new OperationSymbol("*", OperationGroup.Math, OperationType.Multi),
            new OperationSymbol("%", OperationGroup.Math, OperationType.Modulo),
            // type
            new OperationSymbol("is_type", OperationGroup.Type, OperationType.IsType),
            new OperationSymbol("has_type", OperationGroup.Type, OperationType.HasType),
            new OperationSymbol("as_type", OperationGroup.Type, OperationType.AsType),
            new OperationSymbol("get_type", OperationGroup.Type, OperationType.GetType)
        };
        public static readonly MapCollection<string, OperationSymbol> StringMap = new MapCollection<string, OperationSymbol>();

        static Operations()
        {
            for(int i=0; i<Array.Length; i++)
            {
                OperationSymbol operationSymbol = Array[i];
                StringMap.Add(operationSymbol.String, operationSymbol);
            }
        }
    }

    public class OperationSymbol
    {
        public readonly OperationType Type;
        public readonly OperationGroup Group;
        public readonly string String;

        public OperationSymbol(string SymbolString, OperationGroup Group, OperationType Type)
        {
            this.Type = Type;
            this.Group = Group;
            this.String = SymbolString;
        }
    }

}
