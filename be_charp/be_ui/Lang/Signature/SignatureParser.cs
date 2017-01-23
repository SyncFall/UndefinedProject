using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bee.Runtime
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
                //Console.WriteLine(signature);
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

        public TokenSymbol Prev()
        {
            return (TokenPointer.Current != null ? TokenPointer.Prev().Token : null);
        }

        public void BeginStep()
        {
            TokenPointer.StepBegin();
        }

        public void ResetStep()
        {
            TokenPointer.StepReset();
        }
	
        public bool TrySpace()
        {
            bool hasSpace = false;
            while(Token != null && (Token.Group == TokenGroup.Space || Token.Group == TokenGroup.Comment))
            {
                hasSpace = true;
                Next();
            }
            return hasSpace;
        }

        public SeperatorSignature TrySeperator(TokenType seperatorType)
        {
            TrySpace();
            if(TryToken(seperatorType) == null)
            {
                return null;
            }
            SeperatorSignature signatur = new SeperatorSignature(PrevToken);
            TrySpace();
            return signatur;
        }

        public KeywordToken TryKeyword(KeywordType keywordType)
        {
            if(Token != null && Token.Type == TokenType.Keyword && Token.Group == TokenGroup.Keyword && (Token as KeywordToken).KeywordSymbol.Type == keywordType)
            {
                KeywordToken keywordToken = Token as KeywordToken;
                Next();
                return keywordToken;
            }
            return null;
        }

        public TokenSymbol TryToken(TokenType tokenType)
        {
            if(Token != null && Token.Type == tokenType)
            {
                TokenSymbol tokenSymbol = Token;
                Next();
                return tokenSymbol;
            }
            return null;
        }

        public TokenSymbol TryToken(TokenGroup tokenGroup)
        {
            if (Token != null && Token.Group == tokenGroup)
            {
                TokenSymbol tokenSymbol = Token;
                Next();
                return tokenSymbol;
            }
            return null;
        }

        public IdentifierPathSignature TryIdentifierPath()
        {
            IdentifierPathSignature signature = new IdentifierPathSignature();
            while(TryToken(TokenType.Identifier) != null)
            {
                IdentifierPathElementSignatur elementSignature = new IdentifierPathElementSignatur();
                signature.PathElements.Add(elementSignature);
                elementSignature.Identifier = PrevToken as IdentifierToken;
                elementSignature.PointSeperator = TrySeperator(TokenType.Point);
                if(elementSignature.PointSeperator == null)
                {
                    break;
                }
            }
            return (signature.PathElements.Size() > 0 ? signature : null);
        }

        public BlockSignature TryBlock(TokenType blockType)
        {
            TrySpace();
            if(TryToken(blockType) == null)
            {
                return null;
            }
            BlockSignature signatur = new BlockSignature(PrevToken);
            TrySpace();
            return signatur;
        }

        public UnknownSignatur TryUnknown()
        {
            if(Token != null)
            {
                TokenSymbol unknownToken = Token;
                Next();
                return new UnknownSignatur(unknownToken);
            }
            return null;
        }

        public UseSignature TryUse()
        {
            if(TryKeyword(KeywordType.Use) == null)
            {
                return null;
            }
            UseSignature signatur = new UseSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TrySeperator(TokenType.Complete)) == null
            ){
                ; 
            }
            return signatur;
        }

        public ScopeSignature TryScope()
        {
            if(TryKeyword(KeywordType.Scope) == null)
            {
                return null;
            }
            ScopeSignature signatur = new ScopeSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.BlockBegin = TryBlock(TokenType.BlockBegin)) == null ||
                (signatur.ObjectList = TryObjectList()) == null ||
                (signatur.BlockEnd = TryBlock(TokenType.BlockEnd)) == null
            ){
                ;
            }
            return signatur;
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
            return (list.Size() > 0 ? list : null);
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if(TryKeyword(KeywordType.Object) == null)
            {
                return null;
            }
            ObjectSignature signatur = new ObjectSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            if (!TrySpace() ||
                (signatur.Identifier = TryIdentifier()) == null ||
                (signatur.BlockBegin = TryBlock(TokenType.BlockBegin)) == null
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
            if((signatur.BlockEnd = TryBlock(TokenType.BlockEnd)) == null)
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
            SeperatorSignature complete = TrySeperator(TokenType.Complete);
            if(complete != null)
            {
                MemberSignature member = new MemberSignature();
                member.TypeDeclaration = typeDeclaration;
                member.Complete = complete;
                return member;
            }
            SeperatorSignature assigment = TrySeperator(TokenType.Assigment);
            if(assigment != null)
            {
                MemberSignature member = new MemberSignature();
                member.TypeDeclaration = typeDeclaration;
                member.Assigment = assigment;
                member.AssigmentExpression = TryExpression();
                member.Complete = TrySeperator(TokenType.Complete);
                return member;
            }
            SeperatorSignature enclosing = TrySeperator(TokenType.ClosingBegin);
            if (enclosing != null)
            {
                MethodSignature method = new MethodSignature();
                method.TypeDeclaration = typeDeclaration;
                if((method.ParameterDeclaration = TryParameterDeclaration()) == null ||
                   (method.BlockBegin = TryBlock(TokenType.BlockBegin)) == null ||
                   (method.BlockEnd = TryBlock(TokenType.BlockEnd)) == null
                ){
                    ;
                }
                return method;
            }
            return null;
        }

        public TypeDeclarationSignature TryTypeDeclaration()
        {
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            if(TryToken(TokenGroup.Native) != null)
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
            if ((blockBegin = TryBlock(TokenType.ClosingBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            TypeDeclarationSignature typeDeclaration;
            while ((typeDeclaration = TryTypeDeclaration()) != null)
            {
                ParameterDeclartionElementSignature parameterElement = new ParameterDeclartionElementSignature();
                signature.ParameterList.Add(parameterElement);
                parameterElement.ParameterSeperator = TrySeperator(TokenType.Comma);
                if (parameterElement.ParameterSeperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryBlock(TokenType.ClosingEnd);
            return signature;
        }

        public ExpressionSignature TryExpression()
        {
            if (true) return null;
            TrySpace();
            ExpressionSignature signature = new ExpressionSignature();
            BlockSignature blockBegin;
            if((blockBegin = TryBlock(TokenType.ClosingBegin)) != null)
            {
                signature.BlockBegin = blockBegin;
                if((signature.ChildExpression = TryExpression()) == null ||
                   (signature.BlockEnd = TryBlock(TokenType.ClosingEnd)) == null
                ){
                    return signature;
                }
            }
            else
            {
                if((signature.Operand == TryOperand()) == null)
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
            if(TryToken(TokenGroup.Literal) != null)
            {
                signature.AccessSignatureList.Add(new LiteralAccessSignature(PrevToken as LiteralToken));
                if(TrySeperator(TokenType.Point) == null)
                {
                    return signature;
                }
            }
            else if(Token != null && Token.Group == TokenGroup.Identifier)
            {
                ;
            }
            else
            {
                return null;
            }
            while(Token != null && Token.Group == TokenGroup.Identifier)
            {
                IdentifierPathSignature pathSignature = TryIdentifierPath();
                if (TryBlock(TokenType.ClosingBegin) != null)
                {
                    FunctionAccessSignature functionAccess = TryFunctionAccess();
                    signature.AccessSignatureList.Add(functionAccess);
                }
                else if (TryBlock(TokenType.BracketBegin) != null)
                {
                    ArrayAccessSignature arrayAccess = TryArrayAccess();
                    signature.AccessSignatureList.Add(arrayAccess);
                }
                else
                {
                    VariableAccessSignature variableAccess = new VariableAccessSignature(pathSignature);
                    signature.AccessSignatureList.Add(variableAccess);
                }
                Next();
            }
            return signature;
        }

        public FunctionAccessSignature TryFunctionAccess()
        {
            return null;
        }

        public ArrayAccessSignature TryArrayAccess()
        {
            return null;
        }

        public OperationSignature TryOperation()
        {
            TrySpace();
            if(TryToken(TokenGroup.Operation) != null)
            {
                return new OperationSignature(PrevToken as OperationToken);
            }
            return null;
        }
    }
}
