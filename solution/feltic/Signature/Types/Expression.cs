﻿using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class SignatureParser
    {
        public ExpressionSignature TryExpression(bool WithStructedBlock=true)
        {
            if (!Begin()) return null;
            TrySpace();
            ExpressionSignature signature = new ExpressionSignature();
            signature.PreOperation = TryPrePostOperation(true);
            if((signature.BlockBegin = TryNonSpace(StructureType.ClosingBegin)) != null)
            {
                if((signature.ChildExpression = TryExpression(WithStructedBlock)) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.ClosingEnd)) == null
                ){
                    Reset();
                    return null;
                }
                Commit();
                return signature;
            }
            else
            {
                if((signature.Operand = TryOperand(WithStructedBlock)) == null)
                {
                    Reset();
                    return null;
                }
            }
            if(!Begin()) return signature;
            if((signature.Operation = TryOperation(WithStructedBlock)) == null ||
               (signature.ExpressionPair = TryExpression(WithStructedBlock)) == null
            ){
                Reset();
            }else
                Commit();
            signature.PostOperation = TryPrePostOperation(false);
            Commit();
            return signature;
        }

        public OperandSignature TryOperand(bool WitchStructedBlock=true)
        {
            OperandSignature operand = null;
            OperandAccessSignature accessSignatur = null;
            Symbol identifier = null;

            TrySpace();
            if(!Begin()) return null;
            if(Token != null && Token.IsType(TokenType.Literal))
            {
                LiteralOperand literalAccess = new LiteralOperand(Token);
                accessSignatur = literalAccess;
                NextToken();
            }
            else if(WitchStructedBlock && Token != null && Token.IsOperation(OperationType.Less))
            {
                while(true)
                {
                    StructedBlockSignature blockSignature = TryStructedBlock();
                    if (blockSignature == null) break;
                    if (operand == null)
                        operand = new OperandSignature();
                    operand.AccessList.Add(new StructedBlockOperand(blockSignature));
                }
                if(operand != null)
                {
                    Commit();
                    return operand;
                }
                else
                {
                    Reset();
                    return null;
                }
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
                        Reset();
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
                    functionAccess.ParameterDefinition = TryParameters(StructureType.ClosingBegin, StructureType.ClosingEnd);
                    if(functionAccess.ParameterDefinition == null )
                    {
                        break;
                    }
                    accessSignatur = functionAccess;
                }
                else if (identifier != null && Token.IsStructure(StructureType.BracketBegin))
                {
                    ArrayOperand arrayAccess = new ArrayOperand(identifier);
                    arrayAccess.ParameterDefintion = TryParameters(StructureType.BracketBegin, StructureType.BracketEnd);
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

            Commit();
            return operand;
        }

        public ObjectAccessOperand TryObjectOperand()
        {
            Begin();
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
                Reset();
                return null;
            }

            objectOperand.ParameterDefinition = TryParameters(StructureType.ClosingBegin, StructureType.ClosingEnd);
            if(objectOperand.ParameterDefinition == null)
            {
                Reset();
                return null;
            }

            if(TryNonSpace(StructureType.BlockBegin) != null)
            {
                objectOperand.ContentSignatures = TryStatementList();
                TryNonSpace(StructureType.BlockEnd);
            }

            Commit();
            return objectOperand;
        }
        
        public OperationSignature TryOperation(bool WithStructedBlock=true)
        {
            TrySpace();
            if (Token == null || !Token.IsType(TokenType.Operation) || (!WithStructedBlock && Token.IsOperation(OperationType.Greater))) return null;
            NextToken();
            return new OperationSignature(PrevToken);
        }

        public OperationSignature TryPrePostOperation(bool Pre)
        {
            TrySpace();
            if(Token == null || (!Token.IsCategory(OperationCategory.Variable) && (!Pre || !Token.IsStructure(StructureType.Point)))) return null;
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
        public OperationSignature Operation;
        public ExpressionSignature ExpressionPair;
        public OperationSignature PostOperation;
        public Symbol Seperator;

        public ExpressionSignature() : base(SignatureType.Expression)
        { }

        public override string ToString()
        {
            string str = "";
            if (ChildExpression != null)
                str += "child(" + ChildExpression + ")";
            if (Operand != null)
                str += "operand(" + Operand + ")";
            if (Operation != null)
                str += Operation;
            if(ExpressionPair!=null)
                str += "pair("+ExpressionPair+")";
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

        public override string ToString()
        {
            return "op(" + Token.String + ")";
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
        public GenericDeclarationSignature Generics;
        public Symbol ObjectType;
        public ParameterDeclarationSignature ParameterDefinition;
        public SignatureList ContentSignatures;

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
