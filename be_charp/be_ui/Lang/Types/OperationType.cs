using System;

namespace Be.Runtime.Types
{
    public enum OperationTypeEnum
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

    public enum OperationCategoryEnum
    {
        LOGIC,
        BASIC_MATH,
        MATH_ASSIGMENT,
        TYPE,
    }

    public enum OperationGroupEnum
    {
        LOGIC_AND_OR,
        LOGIC_EQUAL_NOT,
        LOGIC_RELATION_COMPARE,
        BASIC_MATH,
        MATH_ASSIGMENT,
        BASIC_ASSIGMENT,
        TYPE,
    }

    public class OperationConst
    {
        public static readonly OperationType[] OperationTypeArray = new OperationType[]
        {
            // logic and-or
            new OperationType("&&", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_AND_OR, OperationTypeEnum.LOGIC_AND),
            new OperationType("||", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_AND_OR, OperationTypeEnum.LOGIC_OR),
            // logic equal-not
            new OperationType("==", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_EQUAL_NOT, OperationTypeEnum.EQUAL),
            new OperationType("!=", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_EQUAL_NOT, OperationTypeEnum.NOT_EQUAL),
            // logic relation-compare
            new OperationType("<=", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_RELATION_COMPARE, OperationTypeEnum.LESS_EQUAL),
            new OperationType(">=", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_RELATION_COMPARE, OperationTypeEnum.GREATER_EQUAL),
            new OperationType("<", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_RELATION_COMPARE, OperationTypeEnum.LESS_THAN),
            new OperationType(">", OperationCategoryEnum.LOGIC, OperationGroupEnum.LOGIC_RELATION_COMPARE, OperationTypeEnum.GREATER_THAN),
            // math-assigment
            new OperationType("+=", OperationCategoryEnum.MATH_ASSIGMENT, OperationGroupEnum.MATH_ASSIGMENT, OperationTypeEnum.ADD_ASSIGMENT),
            new OperationType("-=", OperationCategoryEnum.MATH_ASSIGMENT, OperationGroupEnum.MATH_ASSIGMENT, OperationTypeEnum.MINUS_ASSIGMENT),
            new OperationType("/=", OperationCategoryEnum.MATH_ASSIGMENT, OperationGroupEnum.MATH_ASSIGMENT, OperationTypeEnum.DIVIDE_ASSIGMENT),
            new OperationType("*=", OperationCategoryEnum.MATH_ASSIGMENT, OperationGroupEnum.MATH_ASSIGMENT, OperationTypeEnum.MULTIPLICATE_ASSIGMENT),
            // assigment
            new OperationType("=", OperationCategoryEnum.MATH_ASSIGMENT, OperationGroupEnum.BASIC_ASSIGMENT, OperationTypeEnum.ASSIGMENT),
            // basic-math
            new OperationType("+", OperationCategoryEnum.BASIC_MATH, OperationGroupEnum.BASIC_MATH, OperationTypeEnum.ADD),
            new OperationType("-", OperationCategoryEnum.BASIC_MATH, OperationGroupEnum.BASIC_MATH, OperationTypeEnum.MINUS),
            new OperationType("/", OperationCategoryEnum.BASIC_MATH, OperationGroupEnum.BASIC_MATH, OperationTypeEnum.DIVIDE),
            new OperationType("*", OperationCategoryEnum.BASIC_MATH, OperationGroupEnum.BASIC_MATH, OperationTypeEnum.MULTIPLICATE),
            new OperationType("%", OperationCategoryEnum.BASIC_MATH, OperationGroupEnum.BASIC_MATH, OperationTypeEnum.MODULO),
            // type
            new OperationType("is_type", OperationCategoryEnum.TYPE, OperationGroupEnum.TYPE, OperationTypeEnum.IS_TYPE),
            new OperationType("has_type", OperationCategoryEnum.TYPE, OperationGroupEnum.TYPE, OperationTypeEnum.HAS_TYPE),
            new OperationType("as_type", OperationCategoryEnum.TYPE, OperationGroupEnum.TYPE, OperationTypeEnum.AS_TYPE),
            new OperationType("get_type", OperationCategoryEnum.TYPE, OperationGroupEnum.TYPE, OperationTypeEnum.GET_TYPE)
        };
    }

    public class OperationType
    {
        public string SymbolString;
        public OperationCategoryEnum Category;
        public OperationGroupEnum Group;
        public OperationTypeEnum Type;

        public OperationType(string SymbolString, OperationCategoryEnum Category, OperationGroupEnum Group, OperationTypeEnum Type)
        {
            this.SymbolString = SymbolString;
            this.Category = Category;
            this.Group = Group;
            this.Type = Type;
        }

#if(TRACK)
        public string GetDebugText()
        {
            string str = "operation ";
            str += " | symbol: '"+SymbolString+"'";
            str += " | category: '" + Utils.EnumToString(Category) + "'";
            str += " | group: '" + Utils.EnumToString(Group) + "'";
            str += " | type: '" + Utils.EnumToString(Type) + "'";
            return str;
        }
#endif
    }
}
