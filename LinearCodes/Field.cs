using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class Field: DrawingVisual
    {

        public int Width { get; }
        public int Height { get; }
        public float Delta { get; } = 10;


        public List<StreamingVisual> StreamingVisuals { get; } = new List<StreamingVisual>();
        public DrawingVisual CrossCursor { get; }
        public DrawingVisual SelectRectangle  { get; }
        
        public Field(int width, int height, SimpleShader simpleShader): base(simpleShader)
        {
            Width = width;
            Height = height;
            
            List<Vector2> vertices = new List<Vector2>();
            for (int i = 0; i <= Height; i += (int)Delta)
            {
                vertices.AddRange(Line(new Vector2(0, i), new Vector2(Width, i ), 1f));
            }

            for (int i = 0; i <= Width; i += (int)Delta)
            {
                vertices.AddRange(Line(new Vector2(i , 0), new Vector2(i , Height), 1f));
            }
            Shape = vertices.ToArray();
            InstasingList.Add(new VisualUniforms(Color.LightGray));

            CrossCursor = new DrawingVisual(simpleShader);
            CrossCursor.InstasingList.Add(new VisualUniforms(Color.Blue));
            vertices.Clear();
            vertices.AddRange(Line(-Delta, 0,Delta, 0, 2));
            vertices.AddRange(Line(0,-Delta, 0,Delta, 2));
            CrossCursor.Shape = vertices.ToArray();
            Childrens.Add(CrossCursor);

            SelectRectangle = new DrawingVisual(simpleShader);
            SelectRectangle.InstasingList.Add(new VisualUniforms(new Color4(0,0,0.5f,0.2f)));
            SelectRectangle.IsVisible = false;
            Childrens.Add(SelectRectangle);

      

        }
        
        public void AddingStreamingVisual(StreamingVisual visual, Vector2 translate)
        {
            StreamingVisuals.Add(visual);
            Childrens.Add(visual);
            visual.Translate += translate;
            visual.Animation("Translate", ToDiscret(translate), 250);
        }

        public Vector2 ToDiscret(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X / Delta) * Delta, (float)Math.Round(v.Y / Delta) * Delta);
        }


        public void MouseDown(Vector2 mouseFieldPos)
        {
            SelectRectangle.Translate = ToDiscret(mouseFieldPos);
            SelectRectangle.IsVisible = true;
            SelectRectangle.Shape = new Vector2[0];
            foreach (var streamingVis in StreamingVisuals)
            {
                var min = streamingVis.Translate;
                var max = streamingVis.Translate + streamingVis.Size;
                if (min.X < mouseFieldPos.X &&
                    min.Y < mouseFieldPos.Y &&
                    max.X > mouseFieldPos.X &&
                    max.Y > mouseFieldPos.Y)
                {
                    streamingVis.IsSelect = !streamingVis.IsSelect;
                    return;
                }
            }
        }

        public void MouseMove(Vector2 mouseFieldPos)
        {
            
            var newPos = ToDiscret(mouseFieldPos);
            if (SelectRectangle.IsVisible)
            {
                var size = newPos - SelectRectangle.Translate;
                SelectRectangle.Shape = GetFillRectangle(new Vector2(0,0), size.X, size.Y, 0.5f);
            }
            CrossCursor.Animation("Translate", newPos, 50);
        }


        public void MouseUp(Vector2 mouseFieldPos)
        {
            SelectRectangle.IsVisible = false;
            var discratePos = ToDiscret(mouseFieldPos);
            var selectMin = Vector2.ComponentMin(discratePos, SelectRectangle.Translate);
            var selectMax = Vector2.ComponentMax(discratePos, SelectRectangle.Translate);
            foreach (var streamingVis in StreamingVisuals)
            {
                var min = streamingVis.Translate;
                var max = streamingVis.Translate + streamingVis.Size;
                if (min.X > selectMin.X &&
                    min.Y > selectMin.Y &&
                    max.X < selectMax.X &&
                    max.Y < selectMax.Y)
                {
                    streamingVis.IsSelect = true;
                }
            }
        }
    }
}
