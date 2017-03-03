﻿using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public partial class SignatureParser
    {
        public ExpressionSignature TryExpression()
        {
            TrySpace();
            ExpressionSignature signature = new ExpressionSignature();
            BlockSignature blockBegin;
            if ((blockBegin = TryBlock(StructureType.ClosingBegin)) != null)
            {
                signature.BlockBegin = blockBegin;
                if ((signature.ChildExpression = TryExpression()) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.ClosingEnd)) == null
                )
                {
                    return signature;
                }
            }
            else
            {
                if ((signature.Operand = TryOperand()) == null)
                {
                    return null;
                }
            }
            while (true)
            {
                OperationSignature operation = TryOperation();
                if (operation == null)
                {
                    break;
                }
                ExpressionSignature expressionPair = TryExpression();
                if (expressionPair == null)
                {
                    break;
                }
                signature.OperationList.Add(new ExpressionOperationPair(operation, expressionPair));
            }
            return signature;
        }

        public OperandSignatur TryOperand()
        {
            OperandSignatur signature = new OperandSignatur();
            AccessSignature accessSignatur = null;
            if (TryToken(TokenType.Literal) != null)
            {
                LiteralAccessSignature literalAccess = new LiteralAccessSignature(PrevToken.Symbol as LiteralSymbol);
                accessSignatur = literalAccess;
                signature.AccessSignatureList.Add(literalAccess);
                if ((literalAccess.Seperator = TrySeperator(StructureType.Point)) == null)
                {
                    return signature;
                }
            }
            else if (Token != null && Token.Type == TokenType.Identifier)
            {
                ;
            }
            else
            {
                return null;
            }
            while (Token != null && Token.Type == TokenType.Identifier)
            {
                IdentifierSignature identifier = TryIdentifier();
                if (TryBlock(StructureType.ClosingBegin) != null)
                {
                    FunctionAccessSignature functionAccess = new FunctionAccessSignature(identifier);
                    while (true)
                    {
                        ExpressionSignature expression = TryExpression();
                        if (expression == null)
                        {
                            break;
                        }
                        ParameterSignature parameter = new ParameterSignature(expression);
                        functionAccess.ParameterList.Add(parameter);
                        if ((parameter.Seperator = TrySeperator(StructureType.Seperator)) == null)
                        {
                            break;
                        }
                    }
                    if ((functionAccess.BlockEnd = TryBlock(StructureType.ClosingEnd)) == null)
                    {
                        ;
                    }
                    accessSignatur = functionAccess;
                }
                else if (TryBlock(StructureType.BracketBegin) != null)
                {
                    ArrayAccessSignature arrayAccess = new ArrayAccessSignature(identifier);
                    while (true)
                    {
                        ExpressionSignature expression = TryExpression();
                        if (expression == null)
                        {
                            break;
                        }
                        ParameterSignature parameter = new ParameterSignature(expression);
                        arrayAccess.ParameterList.Add(parameter);
                        if ((parameter.Seperator = TrySeperator(StructureType.Seperator)) == null)
                        {
                            break;
                        }
                    }
                    if ((arrayAccess.BlockEnd = TryBlock(StructureType.BracketEnd)) == null)
                    {
                        ;
                    }
                    accessSignatur = arrayAccess;
                }
                else
                {
                    VariableAccessSignature variableAccess = new VariableAccessSignature(identifier);
                    accessSignatur = variableAccess;
                }
                signature.AccessSignatureList.Add(accessSignatur);
                if ((accessSignatur.Seperator = TrySeperator(StructureType.Point)) == null)
                {
                    return signature;
                }
            }
            return signature;
        }

        public OperationSignature TryOperation()
        {
            TrySpace();
            if (TryToken(TokenType.Operation) == null)
            {
                return null;
            }
            OperationSignature signature = new OperationSignature(PrevToken.Symbol as OperationSymbol);
            TrySpace();
            return signature;
        }
    }


    public class ExpressionSignature : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ExpressionSignature ChildExpression;
        public BlockSignature BlockEnd;
        public OperandSignatur Operand;
        public ExpressionOperationList OperationList = new ExpressionOperationList();

        public ExpressionSignature() : base(SignatureType.Expression)
        { }

        public override string ToString()
        {
            string str = "";
            if (ChildExpression != null)
            {
                str += "child(" + ChildExpression + ")";
            }
            else if (Operand != null)
            {
                str += "operand(" + Operand + ")";
            }
            str += OperationList;
            return str;
        }
    }

    public class OperationSignature : SignatureSymbol
    {
        public OperationSymbol Symbol;

        public OperationSignature(OperationSymbol OperationSymbol) : base(SignatureType.Operation)
        {
            this.Symbol = OperationSymbol;
        }
    }

    public class ExpressionOperationList : ListCollection<ExpressionOperationPair>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i);
            }
            return str;
        }
    }

    public class ExpressionOperationPair
    {
        public OperationSignature Operation;
        public ExpressionSignature ExpressionPair;

        public ExpressionOperationPair(OperationSignature Operation, ExpressionSignature ExpressionPair)
        {
            this.Operation = Operation;
            this.ExpressionPair = ExpressionPair;
        }

        public override string ToString()
        {
            return ", operation(type:" + Operation.Symbol.Type + ", symbol:" + Operation.Symbol.String + "), " + ExpressionPair;
        }
    }

    public class OperandSignatur : SignatureSymbol
    {
        public AccessSignatureList AccessSignatureList = new AccessSignatureList();

        public OperandSignatur() : base(SignatureType.Operand)
        { }

        public override string ToString()
        {
            return AccessSignatureList.ToString();
        }
    }
    
    public class AccessSignatureList : ListCollection<AccessSignature>
    { }

    public abstract class AccessSignature : SignatureSymbol
    {
        public SeperatorSignature Seperator;

        public AccessSignature(SignatureType accessType) : base(accessType)
        { }
    }

    public class LiteralAccessSignature : AccessSignature
    {
        public LiteralSymbol LiteralSymbol;

        public LiteralAccessSignature(LiteralSymbol LiteralSymbol) : base(SignatureType.LiteralAccess)
        {
            this.LiteralSymbol = LiteralSymbol;
        }

        public override string ToString()
        {
            return "literal(type:" + LiteralSymbol.Type + ", symbol:" + LiteralSymbol.String + ")";
        }
    }

    public class VariableAccessSignature : AccessSignature
    {
        public IdentifierSignature Identifier;

        public VariableAccessSignature(IdentifierSignature Identifier) : base(SignatureType.VariableAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "variable(name:" + Identifier.Identifier.String + ")";
        }
    }

    public class FunctionAccessSignature : AccessSignature
    {
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public BlockSignature BlockEnd;

        public FunctionAccessSignature(IdentifierSignature Identifier) : base(SignatureType.FunctionAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "function(name:" + Identifier.Identifier.String + ", parameters(" + ParameterList + "))";
        }
    }

    public class ArrayAccessSignature : AccessSignature
    {
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public BlockSignature BlockEnd;

        public ArrayAccessSignature(IdentifierSignature Identifier) : base(SignatureType.ArrayAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "array(name:" + Identifier.Identifier.String + ", parameters(" + ParameterList + "))";
        }
    }
}
