using System;
using System.Collections.Generic;
using OpenTK;

namespace LinearCodes
{
    class TriangularField: DrawingVisual
    {
        
        private float _x;
        
        public float X
        {
            get { return _x; }
            set
            {
                if (_x == value) return;
                _x = value;
                ModelMatrixUpdate();
            }
        }

        private float _y;

        public float Y
        {
            get { return _y; }
            set
            {
                if (_y == value) return;
                _y = value;
                ModelMatrixUpdate();
            }
        }

        private float _angle;

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (_angle == value) return;
                _angle = value;
                ModelMatrixUpdate();
            }
        }

        private void ModelMatrixUpdate()
        {
            //ModelMatrix = Matrix4.CreateTranslation(_x, _y, 0)*Matrix4.CreateRotationZ(_angle);
        }


        public TriangularField(int size, Shader shader) : base(0, shader)
        {
            List<Vector4> vertices = new List<Vector4>();

            var width = size*20;
            var height = size*10;
            float sin = 2*(float)Math.Sin(Math.PI/3);

            for (int i = 0; i < size ; i++)
            {
                vertices.AddRange(Line(i*10, i*10*sin, width - i*10, i*10*sin,  2));
                vertices.AddRange(Line(i*20, 0, width/2 + i*10, (height - i*10)*sin,  2));
                vertices.AddRange(Line(width - i*20, 0, width/2 - i*10, (height - i*10)*sin, 2));
            }

            Shape = vertices.ToArray();

            InitBuffers();
            ModelMatrixUpdate();
        }
    }
}

