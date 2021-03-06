﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class Validator
    {
        public void ValidateStatementBlock(SignatureList SignatureList)
        {
            if (SignatureList == null) return;
            SignatureSymbol signature;
            StatementSignature statement, lastStatement=null;
            for (int i = 0; i < SignatureList.Size; i++)
            {
                signature = SignatureList[i];
                if(signature.Type != SignatureType.Statement) continue;
                statement = signature as StatementSignature;

                if (statement.Type == StatementType.If)
                {
                    ConditionBlockStatementSignature ifStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(ifStatement.ConditionExpression);
                    ValidateStatementBlock(ifStatement.Elements);
                }
                else if (statement.Type == StatementType.ElseIf)
                {
                    if (lastStatement == null || lastStatement.Type != StatementType.If){
                        ;
                    }
                    ConditionBlockStatementSignature elseIfStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(elseIfStatement.ConditionExpression);
                    ValidateStatementBlock(elseIfStatement.Elements);
                }
                else if (statement.Type == StatementType.Else)
                {
                    BlockStatementSignature elseStatement = statement as BlockStatementSignature;
                    ValidateStatementBlock(elseStatement.Elements);
                }
                else if(statement.Type == StatementType.For)
                {
                    ForLoopStatementSignature forStatement = statement as ForLoopStatementSignature;
                    ValidateExpression(forStatement.ConditionExpression);
                    ValidateStatementBlock(forStatement.Elements);
                }
                else if (statement.Type == StatementType.While)
                {
                    ConditionBlockStatementSignature whileStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(whileStatement.ConditionExpression);
                    ValidateStatementBlock(whileStatement.Elements);
                }
                else if (statement.Type == StatementType.Continue || statement.Type == StatementType.Break)
                {
                    bool foundLoopBlock = false;
                    StatementSignature parentStatement = statement.Parent;
                    while (parentStatement != null)
                    {
                        if (parentStatement.Type == StatementType.While || parentStatement.Type == StatementType.For)
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
                    ValidateExpression(sanityStatement.Expression);
                }
                else if (statement.Type == StatementType.Throw)
                {
                    ExpressionStatementSignature throwStatement = statement as ExpressionStatementSignature;
                    ValidateExpression(throwStatement.Expression);
                }
                else if (statement.Type == StatementType.Try || statement.Type == StatementType.Finally)
                {
                    BlockStatementSignature tryStatement = statement as BlockStatementSignature;
                    ValidateStatementBlock(tryStatement.Elements);
                }
                else if (statement.Type == StatementType.Sync)
                {
                    ConditionBlockStatementSignature syncStatement = statement as ConditionBlockStatementSignature;
                    ValidateExpression(syncStatement.ConditionExpression);
                    ValidateStatementBlock(syncStatement.Elements);
                }
                else if (statement.Type == StatementType.NoOperation)
                {
                    ;
                }
                else if (statement.Type == StatementType.InnerBlock)
                {
                    ValidateStatementBlock(statement.Elements);
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
                    ValidateExpression(expressionStatement.Expression);
                }
                else
                {
                    throw new Exception("invalid state");
                }
                lastStatement = statement;
            }
        }

    }
}
