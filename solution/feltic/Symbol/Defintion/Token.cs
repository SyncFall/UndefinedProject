using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum TokenType
    {
        None=0,
        Structure,
        Literal,
        Native,
        Operation,
        Object,
        Accessor,
        Identifier,
        Statement,
        Comment,
        Region,
        Visual,
        Unknown,
    }
}
