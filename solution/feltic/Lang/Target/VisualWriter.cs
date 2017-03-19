using feltic.Language;
using feltic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class TargetWriter
    {
        int VisualId = 0;

        public void WriteVisualComponent(int tabs, StructedBlockSignature sb)
        {
            StructedBlockSignature structedSignature = sb as StructedBlockSignature;
            if (structedSignature.OpenBlockIdentifiere.Type != TokenType.Visual)
            {
                return;
            }
            ++VisualId;
            WriteLine(tabs, "public class Visual" + VisualId);
            WriteLine(tabs, "{");
            WriteLine(tabs + 1, "public VisualElement Element;");
            WriteLine();
            WriteLine(tabs + 1, "public Visual" + VisualId + "()");
            WriteLine(tabs + 1, "{");
            WriteLine(tabs + 2, "Stack stack = new Stack();");
            WriteLine(tabs + 2, "VisualElement element, parent=null;");
            WriteLine();
            WriteTab(tabs + 2);
            Write("this.Element = ");
            WriteVisualElement(tabs + 2, null, sb);
            WriteLine(tabs+  1, "}");
            WriteLine(tabs, "}");
            WriteLine();
        }

        void WriteVisualElement(int tabs, SignatureSymbol parent, SignatureSymbol sig)
        {
            StructedBlockSignature sb = sig as StructedBlockSignature;
            OperandSignature os = sig as OperandSignature;
            if (sb != null && sb.OpenBlockIdentifiere.Type == TokenType.Visual)
            {
                WriteLine(tabs, "element = new VisualElement(VisualElementType." + (sb.OpenBlockIdentifiere.Symbol as VisualKeywordSymbol).Type + ", parent);");
            }
            else if(os != null)
            {
                string stringData = ((os as OperandSignature).AccessList[0] as LiteralAccessSignature).Literal.String;
                stringData = stringData.Replace("\"", "");
                WriteLine(tabs, "element = new VisualTextElement(\""+ stringData + "\", parent);");
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
                        WriteLine(tabs, "element.RoomFromDefinition."+Char.ToUpper(attr[0])+attr.Substring(1)+" = new Way(WayType."+way.Type+", "+((way.way)+"").Replace(',', '.')+"f);");
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
