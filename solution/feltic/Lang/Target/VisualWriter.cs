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
        public string StringId = (IdCounter++) + "";
        public ObjectSymbol Object;
        public StructedBlockSignature Signature;

        public VisualComponent(ObjectSymbol Object, StructedBlockSignature Signature)
        {
            this.Object = Object;
            this.Signature = Signature;
        }
    }

    public partial class TargetWriter
    {
        ListCollection<VisualComponent> VisualComponents = new ListCollection<VisualComponent>();

        public VisualComponent AddVisualComponent(ObjectSymbol obj, StructedBlockSignature sb)
        {
            VisualComponent visual = new VisualComponent(obj, sb);
            VisualComponents.Add(visual);
            return visual;
        }

        public void WriteVisualComponent(int tabs, VisualComponent visual)
        {
            StructedBlockSignature structedSignature = visual.Signature;
            if (structedSignature.OpenBlockIdentifiere.Type != TokenType.Visual)
            {
                return;
            }
            WriteLine(tabs, "public class Visual" + visual.StringId+" : VisualObject");
            WriteLine(tabs, "{");
            WriteLine(tabs + 1, "public " + visual.Object.Signature.Identifier.String+ " Object;");
            WriteLine();
            WriteLine(tabs + 1, "public Visual" + visual.StringId + "("+visual.Object.Signature.Identifier.String+" Object"+")");
            WriteLine(tabs + 1, "{");
            WriteLine(tabs + 2, "this.Object = Object;");
            WriteLine(tabs + 2, "Stack stack = new Stack();");
            WriteLine(tabs + 2, "VisualElement element, parent=null;");
            WriteLine();
            WriteTab(tabs + 2);
            WriteLine("this.Visual = ");
            WriteVisualElement(tabs + 2, null, visual.Signature);
            WriteLine(tabs+  1, "}");
            WriteLine(tabs, "}");
            WriteLine();
        }

        void WriteVisualElement(int tabs, SignatureSymbol parent, SignatureSymbol sig)
        {
            StructedBlockSignature sb = sig as StructedBlockSignature;
            StatementSignature ss = sig as StatementSignature;
            if (sb != null && sb.OpenBlockIdentifiere.Type == TokenType.Visual)
            {
                if((sb.OpenBlockIdentifiere.Symbol as VisualKeywordSymbol).Type == VisualElementType.Scroll)
                {
                    WriteLine(tabs, "element = new VisualScrollElement(parent);");
                }
                else
                {
                    WriteLine(tabs, "element = new VisualElement(VisualElementType." + (sb.OpenBlockIdentifiere.Symbol as VisualKeywordSymbol).Type + ", parent);");
                }
            }
            else if(ss != null)
            {
                OperandSignature op = (ss as ExpressionStatementSignature).Expression.Operand;
                if (op.AccessList[0] is LiteralAccessSignature)
                {
                    string stringData = (op.AccessList[0] as LiteralAccessSignature).Literal.String;
                    stringData = stringData.Replace("\"", "");
                    WriteLine(tabs, "element = new VisualTextElement(\"" + stringData + "\", parent);");
                }
                else if(op.AccessList[0] is VariableAccessSignature)
                {
                    Write("element.AddChild(this.Object.");
                    for (int i = 0; i < op.AccessList.Size; i++)
                    {
                        Write((op.AccessList[i] as VariableAccessSignature).Identifier.String);
                        if (i < op.AccessList.Size - 1)
                            Write(".");
                    }
                    WriteLine(");");
                }
                else if(op.AccessList[0] is StructedBlockAccessSignature)
                {
                    WriteVisualElement(tabs, sig, (op.AccessList[0] as StructedBlockAccessSignature).StructedBlock);
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
                        string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                        Way way = Way.Try(value);
                        WriteLine(tabs, "element.Room."+Char.ToUpper(attr[0])+attr.Substring(1)+" = new Way(WayType."+way.Type+", "+((way.way)+"").Replace(',', '.')+"f);");
                    }
                }
            }
            if (sb != null && sb.ElementList != null && sb.ElementList.Size > 0)
            {
                WriteLine(tabs, "stack.Push(parent);");
                WriteLine(tabs, "parent = element;");
                for(int i=0; i<sb.ElementList.Size; i++)
                {
                    WriteVisualElement(tabs, sb, sb.ElementList[i]);
                }
                if(parent != null)
                    WriteLine(tabs, "parent = stack.Pop() as VisualElement;");
            }
        }
    }
}
