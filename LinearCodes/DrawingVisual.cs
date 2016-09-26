using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class DrawingVisual: VisualObject, IDisposable
    {
        public SimpleShader SimpleShader { get; }
        public int vId { get; }
        public int vao { get; }

        private Vector4[] _shape;

        public Vector4[] Shape
        {
            get{ return _shape;}
            set
            {
                _shape = value;
                GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
                var size = (IntPtr)(Vector4.SizeInBytes * _shape.Length);
                GL.BufferData(BufferTarget.ArrayBuffer, size, _shape, BufferUsageHint.StreamDraw);
            }
        }

        public bool IsVisible = true;

        public ObservableCollection<VisualUniforms> InstasingList { get; }= new ObservableCollection<VisualUniforms>();
        public ObservableCollection<DrawingVisual> Childrens { get; } = new ObservableCollection<DrawingVisual>();
        private Matrix4 _modelMatrix = Matrix4.Identity;
        public override Matrix4 ModelMatrix
        {
            get { return _modelMatrix; }
            protected set
            {
                _modelMatrix = value;
                foreach (var visualUniformse in InstasingList)
                    visualUniformse.ParentModelMatrix4 = _modelMatrix;
                
                foreach (var drawingVisual in Childrens)
                    drawingVisual.ParentModelMatrix4 = _modelMatrix;
                
            }
        }

        public DrawingVisual(SimpleShader simpleShader)
        {
            SimpleShader = simpleShader;
            InstasingList.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;
                foreach (VisualUniforms visualUniformse in e.NewItems)
                    visualUniformse.ParentModelMatrix4 = _modelMatrix;
            };

            Childrens.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;
                foreach (DrawingVisual drawingVisual in e.NewItems)
                    drawingVisual.ParentModelMatrix4 = _modelMatrix;
            };

            vId = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.UseProgram(SimpleShader.ProgramId);

            var vertexPos = SimpleShader.GetAttribLocation("position");
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            GL.VertexAttribPointer(vertexPos, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, IntPtr.Zero);
            GL.EnableVertexAttribArray(vertexPos);
        }



        // TODO : Должно частично обновлять данные
        public void UpdateData(int offset, Vector4[] newData)
        {
            if (offset + newData.Length <= _shape.Length)
            {
                for (int i = 0; i < newData.Length; i++)
                    _shape[i + offset] = newData[i];

                GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(Vector4.SizeInBytes* offset), Vector4.SizeInBytes * newData.Length, _shape);
            }
            else throw new Exception("Превышение размера массива Shape");
        }

        public void Bind()
        {
            GL.UseProgram(SimpleShader.ProgramId);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
        }

        public void Draw()
        {
            if (!IsVisible) return;

            Bind();
            
            foreach (var visualUniform in InstasingList)
            {
                SimpleShader.UniformModelMatrix.Value = visualUniform.ModelMatrix;
                SimpleShader.UniformColor.Value = visualUniform.Color;
                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, _shape.Length);
            }

            foreach (var children in Childrens)
                children.Draw();
            
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vId);
            GL.DeleteVertexArray(vao);
            _shape = null;
            InstasingList.Clear();
            foreach (var visuals in Childrens)
            {
                visuals.Dispose();
            }
            Childrens.Clear();
        }

        #region Static

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

        public static Vector4[] Sector(Vector2 center, float r, float angleStart, float angleStop,
            float thickness = 1.0f, int resolution = 30, float z = 0.0f)
        {
            if (resolution < 3) return new Vector4[0];

            var count = (resolution + 2) * 2;
            var result = new Vector4[count];
            var halfT = thickness / 2;

            var r0 = r - halfT;
            var r1 = r + halfT;

            for (int i = 0; i < resolution+1; i++)
            {
                var arg = (float)i / resolution * (angleStop - angleStart) + angleStart;
                var sin = (float)Math.Sin(arg);
                var cos = (float)Math.Cos(arg);

                var point0 = center + new Vector2(r1 * cos, r1 * sin);
                var point1 = center + new Vector2(r0 * cos, r0 * sin);
                result[1 + i * 2] = new Vector4(point0.X, point0.Y, z, 1);
                result[2 + i * 2] = new Vector4(point1.X, point1.Y, z, 1);
            }
            result[0] = result[1];
            result[count - 1] = result[count - 2];
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
        #endregion 
    }


}
