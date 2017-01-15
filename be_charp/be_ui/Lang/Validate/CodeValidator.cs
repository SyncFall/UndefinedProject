using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Runtime.Types;

namespace Be.Runtime.Validate
{
    public class CodeValidator
    {
        private ObjectLoader objectLoader;

        public CodeValidator(ObjectLoader objectLoader)
        {
            this.objectLoader = objectLoader;
        }

        public void ValidateSourceType(SourceFile sourceType)
        {
            // check objects
            for (int i = 0; i < sourceType.Namespaces.Size(); i++)
            {
                ObjectCollection objects = sourceType.Namespaces.Get(i).Objects;
                for (int j = 0; j < objects.Size(); j++)
                {
                    ValidateObjectType(sourceType, objects.Get(j));
                }
            }
        }

        private void ValidateObjectType(SourceFile sourceType, ObjectSymbol objectType)
        {
            // validate possible member-initialisations
            for (int i = 0; i < objectType.Members.Size(); i++)
            {
                MemberType memberType = objectType.Members.Get(i);
                if (memberType.InitialisationExpression != null)
                {
                    ValidateExpression(sourceType, objectType, null, null, memberType.InitialisationExpression, ExpressionTypeEnum.MEMBER_INITIALISATION, memberType.ObjectType);
                }
            }
            // validate constructors
            for (int i = 0; i < objectType.Constructors.Size(); i++)
            {
                MethodType constructorType = objectType.Constructors.Get(i);
                ValidateStatementBlock(sourceType, objectType, constructorType.Code, constructorType.Code.Statements);
            }
            // validate methods
            for (int i=0; i<objectType.Methods.Size(); i++)
            {
                MethodType methodType = objectType.Methods.Get(i);
                ValidateStatementBlock(sourceType, objectType, methodType.Code, methodType.Code.Statements);
            }
            // validate properties
            for (int i = 0; i < objectType.Properties.Size(); i++)
            {
                PropertySymbol propertyType = objectType.Properties.Get(i) as PropertySymbol;
                // check possible get code
                if(propertyType.GetCode != null)
                {
                    ValidateStatementBlock(sourceType, objectType, propertyType.GetCode, propertyType.GetCode.Statements);
                }
                // check possible set code
                if(propertyType.SetCode != null)
                {
                    ValidateStatementBlock(sourceType, objectType, propertyType.SetCode, propertyType.SetCode.Statements);
                }
            }
            // validate child-objects
            for (int i=0; i<objectType.Objects.Size(); i++)
            {
                ValidateObjectType(sourceType, objectType.Objects.Get(i));
            }
        }

        private void ValidateAnonymousObjectType(SourceFile sourceType, ObjectSymbol anonObjectType)
        {
            ValidateObjectType(sourceType, anonObjectType);
        }

