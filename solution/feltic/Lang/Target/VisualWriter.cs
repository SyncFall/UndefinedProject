using feltic.Language;
using feltic.Library;
using feltic.UI;
using feltic.UI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class VisualComponent
    {
        public static int IdCounter = 0;
        public string IdentifierString;
        public ObjectSymbol Object;
        public MethodSymbol Method;
        public SignatureSymbol Signature;
        public ListCollection<TypeDeclarationSignature> ParameterVariables = new ListCollection<TypeDeclarationSignature>();
        public StringBuilder Builder;

        public VisualComponent(ObjectSymbol Object, MethodSymbol Method, SignatureSymbol Signature)
        {
            this.Object = Object;
            this.Method = Method;
            this.Signature = Signature;
            BuildIdentifier();
        }

        public void BuildIdentifier()
        {
            IdCounter++;
            string str = "Visual_"+IdCounter;
            if(Object != null)
                str += "_"+Object.Signature.Identifier.String;
            if(Method != null)
                str += "_"+Method.Signature.TypeIdentifier.String;
            this.IdentifierString = str;
        }
    }

    public partial class TargetWriter
    {
        public ListCollection<VisualComponent> VisualComponents = new ListCollection<VisualComponent>();

        public VisualComponent AddVisualComponent(int tabs, ObjectSymbol obj, MethodSymbol mth, SignatureSymbol sig)
        {
            VisualComponent vis = new VisualComponent(obj, mth, sig);
            WriteVisualComponent(tabs, vis);
            VisualComponents.Add(vis);
            return vis;
        }

        public void WriteVisualComponent(int tabs, VisualComponent vis)
        {
            vis.Builder = PushBuilder();

            StringBuilder elementBuilder = PushBuilder();
            WriteVisualElement(tabs + 2, vis, null, vis.Signature);
            PopBuilder();

            WriteLine(tabs, "public class " + vis.IdentifierString+" : VisualObject");
            WriteLine(tabs, "{");
            WriteLine(tabs + 1, "public " + vis.Object.Signature.Identifier.String+ " Object;");
            WriteLine();
            WriteTab(tabs + 1);
            Write("public " + vis.IdentifierString + "("+vis.Object.Signature.Identifier.String+" Object");
            for(int i=0; i<vis.ParameterVariables.Size; i++)
            {
                Write(", ");
                TypeDeclarationSignature typeDec = vis.ParameterVariables[i];
                Write(typeDec.TypeIdentifier.String + " " + typeDec.NameIdentifier.String);
            }
            WriteLine(")");
            WriteLine(tabs + 1, "{");
            WriteLine(tabs + 2, "this.Object = Object;");
            WriteLine(tabs + 2, "Stack<VisualElement> stack = new Stack<VisualElement>();");
            WriteLine(tabs + 2, "VisualElement element, parent=null;");
            WriteLine();
            WriteTab(tabs + 2);
            WriteLine("this.Visual = ");
            Write(elementBuilder.ToString());
            WriteLine(tabs+  1, "}");
            WriteLine(tabs, "}");
            WriteLine();

            PopBuilder();
        }

        void WriteVisualElement(int tabs, VisualComponent vis, SignatureSymbol parent, SignatureSymbol sig)
        {
            StructedBlockSignature sb = (sig.Type == SignatureType.StructedBlockOperand ? (sig as StructedBlockOperand).StructedBlock : null);
            StatementSignature ss = sig as StatementSignature;
            if (sb != null && sb.OpenBlockIdentifiere.IsType(TokenType.Visual))
            {
                if(sb.OpenBlockIdentifiere.IsVisual(VisualType.Scroll))
                {
                    WriteLine(tabs, "element = new VisualScrollElement(parent);");
                }
                else
                {
                    WriteLine(tabs, "element = new VisualElement(" + sb.OpenBlockIdentifiere.Type + ", parent);");
                }
            }
            else if(ss != null)
            {
                ExpressionSignature exp = (ss.Type == StatementType.ExpressionStatement ? (ss as ExpressionStatementSignature).Expression : null);
                if(exp!=null)
                {
                    OperandSignature op = exp.Operand;
                    if (op.AccessList[0].Type == SignatureType.LiteralOperand)
                    {
                        string stringData = (op.AccessList[0] as LiteralOperand).Literal.String;
                        stringData = stringData.Replace("\"", "");
                        WriteLine(tabs, "element = new VisualTextElement(\"" + stringData + "\", parent);");
                    }
                    else if (op.AccessList[0].Type == SignatureType.StructedBlockOperand)
                    {
                        for (int b = 0; b < op.AccessList.Size; b++)
                        {
                            WriteVisualElement(tabs, vis, sig, op.AccessList[b] as StructedBlockOperand);
                        }
                    }
                    else if (exp.PreOperation != null && exp.PreOperation.Token.IsStructure(StructureType.Point) && op.AccessList[0].Type == SignatureType.VariableOperand)
                    {
                        string variableIdentifier = (op.AccessList[0] as VariableOperand).Identifier.String;
                        TypeDeclarationSignature variableDeclaration;
                        if ((variableDeclaration = GetObjectMemberTypeDeclaration(vis.Object, variableIdentifier)) != null)
                        {
                            WriteTab(tabs);
                            Write("element.AddChild(Object." + variableIdentifier);
                            for (int b = 1; b < op.AccessList.Size; b++)
                            {
                                Write(".");
                                variableIdentifier = (op.AccessList[b] as VariableOperand).Identifier.String;
                                Write(variableIdentifier);
                            }
                            WriteLine(".Visual);");
                        }
                        else if((variableDeclaration = GetMethodParameterTypeDeclaration(vis.Method, variableIdentifier)) != null)
                        {
                            WriteLine(tabs, "element = new VisualTextElement(" + variableIdentifier+", parent);");
                            vis.ParameterVariables.Add(variableDeclaration);
                        }
                    }
                }
                else
                {
                    WriteStatement(tabs, vis.Object, vis.Method, vis, ss);
                }
            }
            else
            {
                return;//throw new Exception("invalid state");
            }
            if (sb != null && sb.Attributes != null)
            {
                for (int i = 0; i < sb.Attributes.Size; i++)
                {
                    StructedAttributeSignature attribute = sb.Attributes[i];
                    string attr = attribute.Identifier.String;
                    if (attr == "width" || attr == "height")
                    {
                        string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                        Way way = Way.Try(value);
                        WriteLine(tabs, "element.Room."+Char.ToUpper(attr[0])+attr.Substring(1)+" = new Way("+(int)way.Type+", "+((way.way)+"").Replace(',', '.')+"f);");
                    }
                }
            }
            if (sb != null && sb.Elements != null && sb.Elements.Size > 0)
            {
                WriteLine(tabs, "stack.Push(parent);");
                WriteLine(tabs, "parent = element;");
                for(int i=0; i<sb.Elements.Size; i++)
                {
                    WriteVisualElement(tabs, vis, sb, sb.Elements[i]);
                }
                if(parent != null)
                    WriteLine(tabs, "parent = stack.Pop();");
            }
        }
    }
}
