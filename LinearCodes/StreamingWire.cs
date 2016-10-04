using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class StreamingWire: StreamingVisual
    {
        public float Thickness = 4.0f;
        public float Z = 0.0f;
        
        private DrawingVisual MovingWire;
        private DrawingVisual OldMovingWire;
        private DrawingVisual MovingCircle;

        private int _segment;
        private float _wireLoadTime;
        public float WireLoadTime
        {
            get { return _wireLoadTime; }
            set
            {
                _wireLoadTime = value;
                var path = new List<Vector2>();
                for (int i = 0; i < _segment; i++)
                    path.Add(Path[i]);
                var vector = (Path[_segment] - Path[_segment - 1])*value;
                var currentPos = Path[_segment - 1] + vector;
                path.Add(currentPos);
                MovingCircle.Translate = currentPos;
                MovingWire.Shape = Polyline(path, Thickness, 0.1f);
            }
        }

        private List<Vector2> Path = new List<Vector2>();

        public StreamingWire(SimpleShader simpleShader):
            base(simpleShader, 1, 1)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public void CreateBuffers(IEnumerable<Vector2> path)
        {


            Path = path as List<Vector2> ?? path.ToList();
            var width = Path.Max(v => v.X) - Path.Min(v => v.X) + Thickness + 2;
            var height = Path.Max(v => v.Y) - Path.Min(v => v.Y) + Thickness + 2;
            Size = new Vector2(width, height);

            Shape = Polyline(path, Thickness+2.0f);
            
            MovingWire = new DrawingVisual(SimpleShader);
            MovingWire.InstasingList.Add(new VisualUniforms(Color4.Red));
            MovingWire.Shape = Polyline(path, Thickness, 0.1f);
            OldMovingWire = new DrawingVisual(SimpleShader);
            OldMovingWire.InstasingList.Add(new VisualUniforms(Color4.Red));
            OldMovingWire.Shape = MovingWire.Shape;
            MovingCircle = new DrawingVisual(SimpleShader);
            MovingCircle.InstasingList.Add(new VisualUniforms(Color4.Black));
            MovingCircle.Scale = new Vector2(0,0);
            MovingCircle.Shape = Circle(Vector2.Zero, Thickness/2, 10);
            
            
            Childrens.Add(OldMovingWire);
            Childrens.Add(MovingWire);
            Childrens.Add(MovingCircle);
        }

        protected override void StartAnimation()
        {
            MovingAnimation(1);
            _segment = 1;
            WireLoadTime = 0f;
            MovingCircle.Scale = new Vector2(1, 1);
            MovingWire.InstasingList[0].Color = Bits[0].Char == '0'? Color4.Red: new Color4(0,0.8f,0,1);
        }

        protected void MovingAnimation(int segment)
        {
            if (segment >= Path.Count)
            {
                EndAnimation(Bits[0], 0);
                Bits[0] = null;
                OldMovingWire.InstasingList[0].Color = MovingWire.InstasingList[0].Color;
                MovingCircle.Scale = new Vector2(0, 0);
                return;
            }
            var lenght = (Path[segment] - Path[segment - 1]).Length*10;
            _segment = segment;
            Bits[0].Animation("Translate", 
                Path[segment] + new Vector2(4, 2), 
                (uint)lenght, 
                () => { MovingAnimation(segment + 1); } );
            
            this.Animation("WireLoadTime", 1.0f, (uint)lenght, () =>
            {
                _wireLoadTime = 0f;
            });
        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Path.First();
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Path.Last();
        }
    }
}
