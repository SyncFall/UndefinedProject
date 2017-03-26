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
            Symbol blockBegin = TryNonSpace(StructureType.BlockBegin);
            if(blockBegin == null)
            {
                return null;
            }
            CodeSignature signature = new CodeSignature();
            signature.BlockBegin = blockBegin;
            signature.Elements = TryStatementList();
            if((signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null){
                ;
            }
            return signature;
        }

        public SignatureList TryStatementList()
        {
            SignatureList signature=null;
            SignatureSymbol element;
            while (true)
            {
                if((element = TryStatement()) == null)
                    break;
                if (signature == null)
                    signature = new SignatureList();
                signature.Add(element);
            }
            return signature;
        }

        public StatementSignature TryStatement()
        {
            TrySpace();

            Symbol keyword=null, secondKeyword=null;
            StatementType type;
            StatementCategory group;

            if((keyword = TryToken(StatementKeywordType.If)) != null)
            {
                type = StatementType.If;
                group = StatementCategory.ConditionBlock;
            }
            else if((keyword = TryToken(StatementKeywordType.Else)) != null)
            {
                if((secondKeyword = TryToken(StatementKeywordType.If)) != null)
                {
                    type = StatementType.ElseIf;
                    group = StatementCategory.ConditionBlock;
                }
                else
                {
                    type = StatementType.Else;
                    group = StatementCategory.BlockStatement;
                }
            }
            else if((keyword = TryToken(StatementKeywordType.For)) != null)
            {
                type = StatementType.For;
                group = StatementCategory.ForLoop;
            }
            else if((keyword = TryToken(StatementKeywordType.While)) != null)
            {
                type = StatementType.While;
                group = StatementCategory.ConditionBlock;
            }
            else if((keyword = TryToken(StatementKeywordType.Continue)) != null)
            {
                type = StatementType.Continue;
                group = StatementCategory.KeywordStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Break)) != null)
            {
                type = StatementType.Break;
                group = StatementCategory.KeywordStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Return)) != null)
            {
                type = StatementType.Return;
                group = StatementCategory.ExpressionStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Sanity)) != null)
            {
                type = StatementType.Sanity;
                group = StatementCategory.ExpressionStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Throw)) != null)
            {
                type = StatementType.Throw;
                group = StatementCategory.ExpressionStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Try)) != null)
            {
                type = StatementType.Try;
                group = StatementCategory.BlockStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Finally)) != null)
            {
                type = StatementType.Finally;
                group = StatementCategory.BlockStatement;
            }
            else if((keyword = TryToken(StatementKeywordType.Sync)) != null)
            {
                type = StatementType.Sync;
                group = StatementCategory.ConditionBlock;
            }
            else if(TryToken(StructureType.Complete) != null)
            {
                StatementSignature signature = new StatementSignature(StatementType.NoOperation, StatementCategory.NoOperation, false);
                signature.Keyword = PrevToken;
                return signature;
            }
            else
            {
                Symbol beginBlock = TryNonSpace(StructureType.BlockBegin);
                if(beginBlock != null)
                {
                    BlockStatementSignature signature = new BlockStatementSignature(StatementType.InnerBlock);
                    signature.BlockBegin = beginBlock;
                    if((signature.Elements = TryStatementList()) == null ||
                       (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                    ){
                        ;
                    }
                    return signature;
                }
                TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
                if (typeDeclaration != null)
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

            if(group == StatementCategory.ConditionBlock)
            {
                ConditionBlockStatementSignature signature = new ConditionBlockStatementSignature(type);
                signature.Keyword = keyword;
                if((signature.ConditionBegin = TryNonSpace(StructureType.ClosingBegin)) == null ||
                   (signature.ConditionExpression = TryExpression()) == null ||
                   (signature.ConditionEnd = TryNonSpace(StructureType.ClosingEnd)) == null ||
                   (signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null ||
                   (signature.Elements = TryStatementList()) == null ||
                   (signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null
                ){
                    ;
                }
                return signature;
            }
            else if(group == StatementCategory.KeywordStatement)
            {
                StatementSignature signature = new StatementSignature(type, group, false);
                signature.Keyword = keyword;
                if((signature.Complete = TryNonSpace(StructureType.Complete)) == null)
                {
                    ;
                }
                return signature;
            }
            else if(group == StatementCategory.ExpressionStatement)
            {
                ExpressionStatementSignature signature = new ExpressionStatementSignature(type);
                signature.Keyword = keyword;
                signature.Expression = TryExpression();
                signature.Complete = TryNonSpace(StructureType.Complete);
                return signature;
            }
            else if(group == StatementCategory.BlockStatement)
            {
                BlockStatementSignature signature = new BlockStatementSignature(type, group);
                signature.Keyword = keyword;
                if((signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null ||
                   (signature.Elements = TryStatementList()) == null ||
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
                   (signature.Elements = TryStatementList()) == null ||
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
        public Symbol BlockBegin;
        public SignatureList Elements;
        public Symbol BlockEnd;

        public CodeSignature() : base(SignatureType.Code)
        { }

        public override string ToString()
        {
            return (Elements!=null?Elements.ToString():"");
        }
    }

    public class StatementSignature : SignatureSymbol
    {
        public Symbol Keyword;
        public Symbol Complete;
        public StatementType Type;
        public StatementCategory Group;
        public CodeSignature Code;
        public StatementSignature Parent;
        public SignatureList Elements;

        public StatementSignature(StatementType StatementType, StatementCategory StatementGroup, bool HasChildStatements=false) : base(SignatureType.Statement)
        {
            this.Type = StatementType;
            this.Group = StatementGroup;
            if (HasChildStatements)
            {
                this.Elements = new SignatureList();
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

        public TypeDeclarationStatementSignature() : base(StatementType.TypeDeclaration, StatementCategory.TypeDeclaration)
        { }

        public override string ToString()
        {
            return "type_declaration_statement(" + TypeDeclaration + ")";
        }
    }

    public class ExpressionStatementSignature : StatementSignature
    {
        public ExpressionSignature Expression;

        public ExpressionStatementSignature(StatementType StatementType) : base(StatementType, StatementCategory.ExpressionStatement)
        { }

        public override string ToString()
        {
            return "expression_statement(type:" + Type + ", "+Expression+")";
        }
    }

    public class BlockStatementSignature : StatementSignature
    {
        public Symbol BlockBegin;
        public Symbol BlockEnd;

        public BlockStatementSignature(StatementType StatementType, StatementCategory StatementGroup=StatementCategory.BlockStatement) : base(StatementType, StatementGroup, true)
        { }

        public override string ToString()
        {
            string str = "block_statement(type:" + Type + ")\n";
            if(Elements!=null) str += Elements.ToString();
            return str;
        }
    }

    public class ConditionBlockStatementSignature : BlockStatementSignature
    {
        public Symbol ConditionBegin;
        public Symbol ConditionEnd;
        public ExpressionSignature ConditionExpression;

        public ConditionBlockStatementSignature(StatementType StatementType, StatementCategory StatementGroup=StatementCategory.ConditionBlock) : base(StatementType, StatementGroup)
        { }

        public override string ToString()
        {
            string str = "condition_block_statement(type:" + Type + ", expression(" + ConditionExpression + "))\n";
            str += (Elements!=null?Elements.ToString():"");
            return str;
        }
    }

    public class ForLoopStatementSignature : ConditionBlockStatementSignature
    {
        public SignatureList ParameterList = new SignatureList();
        public Symbol ParameterDeclarationSeperator;
        public Symbol ConditionSeperator;
        public SignatureList PostOperationList = new SignatureList();

        public ForLoopStatementSignature() : base(StatementType.For, StatementCategory.ForLoop)
        { }

        public override string ToString()
        {
            string str = "for_loop(";
            str +="declaration("+ ParameterList + ")";
            str += ", condition_expression(" + ConditionExpression + ")";
            str += ", operations(" + PostOperationList + ")\n";
            str += Elements.ToString();
            return str;
        }
    }
}
