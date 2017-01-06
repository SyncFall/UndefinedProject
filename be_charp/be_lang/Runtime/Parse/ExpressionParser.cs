using Be.Runtime.Types;
using System;

namespace Be.Runtime.Parse
{
    public class ExpressionParser
    {
        private TextParser textParser;
        private GenericsParser genericsParser;
        private ArrayParser arrayParser;

        public ExpressionParser(TextParser TextParser)
        {
            this.textParser = TextParser;
            this.genericsParser = new GenericsParser(this.textParser);
            this.arrayParser = new ArrayParser(this.textParser, this);
        }

        public ExpressionType ParseExpression(SourceFile sourceType)
        {
            return ParseExpression(sourceType, true, false);
        }

        public ExpressionType ParseExpressionCanEmpty(SourceFile sourceType)
        {
            return ParseExpression(sourceType, true, true);   
        }

        public ExpressionType ParseExpression(SourceFile sourceType, bool asEnclosed, bool canEmpty)
        {
            textParser.SkipSpace(true);

            ExpressionType expression = new ExpressionType();
            bool isEmpty = true;
#if (TRACK)
            Utils.LogItem("expression_start");
#endif 
            // parse front-expression-operators
            ExpressionOperatorType expressionFrontOperator = ParseFrontExpressionOperator(sourceType);
            expression.FrontExpressionOperator = expressionFrontOperator;

            // parse child expression
            if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
            {
#if (TRACK)
                Utils.LogItem("child_expression_start");
#endif 
                expression.ChildExpression = ParseExpressionCanEmpty(sourceType);
                isEmpty = false;
                // check inner-end
                if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                {
                    throw new Exception("missing expression declosing-tag");
                }
#if (TRACK)
                Utils.LogItem("child_expression_end");
#endif 
            }
            // parse operand and operand-operations
            else
            {
                expression.Operand = ParseOperandAndOperators(sourceType);
                if (expression.Operand != null)
                {
                    isEmpty = false;
                }
                if(!canEmpty && isEmpty)
                {
                    throw new Exception("has no operand");
                }
            }

            // parse back-expression-operators
            ExpressionOperatorType expressionBackOperator = ParseBackExpressionOperator(sourceType);
            if(expressionBackOperator != null && isEmpty)
            {
                throw new Exception("invalid syntax in expression");
            }
            expression.BackExpressionOperator = expressionBackOperator;

            // own-context
            if (!asEnclosed)
            {
                return expression;
            }

            // get possible operations
            while (true)
            {
                // get possible expression-operation
                OperationType operation = ParseOperation(sourceType);
                if (operation == null)
                {
                    break;
                }

                // get second operation-expression
                ExpressionOperation expressionOperation = new ExpressionOperation();
                expressionOperation.Operation = operation;
                expressionOperation.SecondExpression = ParseExpression(sourceType);
                if (expression.OperationsWithExpressions == null)
                {
                    expression.OperationsWithExpressions = new ExpressionOperationCollection();
                }
                expression.OperationsWithExpressions.Add(expressionOperation);
            }
#if (TRACK)
            Utils.LogItem("expression_end");
#endif 
            return expression;
        }

