using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class CrossCursor: DrawingVisual
    {
        private int Delta { get; } = 10;

        private bool _onPin;

        public bool OnPin
        {
            get { return _onPin; }
            set
            {
                if (_onPin == value) return;
                _onPin = value;
                InstasingList[0].Animation("Color", value ? Color4.Red : Color4.Black, 200);
            }
        }



        public CrossCursor(SimpleShader simpleShader) : base(simpleShader)
        {
            InstasingList.Add(new VisualUniforms(Color.Black));
            var vertices = new List<Vector2>();
            int count = 4;
            for (int i = 0; i < count; ++i)
            {
                double arg = Math.PI * 2 * (i+0.5) / count;
                float cos = (float)Math.Cos(arg) * Delta / 2;
                float sin = (float)Math.Sin(arg) * Delta / 2;

                vertices.AddRange(Line(cos, sin, 0, 0, 2));
            }

            Shape = vertices.ToArray();
        }
    }
}
