using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.Runtime.Types;
using OpenTK.Graphics.OpenGL;
using System.IO;
using Be.UI.Types;

namespace Be.UI
{
    public class FontMetric
    {
        public Font Font;
        public float WhiteSpaceHorizontalAdvance;
        public float TabSpaceHorizontalAdvance;
        public float GlyphVerticalAdvance;
        public float LineSpace;

        public FontMetric(Font Font)
        {
            this.Font = Font;
            this.Create();
        }

        private void Create()
        {
            Glyph spaceGlyph = Font.GetGlyph(' ');
            this.WhiteSpaceHorizontalAdvance = spaceGlyph.HoriziontalAdvance;
            this.TabSpaceHorizontalAdvance = (spaceGlyph.HoriziontalAdvance * 4);
            this.GlyphVerticalAdvance = spaceGlyph.VerticalAdvance;
            Glyph delimeterGlyph = Font.GetGlyph('|');
            this.LineSpace = (delimeterGlyph.Height - delimeterGlyph.VerticalAdvance);
        }
    }

    public class GlyphMetrics
    {
        public Font Font;
        public Glyph SpaceGlyph;
        public Glyph DelimeterGlyph;
        public int LeftSpace;
        public int TopSpace;
        public int SpaceWidth;
        public int TabWidth;
        public int VerticalAdvance;
        public int LineSpace;

        public GlyphMetrics(Font font, int topSpace, int leftSpace)
        {

        }
    }

    public class Font
    {
        public static readonly int DefaultFontSize = 11;
        public static readonly int DefaultDpi = 96;
        public static readonly float DefaultScale = 16;

        private static readonly int PointSize = 64;
        private static readonly int OnTag = 1;
        private static readonly int ConicTag = 0;
        private static readonly int CubicTag = 2;

        public string FilePath;
 
        public float Size = DefaultFontSize;
        public float Scale = DefaultScale;

        public FontMetric Metric;

        public float Ascent;
        public float Descent;
        public float Linegap;

        private static SharpFont.Library FTLibrary;
        private SharpFont.Face FTFace;
        private SharpFont.GlyphSlot FTGlyph;
        private SharpFont.Outline FTOutline;
        private SharpFont.FTVector[] FTPoints;
        private byte[] Tags;

        public Font(string FilePath)
        {
            this.Create(FilePath, DefaultFontSize, null);
        }

        public Font(string FilePath, int FontSize)
        {
            this.Create(FilePath, FontSize, null);
        }

        public Font(string FilePath, int FontSize, FontMetric FontMetric)
        {
            this.Create(FilePath, FontSize, FontMetric);
        }

        private void Create(string filePath, int fontSize, FontMetric fontMetric)
        {
            this.FilePath = filePath;
            this.Size = fontSize;
            this.Metric = fontMetric;

            if (FTLibrary == null)
            {
                FTLibrary = new SharpFont.Library();
            }
            FTFace = new SharpFont.Face(FTLibrary, FilePath);
            FTFace.SetCharSize(0, Size, 0, (uint)DefaultDpi);

            Ascent = FTFace.Ascender / (float)65536 * PointSize * Scale;
            Descent = FTFace.Descender / (float)65536 * PointSize * Scale;
            Linegap = (FTFace.GetSfntTable(SharpFont.TrueType.SfntTag.HorizontalHeader) as SharpFont.TrueType.HoriHeader).LineGap / (float)65536 * PointSize * Scale;

            if (this.Metric == null)
            {
                this.Metric = new FontMetric(this);
            }
        }
  
        private BeePoint GetOutlinePoint(Glyph glyph, int index)
        {
            return new BeePoint(
                FTPoints[index].X.Value / (float)65536 * PointSize * Scale, 
                glyph.Height - FTPoints[index].Y.Value / (float)65536 * PointSize * Scale
            );
        }

        private bool IsTag(int idx, int checkTag)
        {
            int tag = this.Tags[idx];
            if(tag <= 2)
            {
                return (tag == checkTag);
            }
            else
            {
                return (tag % 2 == checkTag);
            }
        }

