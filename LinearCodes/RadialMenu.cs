using System;
using System.Collections.Generic;
using LinearCodes.Streamings;
using OpenTK;
using OpenTK.Graphics;

namespace LinearCodes
{
    public class RadialMenu: DrawingVisual
    {
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if(_visible == value) return;
                _visible = value;
                
                this.Animation("Scale", new Vector2(_visible ? 1:0),200);
                if (!_visible)
                {
                    if (_oldIndex != -1)
                        Sectors[_oldIndex].InstasingList[0].Animation("Color", Color4.AliceBlue, 200);
                }
            }
        }

        public readonly Field Field;

        protected DrawingVisual RoundGrid;
        protected DrawingVisual Arrow;
        protected DrawingVisual[] Sectors;
        protected StreamingRegister Register;
        protected StreamingSummator Summator;
        protected StreamingSplitter Splitter;

        public int ItemsCount { get; } = 4;
        public int Delta { get; set; } = 10;
        public float RadiusHole { get; set; } = 30f;
        public float RadiusMenu { get; set; } = 80f;
        public float MiddleRadius => (RadiusMenu + RadiusHole)/2;
       


        public RadialMenu(Field field, SimpleShader simpleShader): base(simpleShader)
        {
            Field = field;
            Scale = new Vector2(0,0);

            RoundGrid = new DrawingVisual(SimpleShader);
            RoundGrid.InstasingList.Add(new VisualUniforms(Color4.Black));
            var vertices = new List<Vector2>();
            vertices.AddRange(Round(new Vector2(0, 0), RadiusMenu, 2, 120));
            vertices.AddRange(Round(new Vector2(0, 0), RadiusHole, 2, 120));
            for (double i = 0; i < Math.PI*2; i += Math.PI*2/ItemsCount)
            {
                var arg = i + Math.PI/ItemsCount;
                float sin = (float)Math.Sin(arg);
                float cos = (float)Math.Cos(arg);

                vertices.AddRange(Line(
                    new Vector2(cos, sin)* RadiusHole,
                    new Vector2(cos, sin)* RadiusMenu,
                    2));
            }
            RoundGrid.Shape = vertices.ToArray();
            Childrens.Add(RoundGrid);

            Arrow = new DrawingVisual(SimpleShader);
            Arrow.InstasingList.Add(new VisualUniforms(Color4.Black));
            ArrowUpdate(RadiusHole);
            Childrens.Add(Arrow);

            Sectors = new DrawingVisual[ItemsCount];
            for (int i = 0; i < ItemsCount; i++)
            {
                Sectors[i] = new DrawingVisual(SimpleShader);
                Sectors[i].InstasingList.Add(new VisualUniforms(Color4.AliceBlue));
                vertices = new List<Vector2>();
                float arg = (float)(i * Math.PI * 2 / ItemsCount + Math.PI / ItemsCount);
                vertices.AddRange(Sector(new Vector2(0, 0), MiddleRadius, arg,
                    arg + (float)Math.PI*2 / ItemsCount, RadiusMenu - RadiusHole,20));

                Sectors[i].Shape = vertices.ToArray();
                Childrens.Add(Sectors[i]);

            }

            CreateRegister();

            CreateSummator();
            CreateSplitter();

            // Register = new StreamingRegister(simpleShader, Visuals);
            // Register.Delta = Delta;
            // Register.Position = Translate + new Vector2(0, MiddleRadius);
        }

        private void CreateRegister()
        {
            Register = new StreamingRegister(SimpleShader);
            var cos = MiddleRadius * (float)Math.Cos(Math.PI / ItemsCount * 2);
            var sin = MiddleRadius * (float)Math.Sin(Math.PI / ItemsCount * 2);
            Register.Translate = new Vector2(cos - Delta*3, sin - Delta*1.5f);
            Childrens.Add(Register);
        }

        private void CreateSummator()
        {
            Summator = new StreamingSummator(SimpleShader, 2);
            float arg = (float)(Math.PI * 2 / ItemsCount + Math.PI / ItemsCount * 2);
            var cos = MiddleRadius * (float)Math.Cos(arg);
            var sin = MiddleRadius * (float)Math.Sin(arg);
            Summator.Translate = new Vector2(cos - Delta * 2, sin - Delta * 2);
            Childrens.Add(Summator);
        }

        private void CreateSplitter()
        {
            Splitter = new StreamingSplitter(SimpleShader);
            float arg = (float)(2 * Math.PI * 2 / ItemsCount + Math.PI / ItemsCount * 2);
            var cos = MiddleRadius * (float)Math.Cos(arg);
            var sin = MiddleRadius * (float)Math.Sin(arg);
            Splitter.Translate = new Vector2(cos, sin);
            Childrens.Add(Splitter);
        }

        private void ArrowUpdate(float arrowLenght)
        {
            var vertices = new List<Vector2>();

            vertices.AddRange(Polyline(new[] {
                new Vector2(7, arrowLenght-10),
                new Vector2(0, arrowLenght-2),
                new Vector2(-7, arrowLenght-10),
            }, 2));
            vertices.AddRange(Line(new Vector2(0, 0), new Vector2(0, arrowLenght - 2), 2));
            Arrow.Shape = vertices.ToArray();
        }

        private int _oldIndex = -1;
        public void MouseMove(Vector2 mousePos)
        {
            var vec = mousePos - Translate;
            Arrow.Rotate = (float)(Math.Atan2(vec.Y, vec.X) - Math.PI/2);
            var lenght = vec.Length;
            var arrowLenght = lenght;
            if (arrowLenght > RadiusHole) arrowLenght = RadiusHole;
            ArrowUpdate(arrowLenght);


            int index = -1;
            if (lenght > RadiusHole && lenght < RadiusMenu)
            {
                index = (int) Math.Ceiling((Arrow.Rotate - Math.PI*2/ItemsCount)/Math.PI/2*ItemsCount + 0.5);
                if (index >= ItemsCount) index = 0;
                while (index < 0) index += ItemsCount;
                if (index != _oldIndex)
                {
                    if (_oldIndex != -1)
                        Sectors[_oldIndex].InstasingList[0].Animation("Color", Color4.AliceBlue, 200);
                    Sectors[index].InstasingList[0].Animation("Color", Color4.CornflowerBlue, 200);
                }
            }
            else 
            {
                if (_oldIndex != -1)
                    Sectors[_oldIndex].InstasingList[0].Animation("Color", Color4.AliceBlue, 200);
            }
            _oldIndex = index;
        }

        public void MouseUp(Vector2 mousePos)
        {
            var vec = mousePos - Translate;
            var lenght = vec.Length;
            if (lenght > RadiusHole && lenght < RadiusMenu)
            {
                var index = (int) Math.Ceiling((Arrow.Rotate - Math.PI*2/ItemsCount)/Math.PI/2*ItemsCount + 0.5);
                if (index >= ItemsCount) index = 0;
                while (index < 0) index += ItemsCount;
                AddStreamingVisualToFiled(index);
            }
        }

        private void AddStreamingVisualToFiled(int index)
        {
            switch (index)
            {
                case 0:
                    Field.AddingStreamingVisual(Register, Translate);
                    Childrens.Remove(Register);
                    CreateRegister();
                    break;
                case 1:
                    Field.AddingStreamingVisual(Summator, Translate);
                    Childrens.Remove(Summator);
                    CreateSummator();
                    break;
                case 2:
                    Field.AddingStreamingVisual(Splitter, Translate);
                    Childrens.Remove(Splitter);
                    CreateSplitter();
                    break;
                case 3:
                    break;
            }
        }
    }
}
