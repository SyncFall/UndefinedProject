
using System;

namespace Bee.Language
{
    public class SignatureParser
    {
        public TokenPointer TokenPointer;

        public SignatureParser(TokenContainer TokenContainer)
        {
            this.TokenPointer = new TokenPointer(TokenContainer.FirstTokenNode());
        }

        public bool IsEnd()
        {
            return (TokenPointer.Current == null);
        }

        public SignatureSymbol TrySignature()
        {
            if(IsEnd())
            {
                return null;
            }
            TrySpace();
            SignatureSymbol signature = null;
            if((signature = TryUse()) != null ||
               (signature = TryScope()) != null ||
               (signature = TryObject()) != null ||
               (signature = TryObjectElement()) != null ||
               (signature = TryTypeDeclaration()) != null ||
               (signature = TryParameterDeclaration()) != null ||
               (signature = TryIdentifierPath()) != null ||
               (signature = TryIdentifier()) != null ||
               (signature = TryUnknown()) != null
            ){
                Console.WriteLine(signature);
            }
            return signature;
        }

        public TokenSymbol Token
        {
            get{
                return (TokenPointer.Current != null ? TokenPointer.Current.Token : null);
            }
        }

        public TokenSymbol PrevToken
        {
            get{
                return (TokenPointer.Current != null && TokenPointer.Current.Prev != null ? TokenPointer.Current.Prev.Token : null);
            }
        }

        public TokenSymbol Next()
        {
            return (TokenPointer.Current != null && TokenPointer.Next() != null ? TokenPointer.Current.Token : null);
        }

        public void BeginStep()
        {
            TokenPointer.StepBegin();
        }

        public void ResetStep()
        {
            TokenPointer.StepReset();
        }

        public TokenSymbol TryToken(TokenType tokenType)
        {
            if (Token == null || Token.Type != tokenType)
            {
                return null;
            }
            TokenSymbol tokenSymbol = Token;
            Next();
            return tokenSymbol;
        }

        public KeywordToken TryToken(KeywordType keywordType)
        {
            if (Token == null || Token.Type != TokenType.Keyword || (Token as KeywordToken).Symbol.Type != keywordType)
            {
                return null;
            }
            KeywordToken tokenSymbol = Token as KeywordToken;
            Next();
            return tokenSymbol;
        }

        public NativeToken TryToken(NativeType nativeType)
        {
            if (Token == null || Token.Type != TokenType.Native || (Token as NativeToken).Symbol.Type != nativeType)
            {
                return null;
            }
            NativeToken tokenSymbol = Token as NativeToken;
            Next();
            return tokenSymbol;
        }

        public bool TrySpace()
        {
            bool hasSpace = false;
            while(Token != null && Token.IsSpace())
            {
                hasSpace = true;
                Next();
            }
            return hasSpace;
        }

        public SeperatorSignature TrySeperator(StructureType seperatorType)
        {
            TrySpace();
            if(Token == null || !Token.IsStructure(seperatorType))
            { 
                return null;
            }
            SeperatorSignature signatur = new SeperatorSignature(Token);
            Next();
            TrySpace();
            return signatur;
        }

        public BlockSignature TryBlock(StructureType blockType)
        {
            TrySpace();
            if (Token == null || !Token.IsStructure(blockType))
            {
                return null;
            }
            BlockSignature signatur = new BlockSignature(Token);
            Next();
            TrySpace();
            return signatur;
        }

        public UnknownSignatur TryUnknown()
        {
            if (Token == null)
            {
                return null;
            }
            TokenSymbol unknownToken = Token;
            Next();
            return new UnknownSignatur(unknownToken);
        }

