namespace Be.Runtime.Types
{
    public class EnumType : ObjectSymbol
    {
        public EnumItemCollection ItemCollection = new EnumItemCollection();

        public EnumType(string Name, AttributeType Attributes, AccessorType Accessor, ExtendSymbol Extend, ImplementCollection Implement, bool isVirtual) : base(Name, Attributes, Accessor, null, Extend, Implement, isVirtual)
        {
            this.IsEnum = true;
        }
    }

    public class EnumItemCollection : ListCollection<EnumItemType>
    { }

    public class EnumItemType
    {
        public string EnumItemName;
        public EnumItemDeclaratioType DeclarationType = new EnumItemDeclaratioType();
    }

    public class EnumItemDeclaratioType : NativeDeclarationType
    { }
}
