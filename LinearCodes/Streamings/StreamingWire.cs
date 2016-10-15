using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Streamings
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
                MovingWire.Shape = Polyline(path, Thickness);
                
            }
        }

        private List<Vector2> _path= new List<Vector2>();
        
        public List<Vector2> Path
        {
            get { return _path; }
            set
            {
                _path = value; 
                var width = _path.Max(v => v.X) - _path.Min(v => v.X) + Thickness + 2;
                var height = _path.Max(v => v.Y) - _path.Min(v => v.Y) + Thickness + 2;
                Size = new Vector2(width, height);
                Shape = Polyline(_path, Thickness + 2.0f);
                MovingWire.Shape = Polyline(_path, Thickness);
                OldMovingWire.Shape = MovingWire.Shape;
            }
        }

        private PathAnimation pathAnimation;

        public List<Vector2> AnimatedPath
        {
            set
            {
                pathAnimation.StartAnimation(value);
            }
        }

        public StreamingWire(SimpleShader simpleShader):
            base(simpleShader, 1, 1)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
            MovingWire = new DrawingVisual(SimpleShader);
            MovingWire.InstasingList.Add(new VisualUniforms(Color4.Red));
            OldMovingWire = new DrawingVisual(SimpleShader);
            OldMovingWire.InstasingList.Add(new VisualUniforms(Color4.Red));
            MovingCircle = new DrawingVisual(SimpleShader);
            MovingCircle.InstasingList.Add(new VisualUniforms(Color4.Black));
            MovingCircle.Scale = new Vector2(0,0);
            MovingCircle.Shape = Circle(Vector2.Zero, Thickness/2, 10);
            Childrens.Add(OldMovingWire);
            Childrens.Add(MovingWire);
            Childrens.Add(MovingCircle);

            pathAnimation = new PathAnimation(this);
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

        class PathAnimation
        {
            private List<Vector2> _oldList;
            private List<Vector2> _newList;
            private readonly StreamingWire _wire;

            private float _morfing;

            public float Morfing
            {
                get { return _morfing; }
                set
                {
                    _morfing = value;
                    
                    var list = new List<Vector2>(_oldList.Count);
                    for (int i = 0; i < _oldList.Count; i++)
                    {
                        list.Add(_oldList[i] + (_newList[i] - _oldList[i])*_morfing);
                    }
                    _wire.Path = list;
                }
            }

            public PathAnimation(StreamingWire wire)
            {
                _wire = wire;
            }

            public void StartAnimation(List<Vector2> newList)
            {
                _newList = newList;
                _oldList = _wire.Path;
                if (_oldList.Count < _newList.Count)
                {
                    for (int i = _oldList.Count; i < _newList.Count; i++)
                    {
                        _oldList.Add(_oldList.Last());
                    }
                    _wire.Path = _oldList;
                }
                else if (_oldList.Count > _newList.Count)
                {
                    _oldList.RemoveRange(_newList.Count-1, _oldList.Count - _newList.Count);
                    _wire.Path = _oldList;
                }
                
                this.Animation("Morfing", 1.0f, 50, () => _morfing = 0.0f);
            }
        }
    }
}
