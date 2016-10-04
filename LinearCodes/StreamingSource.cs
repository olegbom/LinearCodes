﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public class StreamingSource: StreamingVisual
    {


        public int[] Message { get; }
        public List<Glyph7x5> BitMessage { get; } = new List<Glyph7x5>();

        public StreamingSource(int[] message, SimpleShader simpleShader) 
            : base(simpleShader,  0, 1)
        {
            
            Message = message;

            Size = new Vector2(Delta * (Message.Length + 1), Delta * 2);

            InstasingList.Add(new VisualUniforms(Color4.Black));
            for (int i = 0; i < Message.Length; i++)
            {
                var bit = message[i];
                var glyph = new Glyph7x5(bit == 0?'0':'1',
                    new Vector2(Delta * i+4, 2),
                    SimpleShader);
                BitMessage.Add(glyph);
                Childrens.Add(glyph);
            }
            var vertices = new List<Vector2>();
            vertices.AddRange(Polyline(new[]
            {
                new Vector2(Delta*(Message.Length+1), 0),
                new Vector2(-1,0),
                new Vector2(-1, Delta*2),
                new Vector2(Delta*Message.Length, Delta*2)
            }, 2));
            vertices.AddRange(Circle(
                new Vector2(Delta*(Message.Length + 1), 0),3,15));
            Shape = vertices.ToArray();

        }

        protected override void StartAnimation()
        {
            for (int i = 0; i < BitMessage.Count-1; i++)
            {
                BitMessage[i].Animation("Translate", BitMessage[i+1].Translate, 500);
            }
            var last = BitMessage.Last();
            last.Animation("Translate",
                new Vector2(Delta * (Message.Length + 1) + 1,2),500,() =>
                {
                    EndAnimation(last, 0);
                    BitMessage.Remove(last);
                    
                    var glyph = new Glyph7x5( '0',
                        new Vector2(4, 2),
                        SimpleShader);
                    BitMessage.Insert(0, glyph);
                    Childrens.Add(glyph);
                });
        }

        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Translate;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Translate + new Vector2(Delta * (Message.Length + 1), 0);
        }
    }
}
