using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class InseparableCode
    {
        public List<DrawingVisual> Visuals = new List<DrawingVisual>();


        public StreamingRegister[] Registers { get; }

        public StreamingSummator[] Summators { get; }
        public StreamingSplitter[] Splitters { get; }

        public static int Delta = 20;

        public Button ButtonTickZero { get; }
        public Button ButtonTickOne { get; }

        public StreamingSplitter streamingSplitter;
        public StreamingRegister streamingRegister;
        public StreamingSource streamingSource;
        public StreamingSummator streamingSummator;
        public StreamingReceiver streamingReceiver;

        public Glyph7x5 TestBit;

        public InseparableCode(bool[] gx, Shader shader)
        {
           // if (gx.Length != 4) throw new Exception("Неправильный размер массива");
            int gCount = gx.Length;

            streamingSource = new StreamingSource(shader, Visuals);

            streamingSource.message.AddRange(new[] { 0, 1, 1, 0 });
            streamingSource.CreateBuffers(new Vector2(Delta * 1, Delta * 11));
           

            #region Register
            int regCount = gx.Length-1;
            Registers = new StreamingRegister[regCount];
            for (int i = 0; i < regCount; i++)
            {
                Registers[i] = new StreamingRegister(shader, Visuals)
                {
                    Delta = Delta,
                };
                Registers[i].CreateBuffer(new Vector2(Delta * 8 + i * Delta * 8, Delta * 10));
            }
            
            #endregion


            #region Sumators
            int sumCount = gx.Count(x => x) - 1;

            Splitters = new StreamingSplitter[sumCount];
            Splitters[0] = new StreamingSplitter(shader, Visuals)
                { Position = new Vector2(Delta * 7, Delta * 11) };

            Summators = new StreamingSummator[sumCount];
            for (int i = 1, j = 0; i < gx.Length; i++)
            {
                if (!gx[i]) continue;

                Summators[j] = new StreamingSummator(shader, Visuals, 2)
                {
                    Up = true,
                    Delta = Delta,
                };
                Summators[j].CreateBuffer(new Vector2(Delta * 5 + i * Delta * 8, Delta * 3));
                            
                j++;
                if (j >= sumCount) continue;
                Splitters[j] = new StreamingSplitter(shader, Visuals)
                {
                    Position = new Vector2(Delta * 7 + i * Delta * 8, Delta * 11)
                };

                ConnectWire(Splitters[j], 0, Summators[j-1], 1);
                
            }
            for(int i = 0; i< sumCount - 1; i++)
            {
                ConnectWire(Summators[i], 0, Summators[i+1], 0);
            }

            for (int i = 1, j = 0; i < gx.Length - 1; i++)
            {
                if (!gx[i])
                {
                    ConnectWire(Registers[i-1], 0, Registers[i], 0);
                }
                else
                {
                    j++;
                    ConnectWire(Registers[i - 1], 0, Splitters[j], 0);
                    ConnectWire(Splitters[j], 1, Registers[i], 0);
                }
            }

            #endregion

            streamingReceiver = new StreamingReceiver(shader, Visuals);
            streamingReceiver.Delta = Delta;
            streamingReceiver.CreateBuffers(new Vector2(Delta * 2  + Delta* gx.Length * 8, Delta * 5));
            ConnectWire(streamingSource, 0, Splitters[0], 0);
            ConnectWire(Splitters[0], 0, Summators[0], 0, 
                new[] { new Vector2(Delta * 7, Delta * 5) });
            ConnectWire(Splitters[0], 1, Registers[0], 0);
            ConnectWire(Registers.Last(), 0, Summators.Last(), 1,
                new[] { new Vector2(Delta * gx.Length * 8 - Delta*1, Delta * 11) });

            ConnectWire(Summators.Last(), 0, streamingReceiver, 0);


            ButtonTickZero = new Button(Delta, shader);
            ButtonTickZero.Position = new Vector2(Delta * 4, Delta * 10);
            Visuals.Add(ButtonTickZero);

            TestBit = new Glyph7x5('0', new Vector2(Delta * 3, Delta * 3), shader);
            Visuals.Add(TestBit);
            ButtonTickZero.Click += (s, e) =>
            {
                TestBit.Char = TestBit.Char == '0' ? '=' : '0';
               /* gWires[0].Value = false;
                foreach(Register reg in Registers)
                {
                    reg.RegisterTick();
                }*/
            };

            ButtonTickOne = new Button(Delta, shader);
            ButtonTickOne.Position = new Vector2(Delta * 2, Delta * 10);
            Visuals.Add(ButtonTickOne);




           


            

            

          
            
            
            

            ButtonTickOne.Click += (s, e) =>
            {
                streamingSource.Start();
                /*gWires[0].Value = true;
                foreach (Register reg in Registers)
                {
                    reg.RegisterTick();
                }*/
            };

        }

        public void ConnectWire(StreamingVisual from, int outNum, StreamingVisual to, int inNum, Vector2[] middlePoints)
        {
            StreamingWire wire = new StreamingWire(from.Shader, Visuals);
            Vector2 posOut = from.OutputPosition(outNum);
            Vector2 posIn = to.InputPosition(inNum);

            List<Vector2> points = new List<Vector2>();
            points.Add(posOut);
            points.AddRange(middlePoints);
            points.Add(posIn);
            wire.CreateBuffers(points);
            from.ConnectTo(outNum, wire, 0);
            wire.ConnectTo(0, to, inNum);
        }

        public void ConnectWire(StreamingVisual from, int outNum, StreamingVisual to, int inNum)
        {
            StreamingWire wire = new StreamingWire(from.Shader, Visuals);
 
            List<Vector2> points = new List<Vector2>()
            {
                 from.OutputPosition(outNum),
                 to.InputPosition(inNum)
            };

            wire.CreateBuffers(points);
            from.ConnectTo(outNum, wire, 0);
            wire.ConnectTo(0, to, inNum);
        }


        public void MouseDown(Vector2 mouseCoord)
        {
            ButtonTickOne.MouseDown(mouseCoord);
            ButtonTickZero.MouseDown(mouseCoord);
        }

        public void MouseMove(Vector2 mouseCoord)
        {
            ButtonTickOne.MouseMove(mouseCoord);
            ButtonTickZero.MouseMove(mouseCoord);
        }

        public void Draw()
        {
            Visuals.ForEach(x => x.Draw());
        }
    }
}
