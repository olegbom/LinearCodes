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
                    new Vector2(position.X + Delta * (2 + i), position.Y + Delta * 0.4f),
                    Shader);
                GlyphWord.Add(glyph);
                Visuals.Add(glyph);
            }

            MemoryGlyph = new Glyph7x5('0', 
                new Vector2(position.X + Delta * 2.5f, position.Y + Delta * 2.0f),
                Shader);
            Visuals.Add(MemoryGlyph);
        }

        protected override void StartAnimation()
        {
            GlyphPosAnimation.Create(Bits[0], MemoryGlyph.Position, 1200, ()=>
            {
                MemoryGlyph = Bits[0];
                Bits[0] = null;
            });
           //Bit.Animation("Position", MemoryGlyph.Position, 1000);
            MemoryGlyph.Animation("Position", 
                new Vector2(Position.X + Delta * 6.25f, Position.Y + Delta * 1.25f ),
                1000,() =>
            {
                EndAnimation(MemoryGlyph, 0);
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
