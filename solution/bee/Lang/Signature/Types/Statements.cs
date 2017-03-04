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
        public CodeSignature TryCode()
        {
            TokenSymbol blockBegin = TryBlock(StructureType.BlockBegin);
            if (blockBegin == null)
            {
                return null;
            }
            CodeSignature signature = new CodeSignature();
            signature.BlockBegin = blockBegin;
            if((signature.Statements = TryStatementList()) == null ||
              ((signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null)
            ){
                ;
            }
            return signature;
        }

        public StatementSignatureList TryStatementList()
        {
            StatementSignatureList list = new StatementSignatureList();
            StatementSignature signatur;
            while ((signatur = TryStatement()) != null)
            {
                list.Add(signatur);
            }
            return list;
        }

        public StatementSignature TryStatement()
        {
            TrySpace();
            TokenSymbol keyword=null, secondKeyword=null;
            StatementType statementType;
            bool conditionBlock = false;
            bool keywordStatement = false;
            bool expressionStatement = false;
            bool blockStatement = false;

            if ((keyword = TryToken(StatementKeywordType.If)) != null)
            {
                statementType = StatementType.If;
                conditionBlock = true;
            }
            else if((keyword = TryToken(StatementKeywordType.Else)) != null)
            {
                if((secondKeyword = TryToken(StatementKeywordType.If)) != null)
                {
                    statementType = StatementType.ElseIf;
                    conditionBlock = true;
                }
                else
                {
                    statementType = StatementType.Else;
                    blockStatement = true;
                }
            }
            else if((keyword = TryToken(StatementKeywordType.For)) != null)
            {
                statementType = StatementType.For;
            }
            else if((keyword = TryToken(StatementKeywordType.While)) != null)
            {
                statementType = StatementType.While;
                conditionBlock = true;
            }
            else if((keyword = TryToken(StatementKeywordType.Continue)) != null)
            {
                statementType = StatementType.Continue;
                keywordStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Break)) != null)
            {
                statementType = StatementType.Break;
                keywordStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Return)) != null)
            {
                statementType = StatementType.Return;
                expressionStatement = true;
            }
            else if((keyword = TryToken(StatementKeywordType.Sanity)) != null)
            {
                statementType = StatementType.Sanity;
                expressionStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Throw)) != null)
            {
                statementType = StatementType.Throw;
                expressionStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Try)) != null)
            {
                statementType = StatementType.Try;
                blockStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Finally)) != null)
            {
                statementType = StatementType.Finally;
                blockStatement = true;
            }
            else if ((keyword = TryToken(StatementKeywordType.Sync)) != null)
            {
                statementType = StatementType.Sync;
                conditionBlock = true;
            }
            else if(TryToken(StructureType.Complete) != null)
            {
                StatementSignature signature = new StatementSignature(StatementType.NoOperation, false);
                signature.Keyword = PrevToken;
                return signature;
            }
            else
            {
                TokenSymbol beginBlock = TryBlock(StructureType.BlockBegin);
                if(beginBlock != null)
                {
                    BlockStatementSignature signature = new BlockStatementSignature(StatementType.InnerBlock);
                    signature.BlockBegin = beginBlock;
                    if((signature.ChildStatements = TryStatementList()) == null ||
                       (signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                    ){
                        ;
                    }
                    return signature;
                }
                else
                {
                    TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
                    if(typeDeclaration != null)
                    {
                        TypeDeclarationStatementSignature signature = new TypeDeclarationStatementSignature();
                        signature.TypeDeclaration = typeDeclaration;
                        signature.Complete = TrySeperator(StructureType.Complete);
                        return signature;
                    }
                    else
                    {
                        ExpressionSignature expression = TryExpression();
                        if (expression != null)
                        {
                            ExpressionStatementSignature signature = new ExpressionStatementSignature(StatementType.ExpressionStatement);
                            signature.ConditionExpression = expression;
                            signature.Complete = TrySeperator(StructureType.Complete);
                            return signature;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            if (conditionBlock)
            {
                ConditionBlockStatementSignature signature = new ConditionBlockStatementSignature(statementType);
                signature.Keyword = keyword;
                if((signature.ConditionBegin = TryBlock(StructureType.ClosingBegin)) == null ||
                   (signature.ConditionExpression = TryExpression()) == null ||
                   (signature.ConditionEnd = TryBlock(StructureType.ClosingEnd)) == null ||
                   (signature.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(keywordStatement)
            {
                StatementSignature signature = new StatementSignature(statementType, false);
                signature.Keyword = keyword;
                if((signature.Complete = TrySeperator(StructureType.Complete)) == null)
                {
                    ;
                }
                return signature;
            }
            else if(expressionStatement)
            {
                ExpressionStatementSignature signature = new ExpressionStatementSignature(statementType);
                signature.Keyword = keyword;
                signature.ConditionExpression = TryExpression();
                signature.Complete = TrySeperator(StructureType.Complete);
                return signature;
            }
            else if(blockStatement)
            {
                BlockStatementSignature signature = new BlockStatementSignature(statementType);
                signature.Keyword = keyword;
                if((signature.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(statementType == StatementType.For)
            {
                ForLoopStatementSignature signature = new ForLoopStatementSignature();
                signature.ConditionBegin = TryBlock(StructureType.ClosingBegin);
                while(true)
                {
                    if((signature.ParameterDeclarationSeperator = TrySeperator(StructureType.Complete)) != null)
                    {
                        break;
                    }
                    TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
                    if(typeDeclaration != null)
                    {
                        signature.ParameterList.Add(typeDeclaration);
                        if ((typeDeclaration.Seperator = TrySeperator(StructureType.Seperator)) != null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        ExpressionSignature expression = TryExpression();
                        if (expression != null)
                        {
                            signature.ParameterList.Add(expression);
                            if ((expression.Seperator = TrySeperator(StructureType.Seperator)) != null)
                            {
                                continue;
                            }
                        }
                    }
                    if ((signature.ParameterDeclarationSeperator = TrySeperator(StructureType.Complete)) != null)
                    {
                        break;
                    }
                    //
                    break;
                }
                signature.ConditionExpression = TryExpression();
                signature.ConditionSeperator = TrySeperator(StructureType.Complete);
                while(true)
                {
                    if((signature.ConditionEnd = TryBlock(StructureType.ClosingEnd)) != null)
                    {
                        break;
                    }
                    ExpressionSignature expression = TryExpression();
                    if (expression != null)
                    {
                        signature.PostOperationList.Add(expression);
                        if ((expression.Seperator = TrySeperator(StructureType.Seperator)) != null)
                        {
                            continue;
                        }
                    }
                    if ((signature.ConditionEnd = TryBlock(StructureType.ClosingEnd)) != null)
                    {
                        break;
                    }
                    //
                    break;
                }
                signature.ConditionEnd = TryBlock(StructureType.ClosingEnd);
                if((signature.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else
            {
                throw new Exception("invalid state");
            }
        }
    }

    public class CodeSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public StatementSignatureList Statements = new StatementSignatureList();
        public TokenSymbol BlockEnd;

        public CodeSignature() : base(SignatureType.Code)
        { }

        public override string ToString()
        {
            return Statements.ToString();
        }
    }

    public class StatementSignatureList : ListCollection<StatementSignature>
    {
        public override string ToString()
        {
            string str = "";
            for(int i=0; i<Size; i++)
            {
                str += Get(i);
                if(i < Size-1)
                {
                    str += "\n";
                }
            }
            return str;
        }
    }

    public class StatementSignature : SignatureSymbol
    {
        public TokenSymbol Keyword;
        public TokenSymbol Complete;
        public StatementType Type;
        public CodeSignature Code;
        public StatementSignature Parent;
        public StatementSignatureList ChildStatements;

        public StatementSignature(StatementType StatementType, bool HasChildStatements) : base(SignatureType.Statement)
        {
            this.Type = StatementType;
            if (HasChildStatements)
            {
                this.ChildStatements = new StatementSignatureList();
            }
        }
        
        public override string ToString()
        {
            return "statement(type:"+Type+")";
        }
    }

    public class TypeDeclarationStatementSignature : StatementSignature
    {
        public TypeDeclarationSignature TypeDeclaration;

        public TypeDeclarationStatementSignature() : base(StatementType.TypeDeclaration, false)
        { }

        public override string ToString()
        {
            return "type_declaration_statement(" + TypeDeclaration + ")";
        }
    }

    public class ExpressionStatementSignature : StatementSignature
    {
        public ExpressionSignature ConditionExpression;

        public ExpressionStatementSignature(StatementType StatementType) : base(StatementType, false)
        { }

        public override string ToString()
        {
            return "expression_statement(type:" + Type + ", expression("+ConditionExpression+"))";
        }
    }

    public class BlockStatementSignature : StatementSignature
    {
        public TokenSymbol BlockBegin;
        public TokenSymbol BlockEnd;

        public BlockStatementSignature(StatementType StatementType) : base(StatementType, true)
        { }

        public override string ToString()
        {
            string str = "block_statement(type:" + Type + ")\n";
            str += ChildStatements.ToString();
            return str;
        }
    }

    public class ConditionBlockStatementSignature : BlockStatementSignature
    {
        public TokenSymbol ConditionBegin;
        public TokenSymbol ConditionEnd;
        public ExpressionSignature ConditionExpression;

        public ConditionBlockStatementSignature(StatementType StatementType) : base(StatementType)
        { }

        public override string ToString()
        {
            string str = "condition_block_statement(type:" + Type + ", expression(" + ConditionExpression + "))\n";
            str += ChildStatements.ToString();
            return str;
        }
    }

    public class ForLoopStatementSignature : ConditionBlockStatementSignature
    {
        public SignatureList ParameterList = new SignatureList();
        public TokenSymbol ParameterDeclarationSeperator;
        public TokenSymbol ConditionSeperator;
        public SignatureList PostOperationList = new SignatureList();

        public ForLoopStatementSignature() : base(StatementType.For)
        { }

        public override string ToString()
        {
            string str = "for_loop(";
            str +="declaration("+ ParameterList + ")";
            str += ", condition_expression(" + ConditionExpression + ")";
            str += ", operations(" + PostOperationList + ")\n";
            str += ChildStatements.ToString();
            return str;
        }
    }
}
