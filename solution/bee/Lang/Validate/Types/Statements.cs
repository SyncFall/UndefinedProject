using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public partial class Validator
    {
        public void ValidateStatementBlock(StatementSignatureList StatementList)
        {
            for (int i = 0; i < StatementList.Size(); i++)
            {
                StatementSignature statement = StatementList.Get(i);
                if (statement.Type == StatementType.If)
                {
                    ConditionBlockStatementSignature ifStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(ifStatement.ConditionExpression);
                    ValidateStatementBlock(ifStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.ElseIf)
                {
                    StatementSignature lastLastment = (i > 0 ? StatementList.Get(i - 1) : null);
                    if (lastLastment == null || lastLastment.Type != StatementType.If)
                    {
                        ;
                    }
                    ConditionBlockStatementSignature elseIfStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(elseIfStatement.ConditionExpression);
                    ValidateStatementBlock(elseIfStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.Else)
                {
                    BlockStatementSignature elseStatement = statement as BlockStatementSignature;
                    ValidateStatementBlock(elseStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.While)
                {
                    ConditionBlockStatementSignature whileStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(whileStatement.ConditionExpression);
                    ValidateStatementBlock(whileStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.Continue || statement.Type == StatementType.Break)
                {
                    bool foundLoopBlock = false;
                    StatementSignature parentStatement = statement.Parent;
                    while (parentStatement != null)
                    {
                        if (parentStatement.Type == StatementType.While)
                        {
                            foundLoopBlock = true;
                            break;
                        }
                        parentStatement = parentStatement.Parent;
                    }
                    if (!foundLoopBlock)
                    {
                        ;
                    }
                }
                else if (statement.Type == StatementType.Return)
                {
                    ;
                }
                else if (statement.Type == StatementType.Sanity)
                {
                    ExpressionStatementSignature sanityStatement = statement as ExpressionStatementSignature;
                    ValidateExpression(sanityStatement.ConditionExpression);
                }
                else if (statement.Type == StatementType.Throw)
                {
                    ExpressionStatementSignature throwStatement = statement as ExpressionStatementSignature;
                    ValidateExpression(throwStatement.ConditionExpression);
                }
                else if (statement.Type == StatementType.Try || statement.Type == StatementType.Finally)
                {
                    BlockStatementSignature tryStatement = statement as BlockStatementSignature;
                    ValidateStatementBlock(tryStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.Sync)
                {
                    ConditionBlockStatementSignature syncStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(syncStatement.ConditionExpression);
                    ValidateStatementBlock(syncStatement.ChildStatements);
                }
                else if (statement.Type == StatementType.NoOperation)
                {
                    ;
                }
                else if (statement.Type == StatementType.InnerBlock)
                {
                    ValidateStatementBlock(statement.ChildStatements);
                }
                else if (statement.Type == StatementType.TypeDeclaration)
                {
                    TypeDeclarationStatementSignature typeDeclarationStatement = statement as TypeDeclarationStatementSignature;
                    if (typeDeclarationStatement.TypeDeclaration.TypeIdentifier != null)
                    {
                        if (Registry.GetObjectSymbol(typeDeclarationStatement.TypeDeclaration.TypeIdentifier.String) == null)
                        {
                            ;
                        }
                    }
                }
                else if (statement.Type == StatementType.ExpressionStatement)
                {
                    ExpressionStatementSignature expressionStatement = statement as ExpressionStatementSignature;
                    ValidateExpression(expressionStatement.ConditionExpression);
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
        }

    }
}
