using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes.Textured
{
    public class Sprite
    {
        public Texture2D Texture2D { get; }
        public SpriteRenderer Renderer { get; }
        public TextureShader Shader { get; }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1,1);
        public float Rotate { get; set; }
        public Color4 Color { get; set; } = Color4.White;
        public Vector4 TexturePostiton { get; set; }

        public Sprite(Texture2D texture, SpriteRenderer renderer, TextureShader shader)
        {
            Texture2D = texture;
            Renderer = renderer;
            Shader = shader;
        }

        public void Draw()
        {
            Shader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            Texture2D.Bind();
            Renderer.Draw();
        }

        public void UpdateUniforms()
        {
            Matrix4 model = Matrix4.CreateTranslation(Position.X, Position.Y, 0);

            Vector3 divSize = new Vector3(Size/2f);

            
            model *= Matrix4.CreateRotationZ(Rotate);
            
            model *= Matrix4.CreateScale(Size.X, Size.Y, 1f);

            Shader.UniformModelMatrix.Value = model;
            Shader.UniformTexturePos.Value = TexturePostiton;
            Shader.UniformColor.Value = Color;

        }
    }
}