        public UseSignature TryUse()
        {
            if(TryToken(KeywordType.Use) == null)
            {
                return null;
            }
            UseSignature signatur = new UseSignature();
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TrySeperator(StructureType.Complete)) == null
            ){
                ; 
            }
            return signatur;
        }

        public ScopeSignature TryScope()
        {
            if(TryToken(KeywordType.Scope) == null)
            {
                return null;
            }
            ScopeSignature signatur = new ScopeSignature();
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                (signatur.ObjectList = TryObjectList()) == null ||
                (signatur.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
            ){
                ;
            }
            return signatur;
        }


        public IdentifierPathSignature TryIdentifierPath()
        {
            IdentifierPathSignature signature = new IdentifierPathSignature();
            while (TryToken(TokenType.Identifier) != null)
            {
                IdentifierPathElementSignatur elementSignature = new IdentifierPathElementSignatur();
                signature.PathElements.Add(elementSignature);
                elementSignature.Identifier = PrevToken as IdentifierToken;
                elementSignature.PointSeperator = TrySeperator(StructureType.Point);
                if (elementSignature.PointSeperator == null)
                {
                    break;
                }
            }
            return (signature.PathElements.Size() > 0 ? signature : null);
        }

        public IdentifierSignature TryIdentifier()
        {
            if(TryToken(TokenType.Identifier) == null)
            {
                return null;
            }
            IdentifierSignature signatur = new IdentifierSignature();
            signatur.IdentifiereToken = PrevToken as IdentifierToken;
            return signatur;
        }

        public ObjectSignatureList TryObjectList()
        {
            ObjectSignatureList list = new ObjectSignatureList();
            ObjectSignature signatur;
            while((signatur = TryObject()) != null)
            {
                list.Add(signatur);
            }
            return list;
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if(TryToken(NativeType.Object) == null)
            {
                return null;
            }
            ObjectSignature signatur = new ObjectSignature();
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.Identifier = TryIdentifier()) == null ||
                (signatur.BlockBegin = TryBlock(StructureType.BlockBegin)) == null
            ){
                return signatur;
            }
            SignatureSymbol objectElement;
            while((objectElement = TryObjectElement()) != null)
            {
                if(objectElement.Type == SignatureType.Member)
                {
                    signatur.Members.Add(objectElement as MemberSignature);
                }
                else if(objectElement.Type == SignatureType.Method)
                {
                    signatur.Methods.Add(objectElement as MethodSignature);
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
            if((signatur.BlockEnd = TryBlock(StructureType.BlockEnd)) == null)
            {
                ;
            }
            return signatur;
        }

        public SignatureSymbol TryObjectElement()
        {
            TrySpace();
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
            if(typeDeclaration == null)
            {
                return null;
            }
            SeperatorSignature assigment = TrySeperator(StructureType.Assigment);
            ExpressionSignature assigmentExpression = null;
            if(assigment != null)
            {
                assigmentExpression = TryExpression();
            }
            SeperatorSignature complete = TrySeperator(StructureType.Complete);
            if (assigment != null || complete != null)
            {
                MemberSignature member = new MemberSignature();
                member.TypeDeclaration = typeDeclaration;
                member.Assigment = assigment;
                member.AssigmentExpression = assigmentExpression;
                member.Complete = complete;
                return member;
            }
            if(Token == null)
            {
                return null;
            }
            BeginStep();
            SeperatorSignature enclosing = TrySeperator(StructureType.ClosingBegin);
            if (enclosing != null)
            {
                ResetStep();
                MethodSignature method = new MethodSignature();
                method.TypeDeclaration = typeDeclaration;
                if((method.ParameterDeclaration = TryParameterDeclaration()) == null ||
                   (method.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (method.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return method;
            }
            ResetStep();
            return null;
        }

        public TypeDeclarationSignature TryTypeDeclaration()
        {
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            if(TryToken(TokenType.Native) != null)
            {
                signatur.TypeNative = PrevToken as NativeToken;
            }
            else if(TryToken(TokenType.Identifier) != null)
            {
                signatur.TypeIdentifier = PrevToken as IdentifierToken;
            }
            else
            {
                return null;
            }
            if(!TrySpace() ||
               (signatur.NameIdentifier = TryIdentifier()) == null
            ){
                ;
            }
            return signatur;
        }

        public ParameterDeclarationSignature TryParameterDeclaration()
        {
            TrySpace();
            BlockSignature blockBegin;
            if ((blockBegin = TryBlock(StructureType.ClosingBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            TypeDeclarationSignature typeDeclaration;
            while ((typeDeclaration = TryTypeDeclaration()) != null)
            {
                ParameterDeclartionElementSignature parameterElement = new ParameterDeclartionElementSignature(typeDeclaration);
                signature.ParameterList.Add(parameterElement);
                parameterElement.ParameterSeperator = TrySeperator(StructureType.Comma);
                if (parameterElement.ParameterSeperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryBlock(StructureType.ClosingEnd);
            return signature;
        }

        public ExpressionSignature TryExpression()
        {
            TrySpace();
            ExpressionSignature signature = new ExpressionSignature();
            BlockSignature blockBegin;
            if((blockBegin = TryBlock(StructureType.ClosingBegin)) != null)
            {
                signature.BlockBegin = blockBegin;
                if((signature.ChildExpression = TryExpression()) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.ClosingEnd)) == null
                ){
                    return signature;
                }
            }
            else
            {
                if((signature.Operand = TryOperand()) == null)
                {
                    return signature;
                }
            }
            while(true)
            {
                OperationSignature operation = TryOperation();
                if (operation == null)
                {
                    break;
                }
                ExpressionSignature expressionPair = TryExpression();
                if(expressionPair == null)
                {
                    break;
                }
                signature.ExpressionOperationList.Add(new OperationPair(operation, expressionPair));
            }
            return signature;
        }

        public OperandSignatur TryOperand()
        {
            OperandSignatur signature = new OperandSignatur();
            OperandAccessSignature accessSignatur = null;
            if (TryToken(TokenType.Literal) != null)
            {
                LiteralAccessSignature literalAccess = new LiteralAccessSignature(PrevToken as LiteralToken);
                accessSignatur = literalAccess;
                signature.AccessSignatureList.Add(literalAccess);
                if((literalAccess.Seperator = TrySeperator(StructureType.Point)) == null)
                {
                    return signature;
                }
            }
            else if(Token != null && Token.Type == TokenType.Identifier)
            {
                ;
            }
            else
            {
                return null;
            }
            while(Token != null && Token.Type == TokenType.Identifier)
            {
                IdentifierSignature identifier = TryIdentifier();
                if (TryBlock(StructureType.ClosingBegin) != null)
                {
                    FunctionAccessSignature functionAccess = new FunctionAccessSignature(identifier);
                    while (true)
                    {
                        ExpressionSignature expression = TryExpression();
                        if(expression == null)
                        {
                            break;
                        }
                        FunctionAccessParameterSignature parameter = new FunctionAccessParameterSignature(expression);
                        functionAccess.ParameterList.Add(parameter);
                        if ((parameter.Seperator = TrySeperator(StructureType.Comma)) == null)
                        {
                            break;
                        }
                    }
                    if((functionAccess.BlockEnd = TryBlock(StructureType.ClosingEnd)) == null)
                    {
                        ;
                    }
                    accessSignatur = functionAccess;
                }
                else if (TryBlock(StructureType.BracketBegin) != null)
                {
                    ArrayAccessSignature arrayAccess = null;// todo
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
}
