using feltic.Language;
using feltic.Library;
using feltic.UI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scope;
using feltic.UI.Types;

namespace feltic.Integrator
{
    public class SceneView
    {
        Registry Registry;
        GlyphContainer GlyphContainer = new GlyphContainer(new Font("DroidSansMono.ttf"));
        public static VisualElement Root = new VisualElement(VisualElementType.Compose, null);

        public SceneView()
        {
            SourceList list = new SourceList();
            list.Add(SourceText.FromFile("./Compose/Editor.src"));
            this.Registry = new Registry();
            this.Registry.AddSourceList(list);
            SignatureSymbol signature = Registry.EntryList[0].SourceSymbol.ScopeList[0].VisualElement;
            if (signature != null)
            {
                AddVisualElements(Root, signature);
            }

            VisualElementMetrics.GetSize(Root);
            VisualElementMetrics.GetPosition(Root, 10, 10);
        }

        public void AddVisualElements(VisualElement Parent, SignatureSymbol Signature)
        {
            if(Signature.Type != SignatureType.Statement)
            {
                return;
            }
            if((Signature as StatementSignature).Type != StatementType.ExpressionStatement)
            {
                return;
            }
            ExpressionSignature exp = (Signature as ExpressionStatementSignature).Expression;
            if(exp.Operand.AccessList.Size < 1 || (exp.Operand.AccessList[0].Type != SignatureType.StructedBlockAccess && exp.Operand.AccessList[0].Type != SignatureType.LiteralAccess))
            {
                return;
            }
            OperandSignature operandSignature = exp.Operand;
            StructedBlockSignature structedSignature = (operandSignature.AccessList[0].Type == SignatureType.StructedBlockAccess ? (operandSignature.AccessList[0] as StructedBlockAccessSignature).StructedBlock : null);
            if(structedSignature != null)
            {
                if(structedSignature.OpenBlockIdentifiere.Type != TokenType.Visual)
                {
                    return;
                }
                VisualElementType type = (structedSignature.OpenBlockIdentifiere.Symbol as VisualKeywordSymbol).Type;
                VisualElement element;
                if (type == VisualElementType.Input)
                {
                    element = new VisualInputElement(Parent);
                }
                else
                {
                    element = new VisualElement(type, Parent);
                }
                if(structedSignature.Attributes != null)
                {
                    for(int i=0; i<structedSignature.Attributes.Size; i++)
                    {
                        StructedAttributeSignature attribute = structedSignature.Attributes[i];
                        if(attribute.Identifier.String == "width")
                        {
                            string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                            element.RoomFromDefinition.Width = Way.Try(value);
                        }
                        else if (attribute.Identifier.String == "height")
                        {
                            string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                            element.RoomFromDefinition.Height = Way.Try(value);
                        }
                    }
                }
                if(structedSignature.ElementList != null)
                {
                    for (int i = 0; i < structedSignature.ElementList.Size; i++)
                    {
                        AddVisualElements(element, structedSignature.ElementList[i]);
                    }
                }
            }
            else if(operandSignature != null)
            {
                string stringData = (operandSignature.AccessList[0] as LiteralAccessSignature).Literal.String;
                stringData = stringData.Replace("\"", "");
                VisualTextElement element = new VisualTextElement(stringData, Parent);
                Parent.AddChild(element);
            }
        }

        public void Draw()
        {
            Root.Draw(10, 10);
        }
    }


    public class VisualInputElement : VisualElement
    {
        public string Text
        {
            get{
                if(Childrens!=null&&Childrens[0].Type == VisualElementType.Text)
                    return (Childrens[0] as VisualTextElement).Text;
                return "";
            }
            set{
                if (Childrens != null && Childrens[0].Type == VisualElementType.Text)
                    (Childrens[0] as VisualTextElement).Text = (value!=null?value :"");
            }
        }

        public VisualInputElement(VisualElement Parent) : base(VisualElementType.Input, Parent)
        {
            this.InputListener = new VisualTextInputListener(this);
        }

        public class VisualTextInputListener : VisualInputListener
        {
            public VisualTextInputListener(VisualInputElement Element) : base(Element)
            { }

            public override void Event(InputEvent Event)
            {
                if(!Active) return;
                if(Event.IsText)
                {
                    (Element as VisualInputElement).Text += Event.Text.TextContent;
                }
                else if(Event.IsKey && Event.Key.IsDown)
                {
                    if(Event.Key.Type == Key.BackSpace)
                    {
                        string text = (Element as VisualInputElement).Text;
                        if (text.Length > 0)
                        {
                            text = text.Substring(0, text.Length - 1);
                        }
                        (Element as VisualInputElement).Text = text;
                    }
                }
            }
        }
    }
}
