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
        private Matrix4 _modelMatrix4;


        public Sprite(Texture2D texture, SpriteRenderer renderer, TextureShader shader)
        {
            Texture2D = texture;
            Renderer = renderer;
            Shader = shader;
        }

        public void Draw()
        {
            Shader.Use();
            Shader.UniformModelMatrix.Value = _modelMatrix4;
            Shader.UniformTexturePos.Value = TexturePostiton;
            Shader.UniformColor.Value = Color;
            GL.ActiveTexture(TextureUnit.Texture0);
            Texture2D.Bind();
            Renderer.Draw();
        }


        public void UpdateModelMatrix()
        {
            

            Vector3 divSize = new Vector3(Size/2f);

            _modelMatrix4 = Matrix4.CreateScale(Size.X, Size.Y, 1f);

            _modelMatrix4 *= Matrix4.CreateTranslation(-divSize);
            _modelMatrix4 *= Matrix4.CreateRotationZ(Rotate);
            _modelMatrix4 *= Matrix4.CreateTranslation(divSize);
            _modelMatrix4 *= Matrix4.CreateTranslation(Position.X, Position.Y, 0);
        }
    }
}
