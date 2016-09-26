using OpenTK;

namespace LinearCodes
{
    public class Edge
    {
        public StreamingVisual In;
        public int OutNum;

        public Vector2 Position;
        
        public StreamingVisual Out;
        public int InNum;
        
        public Edge(StreamingVisual _in, int outNum, StreamingVisual _out, int inNum)
        {
            In = _in;
            OutNum = outNum;
            Out = _out;
            InNum = inNum;
        }

        public void TransmitBit(Glyph7x5 bit)
        {
            if (Out.Bits[InNum] == null)
            {
                In.Childrens.Remove(bit);
                Out.Childrens.Add(bit);
                Out.Bits[InNum] = bit;
                bit.ClearIndividualMatrix();
                bit.Translate = Out.InputPosition(InNum) - Out.Translate + new Vector2(4,2);
                Out.Start();
            }
        }
    }
}