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


        public Register[] Registers { get; }
        public Wire[] gWires { get; }

        public List<Wire> OutWires { get; } = new List<Wire>();
        public Sumator[] Sumators { get; }

        public static int Delta = 20;

        public Button ButtonTickZero { get; }
        public Button ButtonTickOne { get; }

        public StreamingWire streamingWire;
        public StreamingWire streamingWire2;
        public StreamingWire streamingWire3;
        public StreamingWire streamingWire4;
        public StreamingWire streamingWire5;
        public StreamingSplitter streamingSplitter;
        public StreamingRegister streamingRegister;
        public StreamingSource streamingSource;
        public StreamingSummator streamingSummator;
        public StreamingReceiver streamingReceiver;


        public InseparableCode(bool[] gx, Shader shader)
        {
            if (gx.Length != 4) throw new Exception("Неправильный размер массива");
            int gCount = gx.Length;

            #region Register
            int regCount = 3;
            Registers = new Register[3];
            for (int i = 0; i < regCount; i++)
            {
                Registers[i] = new Register(shader)
                {
                    Delta = Delta,
                    X = Delta * 10 + i * Delta * 10,
                    Y = Delta * 10,
                };
            }
            
            #endregion


            #region Sumators
            int sumCount = gx.Count(x => x) - 1;

            Sumators = new Sumator[sumCount];
            for (int i = 1, j = 0; i < gx.Length; i++)
            {
                if (!gx[i]) continue;

                Sumators[j] = new Sumator(shader)
                {
                    X = Delta * 6 + i * Delta * 10,
                    Y = Delta * 3,
                    Up = true,
                    Delta = Delta,
                };
                Sumators[j].CreateBuffer();

                j++;
            }
            Visuals.AddRange(Sumators);
            #endregion

            #region Wires
            gWires = new Wire[gCount];



            for (int i = 0; i < gCount; i++)
                gWires[i] = new Wire(shader);

            gWires[0].AddLine(
                    new Vector2(Delta * 6, Delta * 11),
                    new Vector2(Delta * 10, Delta * 11));
            gWires[0].AddPoint(new Vector2(Delta * 8, Delta * 11));
            gWires[0].AddPolyline(new[]
            {
                new Vector2(Delta*8, Delta*11),
                new Vector2(Delta*8, Delta*5),
                new Vector2(Delta*17, Delta*5),
            });


            for (int i = 1; i < gCount - 1; i++)
            {

                gWires[i].AddLine(
                    new Vector2(Delta * 6 + i * Delta * 10, Delta * 11),
                    new Vector2(Delta * 10 + i * Delta * 10, Delta * 11));
                if (!gx[i]) continue;
                gWires[i].AddPoint(new Vector2(Delta * 8 + i * Delta * 10, Delta * 11));
                gWires[i].AddLine(
                    new Vector2(Delta * 8 + i * Delta * 10, Delta * 11),
                    new Vector2(Delta * 8 + i * Delta * 10, Delta * 6));

            }

            var glast = gCount - 1;

            gWires[glast].AddPolyline(new[]
            {
                new Vector2(Delta*6 + glast * Delta * 10, Delta*11),
                new Vector2(Delta*8 + glast * Delta * 10, Delta*11),
                new Vector2(Delta*8 + glast * Delta * 10, Delta*6),
            });

            for (int i = 0; i < gCount; i++)
                gWires[i].CreateBuffer();
            
            Visuals.AddRange(gWires);
            Wire wire;
           
            
            for (int i = 1; i < gCount - 1; i++)
            {
                wire = new Wire(shader);
                var iSkip = 0;
                while (!gx[i + iSkip + 1])
                {
                    iSkip++;
                }
                wire.AddLine(
                    new Vector2(Delta * 9 + i * Delta * 10, Delta * 5),
                    new Vector2(Delta * 17 + (i+iSkip) * Delta * 10, Delta * 5));
                wire.CreateBuffer();
                OutWires.Add(wire);
                
                i += iSkip;
            }
            
            wire = new Wire(shader);
            wire.AddLine(
                new Vector2(Delta * 9 + glast * Delta * 10, Delta * 5),
                new Vector2(Delta * 13 + glast * Delta * 10, Delta * 5));
            wire.AddPolyline(new[]
            {
                new Vector2(Delta * 12 + glast * Delta * 10, Delta * 5.5f),
                new Vector2(Delta * 13 + glast * Delta * 10, Delta * 5),
                new Vector2(Delta * 12 + glast * Delta * 10, Delta * 4.5f)
            });
            wire.CreateBuffer();
            OutWires.Add(wire);
            
            Visuals.AddRange(OutWires);
            #endregion

            for (int i = 0; i < OutWires.Count; i++)
                Sumators[i].WireOut = OutWires[i];
            for(int i = 1; i < Sumators.Length; i++)
            {
                Sumators[i].WiresIn.Add(Sumators[i - 1].WireOut);
            }
            Sumators[0].WiresIn.Add(gWires[0]);
            
            for (int i = 1, si = 0 ; i < gCount; i++)
            {
                if (!gx[i]) continue;
                Sumators[si].WiresIn.Add(gWires[i]);
                si++;
            }


            for (int i = 0; i < regCount; i++)
            {
                Registers[i].WireIn = gWires[i];
                Registers[i].WireOut = gWires[i+1];
                Registers[i].CreateBuffer();
                Visuals.AddRange(Registers[i].Visuals);
            }

            ButtonTickZero = new Button(Delta, shader);
            ButtonTickZero.Position = new Vector2(Delta * 4, Delta * 10);
            Visuals.Add(ButtonTickZero);
            ButtonTickZero.Click += (s, e) =>
            {
                gWires[0].Value = false;
                foreach(Register reg in Registers)
                {
                    reg.RegisterTick();
                }
            };

            ButtonTickOne = new Button(Delta, shader);
            ButtonTickOne.Position = new Vector2(Delta * 2, Delta * 10);
            Visuals.Add(ButtonTickOne);




            streamingSource = new StreamingSource(shader, Visuals);
           
            streamingSource.message.AddRange(new[] { 0, 1, 1, 1});
            streamingSource.CreateBuffers(new Vector2(Delta * 2, Delta * 15));
            streamingSource.Start();

            Vector2[] points = 
            {
                new Vector2(Delta*3, Delta*15), 
                new Vector2(Delta*10, Delta*15), 
            };
       

            

            

            streamingSplitter = new StreamingSplitter(shader, Visuals);
            streamingSplitter.Position = new Vector2(Delta * 10, Delta * 15);
            
            streamingRegister = new StreamingRegister(shader, Visuals);

            streamingRegister.Delta = Delta;
            streamingRegister.CreateBuffer(new Vector2(Delta*13,Delta*14));
                   
            streamingSummator = new StreamingSummator(shader, Visuals, 2);
            streamingSummator.Delta = Delta;
            streamingSummator.CreateBuffer(new Vector2(Delta*21,Delta*13));



           

            streamingReceiver = new StreamingReceiver(shader, Visuals);
            streamingReceiver.Delta = Delta;
            streamingReceiver.CreateBuffers(new Vector2(Delta*27, Delta*15));

            ConnectWire(streamingSource, 0, streamingSplitter, 0);
            ConnectWire(streamingSplitter, 1, streamingRegister, 0);
            ConnectWire(streamingSplitter, 0, streamingSummator, 1,
                new[]{
                    new Vector2(Delta * 10, Delta * 20),
                    new Vector2(Delta * 23, Delta * 20),
                });
            ConnectWire(streamingRegister, 0, streamingSummator, 0);
            ConnectWire(streamingSummator, 0, streamingReceiver, 0);
            
            
            

            ButtonTickOne.Click += (s, e) =>
            {
                streamingSource.Start();
                gWires[0].Value = true;
                foreach (Register reg in Registers)
                {
                    reg.RegisterTick();
                }
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