        private void ValidateStatementBlock(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementCollection statementCollection)
        {
            StatementType statementType;
            StatementType lastStatementType = null;
            // check each statement
            for (int i = 0; i < statementCollection.Size(); i++)
            {
                statementType = statementCollection.Get(i);
                // check if, else if, else - condition-statement-block
                if (statementType.Category == StatementCategoryEnum.CONDITION_BLOCK)
                {
                    // validate control-flow-order of elseif and else statement
                    if (statementType.Type == StatementTypeEnum.ELSE_IF && (lastStatementType == null || (lastStatementType.Type != StatementTypeEnum.IF && lastStatementType.Type != StatementTypeEnum.ELSE_IF)))
                    {
                        throw new Exception("else-if statement can only exist before if-/else-if statement block");
                    }
                    else if (statementType.Type == StatementTypeEnum.ELSE && (lastStatementType == null || (lastStatementType.Type != StatementTypeEnum.IF && lastStatementType.Type != StatementTypeEnum.ELSE_IF)))
                    {
                        throw new Exception("else statement can only exist before if-/else-if statement block");
                    }
                    ConditionStatementType ifBlock = statementType as ConditionStatementType;
                    // check condition, else-statement do not have one
                    if (statementType.Type != StatementTypeEnum.ELSE)
                    {
                        ValidateExpression(sourceType, objectType, codeType, statementType, ifBlock.ConditionExpression, ExpressionTypeEnum.CONDITION, Natives.GetTypeByName("bool"));
                    }
                    // check block statements
                    ValidateStatementBlock(sourceType, objectType, codeType, ifBlock.Statements);
                }
                // local variable declaration and definition statement
                else if(statementType.Type == StatementTypeEnum.DECLARATION)
                {
                    VariableDeclarationStatementType variableDeclarationStatement = statementType as VariableDeclarationStatementType;
                    VariableCollection variableCollection = variableDeclarationStatement.VariableDeclarationCollection;
                    // check variable-declaration
                    ValidateVariableDeclaration(sourceType, objectType, codeType, variableDeclarationStatement, variableCollection);
                    // set variable-context to statement-block
                    variableDeclarationStatement.VariableDeclarationCollection = variableCollection;
                }
                // expression-statement
                else if(statementType.Type == StatementTypeEnum.EXPRESSION_STATEMENT)
                {
                    ExpressionStatementType expressionStatement = statementType as ExpressionStatementType;
                    // check expression-statement
                    if(expressionStatement.StatementExpression == null)
                    {
                        throw new Exception("invalid state");
                    }
                    ValidateExpression(sourceType, objectType, codeType, statementType, expressionStatement.StatementExpression, ExpressionTypeEnum.STATEMENT, null);
                }
                // inner-block statement
                else if(statementType.Type == StatementTypeEnum.INNER_BLOCK)
                {
                    InnerBlockStatementType innerBlock = statementType as InnerBlockStatementType;
                    // check child-statments
                    ValidateStatementBlock(sourceType, objectType, codeType, innerBlock.Statements);
                }
                // check function-control statement
                else if (statementType.Type == StatementTypeEnum.RETURN)
                {
                    FunctionControlStatementType flowStatement = statementType as FunctionControlStatementType;
                    // check for not-return expression if a constructor
                    if(codeType.MethodType.IsConstructor && flowStatement.ReturnExpression != null)
                    {
                        throw new Exception("constructor can not has an return-expression");
                    }
                    ObjectSymbol methodReturnObjectType = codeType.MethodType.ReturnObjectType;
                    // validate possible return-expression and if match method-signatur
                    if (flowStatement.ReturnExpression != null)
                    {
                        ValidateExpression(sourceType, objectType, codeType, statementType, flowStatement.ReturnExpression, ExpressionTypeEnum.FUNCTION_CONTROL, methodReturnObjectType);
                    }
                    // for none return-expression, check if match void-method-signatur
                    else if(!(methodReturnObjectType.IsNative && (methodReturnObjectType as NativeSymbol).Type == NativeType.Void))
                    {
                        throw new Exception("none-return-expression match not method-signatur");
                    }
                }
                // check loop-control statements
                else if (statementType.Category == StatementCategoryEnum.LOOP_CONTROL)
                {
                    LoopControlStatementType loopStatement = statementType as LoopControlStatementType;
                    // check if in any loop-block
                    StatementType parentStatement = loopStatement.ParentStatement;
                    while (true)
                    {
                        // code-start, no-loop parent block found
                        if (parentStatement == null)
                        {
                            throw new Exception("control-flow statement invalid, no surrounding loop-block");
                        }
                        // found loop-block
                        else if ((parentStatement as LoopStatementType) != null)
                        {
                            break; // pass through
                        }
                        parentStatement = parentStatement.ParentStatement;
                    }
                }
                // check for error-control statement
                else if (statementType.Type == StatementTypeEnum.THROW)
                {
                    ErrorControlStatementType errorStatement = statementType as ErrorControlStatementType;
                    // match exception-signatur
                    ValidateExpression(sourceType, objectType, codeType, statementType, errorStatement.ErrorExpression, ExpressionTypeEnum.ERROR_CONTROL, null);
                }
                // check for try-catch-finally statement
                else if(statementType.Category == StatementCategoryEnum.ERROR_PROCESSING)
                {
                    ErrorProcessigStatementType errorProcessingStatement = statementType as ErrorProcessigStatementType;
                    // check for catch, exception object-type
                    if (errorProcessingStatement.Type == StatementTypeEnum.CATCH)
                    {
                        // get exception type
                        ObjectSymbol exceptionObjectType = objectLoader.GetObjectType(sourceType, errorProcessingStatement.DeclarationVariable.TypeName); 
                        if (!exceptionObjectType.IsException)
                        {
                            throw new Exception("catch-statement is no-exception object-type");
                        }
                        errorProcessingStatement.DeclarationVariable.ObjectType = exceptionObjectType;
                    }
                }
                // check lock-statment
                else if(statementType.Type == StatementTypeEnum.THREAD_SYNC)
                {
                    // validate child-statments
                    ValidateStatementBlock(sourceType, objectType, codeType, statementCollection);
                }
                // check for no-operation statement
                else if (statementType.Type == StatementTypeEnum.NO_OPERATION)
                {
                    ; // pass through
                }
                // check while statment
                else if (statementType.Type == StatementTypeEnum.WHILE)
                {
                    WhileLoopStatementType whileBlock = statementType as WhileLoopStatementType;
                    // check condition
                    ValidateExpression(sourceType, objectType, codeType, statementType, whileBlock.ConditionExpression, ExpressionTypeEnum.CONDITION, Natives.GetTypeByName("bool"));
                    // check block statements
                    ValidateStatementBlock(sourceType, objectType, codeType, whileBlock.Statements);
                }
                // check do-while-statment
                else if (statementType.Type == StatementTypeEnum.DO_WHILE)
                {
                    DoWhileLoopStatementType doWhileBlock = statementType as DoWhileLoopStatementType;
                    // check condition
                    ValidateExpression(sourceType, objectType, codeType, statementType, doWhileBlock.ConditionExpression, ExpressionTypeEnum.CONDITION, Natives.GetTypeByName("bool"));
                    // check block statements
                    ValidateStatementBlock(sourceType, objectType, codeType, doWhileBlock.Statements);
                }
                // check for-statement
                else if(statementType.Type == StatementTypeEnum.FOR)
                {
                    ForLoopStatementType forStatement = statementType as ForLoopStatementType;
                    // check possible variable declaration
                    if (forStatement.VariableDeclarationCollection != null)
                    {
                        ValidateVariableDeclaration(sourceType, objectType, codeType, forStatement, forStatement.VariableDeclarationCollection);
                    }
                    // else check possible initialisation-expression-list
                    else if (forStatement.ExpressionInitiationCollection != null)
                    {
                        for (int j = 0; j < forStatement.ExpressionInitiationCollection.Size(); j++)
                        {
                            ExpressionType expressionType = forStatement.ExpressionInitiationCollection.Get(j);
                            // check expression
                            ValidateExpression(sourceType, objectType, codeType, statementType, expressionType, ExpressionTypeEnum.STATEMENT, null);
                        }
                    }
                    // check possible loop-condition
                    if(forStatement.ConditionExpression != null)
                    {
                        ValidateExpression(sourceType, objectType, codeType, statementType, forStatement.ConditionExpression, ExpressionTypeEnum.CONDITION, Natives.GetTypeByName("bool"));
                    }
                    // check possible post-expression-list
                    if(forStatement.PostExpressionCollection != null)
                    {
                        for (int j = 0; j < forStatement.PostExpressionCollection.Size(); j++)
                        {
                            ExpressionType expressionType = forStatement.PostExpressionCollection.Get(j);
                            // check expression
                            ValidateExpression(sourceType, objectType, codeType, statementType, expressionType, ExpressionTypeEnum.STATEMENT, null);
                        }
                    }
                    // check child statement-block
                    ValidateStatementBlock(sourceType, objectType, codeType, forStatement.Statements);
                }
                // invalid statement-type
                else
                {
                    throw new Exception("invalid statement-type");
                }
                // set last-active
                lastStatementType = statementType;
            }
        }

