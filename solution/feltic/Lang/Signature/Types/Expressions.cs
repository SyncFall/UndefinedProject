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
        public ExpressionSignature TryExpression()
        {
            TrySpace();
            ExpressionSignature signature = new ExpressionSignature();
            TokenSymbol blockBegin;
            if ((blockBegin = TryNonSpace(StructureType.ClosingBegin)) != null)
            {
                signature.BlockBegin = blockBegin;
                if((signature.ChildExpression = TryExpression()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.ClosingEnd)) == null
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
                if(!BeginStep())
                {
                    return signature;
                }
                OperationSignature operation = TryOperation();
                if (operation == null)
                {
                    ResetStep();
                    break;
                }
                ExpressionSignature expressionPair = TryExpression();
                if (expressionPair == null)
                {
                    ResetStep();
                    break;
                }
                CommitStep();
                signature.OperationList.Add(new ExpressionOperationPair(operation, expressionPair));
            }
            return signature;
        }

        public OperandSignature TryOperand()
        {
            OperandSignature signature = new OperandSignature();
            AccessSignature accessSignatur = null;
            if (TryToken(TokenType.Literal) != null)
            {
                LiteralAccessSignature literalAccess = new LiteralAccessSignature(PrevToken);
                accessSignatur = literalAccess;
                signature.AccessList.Add(literalAccess);
                if ((literalAccess.Seperator = TryNonSpace(StructureType.Point)) == null)
                {
                    return signature;
                }
            }
            else if(Token != null && Token.Type == TokenType.Identifier)
            {
                ;
            }
            else if(Token != null && Token.IsStructure(StructureType.BlockBegin))
            {
                BlockSignature blockSignature = TryBlock();
                if(blockSignature != null)
                {
                    BlockAccessSignature blockAccess = new BlockAccessSignature(blockSignature);
                    signature.AccessList.Add(blockAccess);
                    if((blockAccess.Seperator = TryNonSpace(StructureType.Point)) == null)
                    {
                        return signature;
                    }
                }
                //
                else
                {
                    return null;
                }  
            }
            else
            {
                StructedBlockSignature blockSignature = TryStructedBlock();
                if(blockSignature != null)
                {
                    StructedBlockAccessSignature blockAccess = new StructedBlockAccessSignature(blockSignature);
                    signature.AccessList.Add(blockAccess);
                    if ((blockAccess.Seperator = TryNonSpace(StructureType.Point)) == null)
                    {
                        return signature;
                    }
                }
                //
                else
                {
                    return null;
                }
            }
            while (true)
            {
                TokenSymbol identifier = TryIdentifier();
                TrySpace();
                if(identifier != null && TryNonSpace(StructureType.ClosingBegin) != null)
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
                        if ((parameter.Seperator = TryNonSpace(StructureType.Seperator)) == null)
                        {
                            break;
                        }
                    }
                    if((functionAccess.BlockEnd = TryNonSpace(StructureType.ClosingEnd)) == null ||
                       (functionAccess.BlockSignature = TryBlock()) == null
                    ){
                        ;
                    }
                    accessSignatur = functionAccess;
                }
                else if(identifier != null && TryNonSpace(StructureType.BracketBegin) != null)
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
                        if ((parameter.Seperator = TryNonSpace(StructureType.Seperator)) == null)
                        {
                            break;
                        }
                    }
                    if ((arrayAccess.BlockEnd = TryNonSpace(StructureType.BracketEnd)) == null)
                    {
                        ;
                    }
                    accessSignatur = arrayAccess;
                }
                else if(Token.IsStructure(StructureType.BlockBegin))
                {
                    BlockSignature blockSignature = TryBlock();
                    if(blockSignature == null)
                    {
                        break;
                    }
                    BlockAccessSignature blockAccess = new BlockAccessSignature(blockSignature, identifier);
                    accessSignatur = blockAccess;
                }
                else if(identifier != null)
                {
                    VariableAccessSignature variableAccess = new VariableAccessSignature(identifier);
                    accessSignatur = variableAccess;

                }
                signature.AccessList.Add(accessSignatur);
                if ((accessSignatur.Seperator = TryNonSpace(StructureType.Point)) == null)
                {
                    break;
                }
            }
            return signature;
        }

        public OperationSignature TryOperation()
        {
            TrySpace();
            if(TryToken(TokenType.Operation) == null)
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
        public OperandSignature Operand;
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

    public class OperandSignature : SignatureSymbol
    {
        public AccessSignatureList AccessList = new AccessSignatureList();

        public OperandSignature(SignatureType Type=SignatureType.Operand) : base(Type)
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

    public class StructedBlockAccessSignature : AccessSignature
    {
        public StructedBlockSignature StructedBlock;

        public StructedBlockAccessSignature(StructedBlockSignature StructedBlock) : base(SignatureType.StructedBlockAccess)
        {
            this.StructedBlock = StructedBlock;
        }

        public override string ToString()
        {
            return "block(" + StructedBlock + ")";
        }
    }

    public class FunctionAccessSignature : AccessSignature
    {
        public TokenSymbol Identifier;
        public TokenSymbol BlockBegin;
        public ListCollection<ParameterSignature> ParameterList = new ListCollection<ParameterSignature>();
        public BlockSignature BlockSignature;
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
        public ListCollection<ParameterSignature> ParameterList = new ListCollection<ParameterSignature>();
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

    public class BlockAccessSignature : AccessSignature
    {
        public TokenSymbol Identifier;
        public BlockSignature BlockSignature;

        public BlockAccessSignature(BlockSignature BlockSignature, TokenSymbol Identifier=null) : base(SignatureType.BlockAccess)
        {
            this.BlockSignature = BlockSignature;
            this.Identifier = Identifier;
        }
    }
}
