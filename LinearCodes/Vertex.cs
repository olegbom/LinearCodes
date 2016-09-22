using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public struct Vertex
    {
        public Vector4 Position;
        public Color4 Color;

        public Vertex(Vector4 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }
}
