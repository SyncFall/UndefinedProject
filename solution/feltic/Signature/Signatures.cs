using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum SignatureType
    {
        None=0,
        // common
        Unknown,
        // source
        Use,
        Scope,
        // path
        Identifier,
        Path,
        PathNode,
        // object, function, variable
        ObjectDef,
        VariableDef,
        FunctionDef,
        PropertyDef,
        BlockDef,
        TypeDef,
        GenericDef,
        GenericElm,
        ArrayDef,
        ArrayParam,
        ParamDef,
        ParamElm,
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
        ObjectOperand,
        LiteralOperand,
        VariableOperand,
        FunctionOperand,
        ArrayOperand,
        BlockOperand,
        StructedBlockOperand,
        // structed-block
        StructedBlock,
        StructedAttribute,
    }
}
