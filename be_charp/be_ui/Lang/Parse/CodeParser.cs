using Be.Runtime.Types;
using System;

namespace Be.Runtime.Parse
{
    public class CodeParser
    {
        private TextParser textParser;
        private ExpressionParser expressionParser;
        private GenericsParser genericsParser;
        private ArrayParser arrayParser;

        public CodeParser(TextParser _textParser)
        {
            this.textParser = _textParser;
            this.expressionParser = new ExpressionParser(this.textParser);
            this.genericsParser = new GenericsParser(this.textParser);
            this.arrayParser = new ArrayParser(this.textParser, this.expressionParser);
        }

        public void ParseCode(SourceFile sourceType, int startPosition, CodeType CodeBlock)
        {
            ParseStatementBlock(sourceType, startPosition, 0, null, CodeBlock.Statements);
        }

        private void ParseStatementBlock(SourceFile sourceType, int startPosition, int depth, StatementType parentStatement, StatementCollection statementCollection)
        {
            textParser.SetPosition(startPosition);
            while (true)
            {
                textParser.SkipSpace(true);

                // check for end
                char chr = textParser.NextChar(false);
                if (chr == '}' || chr == '\0')
                {
                    return;
                }        
                // check for return-statement
                if (textParser.Equal(StatementConst.Return, true))
                {
#if(TRACK)
                    Utils.LogItem("return_statement");
#endif
                    FunctionControlStatementType returnStatement = new FunctionControlStatementType(StatementTypeEnum.RETURN, parentStatement);
                    statementCollection.Add(returnStatement);

                    // check if have a return-expression
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        ExpressionType expression = expressionParser.ParseExpression(sourceType);
                        returnStatement.ReturnExpression = expression;
                        
                        // check end
                        if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                        {
                            throw new Exception("return_statement is not complete");
                        }
                    }
                }
                // check for continue-statement
                else if (textParser.Equal(StatementConst.Continue, true))
                {
#if(TRACK)
                    Utils.LogItem("continue_statement");
#endif
                    LoopControlStatementType continueStatement = new LoopControlStatementType(StatementTypeEnum.CONTINUE, parentStatement);
                    statementCollection.Add(continueStatement);
                    
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        throw new Exception("continue_statement is not complete");
                    }
                }
                // check for break-statement
                else if (textParser.Equal(StatementConst.Break, true))
                {
#if(TRACK)
                    Utils.LogItem("break_statement");
#endif
                    LoopControlStatementType breakStatement = new LoopControlStatementType(StatementTypeEnum.BREAK, parentStatement);
                    statementCollection.Add(breakStatement);
                    
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        throw new Exception("break_statement is not complete");
                    }
                }
                // check for throw-statement
                else if (textParser.Equal(StatementConst.Throw, true))
                {
#if(TRACK)
                    Utils.LogItem("throw_statement");
#endif
                    if (!textParser.EqualSpace(true))
                    {
                        throw new Exception("throw_statement is invalid");
                    }

                    ErrorControlStatementType throwStatement = new ErrorControlStatementType(StatementTypeEnum.THROW, parentStatement);
                    statementCollection.Add(throwStatement);

                    // parse throw-expression
                    ExpressionType expression = expressionParser.ParseExpression(sourceType);
                    throwStatement.ErrorExpression = expression;
                    
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        throw new Exception("throw_statement is not complete");
                    }
                }
                // check for try-statement
                else if(textParser.Equal(StatementConst.Try, true))
                {
#if(TRACK)
                    Utils.LogItem("try_statement");
#endif
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in try-statement");
                    }

                    // parse child statement-block
                    ErrorProcessigStatementType errorStatement = new ErrorProcessigStatementType(StatementTypeEnum.TRY, parentStatement);
                    statementCollection.Add(errorStatement);

                    // parse block-code
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, errorStatement, errorStatement.Statements);

                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in try-statement (block-end)");
                    }
                }
                // check for catch-statement
                else if (textParser.Equal(StatementConst.Catch, true))
                {
#if(TRACK)
                    Utils.LogItem("catch_statement");
#endif
                    // excpetion handler start
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in catch-statement");
                    }

                    // get exception-type
                    string errorTypeName = textParser.GetPathContent(true);
                    if (errorTypeName == null)
                    {
                        throw new Exception("missing exception type");
                    }

                    // check space
                    if (!textParser.EqualSpace(true))
                    {
                        throw new Exception("missing space in exception handler");
                    }

                    // get exception-variable
                    string errorVariableName = textParser.GetPathContent(true);
                    if(errorVariableName == null)
                    {
                        throw new Exception("missing exception variable");
                    }

                    // excpetion handler end
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in catch-statement");
                    }

                    // block start
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in catch-statement");
                    }

                    ErrorProcessigStatementType errorStatement = new ErrorProcessigStatementType(StatementTypeEnum.CATCH, parentStatement);
                    errorStatement.DeclarationVariable = new VariableType(errorVariableName, errorTypeName, false);
                    statementCollection.Add(errorStatement);

                    // parse block-code
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, errorStatement, errorStatement.Statements);

                    // block end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in catch-statement (block-end)");
                    }
                }
                // check for finally-statement
                else if (textParser.Equal(StatementConst.Finally, true))
                {
#if(TRACK)
                    Utils.LogItem("finally_statement");
#endif
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in finally-statement");
                    }

                    // parse child statement-block
                    ErrorProcessigStatementType errorStatement = new ErrorProcessigStatementType(StatementTypeEnum.FINALLY, parentStatement);
                    statementCollection.Add(errorStatement);

                    // parse block-code
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, errorStatement, errorStatement.Statements);

                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in finally-statement (block-end)");
                    }
                }
                // check for lock-statement
                else if (textParser.Equal(StatementConst.Lock, true))
                {
#if(TRACK)
                    Utils.LogItem("lock_statement");
#endif
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in lock-statement");
                    }

                    // parse child statement-block
                    SynchronisationStatementType lockStatement = new SynchronisationStatementType(parentStatement);
                    statementCollection.Add(lockStatement);

                    // parse block-code
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, lockStatement, lockStatement.Statements);

                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in lock-statement (block-end)");
                    }
                }
                // check for if-statement
                else if (textParser.Equal(StatementConst.If, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in if-statement");
                    }
#if(TRACK)
                    Utils.LogItem("if-statement");
#endif
                    // parse condition-expression
                    ExpressionType expression = expressionParser.ParseExpression(sourceType);

                    ConditionStatementType ifBLock = new ConditionStatementType(StatementTypeEnum.IF, expression, parentStatement);
                    statementCollection.Add(ifBLock);

                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in if-statement (expression-end)");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in if-statement (block-start)");
                    }

                    // parse child statement-block
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, ifBLock, ifBLock.Statements);
                    
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in if-statement (block-end)");
                    }
                }
                // check for else-if statement
                else if (textParser.StartEndNoneSpace(StatementConst.Else, StatementConst.If, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in else-if statement");
                    }
#if(TRACK)
                    Utils.LogItem("else-if statement");
#endif
                    // parse condition-expression
                    ExpressionType expression = expressionParser.ParseExpression(sourceType);

                    ConditionStatementType elseIfBlock = new ConditionStatementType(StatementTypeEnum.ELSE_IF, expression, parentStatement);
                    statementCollection.Add(elseIfBlock);

                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in else-if-statement (expression-end)");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in else-if-statement (block-start)");
                    }

                    // parse child statement-list
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, elseIfBlock, elseIfBlock.Statements);

                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in else-if-statement (block-end)");
                    }
                }
                // check for else-statement
                else if (textParser.Equal(StatementConst.Else, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in else-if statement");
                    }
#if(TRACK)
                    Utils.LogItem("else-statement");
#endif
                    ConditionStatementType elseBlock = new ConditionStatementType(StatementTypeEnum.ELSE, null, parentStatement);
                    statementCollection.Add(elseBlock);

                    // parse child statement-list
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, elseBlock, elseBlock.Statements);

                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in else-statement (block-end)");
                    }
                }
                // check for foreach-statement
                else if(textParser.Equal(StatementConst.Foreach, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in foreach-statement");
                    }
#if (TRACK)
                    Utils.LogItem("foreach-statement");
#endif
                    ForeachLoopStatementType foreachBlock = new ForeachLoopStatementType(parentStatement);
                    statementCollection.Add(foreachBlock);

                    // check for variable type and name
                    string variableType = textParser.GetPathContent(true);
                    if (variableType == null)
                    {
                        throw new Exception("invalid variable-type in foreach-statement");
                    }
                    if (!textParser.EqualSpace(true))
                    {
                        throw new Exception("invalid variable-name/type in foreach-statement (missing space)");
                    }
                    string variableName = textParser.GetPathContent(true);
                    if (variableName == null)
                    {
                        throw new Exception("invalid variable-name in foreach-statement");
                    }
                    foreachBlock.DeclarationVariable = new VariableType(variableName, variableType, false);
#if (TRACK)
                    Utils.LogItem("declaration-variable = type: '"+variableName+"' | name: '"+variableType+"'");
#endif
                    // check for seperation-colon
                    if (!textParser.EqualNoneSpace(ObjectConst.Colon, true))
                    {
                        throw new Exception("missing seperation-colon in foreach-statement");
                    }

                    // check for list variable-name
                    string collectionVariableName = textParser.GetPathContent(true);
                    if (collectionVariableName == null)
                    {
                        throw new Exception("invalid collection-variable-name in foreach-statement");
                    }
                    foreachBlock.CollectionVariable = new VariableType(variableName);
#if (TRACK)
                    Utils.LogItem("collection-variable = name: '"+collectionVariableName+"'");
#endif
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in foreach-statement (expression-end)");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in foreach-statement (block-start)");
                    }

                    // parse child statement-block
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, foreachBlock, foreachBlock.Statements);

                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in forech-statement (block-end)");
                    }
                }
                // check for for-statement
                else if (textParser.Equal(StatementConst.For, true))
                {
                    // check statement-start
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in for-statement");
                    }
