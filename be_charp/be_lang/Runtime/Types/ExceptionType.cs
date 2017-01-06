namespace Be.Runtime.Types
{
    public class ExcpetionType : ObjectSymbol
    {
        public ExcpetionType(string Name, AttributeType Attribute, AccessorType Accessor, GenericType Generics, ExtendSymbol Extend, ImplementCollection Implement, bool IsVirtual) : base(Name, Attribute, Accessor, Generics, Extend, Implement, IsVirtual)
        {
            this.IsException = true;
        }
    }
}
