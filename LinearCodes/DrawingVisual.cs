using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class DrawingVisual
    {
        public Vector4[] Shape { get; set; }
        public Shader Shader { get; }
        protected int vId, vao;

        public List<VisualUniforms> InstasingList = new List<VisualUniforms>();

        private Uniform<Matrix4> _modelMatrix;
        private Uniform<Color4> _uniformColor;

        public bool IsModelMatrixTranslate = false;
        public Matrix4 ModelMatrixTranslate = Matrix4.Identity;

        public DrawingVisual(int count, Shader shader)
        {
            Shape = new Vector4[count];
            Shader = shader;
            _modelMatrix = shader.GetUniformMatrix4("modelMatrix");
            _uniformColor = shader.GetUniformColor4("color");
        }

        public void InitBuffers()
        {
            vId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            var size = (IntPtr)(Vector4.SizeInBytes * Shape.Length);
            GL.BufferData(BufferTarget.ArrayBuffer, size, Shape, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.UseProgram(Shader.ProgrammId);

            var vertexPos = Shader.GetAttribLocation("position");

            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);

            GL.VertexAttribPointer(vertexPos, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, IntPtr.Zero);
            GL.EnableVertexAttribArray(vertexPos);
        }

        public void Bind()
        {
            GL.UseProgram(Shader.ProgrammId);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
        }

        public void Draw()
        {
            Bind();
            if(IsModelMatrixTranslate)
                foreach (var visualUniform in InstasingList)
                {
                    _modelMatrix.Value = ModelMatrixTranslate*visualUniform.ModelMatrix;
                    _uniformColor.Value = visualUniform.Color;
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Shape.Length);
                }
            else
                foreach (var visualUniform in InstasingList)
                {
                    _modelMatrix.Value = visualUniform.ModelMatrix;
                    _uniformColor.Value = visualUniform.Color;
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Shape.Length);
                }
            
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public static Vector4[] Line(float x1, float y1, float x2, float y2,
             float thickness = 1.0f, float z = 0.0f)
        {
            return Line(new Vector2(x1, y1), new Vector2(x2, y2), thickness, z);
        }

        public static Vector4[] Line(Vector2 v1, Vector2 v2, float thickness = 1.0f, float z = 0.0f)
        {
            var result = new Vector4[6];
            Vector2 vec = GetNormal(v1,v2);
            thickness /= 2;
            var v1l = v1 + vec * thickness;
            var v1r = v1 - vec * thickness;
            var v2l = v2 + vec * thickness;
            var v2r = v2 - vec * thickness;
            
            result[1] = result[0] = new Vector4(v1l.X, v1l.Y, z, 1);
            result[2] = new Vector4(v1r.X, v1r.Y, z, 1);
            result[3] = new Vector4(v2l.X, v2l.Y, z, 1);
            result[5] = result[4] = new Vector4(v2r.X, v2r.Y, z, 1);
             
            return result;
        }

        public static Vector4[] Rectangle(Vector2 v1, Vector2 v2, 
            float thickness = 1.0f, float z = 0.0f)
        {
            var vec1 = new Vector2(Math.Min(v1.X,v2.X), Math.Min(v1.Y,v2.Y));
            var vec2 = new Vector2(Math.Max(v1.X,v2.X), Math.Max(v1.Y,v2.Y));
            v1 = vec1;
            v2 = vec2;

            var result = new Vector4[12];


            result[9] = result[1] = result[0] = new Vector4(v1.X, v1.Y, z, 1);
            result[11] = result[10] = result[2] = result[1]
                + new Vector4(thickness, thickness, 0, 0);

            result[3] = new Vector4(v2.X, v1.Y, z, 1);
            result[4] = result[3] + new Vector4(-thickness, thickness, 0, 0);

            result[5] = new Vector4(v2.X, v2.Y, z, 1);
            result[6] = result[5] + new Vector4(-thickness, -thickness, 0, 0);

            result[7] = new Vector4(v1.X, v2.Y, z, 1);
            result[8] = result[7] + new Vector4(thickness, -thickness, 0, 0);

            return result;
        }
        
        public static Vector4[] Round(Vector2 center, float r,
             float thickness = 1.0f, int resolution = 30, float z = 0.0f)
        {
            if (resolution < 3) return new Vector4[0];

            var count = (resolution + 2) * 2;
            var result = new Vector4[count];
            var halfT = thickness / 2;

            var r0 = r - halfT;
            var r1 = r + halfT;
            var p0 = center + new Vector2(r1, 0);
            var p1 = center + new Vector2(r0, 0);

            var v0 = new Vector4(p0.X, p0.Y, z, 1);
            result[count - 3] = result[1] = result[0] = v0;
            var v1 = new Vector4(p1.X, p1.Y, z, 1);
            result[count - 1] = result[count - 2] = result[2] = v1;

            for (int i = 1; i < resolution; i++)
            {
                var arg = (float)i / resolution * Math.PI * 2;
                var sin = (float)Math.Sin(arg);
                var cos = (float)Math.Cos(arg);

                var point0 = center + new Vector2(r1 * cos, r1 * sin);
                var point1 = center + new Vector2(r0 * cos, r0 * sin);
                result[1 + i * 2] = new Vector4(point0.X, point0.Y, z, 1);
                result[2 + i * 2] = new Vector4(point1.X, point1.Y, z, 1);
            }

            return result;
        }


        public static Vector4[] Circle(Vector2 center, float r, 
            int resolution = 30, float z = 0.0f)
        {
            if (resolution < 3) return new Vector4[0];
            var count = resolution + 2;
            var result = new Vector4[count];

            var p0 = center + new Vector2(r, 0);
            var v0 = new Vector4(p0.X, p0.Y, z, 1);
                
            result[1] = result[0] = v0;

            for (int i = 2; i < count - 1; i += 2)
            {
                var arg = (float)i / resolution * Math.PI;
                var sin = (float)Math.Sin(arg);
                var cos = (float)Math.Cos(arg);
                var point0 = center + new Vector2(r * cos, r * sin);   
                result[i] = new Vector4(point0.X, point0.Y, z, 1);
            }

            for (int i = 3; i < count - 1; i += 2)
            {
                var arg = (double)(resolution - i + 1) / resolution * Math.PI + Math.PI;
                var sin = (float)Math.Sin(arg);
                var cos = (float)Math.Cos(arg);
                var point0 = center + new Vector2(r * cos, r * sin);
                result[i] = new Vector4(point0.X, point0.Y, z, 1);
            }
            result[count - 1] = result[count - 2];
            return result;
        }

        public static Vector4[] Polyline(IEnumerable<Vector2> points, float thickness = 1.0f, float z = 0.0f)
        {
            if(points == null) return new Vector4[0];
            Vector2[] pointsV2 = points as Vector2[] ?? points.ToArray();
            var count = pointsV2.Length;

            if (count < 2) return new Vector4[0];

            var rCount = count*2 + 2;
            var result = new Vector4[rCount];


            var polylineBegining = GetLineBegining(pointsV2[0], pointsV2[1], thickness);

            result[1] = result[0] = ToVector4(polylineBegining[0], z);
            result[2] = ToVector4(polylineBegining[1], z);

            for (int i = 1; i < count - 1; i++)
            {
                var polylineBending = GetLineBending(pointsV2[i-1], 
                    pointsV2[i], pointsV2[i+1], thickness);

                var rNum = i*2 + 1;

                result[rNum] = ToVector4(polylineBending[0], z);
                result[rNum + 1] = ToVector4(polylineBending[1], z);
            }


            var polylineEnding = GetLineEnding(pointsV2[count-2], pointsV2[count-1], thickness);
            result[rCount - 3] = ToVector4(polylineEnding[0], z);
            result[rCount - 1] = result[rCount - 2] = ToVector4(polylineEnding[1], z);

            return result;
        }

        public static Vector4[] GetFillRectangle(Vector2 p, float width, float height, float z = 0)
        {
            var result = new Vector4[6];
 
            result[1] = result[0] = new Vector4(p.X, p.Y, z, 1);
            result[2] = new Vector4(p.X + width, p.Y, z, 1);
            result[3] = new Vector4(p.X , p.Y + height, z, 1);
            result[5] = result[4] = new Vector4(p.X + width, p.Y + height, z, 1);

            return result;
        }

        public static Vector2[] GetLineBegining(Vector2 v1, Vector2 v2, float thickness)
        {
            var result = new Vector2[2];
            Vector2 vec = GetNormal(v1,v2);
            thickness /= 2;
            result[0] = v1 + vec * thickness;
            result[1] = v1 - vec * thickness;
            return result;
        }

        public static Vector2[] GetLineEnding(Vector2 v1, Vector2 v2, float thickness)
        {
            var result = new Vector2[2];
            Vector2 vec = GetNormal(v1, v2);
            thickness /= 2;
            result[0] = v2 + vec * thickness;
            result[1] = v2 - vec * thickness;
            return result;
        }

        public static Vector2[] GetLineBending(Vector2 v1, Vector2 v2, Vector2 v3, float thickness)
        {
            var result = new Vector2[2];
            Vector2 norm1 = GetNormal(v1, v2);
            Vector2 norm2 = GetNormal(v2, v3);
            Vector2 norm = norm2 + norm1;
            norm.Normalize();
            var angle = Math.Atan2(norm1.Y, norm1.X) - Math.Atan2(norm.Y, norm.X);
            norm = norm/(float)Math.Cos(angle);

            thickness /= 2;
            result[0] = v2 + norm * thickness;
            result[1] = v2 - norm * thickness;
            return result;
        }


        public static Vector4 ToVector4(Vector2 v, float z)
        {
            return new Vector4(v.X, v.Y, z, 1);
        }

        public static Vector2 GetNormal(Vector2 a, Vector2 b)
        {
            Vector2 vec = a - b;
            vec = new Vector2(vec.Y, -vec.X);
            vec.Normalize();
            return vec;
        }

    }


}
