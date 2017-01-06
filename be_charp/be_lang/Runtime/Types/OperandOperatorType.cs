namespace Be.Runtime.Types
{
    public enum OperandOperatorEnum
    {
        MEMBER_ACCESS = 1,
        METHOD_CALL = 2,
        ARRAY_ACCESS = 3,
    }

    public class OperandOperatorCollection : ListCollection<OperandOperatorType>
    { }  

    public abstract class OperandOperatorType
    {
        public OperandOperatorEnum Type;

        public OperandOperatorType(OperandOperatorEnum Type)
        {
            this.Type = Type;
        }

#if(TRACK)
        public string GetDebugText()
        {
            string str = "operand_operation: ";
            str += "'"+Utils.EnumToString(Type)+"' ";
            str += GetChildDebugText();
            return str;
        }

        public virtual string GetChildDebugText()
        {
            return "";
        }
#endif
    }

    public class MemberAccessOperatorType : OperandOperatorType
    {
        public string MemberName;
        public ObjectSymbol MemberObjectType;
        public MemberType MemberType;

        public MemberAccessOperatorType() : base(OperandOperatorEnum.MEMBER_ACCESS)
        { }

#if(TRACK)
        public override string GetChildDebugText()
        {
            return "| member: '" + MemberName+"'";
        }
#endif  
    }

    public class MethodCallOperatorType : OperandOperatorType
    {
        public string MethodName;
        public GenericType GenericType;
        public ExpressionCollection ParameterExpressions = new ExpressionCollection();
        public MethodType MethodType;
        public ObjectSymbol ReturnObjectType;
       
        public MethodCallOperatorType() : base(OperandOperatorEnum.METHOD_CALL)
        { }

#if(TRACK)
        public override string GetChildDebugText()
        {
            return "| method: '" + MethodName+"'";
        }
#endif
    }

    public class ArrayAccessOperatorType : OperandOperatorType
    {
        public ArrayParameter ArrayParameter;

        public ArrayAccessOperatorType() : base(OperandOperatorEnum.ARRAY_ACCESS)
        { }

#if(TRACK)
        public override string GetChildDebugText()
        {
            return "| array_access:"; ;
        }
#endif
    }
}
