using System;
using OpenTK;

namespace LinearCodes.Streamings
{
    public abstract class StreamingComponent: StreamingVisual
    {
        public Connector[] InputConnectors { get; }
        public Connector[] OutputConnectors { get; }


        public StreamingComponent(SimpleShader simpleShader, int inCount, int outCount) 
            : base(simpleShader, inCount, outCount)
        {
            InputConnectors = new Connector[inCount];
            OutputConnectors = new Connector[outCount];
        }

        protected void CreateInput(int inputNumber, 
            ConnectorOrientation orientation, Vector2 position)
        {
            if(inputNumber >= InCount) throw new IndexOutOfRangeException();
            var connector = new Connector(SimpleShader, orientation, ConnectorType.Input);
            connector.Translate = position;
            InputConnectors[inputNumber] = connector;
            Childrens.Add(connector);
        }

        protected void CreateOutput(int outputNumber,
            ConnectorOrientation orientation, Vector2 position)
        {
            if (outputNumber >= OutCount) throw new IndexOutOfRangeException();
            var connector = new Connector(SimpleShader, orientation, ConnectorType.Output);
            connector.Translate = position;
            OutputConnectors[outputNumber] = connector;
            Childrens.Add(connector);
        }


        public override Vector2 InputPosition(int num)
        {
            if (num >= InCount) throw new IndexOutOfRangeException();
            return Translate + InputConnectors[num].Translate;
        }

        public override Vector2 OutputPosition(int num)
        {
            if (num >= OutCount) throw new IndexOutOfRangeException();
            return Translate + OutputConnectors[num].Translate;
        }

        public bool MouseSelectInput(Vector2 mousePos, out int inIndex)
        {
            for (int i = 0; i < InCount; i++)
            {
                var div = InputPosition(i) - mousePos;
                if (Inputs[i] == null && 
                    Math.Abs(div.X) * 2 < Delta &&
                    Math.Abs(div.Y) * 2 < Delta)
                {
                    inIndex = i;
                    return true;
                }
            }
            inIndex = -1;
            return false;
        }

        public bool MouseSelectOutput(Vector2 mousePos, out int outIndex)
        {
            for (int i = 0; i < OutCount; i++)
            {
                var div = OutputPosition(i) - mousePos;
                if (Outputs[i] == null &&
                    Math.Abs(div.X) * 2 < Delta &&
                    Math.Abs(div.Y) * 2 < Delta)
                {
                    outIndex = i;
                    return true;
                }
            }
            outIndex = -1;
            return false;
        }
    }
}
