using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Timers;
using LinearCodes.Textured;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace LinearCodes
{
    public class Program: GameWindow
    {
        private double grad;

        private InseparableCode InseparableCode;
        private Field Field;
        private SimpleShader SimpleShader;
        private TextureShader TextureShader;
        private Texture2D MyTexture2D;
        private Sprite MySprite;

        public RadialMenu menu;

        private float _scale = 1.0f;

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (value == _scale) return;
                _scale = value;
                UpdateprojectionMatrix();
            }
        }

        private Vector2 _translate;
        public Vector2 Translate
        {
            get { return _translate; }
            set
            {
                if (value == _translate) return;
                _translate = value;
                
               // _translate = Vector2.ComponentMax(Vector2.Zero, _translate); 
                UpdateprojectionMatrix();
            }
        }



        public Program()
            : base(900, 700, new GraphicsMode(32, 24, 8, 8,new ColorFormat(24),2,false), "Titul", GameWindowFlags.Default, DisplayDevice.Default, 3,3,GraphicsContextFlags.Default)
        {
            
             //VSync = VSyncMode.On;
            // Timer timer = new Timer(1000);
            // timer.Elapsed += (s, e) => Console.WriteLine($"FPS: {RenderFrequency:F2}");
            // timer.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            SimpleShader = new SimpleShader("Shaders\\SimpleVertexShader.glsl", "Shaders\\SimpleFragmentShader.glsl");

            TextureShader = new TextureShader("Shaders\\TexturedVertexShader.glsl", "Shaders\\TexturedFragmentShader.glsl");
            TextureShader.UniformSampler2D.Value = 0;
            MyTexture2D = new Texture2D();
            MyTexture2D.Generate(new Bitmap("Resources\\bitmap.png"));
            MySprite = new Sprite(MyTexture2D, new SpriteRenderer(), TextureShader);
            MySprite.TexturePostiton = new Vector4(0.0f,0.0f,1.0f,1.0f);
            MySprite.Size = new Vector2(512, 512*0.5f);
            MySprite.UpdateUniforms();

            InseparableCode = new InseparableCode(new[]{ true, true, true, false, true} , SimpleShader);
            //LinearMachine =new MatrixToVisual(new[,]
            //{
            //    {true,  true,  true, },
            //    {true, true,   true,},
            //    {false, true,  true, },
            //    {true,  true,  false, },
            //    {true,  true,  true, }
            //}, SimpleShader);

            
            //menu = new RadialMenu(SimpleShader);
            Field = new Field(Width, Height, SimpleShader);
            menu = new RadialMenu(Field, SimpleShader);

            

            var delta = 10;

           
            
            GL.ClearColor(Color.Beige);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
          
            //  GL.PointSize(2);


            SimpleShader.UniformModelMatrix.Value = new Matrix3x2(1, 0, 0, 1, 0, 0);
            

            Random r = new Random();

            Vector2 coordMouseDown = new Vector2(0,0);
            bool mouseScrollClicked = false;
            Vector2 oldTranslate = Translate;
           
            MouseDown += (s, a) =>
            {
                Vector2 mousePos = PointToMouseCoord(a.Position);
                Vector2 mouseFieldPos = PositionToFieldCoord(mousePos);
                if (a.Button == MouseButton.Middle)
                {
                    coordMouseDown = mousePos;
                    mouseScrollClicked = true;
                    oldTranslate = Translate;
                }
                if (a.Button == MouseButton.Left)
                {
                    InseparableCode.MouseDown(mouseFieldPos);
                    Field.MouseDown(mouseFieldPos);
                }
                if (a.Button == MouseButton.Right)
                {
                    menu.Translate = mouseFieldPos;
                    menu.Visible = true;
                }
            };
            MouseMove += (s, a) =>
            {
                Vector2 mousePos = PointToMouseCoord(a.Position);
                Vector2 mouseFieldPos = PositionToFieldCoord(mousePos);
                if (mouseScrollClicked)
                {
                    Translate = oldTranslate + coordMouseDown - mousePos;
                }
                InseparableCode.MouseMove(mouseFieldPos);
                Field.MouseMove(mouseFieldPos);
                if (menu.Visible)
                {
                    menu.MouseMove(mouseFieldPos);
                }
            };
            MouseUp += (s, a) =>
            {
                Vector2 mousePos = PointToMouseCoord(a.Position);
                Vector2 mouseFieldPos = PositionToFieldCoord(mousePos);
                if (a.Button == MouseButton.Left)
                {
                    Field.MouseUp(mouseFieldPos);
                }
                if (a.Button == MouseButton.Middle)
                {
                    mouseScrollClicked = false;
                }
                if (a.Button == MouseButton.Right)
                {
                    
                    menu.MouseUp(mouseFieldPos);
                    menu.Visible = false;
                }
            };

            MouseWheel += (s, a) =>
            {
                Vector2 mousePos = new Vector2(a.Position.X, Height - a.Position.Y);
                
                var max = 2.0f;
                var min = 1.0f;

                float newScale = Scale * (1+ 0.1f * a.Delta);
                if (newScale > max) newScale = max;
                if (newScale < min) newScale = min;
                var newTranslate = (mousePos + Translate)*newScale/Scale - mousePos;
                Translate = new Vector2((int)newTranslate.X,(int)newTranslate.Y);

                Scale = newScale;
                //this.Animation("Scale", newScale, 200);
            };

            
            KeyDown += (s, a) =>
            {
                if (a.Key == Key.Space)
                {
                    Field.KeyPressSpace();
                }else if (a.Key == Key.Delete || 
                          a.Key == Key.BackSpace)
                {
                    Field.KeyDownRemove();
                }

            };

            
            Timer timer = new Timer(1000);
            timer.Elapsed += (s, a) =>
            {
                Console.WriteLine($"FPS: {RenderFrequency:F2}, grad = {grad:F2}");
            };
            timer.AutoReset = true;
            timer.Start();
        }

      

        private Vector2 PointToMouseCoord(Point point) => new Vector2(point.X, Height - point.Y);
        private Vector2 PositionToFieldCoord(Vector2 vec) => (vec + Translate)/Scale;




        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateprojectionMatrix();
            GL.Viewport(0, 0, Width, Height);  //Слава Богам, оно почему то работает!
        }

        protected void UpdateprojectionMatrix()
        {
            SimpleShader.UniformProjectionMatrix.Value = OrtographicMatrix3x2(Translate.X/Scale,
                (Width + Translate.X)/Scale, Translate.Y/Scale, (Height + Translate.Y)/Scale);

            TextureShader.UniformProjectionMatrix.Value = Matrix4.CreateOrthographicOffCenter(Translate.X / Scale,
               (Width + Translate.X) / Scale, Translate.Y / Scale, (Height + Translate.Y) / Scale, -1, 1);


        }


        protected Matrix3x2 OrtographicMatrix3x2(float left, float right, float bottom, float top)
        {
            var result = Matrix3x2.Zero;
            var width = right - left;
            var height = top - bottom;
            result.M11 = 2/width;
            result.M22 = 2/height;
            result.M31 = -(right + left)/width;
            result.M32 = -(top + bottom)/height;
            return result;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            
            
            
            //    for (int i = 0; i < 600; i++)
            {
               //Visual.DrawLine(i * 6, new Vector2(20 + i * 2, 20), new Vector2(200 + i * 1.3f + (float)Math.Sin(grad*0.1)*10, 200), Color.Black, Color.Brown);
            }

            //GL.BindBuffer(BufferTarget.ArrayBuffer, vId);
            //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, Marshal.SizeOf<Vertex>()*Visual.Count, Visual.Vertices);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

           
        }


        Stopwatch myStopwatch = new Stopwatch();
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            myStopwatch.Restart();
            AnimationStatic.NextFrame(e.Time * 1000);
            AnimationStatic.NextFrameLambda(e.Time * 1000);
            MySprite.Rotate = (float)grad / 100;
            MySprite.UpdateUniforms();


            grad += e.Time * 10;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //   GL.LoadIdentity();
            GL.Enable(EnableCap.Texture2D);
            MySprite.Draw();
            GL.Disable(EnableCap.Texture2D);
          //  Field.Draw();
          //  InseparableCode.Draw();
          //  menu.Draw();
            
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
            myStopwatch.Stop();
            Title = $"FPS: {RenderFrequency:F2}, grad = {grad:F2}, animation delay = {myStopwatch.ElapsedMilliseconds} ms";
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
                program.Run(0,60);

            }
        }
    }
}
