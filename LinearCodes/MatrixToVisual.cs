using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using Color = System.Drawing.Color;
using System;

namespace LinearCodes
{

    using static DrawingVisual;

    public class MatrixToVisual
    {

        public List<DrawingVisual> Visuals = new List<DrawingVisual>();

        public Wire[] WiresInputs { get; }
        public Wire[] WiresOutputs { get; }
        public DrawingVisual InOutPins { get; }

        public bool[,] Matrix;

        public MatrixToVisual(bool[,] matrix, SimpleShader simpleShader) 
        {

            Matrix = matrix;
            int icount = matrix.GetLength(0);
            int jcount = matrix.GetLength(1);

            WiresInputs = new Wire[icount];
            WiresOutputs = new Wire[jcount];
            DigitalComponent[] logicalXors = new DigitalComponent[jcount];

            InOutPins = new DrawingVisual(simpleShader);
            
            var delta = 10;
            var x0 = delta*5; 
            var y0 = delta*5;
            var bigStep = delta*4;
            var xRight = x0 + 60*delta;
            var circleR = delta;
            var xLineRight = xRight;
            var xLineLeft = x0 + circleR;
            InOutPins.Shape = Round(Vector2.Zero, circleR, 4.0f, 50, 0.1f);



            for (int i = 0; i < icount; i++)
            {
                WiresInputs[i] = new Wire(simpleShader);
                WiresInputs[i].Thickness = 2.0f;
                WiresInputs[i].PointRadius = 3.0f;
            }

            float xBase = xLineLeft;

            var pinsCounts = new int[jcount];
            for (int i = 0; i < icount; i++)
                for (int j = 0; j < jcount; j++)
                    if (matrix[i, j]) pinsCounts[j]++;

            


            for (int j = 0; j < jcount; j++)
            {
             
                float xorX = xBase + delta*pinsCounts[j]*2 + delta;
                
                int pinNum = 0;

                logicalXors[j] = new DigitalComponent(simpleShader);

                var rectHalfHeight = pinsCounts[j] * delta;

                //logicalXors[j].X = xorX;

                logicalXors[j].Delta = delta;
                logicalXors[j].PinsCount = pinsCounts[j];
                logicalXors[j].CreateBuffer();
                
                xBase = xorX + logicalXors[j].Width;

                WiresOutputs[j] = new Wire(simpleShader);
                WiresOutputs[j].Color = Color4.Red;
                WiresOutputs[j].Thickness = 2.0f;
                WiresOutputs[j].PointRadius = 3.0f;
               // WiresOutputs[j].AddLine(lineV1, lineV2);
               // WiresOutputs[j].AddPoint(pointOutPin, circleR - 4);
                WiresOutputs[j].CreateBuffer();
            }
            xBase += delta * 2;
            for (int i = 0; i < icount; i++)
            {
                var y = y0 + (icount - i - 1 + jcount) * bigStep;

                WiresInputs[i].AddLine(new Vector2(xLineLeft, y), new Vector2(xBase, y));
                WiresInputs[i].AddPoint(new Vector2(x0, y), circleR - 4);
                WiresInputs[i].AddPoint(new Vector2(xBase + circleR, y), circleR - 4);

                InOutPins.InstasingList.Add(
                    new VisualUniforms(Color.Black)
                    { Translate = new Vector2(x0, y) }
                );
            }

            for (int i = 0; i < matrix.GetLength(1) + matrix.GetLength(0); i++)
            {
                var y = y0 + (icount - i - 1 + jcount) * bigStep;
                // vertices.AddRange(Round(new Vector2(xRight+circleR, y), circleR, 2.0f, 50, 0.1f));
                InOutPins.InstasingList.Add(
                   new VisualUniforms(Color.Black)
                   { Translate = new Vector2(xBase + circleR, y) }
                );
            }

            for (int i = 0; i < icount; i++)
            {
                WiresInputs[i].CreateBuffer();
            }

            Visuals.AddRange(WiresInputs);
            Visuals.AddRange(WiresOutputs);
            Visuals.AddRange(logicalXors);
            Visuals.Add(InOutPins);
        }

        public void MouseDown(Vector2 mouseCoord)
        {
            for(int i = 0; i< WiresInputs.Length; i++)
            {
                var pin = InOutPins.InstasingList[i];
                if ((mouseCoord - pin.Translate).LengthSquared < 100)
                {
                    WiresInputs[i].Value = !WiresInputs[i].Value;
                    CalculateOuts();
                    return;
                }
            }
        }

        private void CalculateOuts()
        {
            for (int j = 0; j < Matrix.GetLength(1); j++)
            {

                int sum = 0;
                for(int i = 0; i < Matrix.GetLength(0); i++)
                {
                    if (Matrix[i, j] && WiresInputs[i].Value)
                        sum++;
                }
                WiresOutputs[j].Value = (sum % 2) == 1;
            }
        }

        Vector2 oldMouseCoord = Vector2.Zero;
        public void MouseMove(Vector2 mouseCoord)
        {
             
            for (int i = 0; i < WiresInputs.Length; i++)
            {
                var pin = InOutPins.InstasingList[i];
                bool isWithing = (mouseCoord - pin.Translate).LengthSquared < 100;
                bool isOldWithing = (oldMouseCoord - pin.Translate).LengthSquared < 100;
                if (isWithing && !isOldWithing)
                {
                    pin.Animation("Color",Color4.BlueViolet, 100);
                    break;
                }
                if(isOldWithing && !isWithing)
                {
                    pin.Animation("Color", Color4.Black, 100);
                    break;
                }
                if (isWithing) break;
            }

            oldMouseCoord = mouseCoord;
        }

        public void Draw()
        {
            Visuals.ForEach(x => x.Draw());
        }
    }
}
