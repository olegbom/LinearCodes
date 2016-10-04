using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform.Windows;

namespace LinearCodes
{
    public class Field: DrawingVisual
    {

        public int Width { get; }
        public int Height { get; }
        public float Delta { get; } = 10;


        public List<StreamingComponent> StreamingComponents { get; } = new List<StreamingComponent>();
        public List<StreamingWire> Wires { get; } = new List<StreamingWire>();
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
            CrossCursor.InstasingList.Add(new VisualUniforms(Color.Black));
            vertices.Clear();
            int count = 5;
            for (int i = 0; i < count; ++i)
            {
                double arg = Math.PI*2*i/count;
                float cos = (float) Math.Cos(arg)*Delta/2;
                float sin = (float) Math.Sin(arg) * Delta/2;

                vertices.AddRange(Line(cos,sin,0,0,1));

            }

            
            CrossCursor.Shape = vertices.ToArray();
            Childrens.Add(CrossCursor);

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
            visual.Animation("Translate", ToDiscret(translate), 250);
        }

        public Vector2 ToDiscret(Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X / Delta) * Delta, (float)Math.Round(v.Y / Delta) * Delta);
        }

        

        private MouseDragVisualObject _dragStreamingVisual;
        private InputWireCreator _inputWireCreator;
        private Vector2 _mouseDownPos;
        public void MouseDown(Vector2 mouseFieldPos)
        {
            
            _mouseDownPos = ToDiscret(mouseFieldPos);
            foreach (var streamingVis in StreamingComponents)
            {
                if (streamingVis.Hit(mouseFieldPos))
                {
                    for (int i = 0; i < streamingVis.InCount; i++)
                    {
                        if (streamingVis.Inputs[i] == null && 
                            streamingVis.InputPosition(i) == _mouseDownPos)
                        {
                            _inputWireCreator = new InputWireCreator(streamingVis, i);
                            Childrens.Add(_inputWireCreator.Wire);
                            return;
                        }
                    }
                    for(int i = 0; i < streamingVis.OutCount; i++)
                    {
                        if (streamingVis.OutputPosition(i) == _mouseDownPos)
                        {
                            throw new NotImplementedException();
                            break;
                        }
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
            if (_inputWireCreator != null)
            {
                _inputWireCreator.MouseMovePos = newPos;
            }

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
            if (_inputWireCreator != null)
            {
                _inputWireCreator.MouseMovePos = mouseUpPos;
                _inputWireCreator.Connecting();
                Wires.Add(_inputWireCreator.Wire);
                _inputWireCreator.Dispose();
                _inputWireCreator = null;
            }
        }

        public void KeyPressSpace()
        {
            InputWireCreator.SourceFirst = !InputWireCreator.SourceFirst;
            _inputWireCreator?.WireUpdate();
        }

        private class InputWireCreator: IDisposable
        {
            public static bool SourceFirst = true;
            public StreamingComponent Visual { get; private set; }
            public Vector2 InputPosition { get; }
            public int InputIndex { get; }
            public StreamingWire Wire { get; private set; }
            public InputWireCreator(StreamingComponent visual, int inputIndex)
            {
                Visual = visual;
                InputIndex = inputIndex;
                InputPosition = visual.InputPosition(InputIndex);
                Wire = new StreamingWire(visual.SimpleShader);
                Wire.Path = new List<Vector2> { InputPosition, InputPosition };

            }

            private Vector2 _mouseMovePos;

            public Vector2 MouseMovePos
            {
                get { return _mouseMovePos; }
                set
                {
                    _mouseMovePos = value;
                    WireUpdate();
                }
            }

            public void WireUpdate()
            {
                if (Math.Abs(InputPosition.X - MouseMovePos.X) < 0.1f ||
                    Math.Abs(InputPosition.Y - MouseMovePos.Y) < 0.1f)
                {
                    Wire.Path = new List<Vector2> {
                        InputPosition , MouseMovePos };
                    return;
                } 
                Wire.Path = new List<Vector2> {
                        InputPosition ,
                        SourceFirst ? new Vector2(InputPosition.X, MouseMovePos.Y) 
                                    : new Vector2(MouseMovePos.X, InputPosition.Y),
                        MouseMovePos };
            }

            public void Connecting()
            {
                Wire.ConnectTo(0, Visual, InputIndex);
            }

            public void Dispose()
            {
                Wire = null;
                Visual = null;
            }
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
