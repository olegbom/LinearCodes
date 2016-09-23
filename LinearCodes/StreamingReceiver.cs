using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class StreamingReceiver: StreamingVisual
    {
        public List<Glyph7x5> BitMessage { get; } = new List<Glyph7x5>();
        public Vector2 Position
        {
            get { return InstasingList[0].Translate; }
            set { InstasingList[0].Translate = value; }
        }
        public float Delta = 20;

        public StreamingReceiver(Shader shader, List<DrawingVisual> visuals) : base(shader, visuals, 1, 0)
        {

            InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public void CreateBuffers(Vector2 position)
        {
            Position = position;
            var vertices = new List<Vector4>();
            vertices.AddRange(Polyline(new[]
            {
                new Vector2(0, 0),
                new Vector2(Delta*8, 0),
                new Vector2(Delta*8, Delta*2),
                new Vector2(Delta, Delta*2)
            }, 2));
            vertices.AddRange(Circle(new Vector2(0,0),3, 15,0.2f));
            Shape = vertices.ToArray();
            InitBuffers();
        }

        protected override void StartAnimation()
        {
            Bits[0].Animation("Position", Position + new Vector2(5+Delta,5), 200);

            BitMessage.Add(Bits[0]);
            Bits[0] = null;
            for (int i = 0; i < BitMessage.Count-1; i++)
            {
                BitMessage[i].Animation("Position",Position 
                    + new Vector2(Delta*(BitMessage.Count - i) + 5,5), 200);
                
            }
        }


        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Position;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Position;
        }
    }
}
