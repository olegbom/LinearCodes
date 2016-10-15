using System;
using System.Collections.Generic;
using System.Drawing;
using LinearCodes.Creator;
using LinearCodes.Streamings;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class Field: DrawingVisual
    {
        public EmploymentMatrix EmploymentMatrix;

        public int Width { get; }
        public int Height { get; }
        public float Delta { get; } = 10;



        public List<StreamingComponent> StreamingComponents { get; } = new List<StreamingComponent>();
        public List<StreamingWire> Wires { get; } = new List<StreamingWire>();
        public CrossCursor CrossCursor { get; }
        public DrawingVisual SelectRectangle  { get; }
        
        

        public Field(int width, int height, SimpleShader simpleShader): base(simpleShader)
        {
            Width = width;
            Height = height;
            EmploymentMatrix = new EmploymentMatrix(Width,Height);
           

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

            CrossCursor = new CrossCursor(simpleShader);
          
         

            SelectRectangle = new DrawingVisual(simpleShader);
            SelectRectangle.InstasingList.Add(new VisualUniforms(new Color4(0,0,0.5f,0.2f)));
            SelectRectangle.IsVisible = false;
            Childrens.Add(SelectRectangle);

      

        }
        
        public void AddingStreamingVisual(StreamingComponent visual, Vector2 translate)
        {
            StreamingComponents.Add(visual);
            Childrens.Add(visual);
            visual.Translate += translate;
            var discPosition = ToDiscret(translate);
            visual.Animation("Translate", discPosition, 250);
            EmploymentMatrix.MountingRectangle(discPosition, visual.Size);
        }

    

        public Vector2 ToDiscret(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X / Delta) * Delta, (float)Math.Round(v.Y / Delta) * Delta);
        }


        public override void Draw()
        {
            base.Draw();
            CrossCursor.Draw();
        }


        private MouseDragVisualObject _dragStreamingVisual;
        private WireCreator _wireCreator;
        private Vector2 _mouseDownPos;
        public void MouseDown(Vector2 mouseFieldPos)
        {
            
            _mouseDownPos = ToDiscret(mouseFieldPos);
            foreach (var streamingVis in StreamingComponents)
            {
                if (streamingVis.Hit(mouseFieldPos))
                {
                    int pin;
                    if (streamingVis.MouseSelectInput(_mouseDownPos,out pin))
                    {
                        _wireCreator = new InputWireCreator(streamingVis, pin, EmploymentMatrix);
                        Childrens.Add(_wireCreator.Wire);
                        return;
                    }
                    if (streamingVis.MouseSelectOutput(_mouseDownPos, out pin))
                    {
                        _wireCreator = new OutputWireCreator(streamingVis, pin, EmploymentMatrix);
                        Childrens.Add(_wireCreator.Wire);
                        return;
                    }

                    _dragStreamingVisual = new MouseDragVisualObject(streamingVis, _mouseDownPos);
                    streamingVis.IsSelect = !streamingVis.IsSelect;
                    return;
                }
            }
            SelectRectangle.Translate = ToDiscret(mouseFieldPos);
            SelectRectangle.IsVisible = true;
            SelectRectangle.Shape = new Vector2[0];
        }

        public void MouseMove(Vector2 mouseFieldPos)
        {
            
            var newPos = ToDiscret(mouseFieldPos);
            if (SelectRectangle.IsVisible)
            {
                var size = newPos - SelectRectangle.Translate;
                SelectRectangle.Shape = GetFillRectangle(new Vector2(0,0), size.X, size.Y, 0.5f);
            }
            if (_dragStreamingVisual != null)
            {
                _dragStreamingVisual.MouseMovePos = newPos;
            }
            if (_wireCreator != null)
            {
                _wireCreator.MouseMovePos = newPos;
            }

            bool hit = false;
            foreach (var streamingVis in StreamingComponents)
            {
                if (!streamingVis.Hit(mouseFieldPos)) continue;
                int pin;
                if (streamingVis.MouseSelectInput(newPos, out pin) ||
                    streamingVis.MouseSelectOutput(newPos, out pin))
                {
                    hit = true;
                    break;
                }
            }
            CrossCursor.OnPin = hit;


            CrossCursor.Animation("Translate", newPos, 50);
        }


        public void MouseUp(Vector2 mouseFieldPos)
        {
            SelectRectangle.IsVisible = false;
            var discratePos = ToDiscret(mouseFieldPos);
            foreach (var streamingVis in StreamingComponents)
            {
                if (streamingVis.Hit(discratePos, SelectRectangle.Translate))
                {
                    streamingVis.IsSelect = true;
                }
            }
            var mouseUpPos = ToDiscret(mouseFieldPos);
            if (_dragStreamingVisual != null)
            {
                _dragStreamingVisual.MouseMovePos = mouseUpPos;
                _dragStreamingVisual.Dispose();
                _dragStreamingVisual = null;
            }
            if (_wireCreator != null)
            {
                _wireCreator.MouseMovePos = mouseUpPos;
                _wireCreator.Connecting();
                var wire = _wireCreator.Wire;
                var isOutputWireCreator = _wireCreator is OutputWireCreator;
                var isInputWireCreator = _wireCreator is InputWireCreator;

                _wireCreator.Dispose();
                _wireCreator = null;
                foreach (var streamingVis in StreamingComponents)
                {
                    if (!streamingVis.Hit(mouseFieldPos)) continue;
                    if (isOutputWireCreator)
                        for (int i = 0; i < streamingVis.InCount; i++)
                        {
                            var div = streamingVis.InputPosition(i) - _mouseDownPos;
                            if (streamingVis.Inputs[i] == null &&
                                Math.Abs(div.X)*2 < Delta && Math.Abs(div.Y)*2 < Delta)
                            {
                                wire.ConnectTo(0,streamingVis, i);
                                Wires.Add(wire);
                                break;
                            }
                        }
                    else if (isInputWireCreator)
                        for (int i = 0; i < streamingVis.OutCount; i++)
                        {
                            var div = streamingVis.OutputPosition(i) - _mouseDownPos;
                            if (streamingVis.Outputs[i] == null &&
                                Math.Abs(div.X)*2 < Delta && Math.Abs(div.Y)*2 < Delta)
                            {
                                streamingVis.ConnectTo(i, wire, 0);
                                Wires.Add(wire);
                                break;
                            }
                        }
                    
                }

                

            }
        }

        public void KeyPressSpace()
        {
            WireCreator.SourceFirst = !WireCreator.SourceFirst;
            _wireCreator?.WireUpdate();
        }


        

        private class MouseDragVisualObject : IDisposable
        {
            public VisualObject VisualObject { get; private set; }
            public Vector2 MouseDownPos { get; }
            private Vector2 OldObjPos { get; }

            public MouseDragVisualObject(VisualObject visualObject, Vector2 mouseDownPos)
            {
                VisualObject = visualObject;
                OldObjPos = VisualObject.Translate;
                MouseDownPos = mouseDownPos;
            }

            
            public Vector2 MouseMovePos
            {
                set { VisualObject.Animation("Translate", OldObjPos + value - MouseDownPos, 50); }
            }

            public void Dispose()
            {
                VisualObject = null;
            }
        }

       
    }
}
