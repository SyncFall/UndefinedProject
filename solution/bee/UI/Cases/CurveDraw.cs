using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class CurveDraw
    {
        public FpsCounter FpsCounter;
        public LineCurve Segement1;
        public LineCurve Segement2;
        public LineCurve Segement3;
        public LineCurve Segement4;

        public CurveDraw()
        {
            FpsCounter = new FpsCounter();
            this.Segement1 = new LineCurve(BeeLineCurveType.Cubic);
            this.Segement1.Anchor1 = new BeePoint(100, 400);
            this.Segement1.Anchor2 = new BeePoint(400, 400);
            this.Segement1.Control1 = new BeePoint(125, 250);
            this.Segement1.Control2 = new BeePoint(275, 250);

            this.Segement2 = new LineCurve(BeeLineCurveType.Cubic);
            this.Segement2.Anchor1 = new BeePoint(400, 400);
            this.Segement2.Anchor2 = new BeePoint(600, 200);
            this.Segement2.Control1 = new BeePoint(450, 500);
            this.Segement2.Control2 = new BeePoint(550, 150);

            this.Segement3 = new LineCurve(BeeLineCurveType.Linear);
            this.Segement3.Anchor1 = new BeePoint(50, 50);
            this.Segement3.Anchor2 = new BeePoint(250, 60);

            this.Segement4 = new LineCurve(BeeLineCurveType.Conic);
            this.Segement4.Anchor1 = new BeePoint(200, 200);
            this.Segement4.Control1 = new BeePoint(300, 25);
            this.Segement4.Anchor2 = new BeePoint(400, 200);
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, 800, 600, 0, 0, 1);

            /*
            Segement1.Draw();
            Segement2.Draw();
            Segement3.Draw();
            Segement4.Draw();
            */

            FpsCounter.Draw();
        }
    }
}
