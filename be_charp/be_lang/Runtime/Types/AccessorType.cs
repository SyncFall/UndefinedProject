namespace Be.Runtime.Types
{
    public enum AccessorTypeEnum
    {
        PRIVATE = 1,
        PUBLIC = 2,
        NONE = 3,
    }

    public class AccessorConst
    {
        public static readonly string PublicStr = "public";
        public static readonly string PrivateStr = "private";
        public static readonly AccessorType PublicType = new AccessorType(AccessorTypeEnum.PUBLIC);
        public static readonly AccessorType PrivateType = new AccessorType(AccessorTypeEnum.PRIVATE);
        public static readonly AccessorType NoneType = new AccessorType(AccessorTypeEnum.NONE);
    }

    public class AccessorType
    {
        public AccessorTypeEnum Type;

        public AccessorType(AccessorTypeEnum Type)
        {
            this.Type = Type;
        }

        public override string ToString()
        {
            return Utils.EnumToString(Type);
        }
    }
}
