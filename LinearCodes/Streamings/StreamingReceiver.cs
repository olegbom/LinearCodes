using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Streamings
{
    public class StreamingReceiver: StreamingComponent
    {
        public List<Glyph7x5> BitMessage { get; } = new List<Glyph7x5>();

        public StreamingReceiver(SimpleShader simpleShader) : base(simpleShader,  1, 0)
        {
            Size = new Vector2(Delta*8, Delta*2);
            InstasingList.Add(new VisualUniforms(Color4.Black));

            var vertices = new List<Vector2>();
            vertices.AddRange(Polyline(new[]
            {
                new Vector2(Delta, 0),
                new Vector2(Delta*8, 0),
                new Vector2(Delta*8, Delta*2),
                new Vector2(Delta, Delta*2)
            }, 2));

            CreateInput(0,ConnectorOrientation.Left, new Vector2(0,0));
            Shape = vertices.ToArray();
        }

        protected override void StartAnimation()
        {
            Bits[0].Animation("Translate", new Vector2(2+Delta,2), 200);

            BitMessage.Add(Bits[0]);
            Bits[0] = null;
            for (int i = 0; i < BitMessage.Count-1; i++)
            {
                BitMessage[i].Animation("Translate", new Vector2(Delta*(BitMessage.Count - i) + 2,2), 200);
            }
        }
        
        
    }
}
