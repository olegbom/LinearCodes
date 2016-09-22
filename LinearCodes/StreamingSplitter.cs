using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    enum ConnectorOrientation
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    public class StreamingSplitter: StreamingVisual
    {
        public float PointSize = 4.0f;
        public float Z = 0.0f;

        public Glyph7x5 CloneBit;

        public Color4 Color
        {
            get { return InstasingList[0].Color; }
            set { InstasingList[0].Color = value; }
        }

        public Vector2 Position
        {
            get { return InstasingList[0].Translate; }
            set { InstasingList[0].Translate = value; }
        }



        public StreamingSplitter(Shader shader, List<DrawingVisual> visuals) 
            : base(shader, visuals, 1, 2)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
            Shape = Circle(Vector2.Zero, PointSize, 20, Z);
            InitBuffers();
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

        protected override void StartAnimation()
        {
            CloneBit = new Glyph7x5(Bits[0].Char, Bits[0].Position, Shader);
            CloneBit.InstasingList[0].Color = Color4.Black;
            Visuals.Add(CloneBit);
          //  CloneBit.Animation("Position", Bit.Position - new Vector2(0,20),400, () =>
          // {

            EndAnimation(Bits[0], 0);
            Bits[0] = null;
            EndAnimation(CloneBit, 1);
            CloneBit = null;
            // });
        }


    }
}
