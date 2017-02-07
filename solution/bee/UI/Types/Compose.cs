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

    public enum PathType
    {
        Line,
        Curve,
    }

    public class Surface : Compose
    {
        public float OffsetX = 50;
        public PathType SurfaceType;
        public CurvePath CurvePath = new CurvePath();
        private SurfaceTransform SurfaceTransform;

        public Surface(PathType Type) : base(ComposeType.Surface)
        {
            this.SurfaceType = Type;
            AddPath(Type);
        }

        public void AddPath(PathType Type)
        {
            Curve Curve=null;
            if (Type == PathType.Line)
            {
                Curve = new Curve(1);           
                Curve.Add(OffsetX, 50);
                Curve.Add(OffsetX+100, 50);
                Curve.Add(OffsetX+100, 100);
                Curve.Add(OffsetX, 100);
                Curve.Add(OffsetX, 50);
            }
            else if(Type == PathType.Curve)
            {
                Curve = new Curve(2);
                Curve.Add(OffsetX, 50);
                Curve.Add(OffsetX + 100, 50);
                Curve.Add(OffsetX + 100, 100);
                Curve.Add(OffsetX, 100);
                Curve.Add(OffsetX, 50);
            }
            OffsetX += 150;
            if(CurvePath.CurveNode == null)
            {
                CurvePath.CurveNode = Curve;
            }
            else
            {
                CurvePath.CurveNode.Next = Curve;
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
            Curve curveNode = CurvePath.CurveNode;
            while(curveNode != null)
            {
                curveNode.Draw(false);
                curveNode = curveNode.Next;
            }
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
