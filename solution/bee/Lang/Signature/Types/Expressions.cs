using Bee.Library;
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
            TokenSymbol blockBegin;
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
                LiteralAccessSignature literalAccess = new LiteralAccessSignature(PrevToken);
                accessSignatur = literalAccess;
                signature.AccessList.Add(literalAccess);
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
                TokenSymbol identifier = TryIdentifier();
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
                signature.AccessList.Add(accessSignatur);
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
            OperationSignature signature = new OperationSignature(PrevToken);
            TrySpace();
            return signature;
        }
    }


    public class ExpressionSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public ExpressionSignature ChildExpression;
        public TokenSymbol BlockEnd;
        public OperandSignatur Operand;
        public ExpressionOperationList OperationList = new ExpressionOperationList();
        public TokenSymbol Seperator;

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
        public TokenSymbol Token;

        public OperationSignature(TokenSymbol OperationToken) : base(SignatureType.Operation)
        {
            this.Token = OperationToken;
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
            return ", operation(type:" + Operation.Token.Type + ", symbol:" + Operation.Token.String + "), " + ExpressionPair;
        }
    }

    public class OperandSignatur : SignatureSymbol
    {
        public AccessSignatureList AccessList = new AccessSignatureList();

        public OperandSignatur() : base(SignatureType.Operand)
        { }

        public override string ToString()
        {
            return AccessList.ToString();
        }
    }
    
    public class AccessSignatureList : ListCollection<AccessSignature>
    { }

    public abstract class AccessSignature : SignatureSymbol
    {
        public TokenSymbol Seperator;

        public AccessSignature(SignatureType accessType) : base(accessType)
        { }
    }

    public class LiteralAccessSignature : AccessSignature
    {
        public TokenSymbol Literal;

        public LiteralAccessSignature(TokenSymbol Literal) : base(SignatureType.LiteralAccess)
        {
            this.Literal = Literal;
        }

        public override string ToString()
        {
            return "literal(type:" + Literal.Type + ", symbol:" + Literal.String + ")";
        }
    }

    public class VariableAccessSignature : AccessSignature
    {
        public TokenSymbol Identifier;

        public VariableAccessSignature(TokenSymbol Identifier) : base(SignatureType.VariableAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "variable(name:" + Identifier.String + ")";
        }
    }

    public class FunctionAccessSignature : AccessSignature
    {
        public TokenSymbol Identifier;
        public TokenSymbol BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public TokenSymbol BlockEnd;

        public FunctionAccessSignature(TokenSymbol Identifier) : base(SignatureType.FunctionAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "function(name:" + Identifier.String + ", parameters(" + ParameterList + "))";
        }
    }

    public class ArrayAccessSignature : AccessSignature
    {
        public TokenSymbol Identifier;
        public TokenSymbol BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public TokenSymbol BlockEnd;

        public ArrayAccessSignature(TokenSymbol Identifier) : base(SignatureType.ArrayAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "array(name:" + Identifier.String + ", parameters(" + ParameterList + "))";
        }
    }
}
