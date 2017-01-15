namespace Be.Runtime.Types
{
    public class CodeType
    {
        public MethodType MethodType;
        public PropertySymbol Property;
        public StatementCollection Statements = new StatementCollection();   

        public CodeType(MethodType parentMethod)
        {
            this.MethodType = parentMethod;
        }
    }
}
