﻿using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class Wire: DrawingVisual
    {

        static readonly Color4 ZeroColor4 = Color4.Red;
        static readonly Color4 OneColor4 = new Color4(0, 0.8f, 0, 1);

        public event EventHandler ValueChaged;

        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
               
                if (_value == value) return;
                _value = value;
                this.Animation("Color", _value ? OneColor4 : ZeroColor4, 200, 
                    () => {
                        ValueChaged?.Invoke(this, new EventArgs());
                    });
            }
        }

        public Color4 Color
        {
            get { return InstasingList[0].Color; }
            set { InstasingList[0].Color = value; }
        }

        public float Thickness = 2.0f;
        public float PointRadius = 3.0f;
        private List<Vector2> vertices = new List<Vector2>();

        public Wire(SimpleShader shader) : base(shader)
        {
            InstasingList.Add(new VisualUniforms(ZeroColor4));    
        }

        public void AddLine(Vector2 v1, Vector2 v2)
        {
            vertices.AddRange(Line(v1, v2, Thickness));
        }

        public void AddPolyline(Vector2[] points)
        {
            vertices.AddRange(Polyline(points, Thickness));
        }

        public void AddPoint(Vector2 v1)
        {
            vertices.AddRange(Circle(v1, PointRadius, 30));
        }

        public void AddPoint(Vector2 v1, float radius)
        {
            int resolution = (int)radius * 3;
            vertices.AddRange(Circle(v1, radius, resolution));
        }

        public void CreateBuffer()
        {
            Shape = vertices.ToArray();
        }

    }
}
