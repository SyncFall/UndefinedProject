using Bee.Library;
using System;

namespace Bee.Language
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

    public enum NativeNumberType
    {
        None,
        Integral,
        Floating,
    }

    public enum NativeNumberGroup
    {
        None,
        Signed,
        Unsigned,
    }

    public class NativeSymbol
    {
        public readonly string String;
        public readonly NativeType Type;
        public readonly NativeGroup Group;
        public readonly NativeNumberType NumberGroup;
        public readonly NativeNumberGroup NumberCategory;

        public NativeSymbol(string TypeName, NativeType Type, NativeGroup Group)
        {
            this.String = TypeName;
            this.Type = Type;
            this.Group = Group;
            this.NumberGroup = NativeNumberType.None;
            this.NumberCategory = NativeNumberGroup.None;
        }

        public NativeSymbol(string TypeName, NativeType Type, NativeGroup Group, NativeNumberType NumberGroup, NativeNumberGroup NumberCategory)
        {
            this.String = TypeName;
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
            new NativeSymbol("byte", NativeType.Byte, NativeGroup.Number, NativeNumberType.Integral, NativeNumberGroup.Signed),
            new NativeSymbol("int", NativeType.Int, NativeGroup.Number, NativeNumberType.Integral, NativeNumberGroup.Signed),
            new NativeSymbol("uint", NativeType.Int, NativeGroup.Number, NativeNumberType.Integral, NativeNumberGroup.Unsigned),
            new NativeSymbol("long", NativeType.Long, NativeGroup.Number, NativeNumberType.Integral, NativeNumberGroup.Signed),
            new NativeSymbol("ulong", NativeType.Long, NativeGroup.Number, NativeNumberType.Integral, NativeNumberGroup.Unsigned),
            new NativeSymbol("float", NativeType.Float, NativeGroup.Number, NativeNumberType.Floating, NativeNumberGroup.Signed),
            new NativeSymbol("ufloat", NativeType.Float, NativeGroup.Number, NativeNumberType.Floating, NativeNumberGroup.Unsigned),
            new NativeSymbol("double", NativeType.Double, NativeGroup.Number, NativeNumberType.Floating, NativeNumberGroup.Signed),
            new NativeSymbol("udouble", NativeType.Double, NativeGroup.Number, NativeNumberType.Floating, NativeNumberGroup.Unsigned),
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
        public static NativeSymbol GetNativeType(NativeType nativeType, NativeNumberGroup nativeNumberType)
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

        public static NativeNumberGroup GetNativeNumberTypeEnum(char chr)
        {
            if (chr == 'u')
            {
                return NativeNumberGroup.Unsigned;
            }
            else if(chr == 's')
            {
                return NativeNumberGroup.Signed;
            }
            else
            {
                return NativeNumberGroup.None;
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
                if (one.NumberCategory == NativeNumberGroup.Unsigned)
                {
                    return one;
                }
                else if(two.NumberCategory == NativeNumberGroup.Unsigned)
                {
                    return two;
                }
            }
            // return base-result
            return result;
        }
    }
}
