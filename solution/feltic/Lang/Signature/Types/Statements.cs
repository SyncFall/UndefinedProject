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
        public CodeSignature TryCode()
        {
            TokenSymbol blockBegin = TryNonSpace(StructureType.BlockBegin);
            if (blockBegin == null)
            {
                return null;
            }
            CodeSignature signature = new CodeSignature();
            signature.BlockBegin = blockBegin;
            if((signature.Statements = TryStatementList()) == null ||
              ((signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null)
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
            StatementType type;
            StatementGroup group;

            if ((keyword = TryToken(StatementKeywordType.If)) != null)
            {
                type = StatementType.If;
                group = StatementGroup.ConditionBlock;
            }
            else if((keyword = TryToken(StatementKeywordType.Else)) != null)
            {
                if((secondKeyword = TryToken(StatementKeywordType.If)) != null)
                {
                    type = StatementType.ElseIf;
                    group = StatementGroup.ConditionBlock;
                }
                else
                {
                    type = StatementType.Else;
                    group = StatementGroup.BlockStatement;
                }
            }
            else if((keyword = TryToken(StatementKeywordType.For)) != null)
            {
                type = StatementType.For;
                group = StatementGroup.ForLoop;
            }
            else if((keyword = TryToken(StatementKeywordType.While)) != null)
            {
                type = StatementType.While;
                group = StatementGroup.ConditionBlock;
            }
            else if((keyword = TryToken(StatementKeywordType.Continue)) != null)
            {
                type = StatementType.Continue;
                group = StatementGroup.KeywordStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Break)) != null)
            {
                type = StatementType.Break;
                group = StatementGroup.KeywordStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Return)) != null)
            {
                type = StatementType.Return;
                group = StatementGroup.ExpressionStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Sanity)) != null)
            {
                type = StatementType.Sanity;
                group = StatementGroup.ExpressionStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Throw)) != null)
            {
                type = StatementType.Throw;
                group = StatementGroup.ExpressionStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Try)) != null)
            {
                type = StatementType.Try;
                group = StatementGroup.BlockStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Finally)) != null)
            {
                type = StatementType.Finally;
                group = StatementGroup.BlockStatement;
            }
            else if ((keyword = TryToken(StatementKeywordType.Sync)) != null)
            {
                type = StatementType.Sync;
                group = StatementGroup.ConditionBlock;
            }
            else if(TryToken(StructureType.Complete) != null)
            {
                StatementSignature signature = new StatementSignature(StatementType.NoOperation, StatementGroup.NoOperation, false);
                signature.Keyword = PrevToken;
                return signature;
            }
            else
            {
                TokenSymbol beginBlock = TryNonSpace(StructureType.BlockBegin);
                if(beginBlock != null)
                {
                    BlockStatementSignature signature = new BlockStatementSignature(StatementType.InnerBlock);
                    signature.BlockBegin = beginBlock;
                    if((signature.ChildStatements = TryStatementList()) == null ||
                       (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                    ){
                        ;
                    }
                    return signature;
                }
                TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
                if(typeDeclaration != null)
                {
                    TypeDeclarationStatementSignature signature = new TypeDeclarationStatementSignature();
                    signature.TypeDeclaration = typeDeclaration;
                    signature.Complete = TryNonSpace(StructureType.Complete);
                    return signature;
                }
                ExpressionSignature expression = TryExpression();
                if (expression != null)
                {
                    ExpressionStatementSignature signature = new ExpressionStatementSignature(StatementType.ExpressionStatement);
                    signature.Expression = expression;
                    signature.Complete = TryNonSpace(StructureType.Complete);
                    return signature;
                }
                //
                return null;
            }

            if(group == StatementGroup.ConditionBlock)
            {
                ConditionBlockStatementSignature signature = new ConditionBlockStatementSignature(type);
                signature.Keyword = keyword;
                if((signature.ConditionBegin = TryNonSpace(StructureType.ClosingBegin)) == null ||
                   (signature.ConditionExpression = TryExpression()) == null ||
                   (signature.ConditionEnd = TryNonSpace(StructureType.ClosingEnd)) == null ||
                   (signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(group == StatementGroup.KeywordStatement)
            {
                StatementSignature signature = new StatementSignature(type, group, false);
                signature.Keyword = keyword;
                if((signature.Complete = TryNonSpace(StructureType.Complete)) == null)
                {
                    ;
                }
                return signature;
            }
            else if(group == StatementGroup.ExpressionStatement)
            {
                ExpressionStatementSignature signature = new ExpressionStatementSignature(type);
                signature.Keyword = keyword;
                signature.Expression = TryExpression();
                signature.Complete = TryNonSpace(StructureType.Complete);
                return signature;
            }
            else if(group == StatementGroup.BlockStatement)
            {
                BlockStatementSignature signature = new BlockStatementSignature(type, group);
                signature.Keyword = keyword;
                if((signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(type == StatementType.For)
            {
                ForLoopStatementSignature signature = new ForLoopStatementSignature();
                signature.ConditionBegin = TryNonSpace(StructureType.ClosingBegin);
                while (true)
                {
                    if ((signature.ParameterDeclarationSeperator = TryNonSpace(StructureType.Complete)) != null)
                    {
                        break;
                    }
                    TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
                    if (typeDeclaration != null)
                    {
                        signature.ParameterList.Add(typeDeclaration);
                        if ((typeDeclaration.Seperator = TryNonSpace(StructureType.Seperator)) != null)
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
                            if ((expression.Seperator = TryNonSpace(StructureType.Seperator)) != null)
                            {
                                continue;
                            }
                        }
                    }
                    if ((signature.ParameterDeclarationSeperator = TryNonSpace(StructureType.Complete)) != null)
                    {
                        break;
                    }
                    //
                    break;
                }
                signature.ConditionExpression = TryExpression();
                signature.ConditionSeperator = TryNonSpace(StructureType.Complete);
                while (true)
                {
                    if ((signature.ConditionEnd = TryNonSpace(StructureType.ClosingEnd)) != null)
                    {
                        break;
                    }
                    ExpressionSignature expression = TryExpression();
                    if (expression != null)
                    {
                        signature.PostOperationList.Add(expression);
                        if ((expression.Seperator = TryNonSpace(StructureType.Seperator)) != null)
                        {
                            continue;
                        }
                    }
                    if ((signature.ConditionEnd = TryNonSpace(StructureType.ClosingEnd)) != null)
                    {
                        break;
                    }
                    //
                    break;
                }
                signature.ConditionEnd = TryNonSpace(StructureType.ClosingEnd);
                if((signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null ||
                   (signature.ChildStatements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                )
                {
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
        public StatementGroup Group;
        public CodeSignature Code;
        public StatementSignature Parent;
        public StatementSignatureList ChildStatements;

        public StatementSignature(StatementType StatementType, StatementGroup StatementGroup, bool HasChildStatements=false) : base(SignatureType.Statement)
        {
            this.Type = StatementType;
            this.Group = StatementGroup;
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

        public TypeDeclarationStatementSignature() : base(StatementType.TypeDeclaration, StatementGroup.TypeDeclaration)
        { }

        public override string ToString()
        {
            return "type_declaration_statement(" + TypeDeclaration + ")";
        }
    }

    public class ExpressionStatementSignature : StatementSignature
    {
        public ExpressionSignature Expression;

        public ExpressionStatementSignature(StatementType StatementType) : base(StatementType, StatementGroup.ExpressionStatement)
        { }

        public override string ToString()
        {
            return "expression_statement(type:" + Type + ", expression("+Expression+"))";
        }
    }

    public class BlockStatementSignature : StatementSignature
    {
        public TokenSymbol BlockBegin;
        public TokenSymbol BlockEnd;

        public BlockStatementSignature(StatementType StatementType, StatementGroup StatementGroup=StatementGroup.BlockStatement) : base(StatementType, StatementGroup, true)
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

        public ConditionBlockStatementSignature(StatementType StatementType, StatementGroup StatementGroup=StatementGroup.ConditionBlock) : base(StatementType, StatementGroup)
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

        public ForLoopStatementSignature() : base(StatementType.For, StatementGroup.ForLoop)
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
