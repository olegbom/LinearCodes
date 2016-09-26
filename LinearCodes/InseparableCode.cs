using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearCodes
{
    public class InseparableCode: DrawingVisual
    {
        public StreamingRegister[] Registers { get; }

        public StreamingSummator[] Summators { get; }
        public StreamingSplitter[] Splitters { get; }

        public static int Delta = 10;

        public Button ButtonTickZero { get; }
        public Button ButtonTickOne { get; }

        public StreamingSource streamingSource;
        public StreamingReceiver streamingReceiver;

        public Glyph7x5 TestBit;

        public InseparableCode(bool[] gx, SimpleShader simpleShader): base (simpleShader)
        {
           // if (gx.Length != 4) throw new Exception("Неправильный размер массива");
            int gCount = gx.Length;

            streamingSource = new StreamingSource(new[] { 0, 1, 1, 0 }, simpleShader)
            {
                Delta = Delta,
                Translate = new Vector2(Delta*1, Delta*11)
            };

            Childrens.Add(streamingSource);
           

            #region Register
            int regCount = gx.Length-1;
            Registers = new StreamingRegister[regCount];
            for (int i = 0; i < regCount; i++)
            {
                Registers[i] = new StreamingRegister(simpleShader)
                {
                    Delta = Delta,
                    Translate = new Vector2(Delta * 8 + i * Delta * 8, Delta * 10)
                };
                Childrens.Add(Registers[i]);
            }
            
            #endregion


            #region Sumators
            int sumCount = gx.Count(x => x) - 1;

            Splitters = new StreamingSplitter[sumCount];
            Splitters[0] = new StreamingSplitter(simpleShader)
                { Translate = new Vector2(Delta * 7, Delta * 11) };
            Childrens.Add(Splitters[0]);
            Summators = new StreamingSummator[sumCount];
            for (int i = 1, j = 0; i < gx.Length; i++)
            {
                if (!gx[i]) continue;

                Summators[j] = new StreamingSummator(simpleShader, 2)
                {
                    Up = true,
                    Delta = Delta,
                    Translate = new Vector2(Delta * 5 + i * Delta * 8, Delta * 3)
                };
                Childrens.Add(Summators[j]);
                j++;
                if (j >= sumCount) continue;
                Splitters[j] = new StreamingSplitter(simpleShader)
                {
                    Translate = new Vector2(Delta * 7 + i * Delta * 8, Delta * 11)
                };
                Childrens.Add(Splitters[j]);
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

            streamingReceiver = new StreamingReceiver(simpleShader)
            {
                Delta = Delta,
                Translate = new Vector2(Delta*2 + Delta*gx.Length*8, Delta*5)
            };
            Childrens.Add(streamingReceiver);
            ConnectWire(streamingSource, 0, Splitters[0], 0);
            ConnectWire(Splitters[0], 0, Summators[0], 0, 
                new[] { new Vector2(Delta * 7, Delta * 5) });
            ConnectWire(Splitters[0], 1, Registers[0], 0);
            ConnectWire(Registers.Last(), 0, Summators.Last(), 1,
                new[] { new Vector2(Delta * gx.Length * 8 - Delta*1, Delta * 11) });

            ConnectWire(Summators.Last(), 0, streamingReceiver, 0);


            ButtonTickZero = new Button(Delta, simpleShader) {Translate = new Vector2(Delta*4, Delta*10)};
            Childrens.Add(ButtonTickZero);

            TestBit = new Glyph7x5('0', new Vector2(Delta * 3, Delta * 3), simpleShader);
            Childrens.Add(TestBit);
            ButtonTickZero.Click += (s, e) =>
            {
                TestBit.Char = TestBit.Char == '0' ? '=' : '0';
             
            };

            ButtonTickOne = new Button(Delta, simpleShader) {Translate = new Vector2(Delta*2, Delta*10)};
            Childrens.Add(ButtonTickOne);

            

            ButtonTickOne.Click += (s, e) =>
            {
                streamingSource.Start();
            };

        }

        public void ConnectWire(StreamingVisual from, int outNum, StreamingVisual to, int inNum, Vector2[] middlePoints)
        {
            StreamingWire wire = new StreamingWire(from.SimpleShader);
            Childrens.Add(wire);
            
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
            StreamingWire wire = new StreamingWire(from.SimpleShader);
            Childrens.Add(wire);
            List<Vector2> points = new List<Vector2>
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
    }
}
