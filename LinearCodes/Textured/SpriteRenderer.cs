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

        public SpriteRenderer()
        {
            int VBO;
            float[] vertices =
            {
                1f, 1f, 1f, 0f,
                1f, 0f, 1f, 1f,
                0f, 1f, 0f, 0f,
                0f, 0f, 0f, 1f,
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

        public void Draw()
        {
            GL.BindVertexArray(_quadVao);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }
    }
}
