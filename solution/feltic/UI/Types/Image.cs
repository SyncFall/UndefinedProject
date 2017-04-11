using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class Image
    {
        public readonly string Filepath;
        private byte[] BitmapRgbaBytes;
        private int BitmapBufferId;
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

        public void Draw(float X=0, float Y=0, float Width=0, float Height=0)
        {
            GL.Color3(1f, 1f, 1f);
            if(Width <= 0) Width = this.Size.Width;
            if(Height <= 0) Height = this.Size.Height;
            GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(X, Y);
            GL.TexCoord2(1, 0);
            GL.Vertex2(X + Width, Y);
            GL.TexCoord2(1, 1);
            GL.Vertex2(X + Width, Y + Height);
            GL.TexCoord2(0, 1);
            GL.Vertex2(X, Y + Height);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
