using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{

    //Возможно не нужно
    public class InOutPins
    {
        DrawingVisual drawing;
        public InOutPins(Shader shader)
        {
            drawing = new DrawingVisual(0, shader);
            drawing.Shape = DrawingVisual.Round(Vector2.Zero, 20, 2, 50, 0.1f);
            drawing.InitBuffers();
        }

        public void AddNew(Vector2 v)
        {
            VisualUniforms uniform = new VisualUniforms(Color4.Black) { Translate = v };
            drawing.InstasingList.Add(uniform);
        }
    }
}
