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
        ObjectDec,
        VariableDec,
        FunctionDec,
        PropertyDec,
        BlockDef,
        TypeDec,
        GenericDec,
        GenericElm,
        ArrayDec,
        ArrayParam,
        ParamDec,
        Param,
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
        // structed/visual-elements
        StructedBlock,
        StructedAttribute,
    }
}
