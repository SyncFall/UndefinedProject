using Bee.Library;
using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public enum ComposeType
    {
        Text,
        Surface,
    }

    public abstract class Compose
    {
        public ComposeType ComposeType;
        private InputListener InputListener;

        public Compose(ComposeType Type)
        {
            this.ComposeType = Type;
        }

        public abstract Size Size
        {
            get;
        }

        public InputListener Input
        {
            get
            {
                return InputListener;
            }
            set
            {
                if(value != null)
                {
                    value.Sender = this;
                    this.InputListener = value;
                }
                else
                {
                    value.Dispose();
                    this.InputListener = null;
                }
            }
        }

        public abstract void Draw();
    }

    public class Surface : Compose
    {
        public float OffsetX = 100;
        public PathType SurfaceType;
        public CurvePath CurvePath = new CurvePath();

        public Surface() : base(ComposeType.Surface)
        { }

        public Curve AddPath(PathType Type, int Detail=25)
        {
            Curve Curve=null;
            if (Type == PathType.Line)
            {
                Curve = new Curve(Type, 1, 2);           
            }
            else if(Type == PathType.Quadratic)
            {
                Curve = new Curve(Type, 2, Detail);
            }
            else if (Type == PathType.Cubic)
            {
                Curve = new Curve(Type, 3, Detail);
            }
            else if(Type == PathType.Nurbs)
            {
                Curve = new Curve(Type, 2, Detail);
            }
            else
            {
                throw new Exception("invalid state");
            }
            Curve.BuildKnots();
            if(CurvePath.CurveNodeBegin == null)
            {
                CurvePath.CurveNodeBegin = Curve;
            }
            else
            {
                Curve node = CurvePath.CurveNodeBegin;
                while(true)
                {
                    if(node.Next != null)
                    {
                        node = node.Next;
                    }
                    else
                    {
                        node.Next = Curve;
                        break;
                    }
                }
            }
            return Curve;
        }

        public override Size Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Draw()
        {
            Curve curveNode = CurvePath.CurveNodeBegin;
            while(curveNode != null)
            {
                curveNode.Draw();
                curveNode = curveNode.Next;
            }
        }
    }
    
    public class Text : Compose
    {
        public string String;
        public GlyphContainer GlyphContainer;
        public TextFormat Format;

        public Text(string String, TextFormat Format) : base(ComposeType.Text)
        {
            this.String = String;
            this.Format = Format;
            this.GlyphContainer = new GlyphContainer(Format.Font);
        }

        public override Size Size
        {
            get
            {
                float width = 0f;
                float maxWidth = width;
                float totalHeight = 0f;
                if(String.Length > 0)
                {
                    totalHeight = GlyphContainer.Font.Metric.GlyphVerticalAdvance;
                }
                for (int i = 0; i < String.Length; i++)
                {
                    char textChar = String[i];
                    if (textChar == ' ')
                    {
                        width += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                    }
                    else if (textChar == '\t')
                    {
                        width += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                    }
                    else if (textChar == '\n')
                    {
                        if (width > maxWidth)
                        {
                            maxWidth = width;
                        }
                        totalHeight += (GlyphContainer.Font.Metric.GlyphVerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                    }
                    else
                    {
                        Glyph glyph = GlyphContainer.GetGlyph(String[i]);
                        width += glyph.HoriziontalAdvance;
                    }
                }
                return new Size(maxWidth, totalHeight);
            }
        }
       
        public override void Draw()
        {
            float currentX = 0;
            float currentY = 0;
            GL.Color3(Format.Color.GetGlColor().Rgb);
            for (int i = 0; i < String.Length; i++)
            {
                char textChar = String[i];
                if(textChar == ' ')
                {
                    currentX += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                }
                else if(textChar == '\t')
                {
                    currentX += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                }
                else if(textChar == '\n')
                {
                    currentY += (GlyphContainer.Font.Metric.GlyphVerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                    currentX = 0;
                }
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(textChar);
                    float glyphX = (currentX + glyph.HoriziontalBearingX);
                    float glyphY = (currentY + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    currentX += glyph.HoriziontalAdvance;
                }
            }
        }
    }

    public class TextFormat
    {
        public Font Font;
        public float Size;
        public Color Color;

        public TextFormat()
        {
            this.Font = new Font("DroidSansMono.ttf");
            this.Color = new Color(0, 100, 150);
        }
    }
}
