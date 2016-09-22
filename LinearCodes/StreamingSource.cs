using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class StreamingSource: StreamingVisual
    {
        public List<int> message { get; } = new List<int>();
        public Vector2 Position
        {
            get { return InstasingList[0].Translate; }
            set { InstasingList[0].Translate = value; }
        }

        public float Delta = 20;

        public StreamingSource(Shader shader, List<DrawingVisual> visuals) 
            : base(shader, visuals, 0, 1)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public List<Glyph7x5> BitMessage { get; } = new List<Glyph7x5>();

        public void CreateBuffers(Vector2 position)
        {
            Position = position;
            for (int i = 0; i < message.Count; i++)
            {
                var bit = message[i];
                var glyph = new Glyph7x5(bit == 0?'0':'1',
                    new Vector2(position.X + Delta * i, position.Y + Delta * 0.4f),
                    Shader);
                BitMessage.Add(glyph);
                Visuals.Add(glyph);
            }
            Shape = Polyline(new[]
            {
                new Vector2(Delta*message.Count, 0),
                new Vector2(0,0),
                new Vector2(0, Delta*2),
                new Vector2(Delta*message.Count, Delta*2)
            }, 2);
            InitBuffers();
        }

        protected override void StartAnimation()
        {
            for (int i = 0; i < BitMessage.Count-1; i++)
            {
                BitMessage[i].Animation("Position", BitMessage[i+1].Position, 500);
            }
            var last = BitMessage.Last();
            last.Animation("Position", 
                BitMessage.Last().Position + new Vector2(Delta,0),500,() =>
                {
                    EndAnimation(last, 0);
                    BitMessage.Remove(last);
                    
                    var glyph = new Glyph7x5( '0',
                        new Vector2(Position.X, Position.Y + Delta * 0.4f),
                        Shader);
                    BitMessage.Insert(0, glyph);
                    Visuals.Add(glyph);
                });
        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Position;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Position + new Vector2(Delta * (message.Count + 1), 0);
        }
    }
}
