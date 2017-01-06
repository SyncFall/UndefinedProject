namespace Be.Runtime.Types
{
    public enum StatementTypeEnum
    {
        // inner-block
        INNER_BLOCK = 1,
        // condition-blocks
        IF = 2,
        ELSE_IF = 3,
        ELSE = 4,
        // loop-block
        WHILE = 10,
        DO_WHILE = 11,
        FOR = 12,
        FOR_EACH = 13,
        // function-control
        RETURN = 30,
        // loop-control
        CONTINUE = 40,
        BREAK = 41,
        // error-control
        THROW = 50,
        // error-processing
        TRY = 51,
        CATCH = 52,
        FINALLY = 53,
        // expression-statement
        EXPRESSION_STATEMENT = 60,
        // empty-statement
        NO_OPERATION = 70,
        // variable declaration and definition
        DECLARATION = 80,
        // synchronisation
        THREAD_SYNC = 90,
    }

    public enum StatementCategoryEnum
    {
        INNER_BLOCK = 1,
        CONDITION_BLOCK = 2,
        LOOP_BLOCK = 3,
        ERROR_BLOCK = 4,
        FUNCTION_CONTROL = 5,
        LOOP_CONTROL = 6,
        ERROR_CONTROL = 7,
        ERROR_PROCESSING = 8,
        EXPRESSION_STATEMENT = 9,
        VARIABLE_DECLARATION = 10,
        NO_OPERATION = 11,
        SYNCHRONISATION = 12,
    }

    public static class StatementConst
    {
        public static readonly string If = "if";
        public static readonly string Else = "else";
        public static readonly string Return = "return";
        public static readonly string Continue = "continue";
        public static readonly string Break = "break";
        public static readonly string Throw = "throw";
        public static readonly string While = "while";
        public static readonly string Do = "do";
        public static readonly string For = "for";
        public static readonly string Foreach = "foreach";
        public static readonly string Try = "try";
        public static readonly string Catch = "catch";
        public static readonly string Finally = "finally";
        public static readonly string Lock = "lock";
    }

    public class StatementCollection : ListCollection<StatementType>
    { }

    public abstract class StatementType
    {
        public StatementTypeEnum Type;
        public StatementCategoryEnum Category;
        public StatementType ParentStatement;
        public StatementCollection Statements = new StatementCollection();

        public StatementType(StatementTypeEnum Type, StatementCategoryEnum Category, StatementType ParentStatement)
        {
            this.Type = Type;
            this.Category = Category;
            this.ParentStatement = ParentStatement;
        }
    }

    public class ConditionStatementType : StatementType
    {
        public ExpressionType ConditionExpression;
        
        public ConditionStatementType(StatementTypeEnum Type, ExpressionType ConditionExpression, StatementType ParentStatement) : base(Type, StatementCategoryEnum.CONDITION_BLOCK, ParentStatement)
        {
            this.ConditionExpression = ConditionExpression;
        }
    }

    public abstract class LoopStatementType : StatementType
    {
        public LoopStatementType(StatementTypeEnum Type, StatementType ParentStatement) : base(Type, StatementCategoryEnum.LOOP_BLOCK, ParentStatement)
        { }
    }

    public class WhileLoopStatementType : LoopStatementType
    {
        public ExpressionType ConditionExpression;

        public WhileLoopStatementType(StatementType ParentStatement) : base(StatementTypeEnum.WHILE, ParentStatement)
        { }
    }

    public class DoWhileLoopStatementType : LoopStatementType
    {
        public ExpressionType ConditionExpression;

        public DoWhileLoopStatementType(StatementType ParentStatement) : base(StatementTypeEnum.DO_WHILE, ParentStatement)
        { }
    }

    public class ForLoopStatementType : LoopStatementType
    {
        public VariableCollection VariableDeclarationCollection;
        public ExpressionCollection ExpressionInitiationCollection;
        public ExpressionType ConditionExpression;
        public ExpressionCollection PostExpressionCollection;

        public ForLoopStatementType(StatementType ParentStatement) : base(StatementTypeEnum.FOR, ParentStatement)
        { }
    }

    public class ForeachLoopStatementType : LoopStatementType
    {
        public VariableType DeclarationVariable;
        public VariableType CollectionVariable;

        public ForeachLoopStatementType(StatementType ParentStatement) : base(StatementTypeEnum.FOR_EACH, ParentStatement)
        { }
    }

    public class LoopControlStatementType : StatementType
    {
        public LoopControlStatementType(StatementTypeEnum Type, StatementType ParentStatement)
            : base(Type, StatementCategoryEnum.LOOP_CONTROL, ParentStatement)
        { }
    }

    public class FunctionControlStatementType : StatementType
    {
        public ExpressionType ReturnExpression;

        public FunctionControlStatementType(StatementTypeEnum Type, StatementType ParentStatement)
            : base(Type, StatementCategoryEnum.FUNCTION_CONTROL, ParentStatement)
        { }
    }

    public class ErrorControlStatementType : StatementType
    {
        public ExpressionType ErrorExpression;

        public ErrorControlStatementType(StatementTypeEnum Type, StatementType ParentStatement)
            : base(Type, StatementCategoryEnum.ERROR_CONTROL, ParentStatement)
        { }
    }

    public class ErrorProcessigStatementType : StatementType
    {
        public VariableType DeclarationVariable;

        public ErrorProcessigStatementType(StatementTypeEnum Type, StatementType ParentStatement)
            : base(Type, StatementCategoryEnum.ERROR_PROCESSING, ParentStatement)
        { }
    }

    public class NoOperationStatementType : StatementType
    {
        public NoOperationStatementType(StatementType ParentStatement)
            : base(StatementTypeEnum.NO_OPERATION, StatementCategoryEnum.NO_OPERATION, ParentStatement)
        { }
    }

    public class InnerBlockStatementType : StatementType
    {
        public InnerBlockStatementType(StatementType ParentStatement) : base(StatementTypeEnum.INNER_BLOCK, StatementCategoryEnum.INNER_BLOCK, ParentStatement)
        { }
    }

    public class ExpressionStatementType : StatementType
    {
        public ExpressionType StatementExpression = null;

        public ExpressionStatementType(StatementType ParentStatement, ExpressionType ExpressionStatement) : base(StatementTypeEnum.EXPRESSION_STATEMENT, StatementCategoryEnum.EXPRESSION_STATEMENT, ParentStatement)
        {
            this.StatementExpression = ExpressionStatement;
        }
    }

    public class VariableDeclarationStatementType : StatementType
    {
        public VariableCollection VariableDeclarationCollection = new VariableCollection();

        public VariableDeclarationStatementType(StatementType ParentStatement) : base(StatementTypeEnum.DECLARATION, StatementCategoryEnum.VARIABLE_DECLARATION, ParentStatement)
        { }
    }

    public class SynchronisationStatementType : StatementType
    {
        public SynchronisationStatementType(StatementType ParentStatement) : base(StatementTypeEnum.THREAD_SYNC, StatementCategoryEnum.SYNCHRONISATION, ParentStatement)
        { }
    }
}