        private void SetSegements(Glyph glyph, int start, int end)
        {
            int idx = start;
            while (idx < end)
            {
                // line-segement
                if( idx+1 <= end &&
                    IsTag(idx, OnTag) && IsTag(idx+1, OnTag)
                ){
                    LineCurve lineCurve = new LineCurve(BeeLineCurveType.Linear);
                    lineCurve.Anchor1 = GetOutlinePoint(glyph, idx);
                    lineCurve.Anchor2 = GetOutlinePoint(glyph, idx + 1);
                    glyph.Outlines.Add(lineCurve);
                    idx += 1;
                }
                // conic-segement
                else if(
                    idx+2 <= end &&
                    IsTag(idx, OnTag) && IsTag(idx+1, ConicTag) && IsTag(idx+2, OnTag)
                ){
                    LineCurve lineCurve = new LineCurve(BeeLineCurveType.Conic);
                    lineCurve.Anchor1 = GetOutlinePoint(glyph, idx);
                    lineCurve.Control1 = GetOutlinePoint(glyph, idx + 1);
                    lineCurve.Anchor2 = GetOutlinePoint(glyph, idx + 2);
                    glyph.Outlines.Add(lineCurve);
                    idx += 2;
                }
                // virtual-conic-segment
                else if(
                    idx+3 <= end &&
                    IsTag(idx, OnTag) && IsTag(idx+1, ConicTag) && IsTag(idx+2, ConicTag) && IsTag(idx+3, OnTag)
                ){
                    LineCurve lineCurveA = new LineCurve(BeeLineCurveType.Conic);
                    lineCurveA.Anchor1 = GetOutlinePoint(glyph, idx);
                    lineCurveA.Control1 = GetOutlinePoint(glyph, idx + 1);

                    LineCurve lineCurveB = new LineCurve(BeeLineCurveType.Conic);
                    lineCurveB.Control1 = GetOutlinePoint(glyph, idx + 2);
                    lineCurveB.Anchor2 = GetOutlinePoint(glyph, idx + 3);

                    BeePoint middlePoint = new BeePoint();
                    middlePoint.x = (lineCurveA.Control1.x + lineCurveB.Control1.x) / (float)2;
                    middlePoint.y = (lineCurveA.Control1.y + lineCurveB.Control1.y) / (float)2;

                    lineCurveA.Anchor2 = middlePoint;
                    lineCurveB.Anchor1 = middlePoint;

                    glyph.Outlines.Add(lineCurveA);
                    glyph.Outlines.Add(lineCurveB);
                    idx += 3;
                }
                // cubic-segment
                else if(
                    idx+3 <= end &&
                    IsTag(idx, OnTag) && IsTag(idx+1, CubicTag) && IsTag(idx+2, CubicTag) && IsTag(idx+3, OnTag))
                {
                    LineCurve lineCurve = new LineCurve(BeeLineCurveType.Cubic);
                    lineCurve.Anchor1 = GetOutlinePoint(glyph, idx);
                    lineCurve.Control1 = GetOutlinePoint(glyph, idx + 1);
                    lineCurve.Control2 = GetOutlinePoint(glyph, idx + 2);
                    lineCurve.Anchor2 = GetOutlinePoint(glyph, idx + 3);
                    glyph.Outlines.Add(lineCurve);
                    idx += 3;
                }
                else
                {
                    throw new Exception("invalid state");
                }
            }
        }

