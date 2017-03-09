using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using SharpFont;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace LinearCodes.Textured
{
    public class Texture2D
    {
        public int Id { get; }
        public PixelInternalFormat InternalFormat = PixelInternalFormat.Rgb;
        public PixelFormat ImageFormat = PixelFormat.Rgb;
        public int WrapS = (int)TextureWrapMode.Repeat;
        public int WrapT = (int)TextureWrapMode.Repeat;
        public int FilterMin = (int) TextureMinFilter.Linear;
        public int FilterMag = (int) TextureMagFilter.Linear;


        public Texture2D()
        {
            Id = GL.GenTexture();
        }

        public void Generate(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat, data.Width, data.Height, 0, ImageFormat, PixelType.UnsignedByte, data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, WrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, WrapT);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, FilterMin);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, FilterMag);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind() => GL.BindTexture(TextureTarget.Texture2D, Id);
    }
}
