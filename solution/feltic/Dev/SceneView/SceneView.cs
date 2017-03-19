using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            list.Add(SourceText.FromFile("./Compose/first.src"));
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
                    if(type == VisualElementType.Text)
                    {
                        element = new VisualTextElement(File.ReadAllText("language.src"), Parent);
                    }
                    else if(type == VisualElementType.Scroll)
                    {
                        element = new VisualScrollElement(Parent);
                    }
                    else
                    {
                        element = new VisualElement(type, Parent);
                    }
                }
                if(structedSignature.Attributes != null)
                {
                    for(int i=0; i<structedSignature.Attributes.Size; i++)
                    {
                        StructedAttributeSignature attribute = structedSignature.Attributes[i];
                        if(attribute.Identifier.String == "width")
                        {
                            string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                            element.Room.Width = Way.Try(value);
                        }
                        else if (attribute.Identifier.String == "height")
                        {
                            string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                            element.Room.Height = Way.Try(value);
                        }
                        else if(attribute.Identifier.String == "color")
                        {
                            string value = (attribute.AssigmentOperand.AccessList[0] as LiteralAccessSignature).Literal.String;
                            value = value.Replace("\"", "");
                            element.Color = Color.Try(value);
                            int q = 0;
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


}
