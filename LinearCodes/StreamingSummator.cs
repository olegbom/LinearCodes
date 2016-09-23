using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class StreamingSummator: StreamingVisual
    {
        public Vector2 Position
        {
            get { return InstasingList[0].Translate; }
            set { InstasingList[0].Translate = value;}   
        }

        public float Delta;
        public bool Up = true;
        public bool Down = false;

        private Glyph7x5 GlyphPlus;
        private Glyph7x5 GlyphEqual;
        private Glyph7x5 GlyphResult;

        public StreamingSummator(Shader shader, List<DrawingVisual> visuals, int inCount) : base(shader, visuals, inCount, 1)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public void CreateBuffer(Vector2 position)
        {
            Position = position;
            
            var vertices = new List<Vector4>();

            var center = new Vector2(Delta * 2,Delta * 2);
            var round = Round(center, Delta, 2, 60, 0.1f);

            vertices.AddRange(round);
            var l = new Vector2(Delta * 1.5f, Delta * 2);
            var r = new Vector2(Delta * 2.5f, Delta * 2);
            vertices.AddRange(Line(l, r, 2));
            vertices.AddRange(
                Line(r + new Vector2(Delta*0.5f, 0),
                     r + new Vector2(Delta*1.5f, 0), 2));
            vertices.AddRange(Circle(
                     r + new Vector2(Delta * 1.5f, 0),
                   3, 12, 0.1f));
            vertices.AddRange(
                Line(l - new Vector2(Delta * 0.5f, 0),
                     l - new Vector2(Delta * 1.5f, 0), 2));
            vertices.AddRange(Circle(
                     l - new Vector2(Delta * 1.5f, 0),
                   3, 12, 0.1f));

            vertices.AddRange(Polyline(new[]
            {
                new Vector2(Delta/2, Delta*2.3f),
                new Vector2(Delta,   Delta*2),
                new Vector2(Delta/2, Delta*1.7f),
            }, 2, 0.1f));




            var u = new Vector2(Delta * 2, Delta * 2.5f);
            var d = new Vector2(Delta * 2, Delta * 1.5f);
            vertices.AddRange(Line(u, d, 2));
            if (Up)
            {
                vertices.AddRange(Polyline(new[]
                {
                    new Vector2(Delta*1.7f, Delta*3.5f),
                    new Vector2(Delta*2,    Delta*3),
                    new Vector2(Delta*2.3f, Delta*3.5f)
                }, 2, 0.1f));
                vertices.AddRange(
                    Line(new Vector2(Delta * 2, Delta * 3),
                         new Vector2(Delta * 2, Delta * 4), 2));
                vertices.AddRange(Circle(
                    new Vector2(Delta * 2f,Delta * 4.0f),
                    3, 12, 0.1f));
            }

            if (Down)
            {
                vertices.AddRange(Polyline(new[]
                {
                    new Vector2(Delta*1.7f, Delta*0.5f),
                    new Vector2(Delta*2,    Delta*1),
                    new Vector2(Delta*2.3f, Delta*0.5f)
                }, 2, 0.1f));
                vertices.AddRange(
                    Line(new Vector2(Delta * 2, Delta * 1),
                         new Vector2(Delta * 2, 0        ), 2));
                vertices.AddRange(Circle(
                    new Vector2(Delta * 2f, 0),
                    3, 12, 0.1f));
            }


            Shape = vertices.ToArray();
            InitBuffers();


           

            GlyphPlus = new Glyph7x5(' ',Position + new Vector2(Delta*1, -5), Shader);
            Visuals.Add(GlyphPlus);

            GlyphEqual = new Glyph7x5(' ', Position + new Vector2(Delta * 3, -5), Shader);
            Visuals.Add(GlyphEqual);
            
        }

        protected override void StartAnimation()
        {
            GlyphResult = new Glyph7x5(' ', Position + new Vector2(Delta * 4, -5),Shader);
            Visuals.Add(GlyphResult);

            Bits[0].Animation("Position",Position - new Vector2(0, 5), 500, () =>
            {
                GlyphResult.Char = Bits.Count(x => x.Char == '1')%2 == 1 ? '1' : '0';
                GlyphResult.Animation("Position", GlyphResult.Position, 400, () =>
                {
                    GlyphPlus.Char = ' ';
                    GlyphEqual.Char = ' ';
                    for (int i = 0; i < Bits.Length; i++)
                    {
                        var localI = i;
                        Bits[localI].Animation("Position", Bits[localI].Position, 200, () =>
                        {
                            Visuals.Remove(Bits[localI]);
                            Bits[localI] = null;
                        });
                        Bits[i].Char = ' ';
                    }
                    GlyphResult.Animation("Position", Position + new Vector2(Delta*4+5, Delta*2 + 5), 200, () =>
                    {
                        EndAnimation(GlyphResult, 0);
                        GlyphResult = null;
                    });
                });
                
                
            });
            
            for (int i = 1; i < Bits.Length; i++)
            {

                int localI = i;
                Bits[localI].Animation("Position", Position
                + new Vector2(Delta * 2, -5), 500);
            }
            GlyphPlus.Char = '+';
            GlyphEqual.Char = '=';


        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            switch (num)
            {
                case 0:
                    return Position + new Vector2(0, Delta*2);
                case 1:
                    if(Up)
                        return Position + new Vector2(Delta*2, Delta * 4);
                    if(Down)
                        return Position + new Vector2(Delta * 2, 0);
                    break;
                case 2:
                    if (Up && Down)
                        return Position + new Vector2(Delta * 2, 0);
                    break;
            }
            return Position;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Position + new Vector2(Delta*4, Delta*2);
        }
    }
}
