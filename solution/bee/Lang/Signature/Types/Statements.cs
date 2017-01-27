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
        public CodeSignature TryCode(MethodSignature Method)
        {
            BlockSignature blockBegin = TryBlock(StructureType.BlockBegin);
            if (blockBegin == null)
            {
                return null;
            }
            CodeSignature signature = new CodeSignature(Method);
            signature.BlockBegin = blockBegin;
            if((signature.Statements = TryStatementList(null, signature)) == null ||
              ((signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null)
            ){
                ;
            }
            return signature;
        }

        public StatementSignatureList TryStatementList(StatementSignature Parent, CodeSignature Code=null)
        {
            StatementSignatureList list = new StatementSignatureList();
            StatementSignature signatur;
            while ((signatur = TryStatement(Parent)) != null)
            {
                list.Add(signatur);
            }
            return list;
        }

        public StatementSignature TryStatement(StatementSignature Parent)
        {
            TrySpace();
            StatementToken first=null, second=null;
            StatementType statementType;
            bool conditionBlock = false;
            bool keywordStatement = false;
            bool expressionStatement = false;
            bool blockStatement = false;

            if ((first = TryToken(StatementKeywordType.If)) != null)
            {
                statementType = StatementType.If;
                conditionBlock = true;
            }
            else if((first = TryToken(StatementKeywordType.Else)) != null)
            {
                if ((second = TryToken(StatementKeywordType.If)) != null)
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
            else if((first = TryToken(StatementKeywordType.While)) != null)
            {
                statementType = StatementType.While;
                conditionBlock = true;
            }
            else if((first = TryToken(StatementKeywordType.Continue)) != null)
            {
                statementType = StatementType.Continue;
                keywordStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Break)) != null)
            {
                statementType = StatementType.Break;
                keywordStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Return)) != null)
            {
                statementType = StatementType.Return;
                expressionStatement = true;
            }
            else if((first = TryToken(StatementKeywordType.Sanity)) != null)
            {
                statementType = StatementType.Sanity;
                expressionStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Throw)) != null)
            {
                statementType = StatementType.Throw;
                expressionStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Try)) != null)
            {
                statementType = StatementType.Try;
                blockStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Finally)) != null)
            {
                statementType = StatementType.Finally;
                blockStatement = true;
            }
            else if ((first = TryToken(StatementKeywordType.Sync)) != null)
            {
                statementType = StatementType.Sync;
                conditionBlock = true;
            }
            else if(TryToken(StructureType.Complete) != null)
            {
                StatementSignature signature = new StatementSignature(StatementType.NoOperation, false, Parent);
                signature.Keyword = new KeywordSignature(PrevToken);
                return signature;
            }
            else
            {
                BlockSignature beginBlock = TryBlock(StructureType.BlockBegin);
                if(beginBlock != null)
                {
                    BlockStatementSignature signature = new BlockStatementSignature(StatementType.InnerBlock, Parent);
                    signature.BlockBegin = beginBlock;
                    if((signature.ChildStatements = TryStatementList(Parent)) == null ||
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
                        TypeDeclarationStatementSignature signature = new TypeDeclarationStatementSignature(Parent);
                        signature.TypeDeclaration = typeDeclaration;
                        if ((signature.Complete = TrySeperator(StructureType.Complete)) == null)
                        {
                            ;
                        }
                        return signature;
                    }
                    else
                    {
                        ExpressionSignature expression = TryExpression();
                        if (expression != null)
                        {
                            ExpressionStatementSignature signature = new ExpressionStatementSignature(StatementType.ExpressionStatement, Parent);
                            signature.ConditionExpression = expression;
                            if ((signature.Complete = TrySeperator(StructureType.Complete)) == null)
                            {
                                ;
                            }
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
                ConditionBlockStatementSignature signature = new ConditionBlockStatementSignature(statementType, Parent);
                signature.Keyword = new KeywordSignature(first);
                if((signature.ConditionExpression = TryExpression()) == null ||
                   (signature.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList(signature)) == null ||
                   (signature.BlockEnd = TryBlock(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(keywordStatement)
            {
                StatementSignature signature = new StatementSignature(statementType, false, Parent);
                signature.Keyword = new KeywordSignature(first);
                if((signature.Complete = TrySeperator(StructureType.Complete)) == null)
                {
                    ;
                }
                return signature;
            }
            else if(expressionStatement)
            {
                ExpressionStatementSignature signature = new ExpressionStatementSignature(statementType, Parent);
                signature.Keyword = new KeywordSignature(first);
                if((signature.ConditionExpression = TryExpression()) == null ||
                   (signature.Complete = TrySeperator(StructureType.Complete)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(blockStatement)
            {
                BlockStatementSignature signature = new BlockStatementSignature(statementType, Parent);
                signature.Keyword = new KeywordSignature(first);
                if((signature.BlockBegin = TryBlock(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList(signature)) == null ||
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
        public MethodSignature Method;
        public BlockSignature BlockBegin;
        public StatementSignatureList Statements = new StatementSignatureList();
        public BlockSignature BlockEnd;

        public CodeSignature(MethodSignature Method) : base(SignatureType.Code)
        {
            this.Method = Method;
        }

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
            for(int i=0; i<Size(); i++)
            {
                str += Get(i);
                if(i < Size()-1)
                {
                    str += "\n";
                }
            }
            return str;
        }
    }

    public class StatementSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public SeperatorSignature Complete;
        public StatementType Type;
        public CodeSignature Code;
        public StatementSignature Parent;
        public StatementSignatureList ChildStatements;

        public StatementSignature(StatementType StatementType, bool HasChildStatements, StatementSignature Parent) : base(SignatureType.Statement)
        {
            this.Type = StatementType;
            this.Code = (Parent != null ? Parent.Code : null);
            this.Parent = Parent;
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

        public TypeDeclarationStatementSignature(StatementSignature Parent) : base(StatementType.TypeDeclaration, false, Parent)
        { }

        public override string ToString()
        {
            return "type_declaration_statement(" + TypeDeclaration + ")";
        }
    }

    public class ExpressionStatementSignature : StatementSignature
    {
        public ExpressionSignature ConditionExpression;

        public ExpressionStatementSignature(StatementType StatementType, StatementSignature Parent) : base(StatementType, false, Parent)
        { }

        public override string ToString()
        {
            return "expression_statement(type:" + Type + ", expression("+ConditionExpression+"))";
        }
    }

    public class BlockStatementSignature : StatementSignature
    {
        public BlockSignature BlockBegin;
        public BlockSignature BlockEnd;

        public BlockStatementSignature(StatementType StatementType, StatementSignature Parent) : base(StatementType, true, Parent)
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
        public ExpressionSignature ConditionExpression;

        public ConditionBlockStatementSignature(StatementType StatementType, StatementSignature Parent) : base(StatementType, Parent)
        { }

        public override string ToString()
        {
            string str = "condition_block_statement(type:" + Type + ", expression(" + ConditionExpression + "))\n";
            str += ChildStatements.ToString();
            return str;
        }
    }
}
