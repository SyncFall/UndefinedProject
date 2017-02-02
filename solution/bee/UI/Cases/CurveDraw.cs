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
        public Curve Curve = new Curve();

        public CurveDraw()
        {
            FpsCounter = new FpsCounter();
            /*
            Curve.Points.Add(new Vec3(150, 50));
            Curve.Points.Add(new Vec3(50, 120));
            Curve.Points.Add(new Vec3(80, 150));
            Curve.Points.Add(new Vec3(100, 120));
            Curve.Points.Add(new Vec3(120, 150));
            Curve.Points.Add(new Vec3(150, 120));
            Curve.Points.Add(new Vec3(50, 50));
            */
        }

        public void Draw()
        {
            Curve.Draw();
            FpsCounter.Draw();
            //DrawUtils.DrawTriangleCycle(500, 300, 250, 60);
        }
    }
}
