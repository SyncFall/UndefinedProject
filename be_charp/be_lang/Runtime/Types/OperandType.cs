using System;
using Be.Runtime.Format;

namespace Be.Runtime.Types
{
    public enum OperandTypeEnum
    {   
        BOOL,
        CHAR,
        NUMBER,
        STRING,
        NEW_TYPE,
        METHOD,
        MEMBER_OR_VARIABLE_OR_PARAMETER,
        MEMBER,
        VARIABLE,
        PARAMETER,
        TYPE_OBJECT,
        THIS,
        BASE,
        NULL,
        VALUE,
    }

    public class OperandConst
    {
        public static readonly string This = "this";
        public static readonly string Base = "base";
        public static readonly string Null = "null";
        public static readonly string Value = "value";
    }

    public abstract class OperandType
    {
        public OperandTypeEnum Type;
        public OperandOperatorCollection Operations = new OperandOperatorCollection();
        public ObjectSymbol OperandObjectType;
        public ObjectSymbol OperationObjectType;

        public OperandType(OperandTypeEnum Type)
        {
            this.Type = Type;
        }

        public NativeSymbol GetNativeType()
        {
            if(this.Type == OperandTypeEnum.BOOL)
            {
                return Natives.GetTypeByName("bool");
            }
            else if(this.Type == OperandTypeEnum.CHAR)
            {
                return Natives.GetTypeByName("char");
            }
            else if(this.Type == OperandTypeEnum.STRING)
            {
                return Natives.GetTypeByName("string");
            }
            else if(this.Type == OperandTypeEnum.NUMBER)
            {
                NumberOperand numberOperand = this as NumberOperand;
                return NativeUtils.GetNativeType(numberOperand.NativeTypeEnum, numberOperand.NativeNumberTypeEnum);
            }
            else
            {
                return null;
            }
        }

#if(TRACK)
        public virtual string GetDebugText()
        {
            return "operand: '"+Utils.EnumToString(Type)+"'";
        }
#endif
    }

    public class BoolOperand : OperandType
    {
        public bool BoolDataValue;

        public BoolOperand(bool boolValue) : base(OperandTypeEnum.BOOL)
        {
            this.BoolDataValue = boolValue;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | bool-value: '" + BoolDataValue + "'";
        }
#endif
    }

    public class CharOperand : OperandType
    {
        public string CharContentString;
        public CharType CharType;

        public CharOperand(string charContentString, CharType charType) : base(OperandTypeEnum.CHAR)
        {
            this.CharContentString = charContentString;
            this.CharType = charType;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | char-value: '" + CharContentString + "'";
        }
#endif
    }

    public class NumberOperand : OperandType
    {
        public string NumberContentString;
        public NativeType NativeTypeEnum;
        public NativeNumberCategory NativeNumberTypeEnum;

        public NumberOperand(string numberContentString, NativeType NativeTypeEnum, NativeNumberCategory NativeNumberTypeEnum) : base(OperandTypeEnum.NUMBER)
        {
            this.NumberContentString = numberContentString;
            this.NativeTypeEnum = NativeTypeEnum;
            this.NativeNumberTypeEnum = NativeNumberTypeEnum;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | number-value: '" + NumberContentString + "'";
        }
#endif
    }

    public class StringOperand : OperandType
    {
        public string StringDataValue;

        public StringOperand(string stringValue) : base(OperandTypeEnum.STRING)
        {
            this.StringDataValue = stringValue;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | string-value: '" + StringDataValue + "'";
        }
#endif
    }

    public class NewTypeOperand : OperandType
    {
        public string NewTypeName;
        public GenericType GenericType;
        public ObjectInitialisationType ObjectDefinitionType;
        public ArrayType ArrayInitiationType;

        public NewTypeOperand(string newTypeName) : base(OperandTypeEnum.NEW_TYPE)
        {
            this.NewTypeName = newTypeName;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | new-type: '" + NewTypeName + "'";
        }
#endif
    }

    public class MethodOperand : OperandType
    {
        public string MethodName;
        public GenericType MethodGeneric;
        public MethodType MethodType;
        public ExpressionCollection ParameterExpressions = new ExpressionCollection();

        public MethodOperand(string methodName, GenericType methodGenerics) : base(OperandTypeEnum.METHOD)
        {
            this.MethodName = methodName;
            this.MethodGeneric = methodGenerics;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | method-name: '" + MethodName + "'";
        }
#endif
    }

    public class MemberVariableOrParameterOperand : OperandType
    {
        public string PathName;
        public MemberType MemberType;
        public VariableType VariableType;
        public ParameterType ParameterType;
        public ObjectSymbol TypeObjectType;

        public MemberVariableOrParameterOperand(string pathName) : base(OperandTypeEnum.MEMBER_OR_VARIABLE_OR_PARAMETER)
        {
            this.PathName = pathName;
        }

#if(TRACK)
        public override string GetDebugText()
        {
            return base.GetDebugText() + " | variable-name: '" + PathName + "'";
        }
#endif
    }

    public class ThisOperand : OperandType
    {
        public ThisOperand() : base(OperandTypeEnum.THIS)
        { }
    }

    public class BaseOperand : OperandType
    {
        public BaseOperand() : base(OperandTypeEnum.BASE)
        { }
    }

    public class NullOperand : OperandType
    {
        public NullOperand() : base(OperandTypeEnum.NULL)
        { }
    }

    public class ValueOperand : OperandType
    {
        public ValueOperand() : base(OperandTypeEnum.VALUE)
        { }
    }
}
