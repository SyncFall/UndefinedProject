using System;

namespace Be.Runtime.Types
{
    public class MemberCollection : ListCollection<MemberType>
    {
        public MemberType GetByName(string Name)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).MemberName.Equals(Name))
                {
                    return this.Get(i);
                }
            }
            return null;
        }

        public bool MatchNativeSignaturOrNone(MapCollection<string, OperandType> memberDeclarationMap)
        {
            if (memberDeclarationMap.Size() == 0)
            {
                return true;
            }
            string[] memberNames = memberDeclarationMap.GetKeys();
            for(int i=0; i<memberNames.Length; i++)
            {
                OperandType targetLiteralOperandType = memberDeclarationMap.GetValue(memberNames[i]);
                NativeSymbol targetNativeType = targetLiteralOperandType.GetNativeType();
                if(targetNativeType == null)
                {
                    throw new Exception("invalid state");
                }
                bool found = false;
                for(int j=0; j<this.Size(); j++)
                {
                    if(this.Get(j).EqualNamePublicAndNativeType(memberNames[i], targetNativeType))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class MemberType
    {
        public AttributeType Attributes;
        public AccessorType Accessor;
        public GenericType GenericType;

        public bool isStatic;
        public bool isReadonly;

        public string MemberName;
        public string TypeName;
        public ExpressionType InitialisationExpression;
        public ObjectSymbol ObjectType;

        public MemberType(string TypeName, string MemberName, AttributeType Attribute, AccessorType Accessor, GenericType Generics)
        {
            this.MemberName = MemberName;
            this.TypeName = TypeName;
            this.Attributes = Attribute;
            this.Accessor = Accessor;
            this.GenericType = Generics;
            if(this.GenericType != null)
            {
                this.GenericType.CreateSignatur();
            }
        }

        public bool EqualName(MemberType memberType)
        {
            return (this.MemberName.Equals(memberType.MemberName));
        }

        public bool EqualNamePublicAndNativeType(string targetMemberName, NativeSymbol targetNativeType)
        {
            return (
                this.MemberName.Equals(targetMemberName) &&
                (this.Accessor.Type == AccessorTypeEnum.NONE || this.Accessor.Type == AccessorTypeEnum.PUBLIC) &&
                this.ObjectType.IsCompilantWith(targetNativeType)
            );
        }
    }
}
