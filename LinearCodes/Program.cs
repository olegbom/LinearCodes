using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Timers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;
using QuickFont.Configuration;
using SharpFont;

namespace LinearCodes
{
    public class Program: GameWindow
    {
        private double grad = 0;

        private MatrixToVisual LinearMachine;
        private InseparableCode InseparableCode;
        public DrawingVisual Grid;


        public Program()
            : base(900, 700, new GraphicsMode(32, 24, 8, 8), "Titul", GameWindowFlags.Default, DisplayDevice.Default, 2,1,GraphicsContextFlags.Default)
        {
            
             //VSync = VSyncMode.On;
            // Timer timer = new Timer(1000);
            // timer.Elapsed += (s, e) => Console.WriteLine($"FPS: {RenderFrequency:F2}");
            // timer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

           



            Shader = new Shader("SimpleVertexShader.vertexshader", "SimpleFragmentShader.fragmentshader");
            InseparableCode = new InseparableCode(new[]{ true, true, true, false, true} ,Shader);
            //LinearMachine =new MatrixToVisual(new[,]
            //{
            //    {true,  true,  true, },
            //    {true, true,   true,},
            //    {false, true,  true, },
            //    {true,  true,  false, },
            //    {true,  true,  true, }
            //}, Shader);

            Grid = new DrawingVisual(0,Shader);
            var delta = 10;

            List<Vector4> vertices = new List<Vector4>();

            for (int i = delta; i < 700; i += delta)
            {
                vertices.AddRange(DrawingVisual.Line(new Vector2(0 + 0.5f, i + 0.5f), new Vector2(900 + 0.5f, i + 0.5f), 1f, -0.5f));
            }

            for (int i = delta; i < 900; i += delta)
            {
                vertices.AddRange(DrawingVisual.Line(new Vector2(i + 0.5f, 0 + 0.5f), new Vector2(i + 0.5f, 800 + 0.5f), 1f, -0.5f));
            }
            Grid.Shape = vertices.ToArray();
            Grid.InitBuffers();
            Grid.InstasingList.Add(new VisualUniforms(Color.LightGray));
            
            GL.ClearColor(Color.Beige);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //  GL.PointSize(2);

            projectionMatrixUniform = Shader.GetUniformMatrix4("projMatrix");
            modelMartixUniform = Shader.GetUniformMatrix4("modelMatrix");
            modelMartixUniform.Value = Matrix4.Identity;
            

            Random r = new Random();

            MouseDown += (s, a) =>
            {
                if(a.Button == MouseButton.Left)
                {
                    Vector2 mousePos = new Vector2(a.Position.X, Height - a.Position.Y);
                    InseparableCode.MouseDown(mousePos);
                }
            };
            MouseMove += (s, a) =>
            {
                Vector2 mousePos = new Vector2(a.Position.X, Height - a.Position.Y);
                InseparableCode.MouseMove(mousePos);
            };


        }
          
        private Uniform<Matrix4> projectionMatrixUniform;
        private Uniform<Matrix4> modelMartixUniform; 
        private Shader Shader;
      

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            projectionMatrixUniform.Value = Matrix4.CreateOrthographicOffCenter(0, Width, 0, Height, -1, 1);
            GL.Viewport(0, 0, Width, Height);  //Слава Богам, оно почему то работает!
        }

        

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            Title = $"FPS: {RenderFrequency:F2}, grad = {grad:F2}";

            
            //    for (int i = 0; i < 600; i++)
            {
               //Visual.DrawLine(i * 6, new Vector2(20 + i * 2, 20), new Vector2(200 + i * 1.3f + (float)Math.Sin(grad*0.1)*10, 200), Color.Black, Color.Brown);
            }

            //GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, Marshal.SizeOf<Vertex>()*Visual.Count, Visual.Vertices);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

           
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            AnimationStatic.NextFrame(e.Time * 1000);
            grad += e.Time * 10;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //   GL.LoadIdentity();

            // float xTr = 1 + 30 * (float)(1 + Math.Cos(grad * 0.3));
            //  float yTr = 1 + 30 * (float)(1 + Math.Sin(grad * 0.3));
            // modelMartixUniform.Value = Matrix4.CreateTranslation(20,20 + (float)Math.Sin(grad)*5,0);
            // LinearMachine.Draw();
            // Field.X = 100 + (float)Math.Cos(grad/10)*100;
           // LinearMachine.Draw();
            Grid.Draw();
            InseparableCode.Draw();

            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * MaxParticleCount), IntPtr.Zero, BufferUsageHint.StreamDraw);
            //// Fill newly allocated buffer
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexC4ubV3f.SizeInBytes * MaxParticleCount), VBO, BufferUsageHint.StreamDraw);
            //// Only draw particles that are alive
            //GL.DrawArrays(PrimitiveType.TriangleStrip, MaxParticleCount - VisibleParticleCount, VisibleParticleCount);
            // GL.BufferData(BufferTarget.ArrayBuffer, Visual.Size, IntPtr.Zero, BufferUsageHint.StreamDraw);
            // Fill newly allocated buffer
            //  GL.BufferData(BufferTarget.ArrayBuffer, Visual.Size, Visual.Vertices, BufferUsageHint.StreamDraw);
            // Only draw particles that are alive
            //  GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Visual.Count);

            SwapBuffers();
        }



        /// <summary>
        /// Entry point of program.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                // Get the title and category  of this example using reflection.

                program.Title = "Линейные коды";
                program.Run(0.0,60);

            }
        }
    }
}
