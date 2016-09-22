using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class Register
    {
        public List<DrawingVisual> Visuals { get; } = new List<DrawingVisual>();

        public float X;
        public float Y;

        public float Delta = 10;
        public string Word = "RG";
        public Shader Shader { get; }
        public DigitalComponent Body { get; private set; }

        public List<Glyph7x5> GlyphWord { get; } = new List<Glyph7x5>();

        public Glyph7x5 MemoryGlyph { get; private set; }


        public Wire WireIn;
        
        public Wire WireOut;
        
        

        public Register(Shader shader) 
        {
            Shader = shader;
        }

        public void RegisterTick()
        {
            if (WireIn == null) return;
            if (WireOut == null) return;
            WireOut.Value = MemoryGlyph.Char == '1';
            MemoryGlyph.Char = WireIn.Value ? '1' : '0';
        }


        public void CreateBuffer()
        {
            Body = new DigitalComponent(Shader)
            {
                PinsCount = 1,
                Delta = Delta,
                Position = new Vector2(X,Y)
            };
            Body.CreateBuffer();

            for (int i = 0; i < Word.Length; i++)
            {
                var letter = Word[i];
                var glyph = new Glyph7x5(letter, new Vector2(X + Delta * (2 + i), Y + Delta * 0.4f), Shader);
                GlyphWord.Add(glyph);
            }
            MemoryGlyph = new Glyph7x5('0' , new Vector2(X + Delta * 2.5f, Y + Delta * 2.0f), Shader);

            Visuals.Add(MemoryGlyph);
            Visuals.Add(Body);
            Visuals.AddRange(GlyphWord);
        }
    }
}