        public ObjectSymbol ValidateExpression(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, ExpressionType expressionType, ExpressionTypeEnum Mode, ObjectSymbol targetObjectType)
        {
            // go to first-top expression-operation depth
            expressionType = ExpressionUtils.GetOperationDepthExpressionType(expressionType);

            // check expression operand and operations
            ValidateOperandAndOperators(sourceType, objectType, codeType, statementType, ExpressionUtils.GetBeginnigOperandType(expressionType));

            // check possible operations with other expressions
            ValidateExpressionAndOperations(sourceType, objectType, codeType, statementType, expressionType);

            // check if expression-result match target-type
            if (targetObjectType != null && !targetObjectType.IsCompilantWith(expressionType.OperationObjectType))
            {
                throw new Exception("no compiliant types");
            }
        
            // return expression-result operation-object-type
            return expressionType.OperationObjectType;
        }

        private void ValidateExpressionAndOperations(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, ExpressionType expressionType)
        {
            ObjectSymbol firstResultObjectType;

            // check for child-expression type
            if (expressionType.ChildExpression != null)
            {
                firstResultObjectType = ValidateExpression(sourceType, objectType, codeType, statementType, expressionType.ChildExpression, ExpressionTypeEnum.PAIR_OPERATION, null);
            }
            // else get operand-operation type
            else
            {
                firstResultObjectType = ExpressionUtils.GetBeginnigOperandType(expressionType).OperationObjectType;
            }

            // check possible front-expression-operator
            if(expressionType.FrontExpressionOperator != null)
            {
                ExpressionOperatorType frontExpressionOperator = expressionType.FrontExpressionOperator;
                // not-value operator for boolean
                if(frontExpressionOperator.OperatorType == ExpressionOperatorTypeEnum.NOT_VALUE && 
                  (!firstResultObjectType.IsNative || (firstResultObjectType as NativeSymbol).Type != NativeType.Bool)
                ){
                    throw new Exception("not-value expression-operator only on boolean");
                }
                // others on numbers
                else if(!firstResultObjectType.IsNative || (firstResultObjectType as NativeSymbol).Group != NativeGroup.Number)
                {
                    throw new Exception("increment/decremennt expression-operator only on numbers");
                }
            }

            // check possible back-expression-operator
            if(expressionType.BackExpressionOperator != null)
            {
                // only on numbers
                if(!firstResultObjectType.IsNative || (firstResultObjectType as NativeSymbol).Group != NativeGroup.Number)
                {
                    throw new Exception("increment/decremennt expression-operator only on numbers");
                }
            }

            // break if no operations on this depth
            if (expressionType.OperationsWithExpressions == null)
            {
                expressionType.OperationObjectType = firstResultObjectType;
                return;
            }

            // control-flag for result-type
            bool hasFinished = false;

            // validate operations
            ExpressionOperation expressionOperation;
            OperationType operation;
            ObjectSymbol secondResultObjectType = null;
            for (int i = 0; i < expressionType.OperationsWithExpressions.Size(); i++,
                firstResultObjectType = secondResultObjectType // forward to next expression-pair
            ){
                expressionOperation = expressionType.OperationsWithExpressions.Get(i);
                operation = expressionOperation.Operation;

                // resolve second expression result object-type recursive
                secondResultObjectType = ValidateExpression(sourceType, objectType, codeType, statementType, expressionOperation.SecondExpression, ExpressionTypeEnum.PAIR_OPERATION, null);

                // void-null-type can operate equal-or-not on strings and objects
                if(operation.Group == OperationGroupEnum.LOGIC_EQUAL_NOT && 
                   ((!firstResultObjectType.IsNative || (firstResultObjectType.IsNative && (firstResultObjectType as NativeSymbol).Type == NativeType.String)) &&
                   (secondResultObjectType.IsNative && (secondResultObjectType as NativeSymbol).Type == NativeType.Void))
                ){
                    expressionType.OperationObjectType = Natives.GetTypeByName("bool");
                    hasFinished = true;
                }
                // logical and-or operation only for boolean
                else if (operation.Group == OperationGroupEnum.LOGIC_AND_OR)
                {
                    if (!firstResultObjectType.IsNative || ((firstResultObjectType as NativeSymbol).Type != NativeType.Bool || (secondResultObjectType as NativeSymbol).Type != NativeType.Bool))
                    {
                        throw new Exception("logical and-or operation is not boolean");
                    }
                    expressionType.OperationObjectType = Natives.GetTypeByName("bool");
                    hasFinished = true;
                }
                // logical equal-or-not operation only for natives of same category
                else if (operation.Group == OperationGroupEnum.LOGIC_EQUAL_NOT)
                {
                    if (firstResultObjectType.IsNative != secondResultObjectType.IsNative)
                    {
                        throw new Exception("logical equal-or-not operations can not between natives and objects");
                    }
                    else if (!firstResultObjectType.IsNative)
                    {
                        throw new Exception("logical equal-or-not operations can not operate on objects");
                    }
                    else if (firstResultObjectType.IsNative && (firstResultObjectType as NativeSymbol).Group != (secondResultObjectType as NativeSymbol).Group)
                    {
                        throw new Exception("logical equal-or-not operations can only on natives of same category like booleans, strings, numbers or null");
                    }
                    expressionType.OperationObjectType = Natives.GetTypeByName("bool");
                    hasFinished = true;
                }
                // logical relation-compare operations only through numbers
                else if (operation.Group == OperationGroupEnum.LOGIC_RELATION_COMPARE)
                {
                    if ((!firstResultObjectType.IsNative || !secondResultObjectType.IsNative) ||
                        ((firstResultObjectType as NativeSymbol).Group != NativeGroup.Number || (secondResultObjectType as NativeSymbol).Group != NativeGroup.Number))
                    {
                        throw new Exception("logical relation-compare operations can only on numbers");
                    }
                    expressionType.OperationObjectType = Natives.GetTypeByName("bool");
                    hasFinished = true;
                }
                // add-math operation between numbers and strings resulting a string-type
                else if (((operation.Type == OperationTypeEnum.ADD) && (
                    (firstResultObjectType.IsNative && ((firstResultObjectType as NativeSymbol).Group == NativeGroup.Number && (secondResultObjectType as NativeSymbol).Group == NativeGroup.String)) ||
                    (secondResultObjectType.IsNative && ((firstResultObjectType as NativeSymbol).Group == NativeGroup.String && (secondResultObjectType as NativeSymbol).Group == NativeGroup.Number))
                )))
                {
                    if (!hasFinished)
                    {
                        expressionType.OperationObjectType = Natives.GetTypeByName("string");
                    }
                }
                // add-assigment-math operation if string, for strings and numbers
                else if ((operation.Type == OperationTypeEnum.ADD_ASSIGMENT) && (
                    (firstResultObjectType.IsNative && ((firstResultObjectType as NativeSymbol).Group == NativeGroup.String) ||
                    (secondResultObjectType.IsNative && ((secondResultObjectType as NativeSymbol).Group == NativeGroup.Number || (secondResultObjectType as NativeSymbol).Group == NativeGroup.String))
                )))
                {
                    if (!hasFinished)
                    {
                        expressionType.OperationObjectType = Natives.GetTypeByName("string");
                        hasFinished = true; // assigment-math!
                    }
                }
                // assigment-math can on natives on same category, and objects on same type
                else if (operation.Type == OperationTypeEnum.ASSIGMENT)
                {
                    // check if native or object missmatch
                    if (firstResultObjectType.IsNative != secondResultObjectType.IsNative)
                    {
                        throw new Exception("assigment operation can only if both natives or objects");
                    }
                    // check for natives 
                    if (firstResultObjectType.IsNative)
                    {
                        // only in same category
                        if ((firstResultObjectType as NativeSymbol).Group != (secondResultObjectType as NativeSymbol).Group)
                        {
                            throw new Exception("assigment operation on natives can only from same category, like boolean, numbers or strings");
                        }
                    }
                    // check for objects
                    else
                    {
                        // only of same type
                        if (!firstResultObjectType.String.Equals(secondResultObjectType.String))
                        {
                            throw new Exception("assignment-operation on objects can only on same type");
                        }
                    }
                    // using assignment-type
                    if (!hasFinished)
                    {
                        expressionType.OperationObjectType = firstResultObjectType;
                        hasFinished = true; // assigment-math
                    }
                }
                // basic-math and math-assigment can only through numbers
                else if (operation.Category == OperationCategoryEnum.BASIC_MATH || operation.Category == OperationCategoryEnum.MATH_ASSIGMENT)
                {
                    if ((!firstResultObjectType.IsNative || (firstResultObjectType as NativeSymbol).Group != NativeGroup.Number) ||
                        (!secondResultObjectType.IsNative || (secondResultObjectType as NativeSymbol).Group != NativeGroup.Number)
                    )
                    {
                        throw new Exception("math-operations can only on numbers");
                    }
                    // left-hand, finished, on assigment variable
                    if (operation.Category == OperationCategoryEnum.MATH_ASSIGMENT)
                    {
                        expressionType.OperationObjectType = firstResultObjectType;
                        hasFinished = true;
                    }
                    // else using highest-number-precesion as result type for basic-math
                    else
                    {
                        // highest precesion number type
                        if (!hasFinished)
                        {
                            expressionType.OperationObjectType = NativeUtils.GetHighestPrecisionNativeType(firstResultObjectType as NativeSymbol, secondResultObjectType as NativeSymbol);
                        }
                    }
                }
                // type-operators
                else if(operation.Category == OperationCategoryEnum.TYPE)
                {
                    // validate 
                    if (!secondResultObjectType.IsNative || !secondResultObjectType.String.Equals("type"))
                    {
                        throw new Exception("type operator need type");
                    }
                    // is/has-type is boolean
                    if (operation.Type == OperationTypeEnum.IS_TYPE || operation.Type == OperationTypeEnum.HAS_TYPE)
                    {
                        expressionType.OperationObjectType = Natives.GetTypeByName("bool");
                        hasFinished = true;
                    }
                    // as-type as defined object-type
                    else if(operation.Type == OperationTypeEnum.AS_TYPE)
                    {
                        // get expression operation object-type
                        MemberVariableOrParameterOperand typeOperand = ExpressionUtils.GetBeginnigOperandType(expressionOperation.SecondExpression) as MemberVariableOrParameterOperand;
                        expressionType.OperationObjectType = typeOperand.TypeObjectType;
                        hasFinished = true;
                    }
                    // get-type as type-native
                    else if(operation.Type == OperationTypeEnum.GET_TYPE)
                    {
                        expressionType.OperationObjectType = Natives.GetTypeByName("type");
                        hasFinished = true;
                    }
                    // invalid
                    else
                    {
                        throw new Exception("invalid state");
                    }
                }
                // invalid state
                else
                {
                    throw new Exception("invalid state");
                }
            }
        }

