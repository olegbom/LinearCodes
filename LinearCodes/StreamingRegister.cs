using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class StreamingRegister: StreamingComponent
    {
        private string _word = "RG";

        public string Word
        {
            get { return _word; }
            set
            {
                _word = value;
                for (int i = 0; i < _word.Length; i++)
                {
                    if (GlyphWord.Count > i)
                        GlyphWord[i].Char = _word[i];
                    else
                    {
                        var glyph = new Glyph7x5(_word[i], new Vector2(Delta*(i+2), Delta*0.2f), SimpleShader);
                        GlyphWord.Add(glyph);
                    }
                }
                for (int i = _word.Length; GlyphWord.Count > _word.Length; )
                {
                    var glyph = GlyphWord[i];
                    GlyphWord.Remove(glyph);
                }
            }
        }
        
        
        public DigitalComponent Body { get; private set; }

        public ObservableCollection<Glyph7x5> GlyphWord { get; } = new ObservableCollection<Glyph7x5>();

        public Glyph7x5 MemoryGlyph { get; private set; }

        public StreamingRegister(SimpleShader simpleShader) 
            : base(simpleShader,  1, 1)
        {
            Size = new Vector2(Delta * 6, Delta * 2);
            InstasingList.Add(new VisualUniforms(Color4.Black));
            var vertices = new List<Vector2>();
            vertices.AddRange(Rectangle(
                new Vector2(Delta, 0),
                new Vector2(Delta*5, Delta*2), 2));
            Shape = vertices.ToArray();
           
            CreateInput(0, ConnectorOrientation.Left, new Vector2(0,Delta));
            CreateOutput(0, ConnectorOrientation.Right, new Vector2(Delta*6,Delta));

            GlyphWord.CollectionChanged += (s, e) =>
            {
                if(e.NewItems != null)
                    foreach (Glyph7x5 glyph in e.NewItems)
                        Childrens.Add(glyph);
                if(e.OldItems != null)
                    foreach (Glyph7x5 glyph in e.OldItems)
                        Childrens.Remove(glyph);   
            };

            Word = _word;
            MemoryGlyph = new Glyph7x5('0',new Vector2(Delta*2.5f, Delta*2 + 2), SimpleShader);
            Childrens.Add(MemoryGlyph);
        }

        protected override void StartAnimation()
        {
            //GlyphPosAnimation.Create(Bits[0], MemoryGlyph.Position, 1200, ()=>
            //{
            //    MemoryGlyph = Bits[0];
            //    Bits[0] = null;
            //});
            Bits[0].PathAnimation("Translate", new List<Vector2>
                {
                    Bits[0].Translate,
                    new Vector2(1, Delta*2 + 2),
                    MemoryGlyph.Translate
                }, 10, (from, to) => (from - to).Length);

            //Bit.Animation("Position", MemoryGlyph.Position, 1000);


            MemoryGlyph.PathAnimation("Translate",
                new List<Vector2>
                {
                    MemoryGlyph.Translate,
                    new Vector2(Delta*5 + 1, Delta*2 + 2),
                    new Vector2(Delta*5 + 1, Delta + 2),
                    new Vector2(Delta*6 + 1, Delta + 2)
                }, 10, (from, to) => (from - to).Length, () =>
                {
                    EndAnimation(MemoryGlyph, 0);
                    MemoryGlyph = Bits[0];
                    Bits[0] = null;
                });
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
                    Glyph.Translate = new Vector2(delX, delY) + center;
                }
            }

            private GlyphPosAnimation(Glyph7x5 glyph, Vector2 to, uint ms, Action action = null)
            {
                To = to;
                Glyph = glyph;
                Old = Glyph.Translate;

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
