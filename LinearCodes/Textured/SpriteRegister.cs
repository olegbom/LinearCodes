using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearCodes.Textured;
using OpenTK;

namespace LinearCodes.Textured
{
    public class SpriteRegister: Sprite
    {
        private static readonly Vector2[] texturePosArray = new[]
        {
            new Vector2(0.028f, 0.312f),
            new Vector2(0.254f, 0.312f),
            new Vector2(0.434f, 0.312f),
            new Vector2(0.621f, 0.312f),
            new Vector2(0.804f, 0.312f),

            new Vector2(0.028f, 0.423f),
            new Vector2(0.215f, 0.423f),
            new Vector2(0.394f, 0.423f),
            new Vector2(0.584f, 0.423f),
            new Vector2(0.768f, 0.423f),

            new Vector2(0.028f, 0.534f),
            new Vector2(0.215f, 0.534f),
            new Vector2(0.394f, 0.534f),
            new Vector2(0.584f, 0.534f),
            new Vector2(0.768f, 0.534f),
        };


        private static readonly Random Rand = new Random();

        public SpriteRegister(Texture2D texture, SpriteRenderer renderer, TextureShader shader) : base(texture, renderer, shader)
        {
            int index = Rand.Next(texturePosArray.Length);

            Size = new Vector2(512*0.17f,512*0.09f);
            TexturePostiton = new Vector4(texturePosArray[index].X,texturePosArray[index].Y, 0.17f, 0.09f);

        }
    }
}
