using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LinearCodes
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
    }
}
