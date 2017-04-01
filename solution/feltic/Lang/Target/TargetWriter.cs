using feltic.Integrator;
using feltic.Library;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class TargetWriter
    {
        public string Filepath;
        public Registry Registry;
        public StringBuilder Builder = new StringBuilder();
        public Stack<StringBuilder> Stack = new Stack<StringBuilder>();

        public TargetWriter(string Filepath)
        {
            this.Filepath = Filepath;
        }

        public void Convert(Registry Registry, SourceSymbol SourceSymbol)
        {
            this.Registry = Registry;
            Stack.Clear();
            Builder.Clear();
            WriteLine("using System;");
            WriteLine("using System.IO;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using feltic.Language;");
            WriteLine("using feltic.Library;");
            WriteLine("using feltic.UI;");
            WriteLine("using feltic.UI.Types;");
            WriteLine("using feltic.Integrator;"); 
            for (int i = 0; i < SourceSymbol.UseList.Size; i++)
            {

            }
            WriteLine();
            ObjectSymbolList objectSymbolList = SourceSymbol.ScopeList[0].ObjectList;
            WriteLine("namespace Scope");
            WriteLine("{");
            for (int i = 0; i < objectSymbolList.Size; i++)
            {
                WriteObject(1, objectSymbolList[i]);
            }
            WriteLine();
            for (int i=0; i<VisualComponents.Size; i++)
            {
                WriteLine(VisualComponents[i].Builder.ToString());
            }
            for(int i=0; i<StateReceivers.Size; i++)
            {
                WriteLine(StateReceivers[i].Builder.ToString());
            }
            WriteLine("}");
            File.WriteAllText(Filepath, Builder.ToString());
            Compile();
        }

        void WriteObject(int tabs, ObjectSymbol obj)
        {
            WriteLine(tabs, "public class " + obj.Signature.Identifier.String);
            WriteLine(tabs, "{");
            for (int j = 0; j < obj.MemberList.Size; j++)
            {
                MemberSymbol mbr = obj.MemberList[j];
                WriteTab(tabs+1);
                Write("public ");
                WriteTypeDeclaration(obj, null, null, mbr.Signature.TypeDeclaration);
                WriteLine(";");
            }
            WriteLine();
            for (int j = 0; j < obj.MethodList.Size; j++)
            {
                MethodSymbol mth = obj.MethodList[j];
                StateTarget stateTarget = null;
                if (mth.Signature.TypeDeclaration!=null && mth.Signature.TypeDeclaration.TypeIdentifier.IsNative(NativeType.State))
                {
                    stateTarget = AddStateTarget(obj, mth);
                    WriteLine(tabs+1, ("public ReceiverContainer_" + stateTarget.IdentifierString + " RC_" + stateTarget.IdentifierString + " = new ReceiverContainer_" + stateTarget.IdentifierString + "();"));
                }
                WriteTab(tabs+1);
                Write("public ");
                if(mth.Signature.TypeIdentifier != null)
                {
                    Write(mth.Signature.TypeIdentifier.String);
                }
                else
                {
                    WriteTypeDeclaration(obj, mth, null, mth.Signature.TypeDeclaration);
                }
                Write("(");
                WriteParameterList(obj, mth, null, mth.Signature.ParameterDeclaration.Elements);
                WriteLine(")");
                WriteLine(tabs+1, "{");
                WriteStatements(3, obj, mth, null, mth.Signature.Code.Elements);
                if(stateTarget != null)
                {
                    WriteTab(3);
                    Write("RC_" + stateTarget.IdentifierString+".EventState(");
                    for (int p = 0; p < mth.Signature.ParameterDeclaration.Elements.Size; p++)
                    {
                        Write(mth.Signature.ParameterDeclaration.Elements[p].TypeDeclaration.NameIdentifier.String);
                        if(p < mth.Signature.ParameterDeclaration.Elements.Size - 1)
                            Write(", ");
                    }
                    WriteLine(");");
                }
                WriteLine(tabs+1, "}");
            }
            WriteLine(tabs, "}");
            WriteLine();
        }

        void WriteStatements(int tabs, ObjectSymbol obj, MethodSymbol mth, VisualComponent vis, SignatureList sigList)
        {
            if (sigList == null) return;
            for(int i=0; i<sigList.Size; i++)
            {
                if (sigList[i].Type == SignatureType.Statement)
                    WriteStatement(tabs, obj, mth, vis, sigList[i] as StatementSignature);
            }
        }

        void WriteStatement(int tabs, ObjectSymbol obj, MethodSymbol mth, VisualComponent vis, StatementSignature stm)
        {
            WriteTab(tabs);
            if (stm.Group == StatementCategory.ConditionBlock)
            {
                Write(stm.Keyword.String+"(");
                WriteExpression(obj, mth, vis, (stm as ConditionBlockStatementSignature).ConditionExpression);
                WriteLine(")");
                WriteLine(tabs, "{");
                WriteStatements(tabs+1, obj, mth, vis, (stm as ConditionBlockStatementSignature).Elements);
                WriteLine(tabs, "}");
            }
            else if(stm.Group == StatementCategory.KeywordStatement)
            {
                WriteLine(tabs, stm.Keyword.String + ";");
            }
            else if(stm.Group == StatementCategory.ExpressionStatement)
            {
                WriteTab(tabs);
                if(stm.Keyword != null)
                {
                    Write(stm.Keyword.String);  
                    Write(" ");
                }
                WriteExpression(obj, mth, vis, (stm as ExpressionStatementSignature).Expression);
                WriteLine(";");
            }
            else if(stm.Group == StatementCategory.BlockStatement)
            {
                if(stm.Keyword != null)
                {
                    WriteLine(stm.Keyword.String);
                }
                WriteLine(tabs, "{");
                WriteStatements(tabs+1, obj, mth, vis, (stm as BlockStatementSignature).Elements);
                WriteLine(tabs, "}");
            }
            else if(stm.Type == StatementType.TypeDeclaration)
            {
                WriteTypeDeclaration(obj, mth, vis, (stm as TypeDeclarationStatementSignature).TypeDeclaration);
                WriteLine(";");
            }
            else if(stm.Type == StatementType.ExpressionStatement)
            {
                WriteExpression(obj, mth, vis, (stm as ExpressionStatementSignature).Expression);
                WriteLine(";");
            }
            else if(stm.Type == StatementType.For)
            {
                Write("for(");
                ForLoopStatementSignature forLoop = stm as ForLoopStatementSignature;
                SignatureList sl = forLoop.ParameterList;
                for (int j = 0; j < sl.Size; j++)
                {
                    if (sl[j] is TypeDeclarationSignature)
                    {
                        WriteTypeDeclaration(obj, mth, vis, (sl[j] as TypeDeclarationSignature));
                    }
                    if (sl[j] is ExpressionSignature)
                    {
                        WriteExpression(obj, mth, vis, (sl[j] as ExpressionSignature));
                    }
                }
                Write(";");
                WriteExpression(obj, mth, vis, forLoop.ConditionExpression);
                Write(";");
                SignatureList ol = forLoop.PostOperationList;
                for (int j = 0; j < ol.Size; j++)
                {
                    WriteExpression(obj, mth, vis, (ol[j] as ExpressionSignature));
                }
                WriteLine(")");
                WriteLine(tabs, "{");
                WriteStatements(tabs + 1, obj, mth, vis, (stm as ConditionBlockStatementSignature).Elements);
                WriteLine(tabs, "}");
            }
            else
            {
                //throw new Exception("invalid state");
            }
        }

        void WriteTypeDeclaration(ObjectSymbol obj, MethodSymbol mth, VisualComponent vis, TypeDeclarationSignature td)
        {
            if(td.TypeIdentifier != null)
                if(td.TypeIdentifier.IsNative(NativeType.State) || td.TypeIdentifier.IsNative(NativeType.Func))
                    Write("void ");
                else
                    Write(td.TypeIdentifier.String + " ");
            if (td.TypeGeneric != null)
                WriteGenericDeclaration(td.TypeGeneric);
            if(td.TypeArray != null)
                WriteArrayDeclaration(td.TypeArray);
            if(td.NameIdentifier != null)
                Write(td.NameIdentifier.String);
            if(td.Assigment != null)
                Write(" = ");
            if(td.AssigmentExpression != null)
                WriteExpression(obj, mth, vis, td.AssigmentExpression, true);
        }

        void WriteGenericDeclaration(GenericDeclarationSignature gn)
        {
            Write("<");
            for(int i=0; i<gn.ElementList.Size; i++)
            {
                GenericElementSignature ge = gn.ElementList[i];
                Write(ge.Identifier.String);
                if(ge.Generic != null)
                {
                    WriteGenericDeclaration(ge.Generic);
                }
                if(i < gn.ElementList.Size-1)
                {
                    Write(", ");
                }
            }
            Write(">");
        }

        void WriteArrayDeclaration(ArrayDeclarationSignature ad)
        {
            Write("[");
            if(ad.DimensionSymbols.Size > 0)
            {
                Write(new string(',', ad.DimensionSymbols.Size));
            }
            Write("]");
        }

        void WriteExpression(ObjectSymbol obj, MethodSymbol mth, VisualComponent vis, ExpressionSignature exp, bool IsAssigment = false)
        {
            if (exp.ChildExpression != null)
            {
                Write("(");
                WriteExpression(obj, mth, vis, exp.ChildExpression);
                Write(")");
                return;
            }

            if (exp.PreOperation != null && vis == null) Write(exp.PreOperation.Token.String);

            // state receiver assigment callback code
            if (exp.Operation != null && exp.ExpressionPair != null &&
                exp.Operand.AccessList[0].Type == SignatureType.VariableOperand &&
                exp.Operation.Token.IsOperation(OperationType.AddAssigment) &&
                exp.ExpressionPair.Operand.AccessList[0].Type == SignatureType.ObjectOperand &&
                (exp.ExpressionPair.Operand.AccessList[0] as ObjectAccessOperand).ObjectType.IsNative(NativeType.State)
            ){
                string objectVariableIdentifier = (exp.Operand.AccessList[0] as VariableOperand).Identifier.String;
                TypeDeclarationSignature objecTypeDeclaration = GetObjectMemberTypeDeclaration(obj, objectVariableIdentifier);
                if (objecTypeDeclaration == null)
                    objecTypeDeclaration = GetMethodParameterTypeDeclaration(mth, objectVariableIdentifier);
                if (objecTypeDeclaration == null)
                    objecTypeDeclaration = GetVariableTypeDeclaration(mth, objectVariableIdentifier);

                string stateMethodIdentifier = (exp.Operand.AccessList[1] as VariableOperand).Identifier.String;
                ObjectSymbol stateObject = Registry.GetObjectSymbol(objecTypeDeclaration.TypeIdentifier.String);
                StateTarget stateTarget = GetStateTarget(stateObject, stateMethodIdentifier);
                SignatureList codeImplementation = (exp.ExpressionPair.Operand.AccessList[0] as ObjectAccessOperand).ContentSignatures;
                WriteStateImplementation(stateTarget, obj, mth, codeImplementation);

                Write("new TargetState_" + stateTarget.IdentifierString + "("+ objectVariableIdentifier+".RC_" + stateTarget.IdentifierString + ", this)");

                return;
            }

            for (int i=0; i < exp.Operand.AccessList.Size; i++)
            {
                OperandAccessSignature access = exp.Operand.AccessList[i];
                if(access.Type == SignatureType.LiteralOperand)
                {
                    if(exp.PreOperation != null && vis != null && exp.PreOperation.Token.IsStructure(StructureType.Point))
                    {
                        string stringData = (access as LiteralOperand).Literal.String;
                        stringData = stringData.Replace("\"", "");
                        Write("new VisualTextElement(\"" + stringData + "\", parent)");
                    }
                    else
                        Write((access as LiteralOperand).Literal.String);
                }
                else if(access.Type == SignatureType.VariableOperand)
                {
                    if(vis == null)
                        Write((access as VariableOperand).Identifier.String);
                    else
                    {
                        string variableIdentfier = (access as VariableOperand).Identifier.String;
                        if(GetObjectMemberTypeDeclaration(vis.Object, variableIdentfier) != null)
                        {
                            Write("Object." + variableIdentfier);
                            if (exp.PreOperation != null && exp.PreOperation.Token.IsStructure(StructureType.Point))
                            {
                                Write(";parent.AddChild(Object."+variableIdentfier+")");
                            }
                        }
                        else
                            Write(variableIdentfier);        
                    }
                }
                else if(access.Type == SignatureType.ObjectOperand)
                {
                    ObjectAccessOperand objectAccess = access as ObjectAccessOperand;
                    Write("new ");
                    Write(objectAccess.ObjectType.String);
                    Write("(");
                    WriteParameterList(obj, mth, vis, objectAccess.ParameterDefinition.Elements);
                    Write(")");
                }
                else if(access.Type == SignatureType.FunctionOperand)
                {
                    FunctionOperand functionAccess = access as FunctionOperand;
                    Write(functionAccess.Identifier.String);
                    Write("(");
                    WriteParameterList(obj, mth, vis, functionAccess.ParameterDefinition.Elements);
                    Write(")");
                }
                else if(access.Type == SignatureType.ArrayOperand)
                {
                    ArrayOperand arrayAccess = access as ArrayOperand;
                    Write(arrayAccess.Identifier.String);
                    Write("[");
                    WriteParameterList(obj, mth, vis, arrayAccess.ParameterDefintion.Elements);
                    Write("]");
                }
                else if(access.Type == SignatureType.StructedBlockOperand)
                {
                    VisualComponent visual = AddVisualComponent(1, obj, mth, access as StructedBlockOperand);
                    if (vis == null)
                        Write("new " + visual.IdentifierString + "(this");
                    else
                    {
                        if (exp.PreOperation != null && vis != null && exp.PreOperation.Token.IsStructure(StructureType.Point))
                        {
                            Write("parent.AddChild(new " + visual.IdentifierString + "(Object");
                        }
                        else
                            Write("parent.AddChild(new " + visual.IdentifierString + "(Object");
                    }
                    for (int p = 0; p < visual.ParameterVariables.Size; p++)
                    {
                        Write(", " + visual.ParameterVariables[i].NameIdentifier.String);
                    }
                    if (vis == null)
                        WriteLine(")");
                    else
                        WriteLine(").Visual)");
                }
                if(access.Seperator != null)
                {
                    Write(access.Seperator.String);
                }
            }
            if (exp.PostOperation != null) Write(exp.PostOperation.Token.String);

            ExpressionSignature expPtr = exp;
            while(expPtr.Operation != null && expPtr.ExpressionPair != null)
            {
                Write(" " + expPtr.Operation.Token.String + " ");
                WriteExpression(obj, mth, vis, expPtr.ExpressionPair);
                expPtr = expPtr.ExpressionPair;
            }
        }
    
        void WriteParameterList(ObjectSymbol obj, MethodSymbol mth, VisualComponent vis, ListCollection<ParameterSignature> pl)
        {
            for(int i=0; i<pl.Size; i++)
            {
                ParameterSignature ps = pl[i];
                if(ps.TypeDeclaration != null)
                {
                    WriteTypeDeclaration(obj, mth, vis, ps.TypeDeclaration);
                }
                if(ps.Expression != null)
                {
                    WriteExpression(obj, mth, vis, ps.Expression);
                }
                if(ps.Seperator != null)
                {
                    Write(ps.Seperator.String);
                    Write(" ");
                }
            }
        }

        private void WriteLine(int TabCount, string String)
        {
            Builder.AppendLine(new string('\t', TabCount) + String);
        }

        private void WriteLine(string String)
        {
            Builder.AppendLine(String);
        }

        private void WriteLine()
        {
            Builder.AppendLine();
        }

        private void Write(string String)
        {
            Builder.Append(String);
        }

        private void WriteTab(int Count)
        {
            Builder.Append(new string('\t', Count));
        }

        private StringBuilder PushBuilder()
        {
            Stack.Push(Builder);
            Builder = new StringBuilder();
            return Builder;
        }

        private StringBuilder PopBuilder()
        {
            Builder = Stack.Pop();
            return Builder;
        }

        private bool Compile()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters(new string[]{ "feltic.exe", "System.dll" });
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.OutputAssembly = Filepath+".dll";
            CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, Filepath);
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    Console.WriteLine("File: " + CompErr.FileName + " | Line: " + CompErr.Line + " | Error: " + CompErr.ErrorNumber + ", '" + CompErr.ErrorText + ";");
                }
                return false;
            }
            return true;
        }

        public static TypeDeclarationSignature GetObjectMemberTypeDeclaration(ObjectSymbol Object, string variableIdentifier)
        {
            for (int i = 0; i < Object.MemberList.Size; i++)
            {
                MemberSymbol mbr = Object.MemberList[i];
                if (mbr.Signature.TypeDeclaration.NameIdentifier.String == variableIdentifier)
                {
                    return mbr.Signature.TypeDeclaration;
                }
            }
            return null;
        }

        public static TypeDeclarationSignature GetMethodParameterTypeDeclaration(MethodSymbol Method, string variableIdentifier)
        {
            for (int i = 0; i < Method.Signature.ParameterDeclaration.Elements.Size; i++)
            {
                ParameterSignature prm = Method.Signature.ParameterDeclaration.Elements[i];
                if (prm.TypeDeclaration.NameIdentifier.String == variableIdentifier)
                {
                    return prm.TypeDeclaration;
                }
            }
            return null;
        }

        public static TypeDeclarationSignature GetVariableTypeDeclaration(MethodSymbol Method, string variableIdentifier)
        {
            for(int i=0; i < Method.Signature.Code.Elements.Size; i++)
            {
                SignatureSymbol stm = Method.Signature.Code.Elements[i];
                if (stm.Type == SignatureType.Statement)
                    if ((stm as StatementSignature).Type == StatementType.TypeDeclaration)
                        if (((stm as TypeDeclarationStatementSignature).TypeDeclaration.NameIdentifier.String == variableIdentifier))
                            return (stm as TypeDeclarationStatementSignature).TypeDeclaration;
            }
            return null;
        }

        
    }
}
