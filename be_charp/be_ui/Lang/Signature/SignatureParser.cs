using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Be.Runtime
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

        public TokenSymbol Token
        {
            get
            {
                if (TokenPointer.Current != null)
                {
                    return TokenPointer.Current.Token;
                }
                return null;
            }
        }

        public TokenSymbol PrevToken
        {
            get
            {
                if(TokenPointer.Current != null && TokenPointer.Current.Prev != null)
                {
                    return TokenPointer.Current.Prev.Token;
                }
                return null;
            }
        }

        public TokenSymbol Next()
        {
            if (TokenPointer.Current != null && TokenPointer.Next() != null)
            {
                return Token;
            }
            return null;
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
            return new SeperatorSignature(PrevToken);
        }

        public KeywordToken TryKeywordToken(KeywordType keywordType)
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

        public IdentifierPathSignature TryPathBreakOn(TokenType breakTokenType)
        {
            IdentifierPathSignature signature = new IdentifierPathSignature();
            while(TryToken(TokenType.Identifier) != null)
            {
                IdentifierPathElementSignatur elementSignature = new IdentifierPathElementSignatur();
                elementSignature.Identifier = PrevToken as IdentifierToken;
                elementSignature.PointSeperator = TrySeperator(TokenType.Point);
                if(elementSignature.PointSeperator == null)
                {
                    break;
                }
            }
            return (signature.Tokens.Size() > 0 ? signature : null);
        }

        public BlockSignature TryBlock(TokenType blockType)
        {
            TrySpace();
            if(TryToken(blockType) == null)
            {
                return null;
            }
            return new BlockSignature(PrevToken);
        }

        public UseSignature TryUse()
        {
            if(TryKeywordToken(KeywordType.Use) == null)
            {
                return null;
            }
            UseSignature signatur = new UseSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            signatur.IdentifierPath = TryPathBreakOn(TokenType.Complete);
            signatur.Complete = TrySeperator(TokenType.Complete);
            return signatur;
        }

        public ScopeSignature TryScope()
        {
            if(TryKeywordToken(KeywordType.Scope) == null)
            {
                return null;
            }
            ScopeSignature signatur = new ScopeSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            signatur.IdentifierPath = TryPathBreakOn(TokenType.BlockBegin);
            signatur.BlockBegin = TryBlock(TokenType.BlockBegin);
            signatur.ObjectList = TryObjectList();
            signatur.BlockEnd = TryBlock(TokenType.BlockEnd);
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
            return list;
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if(TryToken(TokenType.Object) == null)
            {
                return null;
            }
            ObjectSignature signatur = new ObjectSignature();
            signatur.Keyword = new KeywordSignature(PrevToken as KeywordToken);
            signatur.Identifier = TryIdentifier();
            signatur.BlockBegin = TryBlock(TokenType.BlockBegin);
            SignatureSymbol objectElement = null;
            while((objectElement = TryObjectElement()) != null)
            {
                if(objectElement.Type == SignatureType.Member)
                {
                    signatur.Members.Add(objectElement as MemberSignatur);
                }
                else if(objectElement.Type == SignatureType.Method)
                {
                    signatur.Methods.Add(objectElement as MethodSignatur);
                }
                else if(objectElement.Type == SignatureType.Property)
                {
                    signatur.Properties.Add(objectElement as PropertySignatur);
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
            signatur.BlockEnd = TryBlock(TokenType.BlockEnd);
            return signatur;
        }

        public SignatureSymbol TryObjectElement()
        {
            TrySpace();
            TypeDeclarationSignatur typeDeclaration = TryTypeDeclaration();
            SeperatorSignature assigment = TrySeperator(TokenType.Assigment);
            if(assigment != null)
            {
                ;//expression
            }
            SeperatorSignature complete = TrySeperator(TokenType.Complete);
            if(assigment != null && complete != null)
            {
                MemberSignatur member = new MemberSignatur();
                member.TypeDeclaration = typeDeclaration;
                member.Assigment = assigment;
                return member;
            }
            SeperatorSignature enclosing = TrySeperator(TokenType.ClosingBegin);
            if(enclosing != null)
            {
                MethodSignatur method = new MethodSignatur();
                method.TypeDeclaration = typeDeclaration;
                method.ParameterDeclaration = TryParameterDeclaration();
                return method;
            }
            BlockSignature block = TryBlock(TokenType.BlockBegin);
            if(enclosing == null && block != null)
            {
                PropertySignatur property = new PropertySignatur();
                property.TypeDeclaration = typeDeclaration;
                return property;
            }
            return null;
        }

        public TypeDeclarationSignatur TryTypeDeclaration()
        {
            TypeDeclarationSignatur signatur = new TypeDeclarationSignatur();
            if(TryKeywordToken(KeywordType.Native) != null)
            {
                signatur.TypeNative = Token as NativeToken;
            }
            else if(TryToken(TokenType.Identifier) != null)
            {
                signatur.TypeIdentifier = Token as IdentifierToken;
            }
            else
            {
                return null;
            }
            TrySpace();
            signatur.NameIdentifier = TryIdentifier();
            return signatur;
        }
       
    }
}
