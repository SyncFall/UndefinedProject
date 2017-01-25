using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.Language;
using Bee.UI.Types;
using Bee.Integrator;
using OpenTK.Graphics.OpenGL;
using Bee.UI;

namespace Bee.Integrator
{
    public class CodeMessage
    {
        public static GlyphContainer GlyphContainer = new GlyphContainer(new Font(@"D:\dev\UndefinedProject\be-output\source-code-pro-regular.ttf", 11));
        public static int MessagePadding = 8;

        /*
        public static void Draw(TokenStatusSymbol TokenStatus, float X, float Y, CodeText CodeText)
        {
            float currentX = X + MessagePadding;
            float currentY = Y + MessagePadding;
            float maxX = 0;
            float verticalAdvance = 0;

            for (int i=0; i<TokenStatus.Message.Length; i++)
            {
                char charCode = TokenStatus.Message[i];
                Glyph glyph = GlyphContainer.GetGlyph(charCode);
                if (charCode == '\n')
                {
                    currentX = (X + MessagePadding);
                    currentY += (glyph.VerticalAdvance + 1.2f);
                }
                else
                {
                    currentX += glyph.HoriziontalAdvance;
                    if(currentX > maxX)
                    {
                        maxX = currentX;
                    }
                    verticalAdvance = glyph.VerticalAdvance;
                }
            }

            GL.Color3(66 / 255f, 66 / 255f, 69 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(X, Y);
            GL.Vertex2(maxX + MessagePadding, Y);
            GL.Vertex2(maxX + MessagePadding, currentY + verticalAdvance + MessagePadding + 4);
            GL.Vertex2(X, currentY + verticalAdvance + MessagePadding + 4);
            GL.End();

            GL.Color3(220 / 255f, 220 / 255f, 220 / 255f);
            currentX = X + MessagePadding;
            currentY = Y + MessagePadding;
            for (int i = 0; i < TokenStatus.Message.Length; i++)
            {
                char charCode = TokenStatus.Message[i];
                Glyph glyph = GlyphContainer.GetGlyph(charCode);
                if (charCode == '\n')
                {
                    currentX = (X + MessagePadding);
                    currentY += (glyph.VerticalAdvance * 1.2f);
                }
                else
                {
                    float glyphX = currentX + glyph.HoriziontalBearingX;
                    float glyphY = currentY + glyph.VerticalAdvance - glyph.HoriziontalBearingY;
                    glyph.Draw(glyphX, glyphY);
                    currentX += glyph.HoriziontalAdvance;
                }
            }
        }
        */
    }
}
