﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Be.Runtime.Types;
using System.IO;
using Be.UI.Types;
using System.Diagnostics;
using Be.UI.Cases;
using Be.Integrator;

namespace Be.UI
{
    public class BeEntry
    {
        [STAThread]
        public static void Main()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Utils.PrintSourceTreeStatistics(@"..\..");

            /*
            FontType fontType = new FontType("Arial Black");

            FontImageAtlas fontImageAtlas = new FontImageAtlas(fontType.GlyphCollection);
            fontImageAtlas.CreateAtlas();

            DistanceField distanceField = new DistanceField();
            distanceField.CreateDistanceField(new Bitmap("ImageMap.png")).Save("DistanceField.png");
            */

            new BeEntry().RenderCycle();

            stopWatch.Stop();
            //Console.WriteLine("\nDone in: " + Utils.FormatMilliseconds(stopWatch.ElapsedMilliseconds));
            //Console.ReadLine();
        }

        public void RenderCycle()
        {
            float aspect_ratio = 1;

            WindowType WindowType = null;

            LineDraw LineDraw = null;
            RectDraw RectDraw = null;
            TrackBox TrackBox = null;
            PolygonDraw PolygonDraw = null;
            CurveDraw CurveDraw = null;
            SurfaceDraw SurfaceDraw = null;
            FontDraw FontDraw = null;
            IntegratorView IntegratorView = null;
            ScreenDraw ScreenDraw = null;

            using (var gameWindow = new GameWindow(1250, 750, new OpenTK.Graphics.GraphicsMode(32, 24, 0, 0)))
            {
                gameWindow.Load += (sender, e) =>
                {
                    gameWindow.VSync = VSyncMode.On;

                    WindowType = new WindowType(gameWindow);
                    Input.Inititialize(gameWindow);
                    IntegratorView = new IntegratorView(WindowType);
                    //FontDraw = new FontDraw(WindowType);
                    //SurfaceDraw = new SurfaceDraw(WindowType);
                    //CurveDraw = new CurveDraw(WindowType);
                    //PolygonDraw = new PolygonDraw(WindowType);
                    //LineDraw = new LineDraw(WindowType);
                    //RectDraw = new RectDraw(WindowType);
                    //TrackBox = new TrackBox(WindowType);
                    //ScreenDraw = new ScreenDraw(WindowType);
                };

                gameWindow.Resize += (sender, e) =>
                {
                    aspect_ratio = (float)gameWindow.Width / (float)gameWindow.Height;
                    WindowType.Width = gameWindow.Width;
                    WindowType.Height = gameWindow.Height;
                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.VertexArray);
                    GL.Enable(EnableCap.TextureCoordArray);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                };

                gameWindow.Keyboard.KeyDown += (object sender, KeyboardKeyEventArgs e) =>
                {
                };

                gameWindow.UpdateFrame += (sender, e) =>
                {
                };

                gameWindow.RenderFrame += (sender, e) =>
                {
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.ClearColor(30/255f, 30/255f, 30/255f, 1);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

                    if (IntegratorView != null)
                        IntegratorView.Draw();

                    if (LineDraw != null)
                        LineDraw.Draw();

                    if(RectDraw != null)
                        RectDraw.Draw();

                    if (PolygonDraw != null)
                        PolygonDraw.Draw();

                    if (CurveDraw != null)
                        CurveDraw.Draw();

                    if (SurfaceDraw != null)
                        SurfaceDraw.Draw();

                    if (FontDraw != null)
                        FontDraw.Draw();

                    if (ScreenDraw != null)
                        ScreenDraw.Draw();
                    
                    if (TrackBox != null)       
                        TrackBox.Draw();
            

                    // frame
                    GL.Flush();
                    gameWindow.SwapBuffers();
                };

                // Run the game at 60 updates per second
                gameWindow.Run(90.0);
            }
        }
    }
    
}