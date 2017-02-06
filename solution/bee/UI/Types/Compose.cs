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
                value.Sender = this;
                this.InputListener = value;
            }
        }

        public abstract void Draw();
    }

    public enum SurfaceType
    {
        Rect,
        Shape,
        Volume,
    }

    public class Surface : Compose
    {
        public SurfaceType SurfaceType;
        public Curve Curve;
        private SurfaceTransform SurfaceTransform;

        public Surface(SurfaceType Type) : base(ComposeType.Surface)
        {
            this.SurfaceType = Type;
            if(Type == SurfaceType.Rect)
            {
                Curve = new Curve(1);
                Curve.Add(10, 10);
                Curve.Add(100, 10);
                Curve.Add(100, 50);
                Curve.Add(10, 50);
                Curve.Add(10, 10);
            }
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
            Curve.Draw(false);
            if(SurfaceTransform != null)
            {
                SurfaceTransform.Draw();
            }
        }

        public bool Transform
        {
            get
            {
                return (SurfaceTransform != null); 
            }
            set
            {
                SurfaceTransform = new SurfaceTransform(this);
            }
        }
    }

    public class SurfaceTransform 
    {
        public Surface Surface;
        public InputListener InputListener;
        public TransformPointerCollection TransformPointers;

        public SurfaceTransform(Surface Surface)
        {
            this.Surface = Surface;
            this.InputListener = new TransformInput(this);
            this.TransformPointers = new TransformPointerCollection();
            this.TransformPointers.Add(new TransformPointer(Surface.Curve.Points.Get(0)));
            this.TransformPointers.Add(new TransformPointer(Surface.Curve.Points.Get(1)));
            this.TransformPointers.Add(new TransformPointer(Surface.Curve.Points.Get(2)));
            this.TransformPointers.Add(new TransformPointer(Surface.Curve.Points.Get(3)));
        }

        public class TransformInput : InputListener
        {
            public SurfaceTransform SurfaceTransform;

            public TransformInput(SurfaceTransform SurfaceTransform)
            {
                this.SurfaceTransform = SurfaceTransform;
            }

            public override void Input(InputEvent Event)
            {
               
            }
        }

        public void Draw()
        {
            GL.PointSize(15f);
            GL.Color3(System.Drawing.Color.Green);
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < TransformPointers.Size(); i++)
            {
                Point point = TransformPointers.Get(i).Point;
                GL.Vertex2(point.x, point.y);
            }
            GL.End();
        }
    }

    public class TransformPointerCollection : ListCollection<TransformPointer>
    { }

    public class TransformPointer
    {
        public static readonly int IntersectionMargin = 25;
        public Point Point;
        public bool IsPicked;

        public TransformPointer(Point Point)
        {
            this.Point = Point;
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
