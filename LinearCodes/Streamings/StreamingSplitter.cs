using System;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Streamings
{


    public class StreamingSplitter: StreamingComponent
    {
        public float PointSize = 4.0f;

        public Glyph7x5 CloneBit;

        public Color4 Color
        {
            get { return InstasingList[0].Color; }
            set { InstasingList[0].Color = value; }
        }
        
        public StreamingSplitter(SimpleShader simpleShader) 
            : base(simpleShader,  1, 2)
        {
            Size = new Vector2(Delta*2,Delta*2);
            InstasingList.Add(new VisualUniforms(Color4.Black));
            Shape = Circle(new Vector2(Delta, Delta), 0, 20);
            CreateInput(0, ConnectorOrientation.Left, new Vector2(0,Delta));
            
            CreateOutput(0, ConnectorOrientation.Right, new Vector2(Delta*2,Delta));
            CreateOutput(1, ConnectorOrientation.Bottom, new Vector2(Delta, 0));
        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Translate;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Translate;
        }

        protected override void StartAnimation()
        {
            
            Bits[0].Animation("Translate",new Vector2(Delta, Delta), 100, () =>
            {
                CloneBit = new Glyph7x5(Bits[0].Char, Bits[0].Translate, SimpleShader);
                CloneBit.InstasingList[0].Color = Color4.Black;
                Childrens.Add(CloneBit);
                Bits[0].Animation("Translate", OutputConnectors[0].Translate, 100, () =>
                {
                    EndAnimation(Bits[0], 0);
                    Bits[0] = null;
                });

                CloneBit.Animation("Translate", OutputConnectors[1].Translate, 100, () =>
                {
                    EndAnimation(CloneBit, 1);
                    CloneBit = null;
                });
            });

            
        }


    }
}
