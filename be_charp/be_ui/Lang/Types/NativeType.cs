using System;
using Be.Runtime.Format;

namespace Be.Runtime.Types
{
    public enum NativeType
    {
        None,
        Void,
        Bool,
        String,
        Byte,
        Char,
        Int,
        Long,
        Float,
        Double,
        Object,
        Type,
        Var,
    }

    public enum NativeGroup
    {
        None,
        Void,
        Boolean,
        Number,
        String,
        Type,
        Object,
        Var
    }

    public enum NativeNumberGroup
    {
        None,
        Integral,
        Floating,
    }

    public enum NativeNumberCategory
    {
        None,
        Signed,
        Unsigned,
    }

    public class NativeBools
    {
        public static readonly string True = "true";
        public static readonly string False = "false";
    }

    public class NativrCollection : ListCollection<NativeSymbol>
    { }

    public class NativeSymbol : ObjectSymbol
    {
        public readonly string String;
        public readonly NativeType Type;
        public readonly NativeGroup Group;
        public readonly NativeNumberGroup NumberGroup;
        public readonly NativeNumberCategory NumberCategory;

        public NativeSymbol(string TypeName, NativeType Type, NativeGroup Group) : base(TypeName, null, AccessorConst.NoneType, null, null, null, false)
        {
            this.String = TypeName;
            this.IsNative = true;
            this.Type = Type;
            this.Group = Group;
            this.NumberGroup = NativeNumberGroup.None;
            this.NumberCategory = NativeNumberCategory.None;
        }

        public NativeSymbol(string TypeName, NativeType Type, NativeGroup Group, NativeNumberGroup NumberGroup, NativeNumberCategory NumberCategory) : base(TypeName, null, AccessorConst.NoneType, null, null, null, false)
        {
            this.String = TypeName;
            this.IsNative = true;
            this.Type = Type;
            this.Group = Group;
            this.NumberGroup = NumberGroup;
            this.NumberCategory = NumberCategory;
        }

        public bool IsEqual(NativeSymbol compare)
        {
            return (String == compare.String);
        }
    }

    public static class Natives
    {
        public static readonly NativeSymbol[] Array = new NativeSymbol[]
        {
            new NativeSymbol("void", NativeType.Void, NativeGroup.Void),
            new NativeSymbol("bool", NativeType.Bool, NativeGroup.Boolean),
            new NativeSymbol("byte", NativeType.Byte, NativeGroup.Number, NativeNumberGroup.Integral, NativeNumberCategory.Signed),
            new NativeSymbol("int", NativeType.Int, NativeGroup.Number, NativeNumberGroup.Integral, NativeNumberCategory.Signed),
            new NativeSymbol("uint", NativeType.Int, NativeGroup.Number, NativeNumberGroup.Integral, NativeNumberCategory.Unsigned),
            new NativeSymbol("long", NativeType.Long, NativeGroup.Number, NativeNumberGroup.Integral, NativeNumberCategory.Signed),
            new NativeSymbol("ulong", NativeType.Long, NativeGroup.Number, NativeNumberGroup.Integral, NativeNumberCategory.Unsigned),
            new NativeSymbol("float", NativeType.Float, NativeGroup.Number, NativeNumberGroup.Floating, NativeNumberCategory.Signed),
            new NativeSymbol("ufloat", NativeType.Float, NativeGroup.Number, NativeNumberGroup.Floating, NativeNumberCategory.Unsigned),
            new NativeSymbol("double", NativeType.Double, NativeGroup.Number, NativeNumberGroup.Floating, NativeNumberCategory.Signed),
            new NativeSymbol("udouble", NativeType.Double, NativeGroup.Number, NativeNumberGroup.Floating, NativeNumberCategory.Unsigned),
            new NativeSymbol("object", NativeType.Object, NativeGroup.Object),
            new NativeSymbol("type", NativeType.Type, NativeGroup.Type),
            new NativeSymbol("char", NativeType.Char, NativeGroup.String),
            new NativeSymbol("string", NativeType.String, NativeGroup.String),
            new NativeSymbol("var", NativeType.Var, NativeGroup.Var),
        };
        public static readonly MapCollection<string, NativeSymbol> StringMap = new MapCollection<string, NativeSymbol>();

        static Natives()
        {
            for(int i=0; i<Array.Length; i++)
            {
                NativeSymbol nativeType = Array[i];
                StringMap.Add(nativeType.String, nativeType);
            }
        }

        public static NativeSymbol GetTypeByName(string TypeName)
        {
            int index = GetIndexByName(TypeName);
            if(index == -1)
            {
                return null;
            }
            return Array[index];
        }

        public static int GetIndexByName(string TypeName)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i].String.Equals(TypeName))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public class NativeUtils
    {
        public static NativeSymbol GetNativeType(NativeType nativeType, NativeNumberCategory nativeNumberType)
        {
            if(nativeType == NativeType.None)
            {
                return null;
            }
            for(int i=0; i < Natives.Array.Length; i++)
            {
                if(Natives.Array[i].Type == nativeType && 
                   Natives.Array[i].NumberCategory == nativeNumberType
                ){
                    return Natives.Array[i];
                }
            }
            return null;
        }


        public static NativeType GetNativeTypeEnum(char chr)
        {
            if (chr == 'f')
            {
                return NativeType.Float;
            }
            else if (chr == 'd')
            {
                return NativeType.Double;
            }
            else if (chr == 'i')
            {
                return NativeType.Int;
            }
            else if (chr == 'l')
            {
                return NativeType.Long;
            }
            else
            {
                return NativeType.None;
            }
        }

        public static NativeNumberCategory GetNativeNumberTypeEnum(char chr)
        {
            if (chr == 'u')
            {
                return NativeNumberCategory.Unsigned;
            }
            else if(chr == 's')
            {
                return NativeNumberCategory.Signed;
            }
            else
            {
                return NativeNumberCategory.None;
            }
        }

        public static NativeSymbol GetHighestPrecisionNativeType(NativeSymbol one, NativeSymbol two)
        {
            NativeSymbol result;
            // get highest precsion bit-size
            if (Natives.GetIndexByName(one.String) > Natives.GetIndexByName(two.String))
            {
                result = one;
            }
            else
            {
                result = two;
            }
            // prefer unsigned if from same base number-type
            if(one.NumberCategory == two.NumberCategory)
            {
                if (one.NumberCategory == NativeNumberCategory.Unsigned)
                {
                    return one;
                }
                else if(two.NumberCategory == NativeNumberCategory.Unsigned)
                {
                    return two;
                }
            }
            // return base-result
            return result;
        }
    }

    public class NativeDeclarationType
    {
        public ListCollection<OperandType> ConstructorDeclarationList = new ListCollection<OperandType>();
        public MapCollection<string, OperandType> MemberDeclarationMap = new MapCollection<string, OperandType>();
    }
}
