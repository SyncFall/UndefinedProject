using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public enum OperationType
    {
        // logic and-or
        LOGIC_AND,
        LOGIC_OR,
        // logic equal-not
        EQUAL,
        NOT_EQUAL,
        // logic relation-compare
        LESS_EQUAL,
        GREATER_EQUAL,
        LESS_THAN,
        GREATER_THAN,
        // math-assigment
        ADD_ASSIGMENT,
        MINUS_ASSIGMENT,
        DIVIDE_ASSIGMENT,
        MULTIPLICATE_ASSIGMENT,
        // assigment
        ASSIGMENT,
        // basic-math
        ADD,
        MINUS,
        MULTIPLICATE,
        DIVIDE,
        MODULO,
        // type
        IS_TYPE,
        HAS_TYPE,
        AS_TYPE,
        GET_TYPE,
    }

    public enum OperationCategory
    {
        Logic,
        BasicMath,
        MathAssigment,
        Type,
    }

    public enum OperationGroup
    {
        LogicAndOr,
        LogicEqualNot,
        LogicRelationCompare,
        BasicMath,
        MathAssigment,
        BasicAssigment,
        Type,
    }

    public class Operations
    {
        public static readonly OperationSymbol[] Array = new OperationSymbol[]
        {
            // logic and-or
            new OperationSymbol("&&", OperationCategory.Logic, OperationGroup.LogicAndOr, OperationType.LOGIC_AND),
            new OperationSymbol("||", OperationCategory.Logic, OperationGroup.LogicAndOr, OperationType.LOGIC_OR),
            // logic equal-not
            new OperationSymbol("==", OperationCategory.Logic, OperationGroup.LogicEqualNot, OperationType.EQUAL),
            new OperationSymbol("!=", OperationCategory.Logic, OperationGroup.LogicEqualNot, OperationType.NOT_EQUAL),
            // logic relation-compare
            new OperationSymbol("<=", OperationCategory.Logic, OperationGroup.LogicRelationCompare, OperationType.LESS_EQUAL),
            new OperationSymbol(">=", OperationCategory.Logic, OperationGroup.LogicRelationCompare, OperationType.GREATER_EQUAL),
            new OperationSymbol("<", OperationCategory.Logic, OperationGroup.LogicRelationCompare, OperationType.LESS_THAN),
            new OperationSymbol(">", OperationCategory.Logic, OperationGroup.LogicRelationCompare, OperationType.GREATER_THAN),
            // math-assigment
            new OperationSymbol("+=", OperationCategory.MathAssigment, OperationGroup.MathAssigment, OperationType.ADD_ASSIGMENT),
            new OperationSymbol("-=", OperationCategory.MathAssigment, OperationGroup.MathAssigment, OperationType.MINUS_ASSIGMENT),
            new OperationSymbol("/=", OperationCategory.MathAssigment, OperationGroup.MathAssigment, OperationType.DIVIDE_ASSIGMENT),
            new OperationSymbol("*=", OperationCategory.MathAssigment, OperationGroup.MathAssigment, OperationType.MULTIPLICATE_ASSIGMENT),
            // assigment
            new OperationSymbol("=", OperationCategory.MathAssigment, OperationGroup.BasicAssigment, OperationType.ASSIGMENT),
            // basic-math
            new OperationSymbol("+", OperationCategory.BasicMath, OperationGroup.BasicMath, OperationType.ADD),
            new OperationSymbol("-", OperationCategory.BasicMath, OperationGroup.BasicMath, OperationType.MINUS),
            new OperationSymbol("/", OperationCategory.BasicMath, OperationGroup.BasicMath, OperationType.DIVIDE),
            new OperationSymbol("*", OperationCategory.BasicMath, OperationGroup.BasicMath, OperationType.MULTIPLICATE),
            new OperationSymbol("%", OperationCategory.BasicMath, OperationGroup.BasicMath, OperationType.MODULO),
            // type
            new OperationSymbol("is_type", OperationCategory.Type, OperationGroup.Type, OperationType.IS_TYPE),
            new OperationSymbol("has_type", OperationCategory.Type, OperationGroup.Type, OperationType.HAS_TYPE),
            new OperationSymbol("as_type", OperationCategory.Type, OperationGroup.Type, OperationType.AS_TYPE),
            new OperationSymbol("get_type", OperationCategory.Type, OperationGroup.Type, OperationType.GET_TYPE)
        };
    }

    public class OperationSymbol
    {
        public readonly string String;
        public readonly OperationType Type;
        public readonly OperationGroup Group;
        public readonly OperationCategory Category;

        public OperationSymbol(string SymbolString, OperationCategory Category, OperationGroup Group, OperationType Type)
        {
            this.String = SymbolString;
            this.Category = Category;
            this.Group = Group;
            this.Type = Type;
        }
    }

}