#if(TRACK)
                    Utils.LogItem("for-statement");
#endif
                    ForLoopStatementType forBlock = new ForLoopStatementType(parentStatement);
                    statementCollection.Add(forBlock);

                    // check for none-initialation (seperation)
                    if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
#if (TRACK)
                        Utils.LogItem("none-innitiation");
#endif
                    }
                    else
                    {
                        // check for possible variable declarations
                        if (IsPossibleVariableDeclaration())
                        {
#if (TRACK)
                            Utils.LogItem("variable-declaration-initiatiion");
#endif
                            // parse variable declaration-collection
                            forBlock.VariableDeclarationCollection = ParseVariableDeclartionCollection(sourceType);
                        }
                        // else, must be an expression statement-(list)
                        else
                        {
#if (TRACK)
                            Utils.LogItem("expression-list-initiation");
#endif
                            // check for expression-statement
                            ExpressionCollection expressionCollection = expressionParser.ParseExpressionParameterList(sourceType, ObjectConst.ParameterSeperator);
                            if (expressionCollection != null)
                            {
#if (TRACK)
                                Utils.LogItem("expression-statement^");
#endif
                                forBlock.ExpressionInitiationCollection = expressionCollection;
                            }
                            // invalid
                            else
                            {
                                throw new Exception("invalid expressionn-statement");
                            }
                        }
                        // check for seperation
                        if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                        {
                            throw new Exception("missing initialisation-seperation for-statement");
                        }
                    }

                    // check seperation
                    if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
