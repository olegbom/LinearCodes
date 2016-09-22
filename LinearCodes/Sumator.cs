using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class Sumator : DrawingVisual
    {

        public float Width => Delta * 4;
        public float Height => Delta * 4;

        public float X;
        public float Y;

        public float Delta;
        public bool Up = true;
        public bool Down = false;

        public ObservableCollection<Wire> WiresIn { get; } = new ObservableCollection<Wire>();
        public Wire WireOut;

        public Sumator(Shader shader) : base(0, shader)
        {
            InstasingList.Add(new VisualUniforms(Color4.Black));
            WiresIn.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (Wire wire in e.NewItems)
                        wire.ValueChaged += OutWireUpdate;
                if (e.OldItems != null)
                    foreach (Wire wire in e.OldItems)
                        wire.ValueChaged -= OutWireUpdate;
            };
        }

        public void OutWireUpdate(object s, EventArgs e)
        {
            var sum = WiresIn.Count(x => x.Value);
            WireOut.Value = sum % 2 == 1;
        }

        public void CreateBuffer()
        {
            var vertices = new List<Vector4>();

            var center = new Vector2(X + Delta * 2, Y + Delta * 2);
            var round = Round(center, Delta, 2, 60, 0.1f);
            
            
            vertices.AddRange(round);
            var l = new Vector2(X + Delta*1.5f, Y + Delta * 2);
            var r = new Vector2(X + Delta*2.5f, Y + Delta * 2);
            vertices.AddRange(Line(l,r,2));
            vertices.AddRange(Polyline(new[]
            {
                new Vector2(X + Delta/2, Y + Delta*2.3f),
                new Vector2(X + Delta,   Y + Delta*2),
                new Vector2(X + Delta/2, Y + Delta*1.7f),
            },2, 0.1f));



            var u = new Vector2(X + Delta * 2, Y + Delta * 2.5f);
            var d = new Vector2(X + Delta * 2, Y + Delta * 1.5f);
            vertices.AddRange(Line(u, d, 2));
            if (Up)
            {
                vertices.AddRange(Polyline(new[]
                {
                    new Vector2(X + Delta*1.7f, Y + Delta*3.5f),
                    new Vector2(X + Delta*2,    Y + Delta*3),
                    new Vector2(X + Delta*2.3f, Y + Delta*3.5f)
                }, 2,0.1f));
            }

            if (Down)
            {
                vertices.AddRange(Polyline(new[]
                {
                    new Vector2(X + Delta*1.7f, Y + Delta*0.5f),
                    new Vector2(X + Delta*2,    Y + Delta*1),
                    new Vector2(X + Delta*2.3f, Y + Delta*0.5f)
                }, 2,0.1f));
            }

            
            Shape = vertices.ToArray();
            InitBuffers();
        }
    }
}