        private void ValidateOperandAndOperators(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, OperandType operand)
        {
            // check new type
            if (operand.Type == OperandTypeEnum.NEW_TYPE)
            {
                NewTypeOperand newTypeOperand = operand as NewTypeOperand;
                // check for possible object initialisation
                if(newTypeOperand.ObjectDefinitionType != null)
                {
                    ValidateObjectInitiation(sourceType, objectType, codeType, statementType, newTypeOperand);
                }
                // else check for possible array initialisation
                else if(newTypeOperand.ArrayInitiationType != null)
                {
                    ValidateArrayInitiation(sourceType, objectType, codeType, statementType, newTypeOperand);
                }
                // invalid
                else
                {
                    throw new Exception("invalid state");
                }
                operand.OperandObjectType = objectLoader.GetObjectType(sourceType, newTypeOperand.NewTypeName);
            }
            // check number operand
            else if (operand.Type == OperandTypeEnum.NUMBER)
            {
                NumberOperand numberOperand = operand as NumberOperand;
                operand.OperandObjectType = NativeUtils.GetNativeType(numberOperand.NativeTypeEnum, numberOperand.NativeNumberTypeEnum);
            }
            // check native string, data-type-and-value already exist
            else if (operand.Type == OperandTypeEnum.STRING)
            {
                operand.OperandObjectType = Natives.GetTypeByName("string");
            }
            // check native bool, data-type-and-value already exist
            else if (operand.Type == OperandTypeEnum.BOOL)
            {
                operand.OperandObjectType = Natives.GetTypeByName("bool");
            }
            // check native char, set right data-value already exist
            else if (operand.Type == OperandTypeEnum.CHAR)
            {
                operand.OperandObjectType = Natives.GetTypeByName("char");
            }
            // check for method-operand, set right return-type, already exist
            else if (operand.Type == OperandTypeEnum.METHOD)
            {
                MethodOperand methodOperand = operand as MethodOperand;
                // todo: generic-boxing missing per method
                // validate method expression parameters
                for(int i=0; i<methodOperand.ParameterExpressions.Size(); i++)
                {
                    ValidateExpression(sourceType, objectType, codeType, statementType, methodOperand.ParameterExpressions.Get(i), ExpressionTypeEnum.PARAMETER, null);
                }
                // get method-matching type from validated parameter-expressions
                methodOperand.MethodType = objectType.Methods.GetByBasicExpressionObjectTypeSignatur(methodOperand.MethodName, methodOperand.ParameterExpressions);
                if(methodOperand.MethodType == null)
                {
                    throw new Exception("method-name or parameters not match any method-signatur");
                }
                // set method return operation-object type
                operand.OperandObjectType = methodOperand.MethodType.ReturnObjectType;
            }
            // check for member or variable operand
            else if (operand.Type == OperandTypeEnum.MEMBER_OR_VARIABLE_OR_PARAMETER)
            {
                MemberVariableOrParameterOperand memberOrVariableOperand = operand as MemberVariableOrParameterOperand;
                
                // check if variable in method, code, object or anonymous scope
                ObjectSymbol currentObjectType = objectType;
                CodeType currentCodeType = codeType;
                while(currentObjectType != null)
                {
                    // check for constructor/method-parameter, skip properties
                    MethodType currentMethod = currentCodeType.MethodType;
                    if(!currentMethod.IsProperty)
                    {
                        ParameterType parameterType = currentMethod.ParameterCollection.GetByName(memberOrVariableOperand.PathName);
                        if (parameterType != null)
                        {
                            memberOrVariableOperand.ParameterType = parameterType;
                            operand.Type = OperandTypeEnum.PARAMETER;
                            operand.OperandObjectType = parameterType.ObjectType;
                            break;
                        }
                    }             

                    // check for member
                    MemberType memberType = objectType.Members.GetByName(memberOrVariableOperand.PathName);
                    if (memberType != null)
                    {
                        memberOrVariableOperand.MemberType = memberType;
                        operand.Type = OperandTypeEnum.MEMBER;
                        operand.OperandObjectType = memberType.ObjectType;
                        break;
                    }
                     
                    // check for variable
                    VariableType variableType = VariableUtils.GetVariableType(codeType, statementType, memberOrVariableOperand.PathName);
                    if (variableType != null)
                    {
                        // get variable object-type
                        variableType.ObjectType = objectLoader.GetObjectType(sourceType, variableType.TypeName);
                        if (variableType.ObjectType == null)
                        {
                            throw new Exception("variable type not exist");
                        }
                        memberOrVariableOperand.VariableType = variableType;
                        operand.Type = OperandTypeEnum.VARIABLE;
                        operand.OperandObjectType = variableType.ObjectType;
                        break;
                    }

                    // check for object-type
                    ObjectSymbol typeObjectType = objectLoader.GetObjectType(sourceType, memberOrVariableOperand.PathName);
                    if(typeObjectType != null)
                    {
                        memberOrVariableOperand.TypeObjectType = typeObjectType;
                        operand.Type = OperandTypeEnum.TYPE_OBJECT;
                        operand.OperandObjectType = Natives.GetTypeByName("type");
                        break;
                    }
                    
                    // go-top local-code-scope if in anon-context
                    if (operand.OperandObjectType.IsAnonymous)
                    {
                        AnonObjectType anonObjectType = operand.OperandObjectType as AnonObjectType;
                        currentCodeType = anonObjectType.ParentCodeType;
                        currentObjectType = anonObjectType.ParentObjectType;
                    }
                    // variable not found
                    else
                    {
                        throw new Exception("variable not declarated");
                    }
                }
            }
            // check this-operand
            else if(operand.Type == OperandTypeEnum.THIS)
            {
                operand.OperandObjectType = objectType;
            }
            // check null-operand
            else if(operand.Type == OperandTypeEnum.NULL)
            {
                operand.OperandObjectType = Natives.GetTypeByName("void");
            }
            // check base-operand
            else if(operand.Type == OperandTypeEnum.BASE)
            {
                // check for extend-type
                if(objectType.ExtendType == null)
                {
                    throw new Exception("base-operator invalid because no object extends");
                }
                operand.OperandObjectType = objectType.ExtendType.ObjectType;
            }
            // check value-operand
            else if(operand.Type == OperandTypeEnum.VALUE)
            {
                operand.OperandObjectType = codeType.MethodType.ReturnObjectType;
            }
            // invalid state
            else
            {
                throw new Exception("invalid state");
            }

            // set operation object-type
            operand.OperationObjectType = operand.OperandObjectType;

            // check operations if match result-type
            OperandOperatorCollection operandOperators = operand.Operations;
            for (int i = 0; i < operandOperators.Size(); i++)
            {
                OperandOperatorType operandOperator = operandOperators.Get(i);
                // check for array-access
                if(operandOperator.Type == OperandOperatorEnum.ARRAY_ACCESS)
                {
                    ArrayAccessOperatorType arrayOperator = operandOperator as ArrayAccessOperatorType;
                    // validate parameter expression
                    ValidateExpression(sourceType, objectType, codeType, statementType, arrayOperator.ArrayParameter.ParameterExpression, ExpressionTypeEnum.ASSIGMENT, null);
                    operand.OperationObjectType = operand.OperandObjectType;
                }
                // check for method-call
                else if (operandOperator.Type == OperandOperatorEnum.METHOD_CALL)
                {
                    MethodCallOperatorType methodCallOperation = operandOperator as MethodCallOperatorType;
                    // validate method expression parameters
                    for (int j = 0; j < methodCallOperation.ParameterExpressions.Size(); j++)
                    {
                        ValidateExpression(sourceType, objectType, codeType, statementType, methodCallOperation.ParameterExpressions.Get(j), ExpressionTypeEnum.ASSIGMENT, null);
                    }
                    // get method-matching type from validated parameters-expressions
                    methodCallOperation.MethodType = objectType.Methods.GetByBasicExpressionObjectTypeSignatur(methodCallOperation.MethodName, methodCallOperation.ParameterExpressions);
                    if (methodCallOperation.MethodType == null)
                    {
                        throw new Exception("method-name or parameters not match any method-signatur");
                    }
                    // set method return operation-object type
                    operand.OperationObjectType = methodCallOperation.MethodType.ReturnObjectType;
                }
                // check for member-access
                else if (operandOperator.Type == OperandOperatorEnum.MEMBER_ACCESS)
                {
                    MemberAccessOperatorType memberAccessOperation = operandOperator as MemberAccessOperatorType;
                    // check if member exist
                    memberAccessOperation.MemberType = operand.OperationObjectType.Members.GetByName(memberAccessOperation.MemberName);
                    if (memberAccessOperation.MemberType == null)
                    {
                        throw new Exception("member not match name");
                    }
                    // set method return operation-object type
                    operand.OperationObjectType = memberAccessOperation.MemberType.ObjectType;
                }
            }
        }

