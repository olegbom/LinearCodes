using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class Wire: DrawingVisual
    {

        static readonly Color4 ZeroColor4 = Color4.Red;
        static readonly Color4 OneColor4 = new Color4(0, 0.8f, 0, 1);

        public event EventHandler ValueChaged;

        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
               
                if (_value == value) return;
                _value = value;
                this.Animation("Color", _value ? OneColor4 : ZeroColor4, 200, 
                    () => {
                        ValueChaged?.Invoke(this, new EventArgs());
                    });
            }
        }

        public Color4 Color
        {
            get { return InstasingList[0].Color; }
            set { InstasingList[0].Color = value; }
        }

        public float Thickness = 2.0f;
        public float PointRadius = 3.0f;
        public float Z = 0.0f;
        private List<Vector4> vertices = new List<Vector4>();

        public Wire(Shader shader) : base(0, shader)
        {
            InstasingList.Add(new VisualUniforms(ZeroColor4));    
        }

        public void AddLine(Vector2 v1, Vector2 v2)
        {
            vertices.AddRange(Line(v1, v2, Thickness, Z));
        }

        public void AddPolyline(Vector2[] points)
        {
            vertices.AddRange(Polyline(points, Thickness, Z));
        }

        public void AddPoint(Vector2 v1)
        {
            vertices.AddRange(Circle(v1, PointRadius, 30, Z));
        }

        public void AddPoint(Vector2 v1, float radius)
        {
            int resolution = (int)radius * 3;
            vertices.AddRange(Circle(v1, radius, resolution, Z));
        }

        public void CreateBuffer()
        {
            Shape = vertices.ToArray();
            InitBuffers();
        }

    }
}
