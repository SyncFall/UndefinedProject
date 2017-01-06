using System;

namespace Be.Runtime.Types
{
    public class MethodCollection : ListCollection<MethodType>
    {
        public MethodCollection GetAllByName(string methodName)
        {
            methodName = methodName.ToLower();
            MethodCollection methodCollection = new MethodCollection();
            for(int i=0; i<this.Size(); i++)
            {
                if (this.Get(i).Name.ToLower().Equals(methodName))
                {
                    methodCollection.Add(this.Get(i));
                }
            }
            return methodCollection;
        }

        public bool EqualNativeSignatur(ListCollection<OperandType> nativeTypeCollection)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).EqualsBasicNativeTypeSignatur(nativeTypeCollection))
                {
                    return true;
                }
            }
            return false;
        }

        public MethodType GetByBasicExpressionObjectTypeSignatur(ExpressionCollection expressionCollection)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).GetByBasicExpressionObjectTypeSignatur(expressionCollection))
                {
                    return this.Get(i);
                }
            }
            return null;
        }

        public MethodType GetByBasicExpressionObjectTypeSignatur(string MethodName, ExpressionCollection expressionCollection)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).Name.Equals(MethodName) && 
                    this.Get(i).GetByBasicExpressionObjectTypeSignatur(expressionCollection)
                ){
                    return this.Get(i);
                }
            }
            return null;
        }
    }

    public enum MethodCategory
    {
        CONSTRUCTOR,
        METHOD,
        PROPERTY,
    }

    public class MethodType
    {
        public AttributeType Attributes;
        public AccessorType Accessor;
        public GenericType GenericType;

        public bool IsConstructor;
        public bool IsMethod;
        public bool IsProperty;

        public bool IsVirtual;
        public bool IsStatic;
        public bool IsOverride;
        public bool IsExtern;

        public ObjectSymbol OverrideObjectType;

        public string Name;
        public ObjectSymbol ObjectType;
        public string ReturnType;
        public string ReturnGenericTypeName;
        public ObjectSymbol ReturnObjectType;
        public ParameterCollection ParameterCollection;
        public CodeType Code;
       
        public MethodType(string Name, ObjectSymbol objectType, AttributeType Attributes, AccessorType Accessor, GenericType Generics, string ReturnType, ParameterCollection ParameterCollection)
        {
            this.Name = Name;
            this.ObjectType = objectType;
            this.Attributes = Attributes;
            this.Accessor = Accessor;
            this.GenericType = Generics;
            if(this.GenericType != null)
            {
                this.GenericType.CreateSignatur();
            }
            this.ReturnType = ReturnType;
            this.ParameterCollection = ParameterCollection;
            this.Code = new CodeType(this);
        }

        public bool EqualBasicSignatur(MethodType compare)
        {
            if (!this.Name.Equals(compare.Name))
            {
                return false;
            }
            else if(!UtilType.EqualString(this.ReturnType, compare.ReturnType))
            {
                return false;
            }
            else if(!UtilType.EqualString(this.ReturnGenericTypeName, compare.ReturnGenericTypeName))
            {
                return false;
            }
            else if(this.IsMethod  && !this.ParameterCollection.EqualBasicSignatur(compare.ParameterCollection))
            {
                return false;
            }
            return true;
        }

        public bool EqualBaiscConstructorSignatur(MethodType methodType)
        {
            return (
              this.ParameterCollection.EqualBasicSignatur(methodType.ParameterCollection)
            );
        }

        public bool EqualsBasicNativeTypeSignatur(ListCollection<OperandType> nativeTypeCollection)
        {
            return (
              this.ParameterCollection.EqualsBasicNativeTypeSignatur(nativeTypeCollection)
            );
        }

        public bool GetByBasicExpressionObjectTypeSignatur(ExpressionCollection expressionCollection)
        {
            // none parameter methods
            if(this.ParameterCollection.Size() == 0 && expressionCollection.Size() == 0)
            {
                return true;
            }
            // not equals parameter count
            else if(this.ParameterCollection.Size() != expressionCollection.Size())
            {
                return false;
            }
            // equal-parameter object types methods
            for(int i=0; i<expressionCollection.Size(); i++)
            {
                if (!this.ParameterCollection.Get(i).ObjectType.Name.Equals(expressionCollection.Get(i).OperationObjectType.Name))
                {
                    return false;
                }
            }
            // match signatur object-types
            return true;
        }
    }

    public class MethodUtils
    {
        public static MethodCollection GetMethodCollectionByCategoryFromObjectType(MethodType methodType, ObjectSymbol objectSymbol)
        {
            if (methodType.IsConstructor)
            {
                return objectSymbol.Constructors;
            }
            else if (methodType.IsMethod)
            {
                return objectSymbol.Methods;
            }
            else if (methodType.IsProperty)
            {
                return objectSymbol.Properties;
            }
            else
            {
                throw new Exception("invalid state");
            }
        }
    }
}
