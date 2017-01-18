using System;
using System.Text;
using Be.Runtime.Types;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Be.Runtime.Convert
{
    public class ObjectConverter
    {
        public static string OUTPUT_DIRECTORY = @"..\..\..\..\be-output\";
        public static string OUTPUT_ASSEMBLY = "/be-csharp-project.dll";
        private ListCollection<string> SourceFileNames;

        public ObjectConverter()
        { }

        public void WriteSourcesAndCompile(SourceFileList sourceCollection)
        {
            SourceFileNames = new ListCollection<string>();
            if (!Directory.Exists(OUTPUT_DIRECTORY))
            {
                Directory.CreateDirectory(OUTPUT_DIRECTORY);
            }
            for(int i=0; i<sourceCollection.Size(); i++)
            {
                WriteSource(sourceCollection.Get(i));
            }
            Compile();
        }

        private void WriteSource(SourceFile sourceType)
        {
            string filePath = OUTPUT_DIRECTORY + "/" + Path.GetFileName(sourceType.Filepath.Replace(".be-src", ".be-cs"));
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("using System;");
            strBuilder.AppendLine("using System.IO;");
            for(int i=0; i<sourceType.Usings.Size(); i++)
            {
                strBuilder.AppendLine("using " + sourceType.Usings.Get(i).Path + ";");
            }
            strBuilder.AppendLine();
            for(int i=0; i<sourceType.Namespaces.Size(); i++)
            {
                NamespaceSymbol namespaceItem = sourceType.Namespaces.Get(i);
                ObjectCollection objectCollection = namespaceItem.Objects;
                strBuilder.AppendLine("namespace " + namespaceItem.Path);
                strBuilder.AppendLine("{");
                for (int j=0; j< objectCollection.Size(); j++)
                {
                    WriteObject(objectCollection.Get(j), 1, strBuilder);
                }
                strBuilder.AppendLine("}");
            }
            string str = strBuilder.ToString();
            File.WriteAllText(filePath, strBuilder.ToString());
            SourceFileNames.Add(filePath);
        }

        private void WriteObject(ObjectSymbol objectType, int intendLevel, StringBuilder strBuilder)
        {
            if(objectType.Accessor != null && objectType.Accessor.Type == AccessorType.PRIVATE)
            {
                strBuilder.Append(new String('\t', intendLevel) + "private ");
            }
            else
            {
                strBuilder.Append(new String('\t', intendLevel) + "public ");
            }
            strBuilder.Append("class " + objectType.String);
            if(objectType.ExtendType != null)
            {
                strBuilder.Append(" extends " + objectType.ExtendType.TypeName);
            }
            if(objectType.ImplementType != null)
            {
                strBuilder.Append(" implements ");
                for(int i=0; i < objectType.ImplementType.Size(); i++)
                {
                    strBuilder.Append(objectType.ImplementType.Get(i).Path);
                    if(i < objectType.ImplementType.Size() - 1)
                    {
                        strBuilder.Append(", ");
                    }
                }
            }
            strBuilder.AppendLine();
            strBuilder.AppendLine(new String('\t', intendLevel) + "{");
            for(int i=0; i<objectType.Members.Size(); i++)
            {
                WriteMember(objectType.Members.Get(i), intendLevel + 1, strBuilder);
                if (i == objectType.Members.Size() - 1)
                {
                    strBuilder.AppendLine();
                }
            }
            for (int i = 0; i < objectType.Objects.Size(); i++)
            {
                WriteObject(objectType.Objects.Get(i), intendLevel + 1, strBuilder);
                strBuilder.AppendLine();
            }
            for (int i=0; i<objectType.Constructors.Size(); i++)
            {
                WriteMethod(objectType.Constructors.Get(i), intendLevel + 1, strBuilder);
                strBuilder.AppendLine();
            }
            for(int i=0; i<objectType.Methods.Size(); i++)
            {
                WriteMethod(objectType.Methods.Get(i), intendLevel + 1, strBuilder);
                strBuilder.AppendLine();
            }
            for(int i=0; i<objectType.Properties.Size(); i++)
            {
                //WriteMethod(objectType.Properties.Get(i), intendLevel + 1, strBuilder);
                //strBuilder.AppendLine();
            }
            strBuilder.AppendLine(new String('\t', intendLevel) + "}");
        }

        private void WriteMember(MemberType memberType, int intendLevel, StringBuilder strBuilder)
        {
            strBuilder.Append(new String('\t', intendLevel));
            if(memberType.Accessor != null && memberType.Accessor.Type == AccessorType.PRIVATE)
            {
                strBuilder.Append("private ");
            }
            else
            {
                strBuilder.Append("public ");
            }
            strBuilder.Append(memberType.ObjectType.String+ " ");
            strBuilder.Append(memberType.MemberName);
            if (memberType.InitialisationExpression != null)
            {
                strBuilder.Append(" = ");
                WriteExpression(memberType.InitialisationExpression, strBuilder);
            }
            strBuilder.Append(";");
            strBuilder.AppendLine();
        }

        private void WriteMethod(MethodType methodType, int intendLevel, StringBuilder strBuilder)
        {
            strBuilder.Append(new String('\t', intendLevel));
            if(methodType.Accessor != null && methodType.Accessor.Type == AccessorType.PRIVATE)
            {
                strBuilder.Append("private ");
            }
            else
            {
                strBuilder.Append("public ");
            }
            strBuilder.Append(methodType.ReturnType + " ");
            strBuilder.Append(methodType.Name);
            strBuilder.Append("(");
            WriteParameterList(methodType.ParameterCollection, strBuilder);
            strBuilder.Append(")");
            strBuilder.AppendLine();
            strBuilder.AppendLine(new String('\t', intendLevel) + "{");
            WriteStatements(methodType.Code.Statements, intendLevel + 1, strBuilder);
            strBuilder.AppendLine(new String('\t', intendLevel) + "}");
        }

        private void WriteStatements(StatementCollection statementCollection, int intendLevel, StringBuilder strBuilder)
        {
            StatementType statement;
            for(int i=0; i<statementCollection.Size(); i++)
            {
                statement = statementCollection.Get(i);
                if(statement.Type == StatementTypeEnum.INNER_BLOCK)
                {
                    strBuilder.AppendLine(new String('\t', intendLevel)+"{");
                    WriteStatements((statement as InnerBlockStatementType).Statements, intendLevel+1, strBuilder);
                    strBuilder.AppendLine(new String('\t', intendLevel) + "}");
                }
                else if(statement.Category == StatementCategoryEnum.CONDITION_BLOCK)
                {
                    ConditionStatementType conditionStatement = statement as ConditionStatementType;
                    if(conditionStatement.Type == StatementTypeEnum.IF)
                    {
                        strBuilder.Append(new String('\t', intendLevel) + "if(");
                        WriteExpression(conditionStatement.ConditionExpression, strBuilder);
                        strBuilder.AppendLine(")");
                    }
                    else if(conditionStatement.Type == StatementTypeEnum.ELSE_IF)
                    {
                        strBuilder.Append(new String('\t', intendLevel) + "else if(");
                        WriteExpression(conditionStatement.ConditionExpression, strBuilder);
                        strBuilder.AppendLine(")");
                    }
                    else if(conditionStatement.Type == StatementTypeEnum.ELSE)
                    {
                        strBuilder.AppendLine(new String('\t', intendLevel) + "else");
                    }
                    else
                    {
                        throw new Exception("invalid state");
                    }
                    strBuilder.AppendLine(new String('\t', intendLevel) + "{");
                    WriteStatements(conditionStatement.Statements, intendLevel + 1, strBuilder);
                    strBuilder.AppendLine(new String('\t', intendLevel) + "}");
                }
                else if(statement.Type == StatementTypeEnum.FOR)
                {
                    ForLoopStatementType forLoop = statement as ForLoopStatementType;
                    strBuilder.Append(new String('\t', intendLevel) + "for(");
                    if(forLoop.VariableDeclarationCollection != null)
                    {
                        WriteVariableDeclaration(forLoop.VariableDeclarationCollection, intendLevel + 1, strBuilder);
                    }
                    else if(forLoop.ExpressionInitiationCollection != null)
                    {
                        WriteExpressionParameterList(forLoop.ExpressionInitiationCollection, strBuilder);
                    }
                    else
                    {
                        ;
                    }
                    strBuilder.Append(";");
                    if(forLoop.ConditionExpression != null)
                    {
                        WriteExpression(forLoop.ConditionExpression, strBuilder);
                    }
                    strBuilder.Append(";");
                    if(forLoop.PostExpressionCollection != null)
                    {
                        WriteExpressionParameterList(forLoop.PostExpressionCollection, strBuilder);
                    }
                    strBuilder.AppendLine("){");
                    WriteStatements(forLoop.Statements, intendLevel + 1, strBuilder);
                    strBuilder.Append(new String('\t', intendLevel) + "}");
                }
                else if(statement.Type == StatementTypeEnum.WHILE)
                {
                    WhileLoopStatementType whileLoop = statement as WhileLoopStatementType;
                    strBuilder.Append(new String('\t', intendLevel) + "while(");
                    WriteExpression(whileLoop.ConditionExpression, strBuilder);
                    strBuilder.AppendLine("){");
                    WriteStatements(whileLoop.Statements, intendLevel + 1, strBuilder);
                    strBuilder.AppendLine(new String('\t', intendLevel) + "}");
                }
                else if (statement.Category == StatementCategoryEnum.LOOP_CONTROL)
                {
                    if(statement.Type == StatementTypeEnum.CONTINUE)
                    {
                        strBuilder.AppendLine(new String('\t', intendLevel) + "continue;");
                    }
                    else if(statement.Type == StatementTypeEnum.BREAK)
                    {
                        strBuilder.AppendLine(new String('\t', intendLevel) + "break");
                    }
                    else
                    {
                        throw new Exception("invalid state");
                    }
                }
                else if(statement.Type == StatementTypeEnum.THROW)
                {
                    strBuilder.Append(new String('\t', intendLevel) + "throw ");
                    WriteExpression((statement as ErrorControlStatementType).ErrorExpression, strBuilder);
                    strBuilder.AppendLine(";");
                }
                else if(statement.Type == StatementTypeEnum.RETURN)
                {
                    FunctionControlStatementType functionStatement = statement as FunctionControlStatementType;
                    strBuilder.Append(new String('\t', intendLevel) + "return");
                    if(functionStatement.ReturnExpression != null)
                    {
                        strBuilder.Append(" ");
                        WriteExpression(functionStatement.ReturnExpression, strBuilder);
                    }
                    strBuilder.AppendLine(";");
                }
                else if(statement.Type == StatementTypeEnum.NO_OPERATION)
                {
                    strBuilder.AppendLine(new String('\t', intendLevel) + ";");
                }
                else if(statement.Type == StatementTypeEnum.EXPRESSION_STATEMENT)
                {
                    strBuilder.Append(new String('\t', intendLevel));
                    WriteExpression((statement as ExpressionStatementType).StatementExpression, strBuilder);
                    strBuilder.AppendLine(";");
                }
                else if(statement.Type == StatementTypeEnum.DECLARATION)
                {
                    WriteVariableDeclaration((statement as VariableDeclarationStatementType).VariableDeclarationCollection, intendLevel + 1, strBuilder);
                    strBuilder.AppendLine(";");
                }
                else if(statement.Type == StatementTypeEnum.THREAD_SYNC)
                {
                    strBuilder.Append(new String('\t', intendLevel)+ "lock{");
                    WriteStatements((statement as SynchronisationStatementType).Statements, intendLevel + 1, strBuilder);
                    strBuilder.Append(new String('\t', intendLevel) + "}");
                }
                else
                {
                    throw new Exception("not implemented");
                }
            }
        }

        private void WriteVariableDeclaration(VariableCollection variableCollection, int intendLevel, StringBuilder strBuilder)
        {
            strBuilder.Append(new String('\t', intendLevel));
            VariableType firstVariable = variableCollection.Get(0);
            if(firstVariable.IsConst)
            {
                strBuilder.Append("readonly ");
            }
            WriteArrayDeclaration(firstVariable.ArrayDeclarationType, strBuilder);
            WriteGenericDeclaration(firstVariable.GenericDeclarationType, strBuilder);
            strBuilder.Append(firstVariable.TypeName + " " + firstVariable.VariableName);
            for (int i=1; i<variableCollection.Size(); i++)
            {
                strBuilder.Append(", ");
                VariableType nextVariable = variableCollection.Get(i);
                strBuilder.Append(nextVariable.VariableName);
            }
        }

        private void WriteArrayDeclaration(ArrayType arrayType, StringBuilder strBuilder)
        {
            for(int i=0; i<arrayType.DimensionCount; i++)
            {
                strBuilder.Append("[]");
            }
        }

        private void WriteGenericDeclaration(GenericType genericType, StringBuilder strBuilder)
        {
            strBuilder.Append("<");
            for(int i=0; i<genericType.ElementCollection.Size(); i++)
            {
                GenericElementType genericElement = genericType.ElementCollection.Get(i);
                strBuilder.Append(genericElement.TypeName);
                if(genericElement.ExtendTypeName != null)
                {
                    strBuilder.Append(" ?" + genericElement.ExtendTypeName);
                }
                if(genericElement.GenericType != null)
                {
                    WriteGenericDeclaration(genericElement.GenericType, strBuilder);
                }
                if(i < genericType.ElementCollection.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            strBuilder.Append(">");
        }

        private void WriteExpression(ExpressionType expressionType, StringBuilder strBuilder)
        {
            if(expressionType.FrontExpressionOperator != null)
            {
                strBuilder.Append(expressionType.FrontExpressionOperator.SymbolString);
            }
            if(expressionType.ChildExpression != null)
            {
                strBuilder.Append("(");
                WriteExpression(expressionType.ChildExpression, strBuilder);
                strBuilder.Append(")");
            }
            else
            {
                WriteOperandAndOperators(expressionType.Operand, strBuilder);
            }
            if(expressionType.OperationsWithExpressions == null)
            {
                return;
            }
            if(expressionType.BackExpressionOperator != null)
            {
                strBuilder.Append(expressionType.BackExpressionOperator.SymbolString);
            }
            ExpressionOperation expressionOperation;
            for(int i=0; i<expressionType.OperationsWithExpressions.Size(); i++)
            {
                expressionOperation = expressionType.OperationsWithExpressions.Get(i);
                strBuilder.Append(" " + expressionOperation.Operation.SymbolString + " ");
                WriteExpression(expressionOperation.SecondExpression, strBuilder);
            }
        }

        private void WriteObjectInitiation(ObjectInitialisationType objectInitiationType, StringBuilder strBuilder)
        {
            strBuilder.Append("(");
            for(int i=0; i<objectInitiationType.ConstructorDefinitionList.Size(); i++)
            {
                WriteExpression(objectInitiationType.ConstructorDefinitionList.Get(i), strBuilder);
                if (i < objectInitiationType.ConstructorDefinitionList.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            string[] namedKeyes = objectInitiationType.MemberDefinitionMap.GetKeys();
            for(int i=0; i < namedKeyes.Length; i++)
            {
                if (i < namedKeyes.Length - 1)
                {
                    strBuilder.Append(", ");
                }
                strBuilder.Append(namedKeyes[i] + " = ");
                WriteExpression(objectInitiationType.MemberDefinitionMap.GetValue(namedKeyes[i]), strBuilder);
            }
            strBuilder.Append(")");
        }

        private void WriteArrayInitiation(ArrayType arrayType, StringBuilder strBuilder)
        {
            strBuilder.Append("[");
            for(int i=0; i<arrayType.DimensionDepthList.Size(); i++)
            {
                strBuilder.Append(arrayType.DimensionDepthList.Get(i));
                if(i < arrayType.DimensionDepthList.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            strBuilder.Append("]");
            ArrayNodeType rootNode = arrayType.InitialisationRootNode;
            for(int i=0; i< rootNode.ChildNodes.Size(); i++)
            {
                WriteArrayInitiationDepth(rootNode.ChildNodes.Get(i), strBuilder);
                if(i < rootNode.ChildNodes.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
        }

        private void WriteArrayInitiationDepth(ArrayNodeType arrayNodeType, StringBuilder strBuilder)
        {
            strBuilder.Append("{");
            for(int i = 0; i < arrayNodeType.ChildNodes.Size(); i++)
            {
                WriteArrayInitiationDepth(arrayNodeType.ChildNodes.Get(i), strBuilder);
                if (i < arrayNodeType.ChildNodes.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            for(int i = 0; i < arrayNodeType.InitialisationExpressionList.Size(); i++)
            {
                WriteExpression(arrayNodeType.InitialisationExpressionList.Get(i), strBuilder);
                if(i < arrayNodeType.InitialisationExpressionList.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            strBuilder.Append("}");
        }

        private void WriteOperandAndOperators(OperandType operandType, StringBuilder strBuilder)
        {
            if(operandType.Type == OperandTypeEnum.NEW_TYPE)
            {
                NewTypeOperand newTypeOperand = operandType as NewTypeOperand;
                strBuilder.Append("new ");
                strBuilder.Append(newTypeOperand.NewTypeName);
                if(newTypeOperand.GenericType != null)
                {
                    WriteGenericDeclaration(newTypeOperand.GenericType, strBuilder);
                }
                if(newTypeOperand.ObjectDefinitionType != null)
                {
                    WriteObjectInitiation(newTypeOperand.ObjectDefinitionType, strBuilder);
                }
                else if(newTypeOperand.ArrayInitiationType != null)
                {
                    WriteArrayInitiation(newTypeOperand.ArrayInitiationType, strBuilder);
                }
                strBuilder.AppendLine(";");
            }
            else if(operandType.Type == OperandTypeEnum.THIS)
            {
                strBuilder.Append("this");
            }
            else if(operandType.Type == OperandTypeEnum.BASE)
            {
                strBuilder.Append("base");
            }
            else if(operandType.Type == OperandTypeEnum.NULL)
            {
                strBuilder.Append("null");
            }
            else if(operandType.Type == OperandTypeEnum.BOOL)
            {
                strBuilder.Append((operandType as BoolOperand).BoolDataValue ? "true" : "false");
            }
            else if(operandType.Type == OperandTypeEnum.CHAR)
            {
                strBuilder.Append("'" + (operandType as CharOperand).CharContentString + "'");
            }
            else if(operandType.Type == OperandTypeEnum.NUMBER)
            {
                strBuilder.Append((operandType as NumberOperand).NumberContentString);
            }
            else if(operandType.Type == OperandTypeEnum.STRING)
            {
                strBuilder.Append("\"" + (operandType as StringOperand).StringDataValue + "\"");
            }
            else if(
                operandType.Type == OperandTypeEnum.MEMBER_OR_VARIABLE_OR_PARAMETER || 
                operandType.Type == OperandTypeEnum.MEMBER || operandType.Type == OperandTypeEnum.VARIABLE || operandType.Type == OperandTypeEnum.PARAMETER
            ){
                strBuilder.Append((operandType as MemberVariableOrParameterOperand).PathName);
            }
            else if(operandType.Type == OperandTypeEnum.METHOD)
            {
                MethodOperand methodOperand = operandType as MethodOperand;
                strBuilder.Append(methodOperand.MethodName);
                if(methodOperand.MethodGeneric != null)
                {
                    WriteGenericDeclaration(methodOperand.MethodGeneric, strBuilder);
                }
                strBuilder.Append(")");
                WriteExpressionParameterList(methodOperand.ParameterExpressions, strBuilder);
                strBuilder.Append(")");
            }
            else if(operandType.Type == OperandTypeEnum.MEMBER || operandType.Type == OperandTypeEnum.VARIABLE)
            {
                strBuilder.Append((operandType as MemberVariableOrParameterOperand).PathName);
            }
            else
            {
                throw new Exception("not implemented");
            }

            OperandOperatorType operandOperation;
            for(int i=0; i<operandType.Operations.Size(); i++)
            {
                operandOperation = operandType.Operations.Get(i);
                if(operandOperation.Type == OperandOperatorEnum.ARRAY_ACCESS)
                {
                    strBuilder.Append("[");
                    WriteExpression((operandOperation as ArrayAccessOperatorType).ArrayParameter.ParameterExpression, strBuilder);
                    strBuilder.Append("]");
                }
                if(operandOperation.Type == OperandOperatorEnum.MEMBER_ACCESS)
                {
                    strBuilder.Append(".");
                    strBuilder.Append((operandOperation as MemberAccessOperatorType).MemberName);
                }
                else if(operandOperation.Type == OperandOperatorEnum.METHOD_CALL)
                {
                    MethodCallOperatorType methodOperation = operandOperation as MethodCallOperatorType;
                    if(methodOperation.GenericType != null)
                    {
                        WriteGenericDeclaration(methodOperation.GenericType, strBuilder);
                    }
                    strBuilder.Append(methodOperation.MethodName + "(");
                    WriteExpressionParameterList(methodOperation.ParameterExpressions, strBuilder);
                    strBuilder.Append(")");
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
        }

        private void WriteParameterList(ParameterCollection parameterCollection, StringBuilder strBuilder)
        {
            ParameterType parameterType;
            for(int i=0; i<parameterCollection.Size(); i++)
            {
                parameterType = parameterCollection.Get(i);
                strBuilder.Append(parameterType.TypeName + " " + parameterType.VariableName);
                if (i < parameterCollection.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
        }

        private void WriteExpressionParameterList(ExpressionCollection expressionParameterCollection, StringBuilder strBuilder)
        {
            for(int i=0; i<expressionParameterCollection.Size(); i++)
            {
                WriteExpression(expressionParameterCollection.Get(i), strBuilder);
                if(i < expressionParameterCollection.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
        }

        private bool Compile()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.OutputAssembly = (OUTPUT_DIRECTORY + OUTPUT_ASSEMBLY);
            CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, SourceFileNames.ToArray());
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    Console.WriteLine("File: "+CompErr.FileName+" | Line: " + CompErr.Line +" | Error: " + CompErr.ErrorNumber +", '" + CompErr.ErrorText + ";");
                }
                return false;
            }
            return true;
        }
    }
}
