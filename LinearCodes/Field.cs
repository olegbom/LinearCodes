using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LinearCodes
{
    public class Field: DrawingVisual
    {


        public int Width { get; }
        public int Height { get; }
        public int Delta { get; } = 10;
        
        public Field(int width, int height, SimpleShader simpleShader): base(simpleShader)
        {
            Width = width;
            Height = height;
          

           
            List<Vector4> vertices = new List<Vector4>();
            for (int i = Delta; i < Height; i += Delta)
            {
                vertices.AddRange(Line(new Vector2(0.5f, i + 0.5f), new Vector2(Width + 0.5f, i + 0.5f), 1f, -0.5f));
            }

            for (int i = Delta; i < Width; i += Delta)
            {
                vertices.AddRange(Line(new Vector2(i + 0.5f, 0 + 0.5f), new Vector2(i + 0.5f, Height + 0.5f), 1f, -0.5f));
            }
            Shape = vertices.ToArray();
            InstasingList.Add(new VisualUniforms(Color.LightGray));
       }

    }
}