        public void ValidateObjectInitiation(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, NewTypeOperand operandType)
        {
            // get operand object-type
            ObjectSymbol newTypeObjectType = objectLoader.GetObjectType(sourceType, operandType.NewTypeName);
            operandType.OperandObjectType = newTypeObjectType;
            
            // check for possible anonymous-type if no object-type exist
            if (operandType.OperandObjectType == null)
            {
                throw new Exception("unknown new-type in expression");
            }
            
            // check for possible generic-type initiation types
            GenericType genericType = operandType.GenericType;
            
            // validate positional expression-parameters
            ExpressionCollection constructorExpressionList = operandType.ObjectDefinitionType.ConstructorDefinitionList;
            for (int i = 0; i < constructorExpressionList.Size(); i++)
            {
                ValidateExpression(sourceType, objectType, codeType, statementType, constructorExpressionList.Get(i), ExpressionTypeEnum.ASSIGMENT, null);
            }
            // validate if match any constructur-signatur
            MethodType constructorType = objectType.Methods.GetByBasicExpressionObjectTypeSignatur(constructorExpressionList);
            if (constructorType == null)
            {
                throw new Exception("constructor positional-parameters not match any signatur");
            }
            
            // validate named expression-parameters
            MapCollection<string, ExpressionType> memberExpressionMap = operandType.ObjectDefinitionType.MemberDefinitionMap;
            string[] memberNames = memberExpressionMap.GetKeys();
            for (int i = 0; i < memberNames.Length; i++)
            {
                ObjectSymbol resultMemberObjectType = ValidateExpression(sourceType, objectType, codeType, statementType, memberExpressionMap.GetValue(memberNames[i]), ExpressionTypeEnum.ASSIGMENT, null);
                // validate if match member exist in object-type
                MemberType memberObjectType = newTypeObjectType.Members.GetByName(memberNames[i]);
                if (memberObjectType == null)
                {
                    throw new Exception("constructor named-member not exit");
                }
                // validate if match member-type
                if(!memberObjectType.ObjectType.IsCompilantWith(resultMemberObjectType))
                {
                    throw new Exception("no compilant types");
                }
            }
            
            // validate possible anonymous-type implementation and code-referencing
            AnonObjectType anonObjectType = operandType.ObjectDefinitionType.AnonymousObjectType;
            if (anonObjectType != null)
            {
                // reference structure
                anonObjectType.ParentCodeType = codeType;
                anonObjectType.ParentObjectType = objectType;
                // validate anonymous object-type
                ValidateAnonymousObjectType(sourceType, anonObjectType);
            }
        }