#if (TRACK)
                        Utils.LogItem("none-conditiionn");
#endif
                    }
                    else
                    {
                        // get condidition-expression
                        forBlock.ConditionExpression = expressionParser.ParseExpression(sourceType);

                        // check for seperation
                        if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                        {
                            throw new Exception("missing condition-expression for-statement");
                        }
                    }

                    // check seperation
                    if (textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
#if (TRACK)
                        Utils.LogItem("none-post-expression-list");
#endif
                    }
                    else
                    {
                        // check for possible post-expression-parameter-list
                        forBlock.PostExpressionCollection = expressionParser.ParseExpressionParameterList(sourceType, ObjectConst.FunctionDeclosing);
                       
                        // check statement-end
                        if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                        {
                            throw new Exception("missing declosing-tag in post-expression-list in for-statement");
                        }
                    }

                    // check block-start
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in for-statement (block-start)");
                    }

                    // parse child statement-block
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, forBlock, forBlock.Statements);

                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in for-statement (block-end)");
                    }
                }
                // check for while-statement
                else if (textParser.Equal(StatementConst.While, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in while-statement");
                    }
#if(TRACK)
                    Utils.LogItem("while-statement");
#endif
                    // parse condition-expression
                    ExpressionType whileExpression = expressionParser.ParseExpression(sourceType);

                    WhileLoopStatementType whileBlock = new WhileLoopStatementType(parentStatement);
                    whileBlock.ConditionExpression = whileExpression;
                    statementCollection.Add(whileBlock);

                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in while-statement (expression-end)");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in while-statement (block-start)");
                    }

                    // parse child statement-block
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, whileBlock, whileBlock.Statements);
                    
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in while-statement (block-end)");
                    }
                }
                // check do-while statement
                else if (textParser.Equal(StatementConst.Do, true))
                {
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in do-while-statement (block-start)");
                    }
