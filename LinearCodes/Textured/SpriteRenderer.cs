using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes.Textured
{
    public class SpriteRenderer
    {
        private int _quadVao;

        private TextureShader _shader;



        public SpriteRenderer(TextureShader shader)
        {
            _shader = shader;
        }



        public void InitRenderData()
        {
            int VBO;
            float[] vertices =
            {
                0f, 1f, 0f, 1f,
                1f, 0f, 1f, 0f,
                0f, 0f, 0f, 0f,

                0f, 1f, 0f, 1f,
                1f, 1f, 1f, 1f,
                1f, 0f, 1f, 0f
            };

            _quadVao = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,(IntPtr) (vertices.Length*sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_quadVao);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4*sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
