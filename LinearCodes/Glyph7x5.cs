using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFont.PostScript;

namespace LinearCodes
{
    public class Glyph7x5 : DrawingVisual
    {
        #region Constant
        public static readonly float PixelHeight = 3;
        public static readonly float PixelWidth = 3;
        public static readonly int GlyphHeight = 7;
        public static readonly int GlyphWidth = 5;
        public static readonly float Gap = 0;

        public static readonly Dictionary<char, int[,]> GlyphDictionary = new Dictionary<char, int[,]>()
        {
            {'0', new int[,]{
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,1,0,0,1},
                {1,0,1,0,1},
                {1,0,0,1,1},
                {1,0,0,0,1},
                {0,1,1,1,0}}},
            {'1', new int[,]{
                {0,0,1,0,0},
                {0,1,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,1,1,1,0}
                }},
            {'R', new int[,]{
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,0},
                {1,0,1,0,0},
                {1,0,0,1,0},
                {1,0,0,0,1}
                }},
            {'G', new int[,]{
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,0},
                {1,0,0,0,0},
                {1,0,0,1,1},
                {1,0,0,0,1},
                {0,1,1,1,0}}},
            {' ', new int[,]{
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0},
                {0,0,0,0,0}}},
             {'+', new int[,]{
                {0,0,0,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {1,1,1,1,1},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,0,0,0}}},
             {'=', new int[,]{
                {0,0,0,0,0},
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,0},
                {1,1,1,1,1},
                {0,0,0,0,0},
                {0,0,0,0,0}}},
        };

       
        #endregion


        private int[,] GlyphArray;

        private char _char;
        public char Char
        {
            get { return _char; }
            set
            {
                if (_char == value) return;
                _char = value;
               
                if (GlyphDictionary.ContainsKey(_char))
                    GlyphArray = GlyphDictionary[_char];
                else GlyphArray = GlyphDictionary['0'];
                
                int instIndex = 0;
                int instCount = InstasingList.Count;
                for (int i = 0, iCount = GlyphArray.GetLength(0); i < iCount; i++)
                {
                    for (int j = 0, jCount = GlyphArray.GetLength(1); j < jCount; j++)
                    {
                        if (GlyphArray[i, j] == 0) continue;
                        var delta = new Vector2(j * (PixelWidth + Gap), (iCount - i) * (PixelHeight + Gap));
                        if (instCount > instIndex)
                        {
                            InstasingList[instIndex].Animation("Translate",delta, 200);
                            InstasingList[instIndex].Animation("Color", Color4.Black, 200);
                        }
                        else
                        {
                            var uniforms = new VisualUniforms(Color4.Black);
                            uniforms.Translate = InstasingList.LastOrDefault()?.Translate ?? new Vector2(0,0);
                            uniforms.Animation("Translate",delta, 200);
                            InstasingList.Add(uniforms);
                        }
                        instIndex++;
                    }
                }
                if (InstasingList.Count > instIndex)
                {
                    for (int i = instIndex; i < InstasingList.Count; i++)
                    {
                        var uniform = InstasingList[i];

                        uniform.Animation("Translate",
                            InstasingList[instIndex].Translate, 200, () =>
                            {
                                InstasingList.Remove(uniform);
                            });
                        uniform.Animation("Color",new Color4(0,0,0,0), 200);
                    }
                    
                }
            }
        }

        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                ModelMatrixTranslate = Matrix4.CreateTranslation(value.X, value.Y, 0);
            }
        }

        public Glyph7x5(char symbol, Vector2 position, Shader shader) : base(0, shader)
        {
            Char = symbol;
            Position = position;
            IsModelMatrixTranslate = true;
            Shape = GetFillRectangle(new Vector2(0,0), PixelWidth, PixelHeight);
            InitBuffers();
        }
    }
}
