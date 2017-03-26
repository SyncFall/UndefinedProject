using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public static class SymbolLookup
    {
        public static SymbolTable Keywords = new SymbolTable();
        public static SymbolTable Structures = new SymbolTable();
        public static Symbol[] Operations = null;

        static SymbolLookup()
        {
            for (int i = 0; i < ObjectTypes.Array.Length; i++)
            {
                Keywords.AddSymbol(ObjectTypes.Array[i]);
            }
            for (int i = 0; i < LiteralConstants.Array.Length; i++)
            {
                Keywords.AddSymbol(LiteralConstants.Array[i]);
            }
            for (int i = 0; i < Natives.Array.Length; i++)
            {
                Keywords.AddSymbol(Natives.Array[i]);
            }
            for (int i = 0; i < Accessors.Array.Length; i++)
            {
                Keywords.AddSymbol(Accessors.Array[i]);
            }
            for (int i = 0; i < StatementKeywords.Array.Length; i++)
            {
                Keywords.AddSymbol(StatementKeywords.Array[i]);
            }
            for (int i = 0; i < Visuals.Array.Length; i++)
            {
                Keywords.AddSymbol(Visuals.Array[i]);
            }
            for (int i = 0; i < Language.Structures.Array.Length; i++)
            {
                Structures.AddSymbol(Language.Structures.Array[i]);
            }
            Operations = Language.Operations.Array;
        }
    }

}
