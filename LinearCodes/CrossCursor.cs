using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class CrossCursor: DrawingVisual
    {
        private List<Vector2> _verticesDefaultCursor { get; } = new List<Vector2>();
        private List<Vector2> _verticesWireCursor { get; } = new List<Vector2>(); 

        static readonly int Delta = 10;
   
        private bool _onPin;

        public bool OnPin
        {
            get { return _onPin; }
            set
            {
                if (_onPin == value) return;
                _onPin = value;
                Shape = value 
                    ? _verticesWireCursor.ToArray()
                    : _verticesDefaultCursor.ToArray();
                InstasingList[0].Animation("Color", value ? Color4.Red : Color4.Black, 200);
            }
        }
        
        public CrossCursor(SimpleShader simpleShader) : base(simpleShader)
        {
            InstasingList.Add(new VisualUniforms(Color.Black));

            int count = 4;
            for (int i = 0; i < count; ++i)
            {
                double arg = Math.PI * 2 * (i+0.5) / count;
                float cos = (float)Math.Cos(arg) * Delta / 2;
                float sin = (float)Math.Sin(arg) * Delta / 2;

                _verticesDefaultCursor.AddRange(Line(cos, sin, 0, 0, 2));
            }

            _verticesWireCursor.AddRange( new []
            {
                new Vector2(0,0),
                new Vector2(2,0),
                new Vector2(0,2),
                new Vector2(7,5),
                new Vector2(5,7),
                new Vector2(5,7),

                new Vector2(6,8),
                new Vector2(6,8),
                new Vector2(8,6),
                new Vector2(8,10), 
                new Vector2(10,8), 
            });



            Shape = _verticesDefaultCursor.ToArray();
        }
    }
}
