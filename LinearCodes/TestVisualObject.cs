using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    class TestVisualObject:VisualUniforms
    {
        public SimpleShader SimpleShader;
        public int vId { get; }
        public int vao { get; }

        public const int Count = 100000;
        public Vector2[] Shape = new Vector2[Count];

        public TestVisualObject(SimpleShader simpleShader, Color4 color) : base(color)
        {
            SimpleShader = simpleShader;
           

            vId = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.UseProgram(SimpleShader.ProgramId);

            var vertexPos = SimpleShader.GetAttribLocation("position");
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.VertexAttribPointer(vertexPos, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, IntPtr.Zero);
            GL.EnableVertexAttribArray(vertexPos);
        }

        public void Draw()
        {
            GL.UseProgram(SimpleShader.ProgramId);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.MultiTexCoord4(TextureUnit.Texture0, Color.R, Color.G, Color.B, Color.A);
            SimpleShader.UniformModelMatrix.Value = ModelMatrix;
            //SimpleShader.UniformColor.Value = visualUniform.Color;
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Shape.Length);
        }
    }
}
