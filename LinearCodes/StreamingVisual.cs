using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace LinearCodes
{
    public abstract class StreamingVisual:DrawingVisual
    {
        public Edge[] Inputs { get; }
        public Edge[] Outputs { get; }
        public Glyph7x5[] Bits { get; }

        private readonly DrawingVisual _frame;
        public float Delta { get; } = 10;

        private bool _isSelect;
        public bool IsSelect
        {
            get { return _isSelect; }
            set
            {
                _isSelect = value;
                _frame.IsVisible = _isSelect;
            }
        }

        private Vector2 _size;
        public Vector2 Size
        {
            get { return _size; }
            protected set
            {
                _size = value;
                _frame.Shape = Rectangle(new Vector2(-Delta/2,-Delta/2), _size + new Vector2(Delta / 2, Delta / 2), 1f);
            }
        }
        
        public int InCount { get; }
        public int OutCount { get; }
        
        public StreamingVisual(SimpleShader simpleShader, int inCount, int outCount) 
            : base(simpleShader)
        {

            _frame = new DrawingVisual(simpleShader);
            _frame.InstasingList.Add(new VisualUniforms(new Color4(0, 0, 1f, 0.3f)));
            _frame.IsVisible = false;
            Childrens.Add(_frame);
            InCount = inCount;
            OutCount = outCount;
            Inputs = new Edge[inCount];
            Outputs = new Edge[outCount];
            Bits = new Glyph7x5[inCount];
        }

        protected void EndAnimation(Glyph7x5 bit, int outputIndex)
        {
            if (outputIndex >= OutCount) return;

            Outputs[outputIndex].TransmitBit(bit);
        }

        
        public void Start()
        {
            if (Bits.All(x => x != null))
                StartAnimation();
        }

        protected abstract void StartAnimation();
        public abstract Vector2 InputPosition(int num);
        public abstract Vector2 OutputPosition(int num);

        public bool Hit(Vector2 v)
        {
            var min = Translate;
            var max = Translate + Size;
            
            return min.X <= v.X &&
                   min.Y <= v.Y &&
                   max.X >= v.X &&
                   max.Y >= v.Y;
        }
        public bool Hit(Vector2 v1, Vector2 v2)
        {
            var selectMin = Vector2.ComponentMin(v1, v2);
            var selectMax = Vector2.ComponentMax(v1, v2);
            var min = Translate;
            var max = Translate + Size;
            return min.X >= selectMin.X &&
                   min.Y >= selectMin.Y &&
                   max.X <= selectMax.X &&
                   max.Y <= selectMax.Y;
        }

        public void ConnectTo(int outIndex, StreamingVisual toVisual, int inIndex)
        {
            var edge = new Edge
            (
                this, outIndex,
                toVisual, inIndex
            );
            Outputs[outIndex] = edge;
            toVisual.Inputs[inIndex] = edge;
        } 
    }
}
