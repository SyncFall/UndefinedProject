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
            signature.PreOperation = TryPrePostOperation();
            Symbol blockBegin;
            if((blockBegin = TryNonSpace(StructureType.ClosingBegin)) != null)
            {
                signature.BlockBegin = blockBegin;
                if((signature.ChildExpression = TryExpression()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.ClosingEnd)) == null
                ){
                    return signature;
                }
            }
            else
            {
                if((signature.Operand = TryOperand()) == null)
                {
                    return null;
                }
            }
            while(true)
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
                if (signature.OperationList == null)
                    signature.OperationList = new ExpressionOperationList();
                signature.OperationList.Add(new ExpressionOperationPair(operation, expressionPair));
            }
            signature.PostOperation = TryPrePostOperation();
            return signature;
        }

        public OperandSignature TryOperand()
        {
            OperandSignature operand = null;
            OperandAccessSignature accessSignatur = null;
            Symbol identifier = null;

            TrySpace();
            if(Token != null && Token.IsType(TokenType.Literal))
            {
                LiteralOperand literalAccess = new LiteralOperand(Token);
                accessSignatur = literalAccess;
                NextToken();
            }
            else if(Token != null && Token.IsOperation(OperationType.Less))
            {
                while(true)
                {
                    TrySpace();
                    StructedBlockSignature blockSignature = TryStructedBlock();
                    if (blockSignature == null) break;
                    if (operand == null)
                        operand = new OperandSignature();
                    operand.AccessList.Add(new StructedBlockOperand(blockSignature));
                }
                return operand;
            }
            else
            {
                ObjectAccessOperand objectAccess = TryObjectOperand();
                if(objectAccess != null)
                {
                    accessSignatur = objectAccess;
                }
                else
                {
                    if(Token != null && (Token.IsType(TokenType.Identifier) || Token.IsType(TokenType.Native)))
                    {
                        identifier = Token;
                        NextToken();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            operand = new OperandSignature();
            while (true)
            {
                if (identifier != null && Token.IsStructure(StructureType.ClosingBegin))
                {
                    FunctionOperand functionAccess = new FunctionOperand(identifier);
                    functionAccess.ParameterDefinition = TryParameterDefintion(StructureType.ClosingBegin, StructureType.ClosingEnd);
                    if(functionAccess.ParameterDefinition == null )
                    {
                        break;
                    }
                    accessSignatur = functionAccess;
                }
                else if (identifier != null && Token.IsStructure(StructureType.BracketBegin))
                {
                    ArrayOperand arrayAccess = new ArrayOperand(identifier);
                    arrayAccess.ParameterDefintion = TryParameterDefintion(StructureType.BracketBegin, StructureType.BracketEnd);
                    if (arrayAccess.ParameterDefintion == null)
                    {
                        break;
                    }
                    accessSignatur = arrayAccess;
                }
                else if(identifier != null)
                {
                    VariableOperand variableAccess = new VariableOperand(identifier);
                    accessSignatur = variableAccess;
                }

                operand.AccessList.Add(accessSignatur);

                if ((accessSignatur.Seperator = TryNonSpace(StructureType.Point)) == null)
                {
                    break;
                }

                identifier = TryIdentifier();
            }

            return operand;
        }

        public ObjectAccessOperand TryObjectOperand()
        {
            BeginStep();
            TrySpace();
            if(Token == null) return null;

            ObjectAccessOperand objectOperand = null;
            if (Token.IsObject(ObjectType.New))
            {
                objectOperand = new ObjectAccessOperand();
                objectOperand.New = Token;
                NextToken();
            }
            else if(Token.IsNative(NativeType.Func))
            {
                objectOperand = new ObjectAccessOperand();
                objectOperand.Func = Token;
                NextToken();
            }

            TrySpace();
            if(Token == null) return null;
            if(Token.IsType(TokenType.Identifier) || Token.IsType(TokenType.Native))
            {
                if(objectOperand == null)
                    objectOperand = new ObjectAccessOperand();
                objectOperand.ObjectType = Token;
                NextToken();
            }

            if(objectOperand == null)
            {
                ResetStep();
                return null;
            }

            objectOperand.ParameterDefinition = TryParameterDefintion(StructureType.ClosingBegin, StructureType.ClosingEnd);
            if(objectOperand.ParameterDefinition == null)
            {
                ResetStep();
                return null;
            }

            CommitStep();
            return objectOperand;
        }
        
        public OperationSignature TryOperation()
        {
            TrySpace();
            if (Token == null || !Token.IsType(TokenType.Operation)) return null;
            NextToken();
            return new OperationSignature(PrevToken);
        }

        public OperationSignature TryPrePostOperation()
        {
            TrySpace();
            if(Token == null || !Token.IsCategory(OperationCategory.Variable)) return null;
            NextToken();
            return new OperationSignature(PrevToken);
        }
    }


    public class ExpressionSignature : SignatureSymbol
    {
        public OperationSignature PreOperation;
        public Symbol BlockBegin;
        public ExpressionSignature ChildExpression;
        public Symbol BlockEnd;
        public OperandSignature Operand;
        public OperationSignature PostOperation;
        public ExpressionOperationList OperationList;
        public Symbol Seperator;

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
        public Symbol Token;

        public OperationSignature(Symbol OperationToken) : base(SignatureType.Operation)
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

        public OperandSignature() : base(SignatureType.Operand)
        { }

        public override string ToString()
        {
            return AccessList.ToString();
        }
    }
    
    public class AccessSignatureList : ListCollection<OperandAccessSignature>
    { }

    public abstract class OperandAccessSignature : SignatureSymbol
    {
        public Symbol Seperator;

        public OperandAccessSignature(SignatureType accessType) : base(accessType)
        { }
    }

    public class LiteralOperand : OperandAccessSignature
    {
        public Symbol Literal;

        public LiteralOperand(Symbol Literal) : base(SignatureType.LiteralOperand)
        {
            this.Literal = Literal;
        }

        public override string ToString()
        {
            return "literal(type:" + Literal.Type + ", symbol:" + Literal.String + ")";
        }
    }

    public class ObjectAccessOperand : OperandAccessSignature
    {
        public Symbol New;
        public Symbol Func;
        public Symbol ObjectType;
        public ParameterDeclarationSignature ParameterDefinition;
 

        public ObjectAccessOperand() : base(SignatureType.ObjectOperand)
        { }

        public override string ToString()
        {
            string str = "object(";
            if(New != null) str += "new:true, ";
            if(Func != null) str += "func:true, ";
            if(ObjectType != null) str += "type:'" + ObjectType.String + "', ";
            if(ParameterDefinition != null) str += "parameters:true, ";
            return str += ")";
        }
    }

    public class VariableOperand : OperandAccessSignature
    {
        public Symbol Identifier;

        public VariableOperand(Symbol Identifier) : base(SignatureType.VariableOperand)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "variable(name:" + Identifier.String + ")";
        }
    }

    public class StructedBlockOperand : OperandAccessSignature
    {
        public StructedBlockSignature StructedBlock;

        public StructedBlockOperand(StructedBlockSignature StructedBlock) : base(SignatureType.StructedBlockOperand)
        {
            this.StructedBlock = StructedBlock;
        }

        public override string ToString()
        {
            return "block(" + StructedBlock + ")";
        }
    }

    public class FunctionOperand : OperandAccessSignature
    {
        public Symbol Identifier;
        public ParameterDeclarationSignature ParameterDefinition;

        public FunctionOperand(Symbol Identifier) : base(SignatureType.FunctionOperand)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "function(name:" + Identifier.String + ", parameters(" + ParameterDefinition + "))";
        }
    }

    public class ArrayOperand : OperandAccessSignature
    {
        public Symbol Identifier;
        public ParameterDeclarationSignature ParameterDefintion;

        public ArrayOperand(Symbol Identifier) : base(SignatureType.ArrayOperand)
        {
            this.Identifier = Identifier;
        }

        public override string ToString()
        {
            return "array(name:" + Identifier.String + ", parameters(" + ParameterDefintion + "))";
        }
    }
}
