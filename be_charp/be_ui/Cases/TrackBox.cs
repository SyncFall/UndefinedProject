using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Be.UI
{
    public class TrackBox
    {
        public WindowType Window;
        public TrackBallMouseListener MouseListener;
        public bool IsDraging = false;
        public int Width;
        public int Height;
        Vector3 Center = new Vector3(0, 0, 0);
        float Radius = 1;
        Vector3 CurrentMouse = new Vector3(0, 0, 0);
        Vector3 BeginingMouse = new Vector3(0, 0, 0);
        Quaternion CurrentRoation = new Quaternion(0, 0, 0, 1);
        Quaternion EndRotation = new Quaternion(0, 0, 0, 1);
        Matrix4 RoationMatrix = Matrix4.Identity;
    
        public TrackBox(WindowType Window)
        {
            this.Window = Window;
            this.Width = Window._Window.Width;
            this.Height = Window._Window.Height;
            this.Window.Mouse.AddListener(new TrackBallMouseListener(this));
        }

        /// indicates the beginning of the dragging.
        public void BeginDraging()
        {
            IsDraging = true;  // start dragging
            BeginingMouse = CurrentMouse; // remember start position
        }

        /// marks the end of the dragging.
        public void EndDraging()
        {
            IsDraging = false; // stop dragging
            EndRotation = CurrentRoation; // remember rotation
        }

        /// maps the specified mouse position to the sphere defined
        /// with center and radius. the resulting vector lies on the
        /// surface of the sphere.
        public Vector3 MapSphere(Vector3 Mouse, Vector3 Center, float Radius)
        {
            Vector3 bm = Vector3.Multiply(Mouse - Center, 1.0f / Radius);
            double mag = Length2(bm);
			if (mag > 1.0) {
				bm.Normalize();
				bm[2] = 0.0f;
			} else {
				bm[2] = (float)Math.Sqrt(1.0f - mag);
            }
			return bm;			
		}

        public float Length2(Vector3 v)
        {
            return (v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
        }

        public float Length2(Quaternion q)
        {
            return (q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z);
        }

        /// calculates and returns the quaternion which describes the
        /// arc between the two specified positions on the surface
        /// of a sphere (virtual trackball).
        public Quaternion FromBallPoints(Vector3 from, Vector3 to)
        {
            return new Quaternion(
                  from[1] * to[2] - from[2] * to[1]
                , from[2] * to[0] - from[0] * to[2]
                , from[0] * to[1] - from[1] * to[0]
                , from[0] * to[0] + from[1] * to[1] + from[2] * to[2]
            );
        }

        /// maps the specified quaternion to the 4x4 matrix
        /// which represents a rotation. works for right-handed
        /// coordinate systems.
        public Matrix4 ToMatrix(Matrix4 m, Quaternion q)
        {
            double l = Length2(q);
            double s = (l > 0.0) ? (2.0 / l) : 0.0;

            double xs = q.X * s;
            double ys = q.Y * s;
            double zs = q.Z * s;

            double wx = q.W * xs;
            double wy = q.W * ys;
            double wz = q.W * zs;

            double xx = q.X * xs;
            double xy = q.X * ys;
            double xz = q.X * zs;

            double yy = q.Y * ys;
            double yz = q.Y * zs;

            double zz = q.Z * zs;

            m[0, 0] = (float)(1.0 - (yy + zz));
            m[0, 1] = (float)(xy + wz);
            m[0, 2] = (float)(xz - wy);
            m[0, 3] = (float)(0.0);

            m[1, 0] = (float)(xy - wz);
            m[1, 1] = (float)(1.0 - (xx + zz));
            m[1, 2] = (float)(yz + wx);
            m[1, 3] = (float)(0.0);

            m[2, 0] = (float)(xz + wy);
            m[2, 1] = (float)(yz - wx);
            m[2, 2] = (float)(1.0 - (xx + yy));
            m[2, 3] = (float)(0.0);

            m[3, 0] = (float)0.0;
            m[3, 1] = (float)0.0;
            m[3, 2] = (float)0.0;
            m[3, 3] = (float)1.0;
            return m;
        }

        /// update of the rotation matrix 'mat_cur', using the position
        /// of the beginning of the dragging and the current position.
        /// both coordinates mapped to the surface of the virtual trackball.
        public void Update()
        {
            Vector3 v_from = MapSphere(BeginingMouse, Center, Radius);
            Vector3 v_to = MapSphere(CurrentMouse, Center, Radius);
            if (IsDraging)
            {
                CurrentRoation = FromBallPoints(v_from, v_to) * EndRotation;
            }
            RoationMatrix = ToMatrix(RoationMatrix, CurrentRoation);
        }

        /// sets the current position and calculates the current
        /// rotation matrix.
        public void SetCurrentPosition(int x, int y)
        {
            CurrentMouse[0] = (float)(2.0 * ((double)x / Width) - 1.0);
            CurrentMouse[1] = (float)(2.0 * ((double)(Height - y) / Height) - 1.0);
            CurrentMouse[2] = 0.0f;
            Update(); // calculate rotation matrix
        }

        /// returns the rotation matrix to be used directly
        /// for the OpenGL command 'glMultMatrixf'.
        public Matrix4Double GetRoationMatrix()
        {
            return new Matrix4Double(RoationMatrix);
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Frustum(0, Window.Width, Window.Height, 0, 0, 1);

            GL.MultMatrix(this.GetRoationMatrix().Mat);

            GL.LineWidth(2.5f);
            GL.Scale(0.6, 0.6, 0.6);

            GL.Begin(PrimitiveType.Lines);
            GL.Color4(1.0, 0.0, 0.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(10.0, 0.0, 0.0);
            GL.Color4(0.0, 1.0, 0.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 10.0, 0.0);
            GL.Color4(0.0, 0.0, 1.0, 1.0);
            GL.Vertex3(0.0, 0.0, 0.0);
            GL.Vertex3(0.0, 0.0, 10.0);
            GL.End();

            DrawUtils.DrawBox();
        }
    }

    public struct Matrix4Double
    {
        public double[] Mat;

        public Matrix4Double(Matrix4 Source)
        {
            Mat = new double[16];
            int idx = 0;
            for(int i=0; i<4; i++)
            {
                for(int j=0; j<4; j++)
                {
                    Mat[idx++] = Source[i, j];
                }
            }
        }
    }

    public class TrackBallMouseListener : MouseListener
    {
        public TrackBox TrackBall;

        public TrackBallMouseListener(TrackBox trackball)
        {
            this.TrackBall = trackball;
        }

        public override void MouseEvent(MouseResult Result)
        {
            if (Result.Type == MouseType.BUTTON_EVENT)
            {
                if (Button.Key == ButtonKey.LEFT && Button.Event == ButtonEvent.DOWN)
                {
                    TrackBall.SetCurrentPosition(Cursor.X, Cursor.Y);
                    TrackBall.BeginDraging();
                }
                else
                {
                    TrackBall.EndDraging();
                }
            }
            else if(Result.Type == MouseType.CURSOR_EVENT && TrackBall.IsDraging)
            {
                TrackBall.SetCurrentPosition(Cursor.X, Cursor.Y);
            }
        }
    }
}