        public void ValidateArrayInitiation(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, NewTypeOperand operandType)
        {
            ArrayType arrayType = operandType.ArrayInitiationType;
            // get array-declaration object-type
            arrayType.ObjectType = objectLoader.GetObjectType(sourceType, arrayType.TypeName);
            if(arrayType.ObjectType == null)
            {
                throw new Exception("array-declaration object-type not exit");
            }
            // check for possible array-initialisation
            if (arrayType.InitialisationRootNode != null)
            {
                // check root-elements
                ArrayNodeType arrayNodeType = arrayType.InitialisationRootNode;
                for (int i = 0; i < arrayNodeType.ChildNodes.Size(); i++)
                {
                    ValidateArrayInitiationDepth(sourceType, objectType, codeType, statementType, arrayType, arrayNodeType.ChildNodes.Get(i), 0);
                }
            }
        }

        public void ValidateArrayInitiationDepth(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, ArrayType arrayType, ArrayNodeType arrayNodeType, int depth)
        {
            // validating if a specified dimension on this depth match element-count if set
            if(depth >= arrayType.DimensionDepthList.Size())
            {
                throw new Exception("item-depth in array-dimension missmatch disclaration");
            }
            else if (arrayType.DimensionDepthList.Get(depth) > 0)
            {
                if (arrayNodeType.ChildNodes.Size() != (int)arrayType.DimensionDepthList.Get(depth))
                {
                    throw new Exception("item-count in array-dimension missmatch declaration");
                }
            }
            // validate expression-paramaters
            for(int i=0; i<arrayNodeType.InitialisationExpressionList.Size(); i++)
            {
                // match object-type from variable array-declaration
                ValidateExpression(sourceType, objectType, codeType, statementType, arrayNodeType.InitialisationExpressionList.Get(i), ExpressionTypeEnum.ASSIGMENT, arrayType.ObjectType); 
            }
            // check child-elements
            for(int i=0; i<arrayNodeType.ChildNodes.Size(); i++)
            {
                ValidateArrayInitiationDepth(sourceType, objectType, codeType, statementType, arrayType, arrayNodeType.ChildNodes.Get(i), depth + 1);
            }
        }

