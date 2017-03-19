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
        public StringBuilder Builder = new StringBuilder();
        public ListCollection<StructedBlockSignature> StructedBlockList = new ListCollection<StructedBlockSignature>();

        public TargetWriter(string Filepath)
        {
            this.Filepath = Filepath;
        }

        public void Convert(SourceSymbol SourceSymbol)
        {
            Builder.Clear();
            WriteLine("using System;");
            WriteLine("using System.IO;");
            WriteLine("using System.Collections;");
            WriteLine("using feltic.Language;");
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
            StructedBlockSignature visual = ((SourceSymbol.ScopeList[0].VisualElement as ExpressionStatementSignature).Expression.Operand.AccessList[0] as StructedBlockAccessSignature).StructedBlock;
            if(visual != null){
                WriteVisualComponent(1, visual);
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
                WriteTypeDeclaration(mbr.Signature.TypeDeclaration);
                WriteLine(";");
            }
            WriteLine();
            for (int j = 0; j < obj.MethodList.Size; j++)
            {
                MethodSymbol mth = obj.MethodList[j];
                WriteTab(tabs+1);
                Write("public ");
                WriteTypeDeclaration(mth.Signature.TypeDeclaration);
                Write("(");
                WriteParameterList(mth.Signature.ParameterDeclaration.ParameterList);
                WriteLine(")");
                WriteLine(tabs+1, "{");
                WriteStatements(3, mth.Signature.Code.Statements);
                WriteLine(tabs+1, "}");
            }
            WriteLine(tabs, "}");
            WriteLine();
        }

        void WriteStatements(int tabs, StatementSignatureList stmList)
        {
            for(int i=0; i<stmList.Size; i++)
            {
                WriteTab(tabs);
                StatementSignature stm = stmList[i];
                if (stm.Group == StatementGroup.ConditionBlock)
                {
                    Write(stm.Keyword.String+"(");
                    WriteExpression((stm as ConditionBlockStatementSignature).ConditionExpression);
                    WriteLine(")");
                    WriteLine(tabs, "{");
                    WriteStatements(tabs+1, (stm as ConditionBlockStatementSignature).ChildStatements);
                    WriteLine(tabs, "}");
                }
                else if(stm.Group == StatementGroup.KeywordStatement)
                {
                    WriteLine(tabs, stm.Keyword.String + ";");
                }
                else if(stm.Group == StatementGroup.ExpressionStatement)
                {
                    WriteTab(tabs);
                    if(stm.Keyword != null)
                    {
                        Write(stm.Keyword.String);  
                        Write(" ");
                    }
                    WriteExpression((stm as ExpressionStatementSignature).Expression);
                }
                else if(stm.Group == StatementGroup.BlockStatement)
                {
                    if(stm.Keyword != null)
                    {
                        WriteLine(stm.Keyword.String);
                    }
                    WriteLine(tabs, "{");
                    WriteStatements(tabs+1, (stm as BlockStatementSignature).ChildStatements);
                    WriteLine(tabs, "}");
                }
                else if(stm.Type == StatementType.TypeDeclaration)
                {
                    WriteTypeDeclaration((stm as TypeDeclarationStatementSignature).TypeDeclaration);
                    WriteLine(";");
                }
                else if(stm.Type == StatementType.ExpressionStatement)
                {
                    WriteExpression((stm as ExpressionStatementSignature).Expression);
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
                            WriteTypeDeclaration((sl[j] as TypeDeclarationSignature));
                        }
                        if (sl[j] is ExpressionSignature)
                        {
                            WriteExpression((sl[j] as ExpressionSignature));
                        }
                    }
                    Write(";");
                    WriteExpression(forLoop.ConditionExpression);
                    Write(";");
                    SignatureList ol = forLoop.PostOperationList;
                    for (int j = 0; j < ol.Size; j++)
                    {
                        WriteExpression((ol[j] as ExpressionSignature));
                    }
                    WriteLine(")");
                    WriteLine(tabs, "{");
                    WriteStatements(tabs + 1, (stm as ConditionBlockStatementSignature).ChildStatements);
                    WriteLine(tabs, "}");
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
        }

        void WriteTypeDeclaration(TypeDeclarationSignature td)
        {
            if(td.TypeNative != null)
                Write(td.TypeNative.String + " ");
            if(td.TypeIdentifier != null)
                Write(td.TypeIdentifier.String + " ");
            if(td.TypeGeneric != null)
                WriteGenericDeclaration(td.TypeGeneric);
            if(td.TypeArray != null)
                WriteArrayDeclaration(td.TypeArray);
            if(td.NameIdentifier != null)
                Write(td.NameIdentifier.String);
            if(td.Assigment != null)
                Write(" = ");
            if(td.AssigmentExpression != null)
                WriteExpression(td.AssigmentExpression);
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

        void WriteExpression(ExpressionSignature exp)
        {
            if(exp.ChildExpression != null)
            {
                Write("(");
                WriteExpression(exp.ChildExpression);
                Write(")");
                return;
            }
            
            for(int i=0; i < exp.Operand.AccessList.Size; i++)
            {
                AccessSignature access = exp.Operand.AccessList[i];
                if(access.Type == SignatureType.LiteralAccess)
                {
                    Write((access as LiteralAccessSignature).Literal.String);
                }
                else if(access.Type == SignatureType.VariableAccess)
                {
                    Write((access as VariableAccessSignature).Identifier.String);
                }
                else if(access.Type == SignatureType.FunctionAccess)
                {
                    FunctionAccessSignature functionAccess = access as FunctionAccessSignature;
                    Write(functionAccess.Identifier.String);
                    Write("(");
                    WriteParameterList(functionAccess.ParameterList);
                    Write(")");
                }
                else if(access.Type == SignatureType.ArrayAccess)
                {
                    ArrayAccessSignature arrayAccess = access as ArrayAccessSignature;
                    Write(arrayAccess.Identifier.String);
                    Write("[");
                    WriteParameterList(arrayAccess.ParameterList);
                    Write("]");
                }
                else if(access.Type == SignatureType.StructedBlockAccess)
                {
                    StructedBlockAccessSignature blockAcess = access as StructedBlockAccessSignature;
                    this.StructedBlockList.Add(blockAcess.StructedBlock);
                }
                if(access.Seperator != null)
                {
                    Write(access.Seperator.String);
                }
            }
        
            for(int i=0; i < exp.OperationList.Size; i++)
            {
                ExpressionOperationPair opPair = exp.OperationList[i];
                OperationSignature op = opPair.Operation;
                Write(" " + op.Token.String + " ");
                WriteExpression(opPair.ExpressionPair);
            }
        }

        void WriteParameterList(ListCollection<ParameterSignature> pl)
        {
            for(int i=0; i<pl.Size; i++)
            {
                ParameterSignature ps = pl[i];
                if(ps.TypeDeclaration != null)
                {
                    WriteTypeDeclaration(ps.TypeDeclaration);
                }
                if(ps.Expression != null)
                {
                    WriteExpression(ps.Expression);
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

        private bool Compile()
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters(new string[]{ "BeeRuntime.exe" });
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
    }
}
