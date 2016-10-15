using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes.Streamings  
{
    public class StreamingSummator: StreamingComponent
    {
        public bool Up = true;
        public bool Down = false;

        private Glyph7x5 GlyphPlus;
        private Glyph7x5 GlyphEqual;
        private Glyph7x5 GlyphResult;

        public StreamingSummator(SimpleShader simpleShader, int inCount)
            : base(simpleShader, inCount, 1)
        {
            Size = new Vector2(Delta * 4, Delta * 4);
            InstasingList.Add(new VisualUniforms(Color4.Black));

            var vertices = new List<Vector2>();

            var center = new Vector2(Delta * 2,Delta * 2);

            vertices.AddRange(Round(center, Delta, 2, 60));
            var l = new Vector2(Delta * 1.5f, Delta * 2);
            var r = new Vector2(Delta * 2.5f, Delta * 2);
            vertices.AddRange(Line(l, r, 2));
            var u = new Vector2(Delta * 2, Delta * 2.5f);
            var d = new Vector2(Delta * 2, Delta * 1.5f);
            vertices.AddRange(Line(u, d, 2));

            CreateInput(0, ConnectorOrientation.Left, new Vector2(0,Delta*2));
            CreateOutput(0, ConnectorOrientation.Right, new Vector2(Delta*4,Delta*2));

            if (Up)
            {
                CreateInput(1, ConnectorOrientation.Top, new Vector2(Delta*2, Delta * 4));
            } else if (Down)
            {
                CreateInput(1, ConnectorOrientation.Bottom, new Vector2(Delta * 2, 0));
            }
            
            Shape = vertices.ToArray();

            GlyphPlus = new Glyph7x5(' ', new Vector2(Delta*1, -5), SimpleShader);
            Childrens.Add(GlyphPlus);
            
            GlyphEqual = new Glyph7x5(' ', new Vector2(Delta * 3, -5), SimpleShader);
            Childrens.Add(GlyphEqual);
        }


        protected override void StartAnimation()
        {
            GlyphResult = new Glyph7x5(' ', new Vector2(Delta * 4, -5),SimpleShader);
            Childrens.Add(GlyphResult);

            Bits[0].Animation("Translate", new Vector2(0, -5), 500, () =>
            {
                GlyphResult.Char = Bits.Count(x => x.Char == '1')%2 == 1 ? '1' : '0';
                GlyphResult.Animation("Translate", GlyphResult.Translate, 400, () =>
                {
                    GlyphPlus.Char = ' ';
                    GlyphEqual.Char = ' ';
                    for (int i = 0; i < Bits.Length; i++)
                    {
                        var localI = i;
                        Bits[localI].Animation("Translate", Bits[localI].Translate, 200, () =>
                        {
                            Childrens.Remove(Bits[localI]);
                            Bits[localI] = null;
                        });
                        Bits[i].Char = ' ';
                    }
                    GlyphResult.Animation("Translate", new Vector2(Delta*4+2, Delta*2 + 2), 200, () =>
                    {
                        EndAnimation(GlyphResult, 0);
                        GlyphResult = null;
                    });
                });
                
                
            });
            
            for (int i = 1; i < Bits.Length; i++)
            {

                int localI = i;
                Bits[localI].Animation("Translate", new Vector2(Delta * 2, -5), 500);
            }
            GlyphPlus.Char = '+';
            GlyphEqual.Char = '=';
        }

      
    }
}
