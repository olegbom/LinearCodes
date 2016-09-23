using System;
using System.Collections.Generic;
using OpenTK;

namespace LinearCodes
{
    public class StreamingRegister: StreamingVisual
    {

        public Vector2 Position { get; private set; }

        public float Delta = 10;
        public string Word = "RG";
        
        public DigitalComponent Body { get; private set; }

        public List<Glyph7x5> GlyphWord { get; } = new List<Glyph7x5>();

        public Glyph7x5 MemoryGlyph { get; private set; }

        public StreamingRegister(Shader shader, List<DrawingVisual> visuals) 
            : base(shader, visuals, 1, 1)
        {
           
        }

        public void CreateBuffer(Vector2 position)
        {
            Position = position;
            Body = new DigitalComponent(Shader)
            {
                PinsCount = 1,
                Delta = Delta,
                Position =  position
            };
            Body.CreateBuffer();
            Visuals.Add(Body);
            for (int i = 0; i < Word.Length; i++)
            {
                var letter = Word[i];
                var glyph = new Glyph7x5(letter, 
                    position + new Vector2(Delta * (2 + i), 5),
                    Shader);
                GlyphWord.Add(glyph);
                Visuals.Add(glyph);
            }

            MemoryGlyph = new Glyph7x5('0',
                position + new Vector2(Delta * 2.5f + 5, Delta * 2f + 5),
                Shader);
            Visuals.Add(MemoryGlyph);
        }

        protected override void StartAnimation()
        {
            //GlyphPosAnimation.Create(Bits[0], MemoryGlyph.Position, 1200, ()=>
            //{
            //    MemoryGlyph = Bits[0];
            //    Bits[0] = null;
            //});
            Bits[0].PathAnimation("Position", new List<Vector2>
                {
                    Bits[0].Position,
                    Position + new Vector2(5, Delta*2 + 5),
                    MemoryGlyph.Position
                }, 10, (from, to) => (from - to).Length);

            //Bit.Animation("Position", MemoryGlyph.Position, 1000);


            MemoryGlyph.PathAnimation("Position",
                new List<Vector2>
                {
                    MemoryGlyph.Position,
                    Position + new Vector2(Delta*5 + 5, Delta*2 + 5),
                    Position + new Vector2(Delta*5 + 5, Delta + 5),
                    Position + new Vector2(Delta*6 + 5, Delta + 5)
                }, 10, (from, to) => (from - to).Length, () =>
                {
                    EndAnimation(MemoryGlyph, 0);
                    MemoryGlyph = Bits[0];
                    Bits[0] = null;
                });
        }





        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Position + new Vector2(0, Delta);
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Position + new Vector2(6*Delta, Delta); 
        }

        public class GlyphPosAnimation: IDisposable
        {
            Glyph7x5 Glyph;
            private Vector2 Old { get; }
            private Vector2 To { get; }

            private Vector2 center;
            private float a,b;
            private double _n;
            public double N
            {
                get{return _n;}
                set
                {
                    _n = value;
                    var delX = -(float) Math.Cos(_n*Math.PI/2)*a;
                    var delY = (float) Math.Sin(_n*Math.PI/2)*b;
                    Glyph.Position = new Vector2(delX, delY) + center;
                }
            }

            private GlyphPosAnimation(Glyph7x5 glyph, Vector2 to, uint ms, Action action = null)
            {
                To = to;
                Glyph = glyph;
                Old = Glyph.Position;

                center = new Vector2(to.X,Old.Y);
                a = To.X - Old.X;
                b = To.Y - Old.Y;
                this.Animation("N", 1.0d, ms, () => { Dispose(); action?.Invoke(); });
            }

            public static void Create(Glyph7x5 glyph, Vector2 to, uint ms, Action action = null)
            {
                new GlyphPosAnimation(glyph, to, ms, action);
            }

            public void Dispose()
            {
                Glyph = null;
            }
        }
    }
}
