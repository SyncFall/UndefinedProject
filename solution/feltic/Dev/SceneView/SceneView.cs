﻿using feltic.Language;
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
        Registry Registry;
        GlyphContainer GlyphContainer = new GlyphContainer(new Font("DroidSansMono.ttf"));
        public static VisualElement Root = new VisualElement(VisualType.Block, null);
        public static VisualElement Code = null;
        public CodeText CodeText;

        public SceneView()
        {
            SourceList list = new SourceList();
            list.Add(SourceText.FromFile("./Compose/second.src"));
            CodeText = new CodeText();
            this.Registry = new Registry();
            this.Registry.AddSourceList(list);
            SignatureSymbol signature = Registry.EntryList[0].SourceSymbol.ScopeList[0].VisualElement;
            if (signature != null)
            {
                AddVisualElements(Root, signature);
            }
            else
            {
                Editor editor = new Editor();
                Root.AddChild(editor.Root.Visual);
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
                            if (attribute.Identifier.String == "width")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                element.Room.Width = Way.Try(value);
                            }
                            else if (attribute.Identifier.String == "height")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                element.Room.Height = Way.Try(value);
                            }
                            else if (attribute.Identifier.String == "color")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                value = value.Replace("\"", "");
                                element.Color = Color.Try(value);
                                int q = 0;
                            }
                            if (attribute.Identifier.String == "id")
                            {
                                string value = (attribute.AssigmentOperand.AccessList[0] as LiteralOperand).Literal.String;
                                if (value.Replace("\"", "") == "code")
                                {
                                    CodeText.VisualCode = element;
                                    CodeText.CodeContainer.Build();
                                    CodeText.CodeInput = new CodeInput(CodeText, element);
                                }

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
                CodeText.CodeSelection.Draw();
                Root.Draw(10, 10);
                CodeText.CodeCursor.Draw();
            }
        }
    }


}
