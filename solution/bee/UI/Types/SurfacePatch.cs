using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public enum BeeSurfacePatchType
    {
        BiCubic,
        TriCubic,
    }

    // bi and tri cubic bezier patch
    public class BeeSurfacePatch
    {
        public BeeSurfacePatchType Type;
        public Vec3[,] Points;

        public Vec3[] VertexArray;
        public Vec3[] ColorArray;

        public BeeBuffer Buffer;

        public BeeSurfacePatch(BeeSurfacePatchType Type)
        {
            this.Type = Type;
            if(Type == BeeSurfacePatchType.BiCubic)
            {
                Points = new Vec3[4, 4];
            }
            else if(Type == BeeSurfacePatchType.TriCubic)
            {
                Points = new Vec3[3, 4];
            }
        }

        public void Build()
        {
            VertexArray = new Vec3[54];
            int idx = 0;
            // from top-down segment
            for(int i = 0; i < 3; i++)
            {
                // to left-right segment
                for(int j = 0; j < 3; j++)
                {
                    // triangle on
                    VertexArray[idx++] = Points[i, j];
                    VertexArray[idx++] = Points[i + 1, j];
                    VertexArray[idx++] = Points[i, j + 1];

                    // triangle two
                    VertexArray[idx++] = Points[i + 1, j];
                    VertexArray[idx++] = Points[i, j + 1];
                    VertexArray[idx++] = Points[i + 1, j + 1];
                }
            }

            Random random = new Random(255);
            ColorArray = new Vec3[9 * 2 * 3];
            for(int i = 0; i < ColorArray.Length; i++)
            {
                ColorArray[i] = new Vec3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            }

            Buffer = new BeeBuffer(VertexArray, ColorArray);
            Buffer.Build();
        }

        public void Draw()
        {
            Buffer.Draw();
        }
    }
}
