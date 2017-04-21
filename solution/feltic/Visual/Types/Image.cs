using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
{
    public class Image
    {
        public readonly string Filepath;
        public byte[] BitmapRgbaBytes;
        public int BitmapBufferId;
        public Size Size;

        public Image(string ImagePath)
        {
            this.Filepath = ImagePath;
            ReadFile();
            UploadTexture();
        }

        public void ReadFile()
        {
            Bitmap bitmap = new Bitmap(Filepath);
            this.Size = new Size(bitmap.Width, bitmap.Height);
            this.BitmapRgbaBytes = BitmapToByteArray(bitmap);
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;
            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;
                Marshal.Copy(ptr, bytedata, 0, numbytes);
                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }
        }

        public void UploadTexture()
        {
            BitmapBufferId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)Size.Width, (int)Size.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Utils.ByteArrayToIntPtr(BitmapRgbaBytes));
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Draw(Position Position, Size Size, Position Offset=null, Size Clip=null)
        {
            float texLeft = (Offset != null ? (Offset.X / (Size.Width + Offset.X)) : 0f);
            float texTop = (Offset != null ? (Offset.Y / (Size.Height + Offset.Y)) : 0f);
            float texWidth = (Clip != null && Clip.Width > 0f ? 1f - (Clip.Width / (Size.Width + Clip.Width)) : 1f);
            float texHeight = (Clip != null && Clip.Height > 0f ? 1f - (Clip.Height / (Size.Height + Clip.Height)) : 1f);

            GL.Color3(1f, 1f, 1f);
            GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texLeft, texTop);
            GL.Vertex2(Position.X, Position.Y);
            GL.TexCoord2(texWidth, texTop);
            GL.Vertex2(Position.X + Size.Width, Position.Y);
            GL.TexCoord2(texWidth, texHeight);
            GL.Vertex2(Position.X + Size.Width, Position.Y + Size.Height);
            GL.TexCoord2(texLeft, texHeight);
            GL.Vertex2(Position.X, Position.Y + Size.Height);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

    }
}
