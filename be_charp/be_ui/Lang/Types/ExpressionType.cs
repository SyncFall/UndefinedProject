namespace Be.Runtime.Types
{
    public enum ExpressionTypeEnum
    {
        MEMBER_INITIALISATION,
        PAIR_OPERATION,
        CONDITION,
        FUNCTION_CONTROL,
        ERROR_CONTROL,
        ASSIGMENT,
        STATEMENT,
        PARAMETER,
    }

    public class ExpressionCollection : ListCollection<ExpressionType>
    { }

    public class ExpressionType
    {
        public ExpressionOperatorType FrontExpressionOperator;
        public ExpressionOperatorType BackExpressionOperator;
        public ExpressionType ChildExpression;
        public OperandType Operand;
        public ExpressionOperationCollection OperationsWithExpressions;
        public ObjectSymbol OperationObjectType;
    }

    public class ExpressionOperationCollection : ListCollection<ExpressionOperation>
    { }

    public class ExpressionOperation
    {
        public OperationType Operation;
        public ExpressionType SecondExpression;
    }

    public class ExpressionUtils
    {
        public static ExpressionType GetOperationDepthExpressionType(ExpressionType expressionType)
        {
            while (expressionType.OperationsWithExpressions == null && expressionType.ChildExpression != null)
            {
                expressionType = expressionType.ChildExpression;
            }
            return expressionType;
        }

        public static ExpressionType GetBeginningExpressionType(ExpressionType expressionType)
        {
            while (expressionType.Operand == null && expressionType.ChildExpression != null)
            {
                expressionType = expressionType.ChildExpression;
            }
            return expressionType;
        }

        public static OperandType GetBeginnigOperandType(ExpressionType expressionType)
        {
            return GetBeginningExpressionType(expressionType).Operand;
        }
    }
}
