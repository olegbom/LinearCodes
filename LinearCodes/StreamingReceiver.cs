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
            Shape = Polyline(new[]
            {
                new Vector2(0,0),
                new Vector2(Delta*7, 0),
                new Vector2(Delta*7, Delta*2),
                new Vector2(0, Delta*2)
            }, 2);
            InitBuffers();
        }

        protected override void StartAnimation()
        {
            Bits[0].Animation("Position", Position + new Vector2(5,5), 500);

            BitMessage.Add(Bits[0]);
            Bits[0] = null;
            for (int i = 0; i < BitMessage.Count-1; i++)
            {
                BitMessage[i].Animation("Position",Position 
                    + new Vector2(Delta*(BitMessage.Count - 1 - i) + 5,5), 500);
            }
        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Position - new Vector2(Delta, 0);
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Position;
        }
    }
}
