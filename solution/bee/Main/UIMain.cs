using Bee.Integrator;
using Bee.UI;
using Bee.UI.Types;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using Bee.Library;

namespace Bee
{
    public class MainUI
    {
        [STAThread]
        public static void Main()
        {
            Utils.PrintSourceTreeStatistics(@"..\solution\bee\");

            /*
            Graph g = new Graph();
            g.AddBidirectionalEdge(new Vertex(2, 1), new Vertex(2, 5));
            g.AddBidirectionalEdge(new Vertex(2, 1), new Vertex(3, 1));
            g.AddBidirectionalEdge(new Vertex(2, 5), new Vertex(4, 5));
            g.AddBidirectionalEdge(new Vertex(3, 1), new Vertex(6, 1));
            g.AddBidirectionalEdge(new Vertex(3, 1), new Vertex(4, 5));
            g.AddBidirectionalEdge(new Vertex(4, 5), new Vertex(7, 5));
            g.AddBidirectionalEdge(new Vertex(7, 5), new Vertex(7, 2));
            g.AddBidirectionalEdge(new Vertex(7, 5), new Vertex(9, 5));
            g.AddBidirectionalEdge(new Vertex(9, 5), new Vertex(9, 4));
            g.AddBidirectionalEdge(new Vertex(9, 4), new Vertex(7, 2));
            g.AddBidirectionalEdge(new Vertex(9, 4), new Vertex(9, 1));
            g.AddBidirectionalEdge(new Vertex(9, 1), new Vertex(7, 1));
            g.AddBidirectionalEdge(new Vertex(7, 1), new Vertex(7, 2));
            g.AddBidirectionalEdge(new Vertex(7, 1), new Vertex(6, 1));
            g.AddBidirectionalEdge(new Vertex(7, 2), new Vertex(6, 1));

            ListCollection<ListCollection<Vertex>> list = new GraphCycles().ComputeCycleBasisOfGraph(g);
            for (int i = 0; i < list.Size(); i++)
            {
                ListCollection<Vertex> circle = list[i];
                for (int j = 0; j < circle.Size(); j++)
                {
                    Console.Write(circle[j]);
                }
                Console.WriteLine();
            }

            Console.ReadKey();
            */

            new MainUI().RenderCycle();
        }

        public void RenderCycle()
        {
            float aspect_ratio = 1;

            IntegratorView IntegratorView = null;
            SurfaceDraw SurfaceDraw = null;

            GameWindow gameWindow = null;
            using (gameWindow = new GameWindow(1250, 750, new OpenTK.Graphics.GraphicsMode(32, 24, 0, 4)))
            {
                gameWindow.Load += (sender, e) =>
                {
                    gameWindow.VSync = VSyncMode.On;
                    Input.Inititialize(gameWindow);
                    IntegratorView = new IntegratorView();
                    //SurfaceDraw = new SurfaceDraw();
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

                    if (SurfaceDraw != null)
                        SurfaceDraw.Draw();

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