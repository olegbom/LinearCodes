using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LinearCodes
{
    public abstract class StreamingVisual:DrawingVisual
    {
        public Edge[] Inputs { get; }
        public Edge[] Outputs { get; }
        public Glyph7x5[] Bits { get; }

        public int InCount { get; }
        public int OutCount { get; }
        protected readonly List<DrawingVisual> Visuals; 
        
        public StreamingVisual(Shader shader, List<DrawingVisual> visuals, int inCount, int outCount) 
            : base(0, shader)
        {
            InCount = inCount;
            OutCount = outCount;
            Inputs = new Edge[inCount];
            Outputs = new Edge[outCount];
            Bits = new Glyph7x5[inCount];
            Visuals = visuals;
            Visuals.Add(this);
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
