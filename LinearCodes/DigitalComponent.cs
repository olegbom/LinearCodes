﻿using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class DigitalComponent: DrawingVisual
    {
        public float Width => Delta * 6;
        public float Height => Delta * 2 * PinsCount;

        public float Delta;
        public int PinsCount;
        
        public Vector2 Position
        {
            get { return InstasingList[0].Translate; }
            set { InstasingList[0].Translate = value; }
        }

        public DigitalComponent(Shader shader) : base(0, shader)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
        }

        public void CreateBuffer()
        {
            var vertices = new List<Vector4>();
            var rectV1 = new Vector2(Delta, 0);
            var rectV2 = new Vector2(Delta * 5,PinsCount*Delta*2 );
            var rect = Rectangle(rectV1, rectV2, 2);
            vertices.AddRange(rect);

            Vector2 pointA;
            Vector2 pointB;
            for (int i = 0; i < PinsCount; i++)
            {
                pointA = new Vector2(0, Delta*(2*i + 1));
                pointB = new Vector2(Delta, pointA.Y);
                vertices.AddRange(Line(pointA, pointB, 2f));
                vertices.AddRange(Circle(pointA, 3, 12, 0.1f));
            }
            
            pointB = new Vector2(Delta * 6, PinsCount * Delta);
            pointA = pointB - new Vector2(Delta, 0);
            vertices.AddRange(Line(pointA, pointB, 2f));
            vertices.AddRange(Circle(pointB, 3, 12, 0.1f));

            Shape = vertices.ToArray();
            InitBuffers();
        }
    }
}