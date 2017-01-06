using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{

    public enum ExpressionOperatorTypeEnum
    {
        NOT_VALUE,
        POST_INCREMENT,
        POST_DECREMENT,
        PRE_INCREMENT,
        PRE_DECREMENT,
        POSITIV_VALUE,
        NEGATIV_VALUE,
    }

    public class ExpressionOperatorConst
    {
        public static readonly string Not = "!";
        public static readonly string Increment = "++";
        public static readonly string Decrement = "--";
        public static readonly string Negativ = "-";
        public static readonly string Positiv = "+";
    }

    public class ExpressionOperatorType
    {
        public ExpressionOperatorTypeEnum OperatorType;
        public string SymbolString;

        public ExpressionOperatorType(ExpressionOperatorTypeEnum operatorType, string SymbolString)
        {
            this.OperatorType = operatorType; 
        }

#if(TRACK)
        public string GetDebugText()
        {
            return "expression_operation: " + Utils.EnumToString(this.OperatorType);
        }
#endif
    }
}
