using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class Button : DrawingVisual
    {
        static readonly Color4 DefaultColor4 = new Color4(0, 0.5f, 0, 0.5f);
        static readonly Color4 MoveColor4 = new Color4(0, 0.6f, 0, 0.7f);
        static readonly Color4 PressColor4 = new Color4(0, 0.3f, 0, 0.8f);

        public float Radius { get; }

        public Color4 Color
        {
            get { return InstasingList[0].Color; }
            set { InstasingList[0].Color = value; }
        }

        public Button(float radius, SimpleShader simpleShader) : base(simpleShader)
        {
            Radius = radius;
            InstasingList.Add(new VisualUniforms(DefaultColor4));
            Shape = Circle(new Vector2(0,0), radius, 60);

        }

        Vector2 oldMouseCoord = Vector2.Zero;
        public void MouseMove(Vector2 mouseCoord)
        {
            bool isWithing = (mouseCoord - Translate).LengthSquared < Radius * Radius;
            bool isOldWithing = (oldMouseCoord - Translate).LengthSquared < Radius * Radius;
            if (isWithing && !isOldWithing)
                this.Animation("Color", MoveColor4, 100);
            else if (isOldWithing && !isWithing)
                this.Animation("Color", DefaultColor4, 100);

            oldMouseCoord = mouseCoord;
        }

        public void MouseDown(Vector2 mouseCoord)
        {
            if ((mouseCoord - Translate).LengthSquared < Radius*Radius)
            {
                this.Animation("Color", PressColor4, 100, 
                    () => this.Animation("Color", DefaultColor4, 100));
                Click?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler Click;
        
           
        
    }
}
