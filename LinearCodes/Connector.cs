using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public enum ConnectorOrientation
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    public enum ConnectorType
    {
        Input = 0,
        Output = 1
    }

    public class Connector: DrawingVisual
    {
        
        public ConnectorOrientation Orientation { get; }
        public ConnectorType Type {get;}

        public float Delta { get; } = 10;

        public Connector(SimpleShader simpleShader, ConnectorOrientation orientation, ConnectorType type) 
            : base(simpleShader)
        {
            Orientation = orientation;
            Type = type;
            InstasingList.Add(new VisualUniforms(Color4.Black));
            var vertices = new List<Vector2>();
            vertices.AddRange(Circle(new Vector2(0, 0),3, 12));
            vertices.AddRange(Line(new Vector2(0, 0),new Vector2(Delta, 0), 2));
            switch (Type)
            {
                case ConnectorType.Input:
                    vertices.AddRange(Polyline(new[]
                    {
                        new Vector2(Delta/2, Delta*0.3f),
                        new Vector2(Delta*0.9f,   0),
                        new Vector2(Delta/2,-Delta*0.3f),
                    }, 2));
                    break;
                case ConnectorType.Output:
                    vertices.AddRange(Polyline(new[]
                    {
                        new Vector2(Delta/2, Delta*0.3f),
                        new Vector2(Delta*0.1f,   0),
                        new Vector2(Delta/2,-Delta*0.3f),
                    }, 2));
                    break;
            }
            Shape = vertices.ToArray();
            switch (Orientation)
            {
                case ConnectorOrientation.Left:
                    Rotate = (float)0;
                    break;
                case ConnectorOrientation.Top:
                    Rotate = (float)Math.PI*3/2;
                    break;
                case ConnectorOrientation.Right:
                    Rotate = (float)Math.PI;
                    break;
                case ConnectorOrientation.Bottom:
                    Rotate = (float)Math.PI/2;
                    break;
            }
            
        }

       
    }
}
