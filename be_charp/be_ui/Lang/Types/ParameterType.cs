using System;

namespace Be.Runtime.Types
{
    public class ParameterCollection : ListCollection<ParameterType>
    {
        public ParameterType GetByName(string variableName)
        {
            for(int i=0; i<this.Size(); i++)
            {
                if (this.Get(i).EqualVariableName(variableName))
                {
                    return this.Get(i);
                }
            }
            return null;
        }

        public bool EqualName(string variableName)
        {
            return (GetByName(variableName) != null);
        }

        public bool EqualBasicSignatur(ParameterCollection paramaterCollection)
        {
            if(this.Size() != paramaterCollection.Size())
            {
                return false;
            }
            for (int i = 0; i < this.Size(); i++)
            {
                if (!this.Get(i).EqualBasicSignatur(paramaterCollection.Get(i)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool EqualsBasicNativeTypeSignatur(ListCollection<OperandType> nativeTypeCollection)
        {
            if (this.Size() != nativeTypeCollection.Size())
            {
                return false;
            }
            for (int i = 0; i < this.Size(); i++)
            {
                if (!this.Get(i).EqualsBasicNativeTypeSignatur(nativeTypeCollection.Get(i)))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ParameterType
    {
        public string TypeName;
        public string VariableName;
        public string GenericTypeName;
        public ObjectSymbol ObjectType;

        public ParameterType(string TypeName, string VariableName)
        {
            this.TypeName = TypeName;
            this.VariableName = VariableName;
        }

        public bool EqualBasicSignatur(ParameterType compare)
        {
            if (!UtilType.EqualString(TypeName, compare.TypeName))
            {
                return false;
            }
            else if (!UtilType.EqualString(GenericTypeName, compare.GenericTypeName))
            {
                return false;
            }
            return true;
        }

        public bool EqualVariableName(string variablename)
        {
            return (this.VariableName.Equals(variablename));
        }

        public bool EqualVariableName(ParameterType parameterType)
        {
            return (this.VariableName.Equals(parameterType.VariableName));
        }

        public bool EqualsBasicNativeTypeSignatur(OperandType operandLitaral)
        {
            NativeSymbol nativeType = operandLitaral.GetNativeType();
            if(nativeType == null)
            {
                throw new Exception("invalid state");
            }
            else if(!this.ObjectType.IsCompilantWith(nativeType))
            {
                return false;
            }
            return true;
        }
    }
}
