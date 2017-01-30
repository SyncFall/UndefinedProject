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
                LiteralAccessSignature literalAccess = new LiteralAccessSignature(PrevToken as LiteralToken);
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
                        FunctionAccessParameterSignature parameter = new FunctionAccessParameterSignature(expression);
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
                    ArrayAccessSignature arrayAccess = new ArrayAccessSignature();
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
            OperationSignature signature = new OperationSignature(PrevToken as OperationToken);
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
        public OperationToken OperationToken;

        public OperationSignature(OperationToken OperationToken) : base(SignatureType.Operation)
        {
            this.OperationToken = OperationToken;
        }
    }

    public class ExpressionOperationList : ListCollection<ExpressionOperationPair>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size(); i++)
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
            return ", operation(type:" + Operation.OperationToken.Symbol.Type + ", symbol:" + Operation.OperationToken.Symbol.String + "), " + ExpressionPair;
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
        public LiteralToken LiteralToken;

        public LiteralAccessSignature(LiteralToken LiteralToken) : base(SignatureType.LiteralAccess)
        {
            this.LiteralToken = LiteralToken;
        }

        public override string ToString()
        {
            return "literal(type:" + LiteralToken.Symbol.Type + ", symbol:" + LiteralToken.Symbol.String + ")";
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
            return "variable(name:" + Identifier.IdentifiereToken.String + ")";
        }
    }

    public class FunctionAccessSignature : AccessSignature
    {
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public FunctionAccessParameterList ParameterList = new FunctionAccessParameterList();
        public BlockSignature BlockEnd;

        public FunctionAccessSignature(IdentifierSignature Identifier) : base(SignatureType.FunctionAccess)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "function(name:" + Identifier.IdentifiereToken.String + ", parameters(" + ParameterList + "))";
        }
    }

    public class FunctionAccessParameterList : ListCollection<FunctionAccessParameterSignature>
    { }

    public class FunctionAccessParameterSignature : SignatureSymbol
    {
        public ExpressionSignature Expression;
        public SeperatorSignature Seperator;

        public FunctionAccessParameterSignature(ExpressionSignature Expression) : base(SignatureType.FunctionAccessParameter)
        {
            this.Expression = Expression;
        }

        public override string ToString()
        {
            return "parameter(" + Expression + ")";
        }
    }

    public class ArrayAccessSignature : AccessSignature
    {
        public ArrayAccessSignature() : base(SignatureType.ArrayAccess)
        { }
    }
}