        private void ValidateVariableDeclaration(SourceFile sourceType, ObjectSymbol objectType, CodeType codeType, StatementType statementType, VariableCollection variableCollection)
        {
            for (int i = 0; i < variableCollection.Size(); i++)
            {
                VariableType variableType = variableCollection.Get(i);
                // check for name duplicates
                for (int j = i + 1; j < variableCollection.Size(); j++)
                {
                    if (variableType.EqualName(variableCollection.Get(j)))
                    {
                        throw new Exception("duplicate variable declaration in scope");
                    }
                }
                // todo: check of possible object-method-generic types
                // check variable object-type
                variableType.ObjectType = objectLoader.GetObjectType(sourceType, variableType.TypeName);
                if (variableType.ObjectType == null)
                {
                    throw new Exception("variable declaration type not exit");
                }
                // check possible variable initiation
                if (variableType.InitialisationExpression != null)
                {
                    ValidateExpression(sourceType, objectType, codeType, statementType, variableType.InitialisationExpression, ExpressionTypeEnum.MEMBER_INITIALISATION, variableType.ObjectType);
                }
            }
            // check to-top depth if not overlapping exist variables or in object local-block-hierachie
            CodeType currentCodeType = codeType;
            ObjectSymbol currentObjectType = objectType;
            StatementType currentStatementType = statementType.ParentStatement;
            while (currentObjectType != null)
            {
                // check method-parameters
                MethodType currentMethod = currentCodeType.MethodType;
                for(int i=0; i<currentMethod.ParameterCollection.Size(); i++)
                {
                    if (variableCollection.EqualName(currentMethod.ParameterCollection.Get(i).VariableName))
                    {
                        throw new Exception("variable already defined as method parameter");
                    }
                }

                // check local-variable scopes
                while (currentStatementType != null)
                {
                    // get possible local variables declaration
                    VariableCollection localVariableCollection = VariableUtils.GetVariableCollection(currentStatementType);
                    // check possible local variables in scope-block
                    if (localVariableCollection != null)
                    {
                        // check for exist duplicate variables
                        for (int i = 0; i < localVariableCollection.Size(); i++)
                        {
                            if (variableCollection.EqualName(localVariableCollection.Get(i)))
                            {
                                throw new Exception("local variable already exist in top-context");
                            }
                        }
                    }
                    // goto parent statement-scope
                    else if (currentStatementType.ParentStatement != null)
                    {
                        currentStatementType = currentStatementType.ParentStatement;
                    }
                    // break on code-top depth
                    else
                    {
                        break;
                    }
                }

                // check object member-extend scopes
                ObjectSymbol currentExtendObjectType = currentObjectType;
                while (currentExtendObjectType != null)
                {
                    // check each object member
                    for (int i = 0; i < currentExtendObjectType.Members.Size(); i++)
                    {
                        MemberType memberType = currentExtendObjectType.Members.Get(i);
                        if (variableCollection.EqualName(memberType.TypeName))
                        {
                            throw new Exception("local variable already exist in object-hierachie");
                        }
                    }
                    // go top of extend type if exist
                    if (currentExtendObjectType.ExtendType != null)
                    {
                        currentExtendObjectType = currentExtendObjectType.ExtendType.ObjectType;
                    }
                    // or break
                    else
                    {
                        break;
                    }
                }

                // go-top code-scope if in anonymous-context
                if (currentObjectType.IsAnonymous)
                {
                    AnonObjectType anonObjectType = currentObjectType as AnonObjectType;
                    currentCodeType = anonObjectType.ParentCodeType;
                    currentObjectType = anonObjectType.ParentObjectType;
                    currentStatementType = (currentCodeType.Statements.Size()>0 ? currentCodeType.Statements.Get(0) : null);
                }
                // or break
                else
                {
                    currentObjectType = null;
                }
            }
        }
    }
}