#if(TRACK)
                    Utils.LogItem("do-while statement");
#endif
                    DoWhileLoopStatementType doWhileBlock = new DoWhileLoopStatementType(parentStatement);
                    statementCollection.Add(doWhileBlock);

                    // parse child statement-block
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, doWhileBlock, doWhileBlock.Statements);

                    // check 
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in do-while-statement (block-end)");
                    }
                    // check while-statement
                    if (!textParser.EqualNoneSpace(StatementConst.While, true))
                    {
                        throw new Exception("missing while-tag in do-while-statement (block-end)");
                    }
                    // check
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        throw new Exception("missing enclosing-tag in do-while-statement");
                    }

                    // parse condition-expression
                    doWhileBlock.ConditionExpression = expressionParser.ParseExpression(sourceType);

                    // check
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in do-while-statement");
                    }
                    // check complete-state
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        throw new Exception("missing complate-statement in do-while-statement");
                    }
                }
                // check for inner-block
                else if (textParser.Equal(ObjectConst.BlockEnclosing, true))
                {
                    InnerBlockStatementType innerBlockStatement = new InnerBlockStatementType(parentStatement);
                    statementCollection.Add(innerBlockStatement);

                    // parse child statements
                    ParseStatementBlock(sourceType, textParser.GetPosition(), depth + 1, innerBlockStatement, innerBlockStatement.Statements);

                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("missing declosing-tag in empty statement");
                    }
                }
                // check for empty-statement
                else if (textParser.Equal(ObjectConst.StatementComplete, true))
                {
#if (TRACK)
                    Utils.LogItem("empty_statement");
#endif
                    NoOperationStatementType emptyStatement = new NoOperationStatementType(parentStatement);
                    statementCollection.Add(emptyStatement);
                }
                // check for declaration or expression statement
                else
                {
                    // check for possible variable declarations
                    if (IsPossibleVariableDeclaration())
                    {
                        // parse variable declartion
                        VariableDeclarationStatementType variableDeclartionStatement = new VariableDeclarationStatementType(parentStatement);
                        variableDeclartionStatement.VariableDeclarationCollection = ParseVariableDeclartionCollection(sourceType);
                        statementCollection.Add(variableDeclartionStatement);
                        // check for complete-statement
                        if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                        {
                            throw new Exception("variable-declaration has no complete-statement");
                        }
                    }
                    // else, must be an expression statement
                    else
                    {
#if (TRACK)
                        Utils.LogItem("expression-statement");
#endif
                        // check for expression-statement
                        ExpressionType statementExpression = expressionParser.ParseExpression(sourceType);
                        if (statementExpression != null)
                        {
                            ExpressionStatementType expressionStatement = new ExpressionStatementType(parentStatement, statementExpression);
                            statementCollection.Add(expressionStatement);
                        }
                        // invalid
                        else
                        {
                            throw new Exception("invalid syntax");
                        }
                        // check for complete-statement
                        if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                        {
                            throw new Exception("expression-statement has no complete-statement");
                        }
                    }

                }

            }
        }

        private bool IsPossibleVariableDeclaration()
        {
            int startPosition = textParser.GetPosition();
            // check object-name
            if (textParser.GetPathContent(true) == null)
            {
                textParser.SetPosition(startPosition);
                return false;
            }
            // forward possible array declaration
            arrayParser.ParseArrayDeclaration();
            // forward possible generics declaration
            genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.VARIABLE);
            // check space and variable-name
            if(!textParser.EqualSpace(true) || textParser.GetPathContent(true) == null)
            {
                textParser.SetPosition(startPosition);
                return false;
            }
            // rewind position
            textParser.SetPosition(startPosition);
            return true;
        }

        private VariableCollection ParseVariableDeclartionCollection(SourceFile sourceType)
        {
            VariableCollection variableCollection = new VariableCollection();

            // check for const-type
            bool isConst = false;
            if(textParser.EqualEndSpace(ObjectConst.Readonly, true))
            {
                isConst = true;
            }

            // get variable type
            string variableType = textParser.GetPathContent(true);
            if (variableType == null)
            {
                throw new Exception("invalid variable-type in statement");
            }

            // check for array declaration
            ArrayType arrayDeclarationType = arrayParser.ParseArrayDeclaration();

            // check for generics declarartion
            GenericType genericDeclarationType = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.VARIABLE);

            // check space
            if (!textParser.EqualSpace(true))
            {
                throw new Exception("invalid variable-name/type in statement (missing space)");
            }

            // get variable name
            string variableName = textParser.GetPathContent(true);
            if (variableName == null)
            {
                throw new Exception("invalid variable-name in statement");
            }
