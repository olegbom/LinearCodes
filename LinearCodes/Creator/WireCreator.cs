﻿using System;
using System.Collections.Generic;
using LinearCodes.Streamings;
using OpenTK;

namespace LinearCodes.Creator
{
    public abstract class WireCreator : IDisposable
    {
        public float Delta { get; } = 10;
        public static bool SourceFirst = true;
        public StreamingWire Wire { get; private set; }
        public StreamingComponent Visual { get; private set; }
        public Vector2 PinPosition { get; protected set; }
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
        public int PinIndex { get; }

        public abstract Vector2 FirstPoint { get; }
        public abstract Vector2 LastPoint { get; }

        

        public EmploymentMatrix EmploymentMatrix { get; }

        

        protected WireCreator(StreamingComponent visual, int pinIndex, EmploymentMatrix employmentMatrix)
        {
            Visual = visual;
            PinIndex = pinIndex;
            Wire = new StreamingWire(visual.SimpleShader);
            EmploymentMatrix = employmentMatrix;
        }

        public void WireUpdate()
        {
            var path = new List<Vector2>();

            var delta =  LastPoint - FirstPoint;
            Vector2 lastPoint = EmploymentMatrix.GoToWall(FirstPoint, 
                Math.Abs(delta.X) > Math.Abs(delta.Y) 
                    ? new Vector2(delta.X, 0) 
                    : new Vector2(0, delta.Y));
            Wire.AnimatedPath = new List<Vector2> {
                        FirstPoint ,
                        lastPoint};
        }

        public abstract void Connecting();

        public void Dispose()
        {
            Wire = null;
            Visual = null;

        }
    }
}