        public OperandType ParseOperandAndOperators(SourceFile sourceType)
        {
            textParser.SkipSpace(true);

            OperandType operand;

            // check new-object operand
            if (textParser.EqualEndSpace(ObjectConst.NewType, true))
            {
                // get type-name
                string typeName = textParser.GetPathContent(true);
                if (typeName == null)
                {
                    throw new Exception("invalid new-type-name");
                }
                NewTypeOperand newTypeOperand = new NewTypeOperand(typeName);
                operand = newTypeOperand;
                // check for possible generic-types
                newTypeOperand.GenericType = genericsParser.ParseGenericsDeclaration(GenericsMode.DEFINITION, GenericCategoryEnum.VARIABLE);
                // check for object-definition
                newTypeOperand.ObjectDefinitionType = ParseObjectInitialisation(sourceType);
                // else check for possible array-definition
                if(newTypeOperand.ObjectDefinitionType == null)
                {
                    newTypeOperand.ArrayInitiationType = arrayParser.ParseArrayInitiation(sourceType);
                }
            }
            // check for this-operand
            else if (textParser.Equal(OperandConst.This, true))
            {
                operand = new ThisOperand();
            }
            // check for base-operand
            else if (textParser.Equal(OperandConst.Base, true))
            {
                operand = new BaseOperand();
            }
            // check for null-operand
            else if (textParser.Equal(OperandConst.Null, true))
            {
                operand = new NullOperand();
            }
            // check for value-operand
            else if(textParser.Equal(OperandConst.Value, true))
            {
                operand = new ValueOperand();
            }
            else
            {
                // parse possible basic-native type literal-operand
                operand = ParseLiteralOperand(sourceType);
                
                if(operand == null)
                {
                    // identify operand-type on first char
                    char chr = textParser.NextChar(false);

                    // check for variable-literal
                    if (Char.IsLetter(chr))
                    {
                        // get variable-name
                        string variableName = textParser.GetPathContent(true);
                        if (variableName == null)
                        {
                            throw new Exception("invalid var-name-content");
                        }

                        // get possible generic-types (only for method)
                        GenericType genericsType = genericsParser.ParseGenericsDeclaration(GenericsMode.DEFINITION, GenericCategoryEnum.VARIABLE);

                        // further check on next char
                        char nextChr = textParser.NextChar(false);

                        // check if default-depth method access
                        if (nextChr == '(')
                        {
                            chr = textParser.NextChar(true);
                            operand = new MethodOperand(variableName, genericsType);
                            // get function-paramater expression list
                            (operand as MethodOperand).ParameterExpressions = ParseExpressionParameterList(sourceType, ObjectConst.FunctionDeclosing);
                            // check end
                            if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                            {
                                throw new Exception("method-call no declosing-tag");
                            }
                        }
                        // member or variable access
                        else
                        {
                            operand = new MemberVariableOrParameterOperand(variableName);
                        }
                    }
                }
            }
#if (TRACK)
            Utils.LogItem(operand.GetDebugText());
#endif
            // parse operand operators
            OperandOperatorCollection operandOperators = operand.Operations;
            while (true)
            {
                // identify operand-operation on first char
                char chr = textParser.NextChar(false);

                // check for end of operations
                if (chr != '.' && chr != '[')
                {
                    return operand;
                }

                // in-depth member/method/array access
                textParser.NextChar(true);

                // get variable-name
                string variableName = textParser.GetNameContent(true);
                if (variableName == null)
                {
                    throw new Exception("invalid var-name-content");
                }

                // get possible generic-types
                GenericType genericType = genericsParser.ParseGenericsDeclaration(GenericsMode.DEFINITION, GenericCategoryEnum.VARIABLE);

                // check for array-acces
                if(textParser.EqualNoneSpace(ObjectConst.BracketEnclosing, true))
                {
                    ArrayAccessOperatorType arrayOperator = new ArrayAccessOperatorType();
                    arrayOperator.ArrayParameter = new ArrayParameter(ParseExpression(sourceType));
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                    {
                        throw new Exception("array-access no declosing-tag");
                    }
#if (TRACK)
                    Utils.LogItem(arrayOperator.GetDebugText());
#endif
                    operandOperators.Add(arrayOperator);
                }
                // check for method-access
                else if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                {
                    MethodCallOperatorType methodCallOperator = new MethodCallOperatorType();
                    methodCallOperator.MethodName = variableName;
                    methodCallOperator.GenericType = genericType;
                    // get function-paramater expression list
                    methodCallOperator.ParameterExpressions = ParseExpressionParameterList(sourceType, ObjectConst.FunctionDeclosing);
                    // check end
                    if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        throw new Exception("function-call no declosing-tag");
                    }
#if(TRACK)
                    Utils.LogItem(methodCallOperator.GetDebugText());
#endif
                    operandOperators.Add(methodCallOperator);
                }
                // is an member-access
                else
                {
                    MemberAccessOperatorType memberAccessOperator = new MemberAccessOperatorType();
                    memberAccessOperator.MemberName = variableName;
#if(TRACK)
                    Utils.LogItem(memberAccessOperator.GetDebugText());
#endif
                    operandOperators.Add(memberAccessOperator);
                }
            }
        }