#if (TRACK)
            Utils.LogItem("variable_declaration: type | type: '" + variableType + "' | name:'" + variableName + "' | array: '"+(arrayDeclarationType!=null)+"'");
#endif
            // check declartion with definition statement
            if (textParser.EqualNoneSpace(ObjectConst.Assigment, true))
            {
#if (TRACK)
                Utils.LogItem("variable_initalisation");
#endif
                // parse variable-initialisation
                ExpressionType initialisationExpression = expressionParser.ParseExpression(sourceType);
                VariableType variable = new VariableType(variableName, variableType, initialisationExpression, isConst);
                // set possible array and declaration
                variable.ArrayDeclarationType = arrayDeclarationType;
                variable.GenericDeclarationType = genericDeclarationType;
                // add to list
                variableCollection.Add(variable);
            }
            // only declaration-statement
            else
            {
                // parse variable
                VariableType variable = new VariableType(variableName, variableType, isConst);
                // set possible array and declaration
                variable.ArrayDeclarationType = arrayDeclarationType;
                variable.GenericDeclarationType = genericDeclarationType;
                // add to list
                variableCollection.Add(variable);
            }
            // further more variable declartions, only names, not types further allowed
            while (true)
            {
                // check next parameter
                if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    break;
                }
                // check end
                if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, false))
                {
                    break;
                }

                // get another-variable name
                string anotherVariableName = textParser.GetPathContent(true);
                if (anotherVariableName == null)
                {
                    throw new Exception("invalid another variable-name in declaration");
                }
#if (TRACK)
                Utils.LogItem("variable_declaration: type:'" + variableType + "' another-name:'" + anotherVariableName + "'");
#endif
                // check declartion with definition statement
                if (textParser.EqualNoneSpace(ObjectConst.Assigment, true))
                {
#if (TRACK)
                    Utils.LogItem("variable_initalisation");
#endif
                    // parse variable-initialisation
                    ExpressionType initialisationExpression = expressionParser.ParseExpression(sourceType);
                    VariableType variable = new VariableType(anotherVariableName, variableType, initialisationExpression, isConst);
                    // set possible array and declaration
                    variable.ArrayDeclarationType = arrayDeclarationType;
                    variable.GenericDeclarationType = genericDeclarationType;
                    // add to list
                    variableCollection.Add(variable);
                }
                // only declaration-statement
                else
                {
                    // parse variable-initialisation
                    VariableType variable = new VariableType(anotherVariableName, variableType, isConst);
                    // set possible array and declaration
                    variable.ArrayDeclarationType = arrayDeclarationType;
                    variable.GenericDeclarationType = genericDeclarationType;
                    // add to list
                    variableCollection.Add(variable);
                }
            }

            // return variable collection
            return variableCollection;
        }
    }
}
