using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public enum SignatureType
    {
        // common
        Unknown,
        Seperator,
        Block,
        Keyword,
        // source-file
        Use,
        Scope,
        // path
        Identifier,
        IdentifierPath,
        IdentifierPathElement,
        // object, function, variable
        Object,
        Member,
        Method,
        Property,
        TypeDeclartion,
        ParameterDeclaration,
        ParameterElementDeclaration,
        // code
        Code,
        Statement,
        // expressions
        Expression,   
        ExpressionOperation,
        Operation,
        Operand,
        OperandOperation,
        // operand/-access
        LiteralAccess,
        VariableAccess,
        FunctionAccess,
        FunctionAccessParameter,
        ArrayAccess,
    }
}
