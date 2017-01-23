using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Integrator
{
    public enum CodeColorType
    {
        Normal,
        Keyword,
        Comment,
        Region,
        Error,
        String,
        None,
    }

    public class CodeColor
    {
        public static readonly float[] ColorNormal = new float[] { 220 / 255f, 220 / 255f, 220 / 255f };
        public static readonly float[] ColorKeyword = new float[] { 57 / 255f, 135 / 255f, 214 / 255f };
        public static readonly float[] ColorComment = new float[] { 87 / 255f, 166 / 255f, 74 / 255f };
        public static readonly float[] ColorRegion = new float[] { 155 / 255f, 155 / 255f, 155 / 255f };
        public static readonly float[] ColorError = new float[] { 162 / 255f, 49 / 255f, 44 / 255f };
        public static readonly float[] ColorString = new float[] { 214 / 255f, 134 / 255f, 75 / 255f };

        public CodeColorType CurrentColor = CodeColorType.None;

        public void Set(CodeColorType Color)
        {
            if (CurrentColor != Color)
            {
                switch(Color)
                {
                    case CodeColorType.Normal:
                        GL.Color3(ColorNormal);
                        break;
                    case CodeColorType.Keyword:
                        GL.Color3(ColorKeyword);
                        break;
                    case CodeColorType.Comment:
                        GL.Color3(ColorComment);
                        break;
                    case CodeColorType.Region:
                        GL.Color3(ColorRegion);
                        break;
                    case CodeColorType.Error:
                        GL.Color3(ColorError);
                        break;
                    case CodeColorType.String:
                        GL.Color3(ColorString);
                        break;
                    case CodeColorType.None:
                    default:
                        throw new Exception("invalid date");
                }
                CurrentColor = Color;
            }
        }
    }
}
