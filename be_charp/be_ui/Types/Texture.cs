using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public class TextureType
    {
        public int Id;
        public string Filepath;
        public Bitmap _Bitmap;
        public int Width;
        public int Height;

        public TextureType(string Filepath)
        {
            this.Filepath = Filepath;
        }

        public void Create()
        {
            _Bitmap = new Bitmap(Filepath);
            Width = _Bitmap.Width;
            Height = _Bitmap.Height;

            this.Id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, this.Id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData bitmapData = _Bitmap.LockBits(new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            _Bitmap.UnlockBits(bitmapData);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