        public Glyph GetGlyph(char charCode)
        {
            // seek glyph
            uint glyphIndex = FTFace.GetCharIndex(charCode);
            FTFace.LoadGlyph(glyphIndex, SharpFont.LoadFlags.Render, SharpFont.LoadTarget.Light);
            FTGlyph = FTFace.Glyph;
            FTOutline = FTGlyph.Outline;

            Glyph glyph = new Glyph(this, charCode);
         
            // set metrics
            glyph.Width = FTGlyph.Metrics.Width.Value / (float)65536 * PointSize * Scale;
            glyph.Height = FTGlyph.Metrics.Height.Value / (float)65536 * PointSize * Scale;
            glyph.HoriziontalAdvance = FTGlyph.Metrics.HorizontalAdvance.Value / (float)65536 * PointSize * Scale;
            glyph.HoriziontalBearingX = FTGlyph.Metrics.HorizontalBearingX.Value / (float)65536 * PointSize * Scale;
            glyph.HoriziontalBearingY = FTGlyph.Metrics.HorizontalBearingY.Value / (float)65536 * PointSize * Scale;
            glyph.VerticalAdvance = FTGlyph.Metrics.VerticalAdvance.Value / (float)65536 * PointSize * Scale;
            glyph.VerticalBearingX = FTGlyph.Metrics.VerticalBearingX.Value / (float)65536 * PointSize * Scale;
            glyph.VerticalBearingY = FTGlyph.Metrics.VerticalBearingY.Value / (float)65536 * PointSize * Scale;

            // set anti-aliased gray-alpha-bitmap
            if (FTGlyph.Bitmap.Width > 0)
            {
                int idx;
                byte[] bitmap;
                // rgba
                /*
                bitmap = FTGlyph.Bitmap.BufferData;
                glyph.BitmapRgba = new byte[(FTGlyph.Bitmap.Width/3*4) * FTGlyph.Bitmap.Rows];
                idx = 0;
                int idxRgba = 0;
                for (int i = 0; i < FTGlyph.Bitmap.Rows; i++)
                {
                    for (int j = 0; j < FTGlyph.Bitmap.Width; j += 3)
                    {
                        glyph.BitmapRgba[idxRgba] = bitmap[idx];
                        glyph.BitmapRgba[idxRgba+1] = bitmap[idx+1];
                        glyph.BitmapRgba[idxRgba+2] = bitmap[idx+2];
                        if (bitmap[idx] == 0 && bitmap[idx+1] == 0 && bitmap[idx+2] == 0)
                        {
                            glyph.BitmapRgba[idxRgba + 3] = 0;
                        }
                        else
                        {
                            glyph.BitmapRgba[idxRgba + 3] = 255;
                        }
                        idx += 3;
                        idxRgba += 4;
                    }
                }
                */
                // gray
                bitmap = FTGlyph.Bitmap.BufferData;
                glyph.BitmapAlpha = new byte[2 * FTGlyph.Bitmap.Width * FTGlyph.Bitmap.Rows];
                idx = 0;
                int idxAlpha = 0;
                for(int i=0; i<FTGlyph.Bitmap.Rows; i++)
                {
                    for(int j=0; j<FTGlyph.Bitmap.Width; j++)
                    {
                        glyph.BitmapAlpha[idxAlpha] = (bitmap[idx] < 255 ? (byte)255 : bitmap[idx]);
                        glyph.BitmapAlpha[idxAlpha+1] = bitmap[idx] * 1.35 > 255 ? (byte)255 : (byte)(bitmap[idx] * 1.35);
                        idx++;
                        idxAlpha += 2;
                    }
                }
            }

            return glyph;

            // set outlines
            int segStart, segEnd=-1, start, end;
            for (int seg = 0; seg < FTOutline.ContoursCount; seg++)
            {
                start = segStart = segEnd + 1;
                end = segEnd = FTOutline.Contours[seg];

                // reset references
                Tags = FTOutline.Tags;
                FTPoints = FTOutline.Points;

                // shift ordered start
                while (!IsTag(start, OnTag))
                {
                    start++;
                }
                // shift ordered end
                while(!IsTag(end, OnTag))
                {
                    end--;
                }
                // get ordered points
                SetSegements(glyph, start, end);

                // unordered start/end points and closed contour
                int left = ((segEnd - segStart) - (end - start) + 2);
                Tags = new byte[left];
                FTPoints = new SharpFont.FTVector[left];
                int idx = 0;
                for (int i=end; i <= segEnd; i++, idx++)
                {
                    Tags[idx] = FTOutline.Tags[i];
                    FTPoints[idx] = FTOutline.Points[i];
                }
                for(int i=segStart; i <= start; i++, idx++)
                {
                    Tags[idx] = FTOutline.Tags[i];
                    FTPoints[idx] = FTOutline.Points[i];
                }
                // get points
                SetSegements(glyph, 0, idx-1);
            }

            return glyph;
        }
    }
}
