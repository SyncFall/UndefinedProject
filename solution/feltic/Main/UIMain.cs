using feltic.Integrator;
using feltic.UI;
using feltic.UI.Types;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using feltic.Library;
using feltic.Language;

namespace feltic
{
    public class MainUI
    {
        [STAThread]
        public static void Main()
        {
            Utils.PrintSourceTreeStatistics(@"..\solution\feltic\");
            SourceList list = new SourceList();
            list.Add(SourceText.FromFile("Compose/first.src"));
            Registry registry = new Registry();
            registry.AddSourceList(list);
            registry.WriteToTarget("D:\\dev\\UndefinedProject\\output\\csharp.cs");

            //Visual1 elm = new Visual1();

            new MainUI().RenderCycle();
        }

        public void RenderCycle()
        {
            float aspect_ratio = 1;

            IntegratorView IntegratorView = null;
            
            GameWindow gameWindow = null;
            using (gameWindow = new GameWindow(1250, 750, new OpenTK.Graphics.GraphicsMode(32, 32, 0, 0)))
            {
                gameWindow.Load += (sender, e) =>
                {
                    gameWindow.VSync = VSyncMode.On;
                    Input.Inititialize(gameWindow);
                    IntegratorView = new IntegratorView();
                };

                gameWindow.Resize += (sender, e) =>
                {
                    aspect_ratio = (float)gameWindow.Width / (float)gameWindow.Height;
                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.VertexArray);
                    GL.Enable(EnableCap.TextureCoordArray);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                };

               
                gameWindow.UpdateFrame += (sender, e) =>
                {
                };

                gameWindow.RenderFrame += (sender, e) =>
                {
                    /*
                    todo: multi-threaded, non-blocking environment processing
                    todo: non-blocking render thread
                    new Thread(() =>
                    {
                        IGraphicsContext context = new GraphicsContext(GraphicsMode.Default, gameWindow.WindowInfo);
                        context.MakeCurrent(gameWindow.WindowInfo);

                    }).Start();
                    */

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.ClearColor(30 / 255f, 30 / 255f, 30 / 255f, 1);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, gameWindow.Width, gameWindow.Height, 0, 0, 1);

                    if (IntegratorView != null)
                        IntegratorView.Draw();

                    // frame
                    gameWindow.SwapBuffers();
                };

                // Run the game at 60 updates per second
                gameWindow.Run(60.0);

            };
            Environment.Exit(1);
        }
    }
}