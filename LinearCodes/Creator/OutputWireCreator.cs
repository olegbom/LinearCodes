using System.Collections.Generic;
using LinearCodes.Streamings;
using OpenTK;

namespace LinearCodes.Creator
{
    public class OutputWireCreator : WireCreator
    {
        public OutputWireCreator(StreamingComponent visual, int pinIndex, EmploymentMatrix employmentMatrix) : base(visual, pinIndex, employmentMatrix)
        {
            PinPosition = visual.OutputPosition(pinIndex);
            Wire.Path = new List<Vector2> { PinPosition, PinPosition, PinPosition };
        }

        public override Vector2 FirstPoint => PinPosition;
        public override Vector2 LastPoint => MouseMovePos;

        public override void Connecting()
        {
            Visual.ConnectTo(PinIndex, Wire, 0);
        }
    }
}
