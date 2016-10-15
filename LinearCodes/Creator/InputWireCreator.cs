using System.Collections.Generic;
using LinearCodes.Streamings;
using OpenTK;

namespace LinearCodes.Creator
{
    public class InputWireCreator : WireCreator
    {
        public InputWireCreator(StreamingComponent visual, int inputIndex, EmploymentMatrix employmentMatrix) : base(visual, inputIndex, employmentMatrix)
        {
            PinPosition = visual.InputPosition(inputIndex);
            Wire.Path = new List<Vector2> { PinPosition, PinPosition, PinPosition };
        }
        
        public override Vector2 FirstPoint => MouseMovePos;
        public override Vector2 LastPoint => PinPosition;

        public override void Connecting()
        {
            Wire.ConnectTo(0, Visual, PinIndex);
        }
    }
}
