using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class SignatureParser
    {
        public BlockSignature TryBlock()
        {
            TokenSymbol openBlock = TryNonSpace(StructureType.BlockBegin);
            if(openBlock == null)
            {
                return null;
            }
            BlockSignature signature = new BlockSignature();
            signature.BlockStart = openBlock;
            if((signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null)
            {
                ;
            }
            return signature;
        }
    }

    public class BlockSignature : SignatureSymbol
    {
        public TokenSymbol BlockStart;
        public SignatureList ElementList;
        public TokenSymbol BlockEnd;

        public BlockSignature() : base(SignatureType.Block)
        {

        }
    }
}