        public OperandType ParseLiteralOperand(SourceFile sourceType)
        {
            textParser.SkipSpace(true);

            OperandType operand = null;

            // check boolean-true
            if (textParser.Equal(NativeBools.True, true))
            {
                operand = new BoolOperand(true);
            }
            // check boolean-false
            else if (textParser.Equal(NativeBools.False, true))
            {
                operand = new BoolOperand(false);
            }
            // else
            else
            {
                // identify on first char
                char chr = textParser.NextChar(false);

                // check for string-literal
                if (chr == '"')
                {
                    TextParseResult parseResult = textParser.ParseStringContent(true);
                    if (parseResult.IsFailed())
                    {
                        throw new Exception("string parse failed: '" + parseResult.CauseString() + "'");
                    }
                    operand = new StringOperand(parseResult.Result);
                }
                // check for number-literal
                else if (Char.IsDigit(chr))
                {
                    TextParseNumberResult parseResult = textParser.ParseNumberContent(true);
                    if (parseResult.IsFailed())
                    {
                        throw new Exception("number parse failed: '"+ parseResult.CauseString() + "'");
                    }
                    operand = new NumberOperand(parseResult.Result, parseResult.NativeType, parseResult.NativeNumberType);
                }
                // check for char-literal
                else if (chr == '\'')
                {
                    TextParseCharResult parseResult = textParser.ParseCharContent(true);
                    if (parseResult.IsFailed())
                    {
                        throw new Exception("char parse failed: '"+ parseResult.CauseString() + "'");
                    }
                    operand = new CharOperand(parseResult.Result, parseResult.CharType);
                }
                // none
                else
                {
                    ;
                }
            }

            // return possible literal-operand
            return operand;
        }

