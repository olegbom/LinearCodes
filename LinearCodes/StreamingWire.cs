using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{

    
    public class StreamingWire: StreamingVisual
    {
        public float Thickness = 4.0f;
        public float Z = 0.0f;
        
        private List<Vector2> Path = new List<Vector2>(); 
        
        public StreamingWire(Shader shader, List<DrawingVisual> visuals):
            base(shader,visuals, 1, 1)
        {
           InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public void CreateBuffers(IEnumerable<Vector2> path)
        {
            Path = path as List<Vector2> ?? path.ToList();
            Shape = Polyline(path, Thickness, Z);
            InitBuffers();
        }

        protected override void StartAnimation()
        {
            MovingAnimation(1);
        }

        protected void MovingAnimation(int segment)
        {
            if (segment >= Path.Count)
            {
                EndAnimation(Bits[0], 0);
                Bits[0] = null;
                return;
            }
            var lenght = (Path[segment] - Path[segment - 1]).Length*10;
            Bits[0].Animation("Position", 
                Path[segment] + new Vector2(5, 5), 
                (uint)lenght, 
                () => { MovingAnimation(segment + 1); } );
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
