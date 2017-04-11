using feltic.Language;
using feltic.UI;
using Scope;
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
        GlyphContainer GlyphContainer = new GlyphContainer(new Font("DroidSansMono.ttf"));
        public static VisualElement Root = new VisualElement(VisualType.Block, null);
        public static VisualElement Code = null;
        public CodeText CodeText;

        public SceneView()
        {
            //Editor editor = new Editor();
            FileExplorer fe = new FileExplorer();
            //AddCode(editor);
            Root.AddChild(fe.Root.Visual);
        }

        /*
        public void AddCode(Editor editor)
        {
            VisualElement block = new VisualElement(VisualType.Block, editor.workspace.Root.Visual);
            new VisualTextElement(File.ReadAllText("./Compose/four.src"), block, CodeColor.String);
        }
        */

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
            if(exp.Operand.AccessList.Size < 1 || (exp.Operand.AccessList[0].Type != SignatureType.StructedBlockOperand && exp.Operand.AccessList[0].Type != SignatureType.LiteralOperand))
            {
                return;
            }
            OperandSignature operandSignature = exp.Operand;
            for(int i=0; i<operandSignature.AccessList.Size; i++)
            {
                StructedBlockSignature structedSignature = (operandSignature.AccessList[i].Type == SignatureType.StructedBlockOperand ? (operandSignature.AccessList[i] as StructedBlockOperand).StructedBlock : null);
                if (structedSignature != null)
                {
                    if (!structedSignature.OpenBlockIdentifiere.IsType(TokenType.Visual))
                    {
                        return;
                    }
                    VisualType type = (VisualType)(structedSignature.OpenBlockIdentifiere as VisualSymbol).Type;
                    VisualElement element;
                    if (type == VisualType.Input)
                    {
                        element = new VisualInputElement(Parent);
                    }
                    else
                    {
                        if (type == VisualType.Text)
                        {
                            element = new VisualTextElement("text", Parent);
                        }
                        else if (type == VisualType.Scroll)
                        {
                            element = new VisualScrollElement(Parent);
                        }
                        else
                        {
                            element = new VisualElement(type, Parent);
                        }
                    }
                    if (structedSignature.Attributes != null)
                    {
                        for (int j = 0; j < structedSignature.Attributes.Size; j++)
                        {
                            StructedAttributeSignature attribute = structedSignature.Attributes[j];
                            string attrName = attribute.Identifier.String;
                            if (attrName == "width")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                element.Room.Width = Way.Try(value);
                            }
                            else if (attrName == "height")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                element.Room.Height = Way.Try(value);
                            }
                            else if (attrName == "color")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                value = value.Replace("\"", "");
                                element.Color = Color.Try(value);
                                int q = 0;
                            }
                            else if (attrName == "id")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                if (value.Replace("\"", "") == "code")
                                {
                                    CodeText.VisualCode = element;
                                    CodeText.CodeContainer.Build();
                                    CodeText.CodeInput = new CodeInput(CodeText, element);
                                }

                            }
                            else if(attrName == "padding" || attrName == "margin")
                            {
                                ObjectAccessOperand objectOperand = (attribute.AssigmentOperand.AccessList[0] as ObjectAccessOperand);
                                Spacing spacing = new Spacing();
                                spacing.Left = Way.Try((objectOperand.ParameterDefinition.Elements[0].Expression.Operand.AccessList[0] as LiteralOperand).Literal.String);
                                spacing.Top = Way.Try((objectOperand.ParameterDefinition.Elements[1].Expression.Operand.AccessList[0] as LiteralOperand).Literal.String);
                                spacing.Right = Way.Try((objectOperand.ParameterDefinition.Elements[2].Expression.Operand.AccessList[0] as LiteralOperand).Literal.String);
                                spacing.Bottom = Way.Try((objectOperand.ParameterDefinition.Elements[3].Expression.Operand.AccessList[0] as LiteralOperand).Literal.String);
                                if (attrName == "padding")
                                    element.Padding = spacing;
                                else if (attrName == "margin")
                                    element.Margin = spacing;
                            }
                        }
                    }
                    if (structedSignature.Elements != null)
                    {
                        for (int j = 0; j < structedSignature.Elements.Size; j++)
                        {
                            AddVisualElements(element, structedSignature.Elements[j]);
                        }
                    }
                }
                else if (operandSignature != null)
                {
                    string stringData = (operandSignature.AccessList[0] as LiteralOperand).Literal.String;
                    stringData = stringData.Replace("\"", "");
                    VisualTextElement element = new VisualTextElement(stringData, Parent);
                }

            }
        }

        public void Draw()
        {
            if(Root != null)
            {
                //CodeText.CodeSelection.Draw();
                Position offset = new Position(20, 20);
                Root.CalculateSizeAndPosition(offset);
                Root.Draw(offset.x, offset.y);
                //CodeText.CodeCursor.Draw();
            }
        }
    }
}
