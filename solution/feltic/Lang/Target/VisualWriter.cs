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
        public ListCollection<VisualListenerDelegate> ListenerFunctions = new ListCollection<VisualListenerDelegate>();
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
            if(Method != null && Method.Signature.TypeIdentifier != null)
                str += "_"+Method.Signature.TypeIdentifier.String;
            this.IdentifierString = str;
        }
    }

    public class VisualListenerDelegate
    {
        public static int IdCounter = 0;
        public string IdentifierString;
        public VisualComponent Visual;
        public string MethodName;
        public StringBuilder Builder;

        public VisualListenerDelegate(VisualComponent Visual, string MethodName)
        {
            this.Visual = Visual;
            this.MethodName = MethodName;
            BuildIdentifier();
        }

        public void BuildIdentifier()
        {
            IdCounter++;
            string str = "Visual_Listener_" + IdCounter;
            if (Visual.Object != null)
                str += "_" + Visual.Object.Signature.Identifier.String;
            if (MethodName != null)
                str += "_" + MethodName;
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
            WriteLine(tabs + 2, "List<VisualElement> listeners = new List<VisualElement>();");
            WriteLine(tabs + 2, "VisualElement element, parent=null;");
            WriteLine();
            WriteTab(tabs + 2);
            WriteLine("this.Visual = ");
            Write(elementBuilder.ToString());
            WriteLine();
            for(int i=0; i<vis.ListenerFunctions.Size; i++)
            {
                WriteLine(tabs + 2, "new "+vis.ListenerFunctions[i].IdentifierString+"(Object, listeners["+i+"]);");
            }
            WriteLine(tabs+  1, "}");
            WriteLine(tabs, "}");
            WriteLine();

            for (int i=0; i<vis.ListenerFunctions.Size; i++)
            {
                WriteVisualListenerItem(tabs, vis.ListenerFunctions[i]);
                Write(vis.ListenerFunctions[i].Builder.ToString());
            }

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
                else if(sb.OpenBlockIdentifiere.IsVisual(VisualType.Image))
                {
                    WriteLine(tabs, "element = new VisualImageElement(parent);");
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
                        WriteLine(tabs, "element.Room."+char.ToUpper(attr[0])+attr.Substring(1)+" = new Way("+(int)way.Type+", "+((way.way)+"").Replace(',', '.')+"f);");
                    }
                    if (attr == "marginLeft")
                    {
                        string value = (attribute.AssigmentOperand.AccessList[0] as VariableOperand).Identifier.String;
                        WriteLine(tabs, "element.Margin = new Spacing(Object." + value + ", 0, 0, 0);");
                    }
                    else if (attr == "display")
                    {
                        string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                        WriteLine(tabs, "element.Display = " + bool.Parse(value).ToString().ToLower() + ";");
                    }
                    else if (attr == "listener")
                    {
                        string func = (attribute.AssigmentOperand.AccessList[0] as VariableOperand).Identifier.String;
                        vis.ListenerFunctions.Add(new VisualListenerDelegate(vis, func));
                        WriteLine(tabs, "listeners.Add(element);");
                    }
                    else if (attr == "source")
                    {
                        string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                        WriteLine(tabs, "element.source = " + value + ";");
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


        public void WriteVisualListenerItem(int tabs, VisualListenerDelegate vlstn)
        {
            vlstn.Builder = PushBuilder();

            WriteLine(tabs, "public class " + vlstn.IdentifierString + " : VisualListener");
            WriteLine(tabs, "{");
            WriteLine(tabs + 1, "public " + vlstn.Visual.Object.Signature.Identifier.String + " Object;");
            WriteLine();
            WriteLine(tabs + 1, "public " + vlstn.IdentifierString + "(" + vlstn.Visual.Object.Signature.Identifier.String + " Object, VisualElement Element) : base(Element)");
            WriteLine(tabs + 1, "{");
            WriteLine(tabs + 2, "this.Object = Object;");
            WriteLine(tabs + 2, "this.Element.InputListener = this;");
            WriteLine(tabs + 1, "}");
            WriteLine();
            WriteLine(tabs + 1, "public override void Event(InputEvent Event){");
            WriteLine(tabs + 2, "this.Object." + vlstn.MethodName + "(Event);");
            WriteLine(tabs + 1, "}");
            WriteLine(tabs, "}");

            PopBuilder();
        }
    }
}