        public ObjectInitialisationType ParseObjectInitialisation(SourceFile sourceType)
        {
            // check constructor-declaration
            if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
            {
                return null;
            }

            ObjectInitialisationType declarationType = new ObjectInitialisationType();
#if (TRACK)
            Utils.LogItem("object-member-initialisation start");
#endif
            // get possible positional-expression-parameters
            while (true)
            {
                // check for end 
                if (textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                {
                    break;
                }
                // check for named-parameter seperator
                if(textParser.EqualNoneSpace(ObjectConst.Colon, false))
                {
                    break;
                }
#if (TRACK)
                Utils.LogItem("declaration_positional_item");
#endif
                // get expression-parameter
                ExpressionType expressionType = ParseExpression(sourceType);
                declarationType.ConstructorDefinitionList.Add(expressionType);

                // check for seperation
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check for end 
                else if (textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                {
                    break;
                }
                // check for named-parameter seperator
                else if (textParser.EqualNoneSpace(ObjectConst.Colon, false))
                {
                    break;
                }
                // invalid
                else
                {
                    throw new Exception("invalid constructur declaration (constructur-end)");
                }
            }

            // get possible named-expression-parameters
            if (textParser.EqualNoneSpace(ObjectConst.Colon, true))
            {
                while (true)
                {
                    // check for end 
                    if (textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        break;
                    }
#if (TRACK)
                    Utils.LogItem("declaration_named_item");
#endif
                    // get name-variable
                    string memberName = textParser.GetNameContent(true);
                    if (memberName == null)
                    {
                        throw new Exception("invalid member variable");
                    }
                    // check assigment-symbol
                    if(!textParser.EqualNoneSpace(ObjectConst.Assigment, true))
                    {
                        throw new Exception("missing assigment-symbol");
                    }
                    
                    // get expression-parameter
                    ExpressionType expressionType = ParseExpression(sourceType);
                    declarationType.MemberDefinitionMap.Add(memberName, expressionType);

                    // check for seperation
                    if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                    {
                        continue;
                    }
                    // check for end 
                    else if (textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
                    {
                        break;
                    }
                    // invalid
                    else
                    {
                        throw new Exception("invalid constructur declaration (constructur-end)");
                    }
                }
            }
#if(TRACK)
            Utils.LogItem("constructor-declaration end");
#endif
            // check for possible anonymous-type implementation
            if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, false))
            {
                return declarationType;
            }
#if (TRACK)
            Utils.LogItem("object-anonymous-implementation start");
#endif
            // parse possible member and method implementation (anonymous type)
            AnonObjectType anonObjectType = new SourceParser(sourceType).ParseAnonymousObject(textParser.GetPosition());
            declarationType.AnonymousObjectType = anonObjectType;
#if (TRACK)
            Utils.LogItem("object-anonymous-implementation end");
#endif
            // return
            return declarationType;
        }

        public ExpressionOperatorType ParseFrontExpressionOperator(SourceFile sourceType)
        {
            textParser.SkipSpace(true);

            ExpressionOperatorType expressionOperator = null;

            // check for not-value
            if (textParser.Equal(ExpressionOperatorConst.Not, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.NOT_VALUE, ExpressionOperatorConst.Not);
            }
            // check for pre increment/decrement
            else if (textParser.Equal(ExpressionOperatorConst.Increment, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.PRE_INCREMENT, ExpressionOperatorConst.Increment);
            }
            else if (textParser.Equal(ExpressionOperatorConst.Decrement, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.PRE_DECREMENT, ExpressionOperatorConst.Decrement);
            }
            // check for postive/nagative value
            else if (textParser.Equal(ExpressionOperatorConst.Positiv, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.POSITIV_VALUE, ExpressionOperatorConst.Positiv);
            }
            else if (textParser.Equal(ExpressionOperatorConst.Negativ, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.NEGATIV_VALUE, ExpressionOperatorConst.Negativ);
            }
#if (TRACK)
            if (expressionOperator != null)
            {
                Utils.LogItem(expressionOperator.GetDebugText());
            }   
#endif
            return expressionOperator;
        }

        public ExpressionOperatorType ParseBackExpressionOperator(SourceFile sourceType)
        {
            // skip spaces
            textParser.SkipSpace(true);

            ExpressionOperatorType expressionOperator = null;

            // check for post increment/decrement
            if (textParser.Equal(ExpressionOperatorConst.Increment, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.POST_INCREMENT, ExpressionOperatorConst.Increment);
            }
            else if (textParser.Equal(ExpressionOperatorConst.Decrement, true))
            {
                expressionOperator = new ExpressionOperatorType(ExpressionOperatorTypeEnum.POST_DECREMENT, ExpressionOperatorConst.Decrement);
            }
#if (TRACK)
            if (expressionOperator != null)
            {
                Utils.LogItem(expressionOperator.GetDebugText());
            }
#endif
            // none
            return null;
        }

        public OperationType ParseOperation(SourceFile sourceType)
        {
            textParser.SkipSpace(true);

            // get possible operation
            OperationType operation = null;
            for(int i=0; i<OperationConst.OperationTypeArray.Length; i++)
            {
                if (textParser.Equal(OperationConst.OperationTypeArray[i].SymbolString, true))
                {
                    operation = OperationConst.OperationTypeArray[i];
                    break;
                }
            }
#if (TRACK)
            if (operation != null)
            {
                Utils.LogItem(operation.GetDebugText());
            }
#endif
            // return possible operation
            return operation;
        }

        public ExpressionCollection ParseExpressionParameterList(SourceFile sourceType, string listEndString)
        {
            ExpressionCollection expressionParameters = new ExpressionCollection();

            // check for empty function-paramater-list
            if (textParser.EqualNoneSpace(listEndString, false))
            {
#if(TRACK)
                Utils.LogItem("function_parameter_list_empty");
#endif
                return expressionParameters;
            }
#if(TRACK)
            Utils.LogItem("function_parameter_list_start");      
#endif
            while (true)
            {
#if(TRACK)
                Utils.LogItem("function_parameter_start");
#endif
                // parse expression-parameter
                ExpressionType expression = ParseExpression(sourceType);
                expressionParameters.Add(expression);

                // check for next parameter
                if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    break;
                }
            }
#if(TRACK)
            Utils.LogItem("function_parameter_list_end");
#endif
            return expressionParameters;
        }
    }
}
