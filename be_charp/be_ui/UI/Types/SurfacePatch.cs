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
        public BeePoint[,] Points;

        public BeePoint[] VertexArray;
        public BeePoint[] ColorArray;

        public BeeBuffer Buffer;

        public BeeSurfacePatch(BeeSurfacePatchType Type)
        {
            this.Type = Type;
            if(Type == BeeSurfacePatchType.BiCubic)
            {
                Points = new BeePoint[4, 4];
            }
            else if(Type == BeeSurfacePatchType.TriCubic)
            {
                Points = new BeePoint[3, 4];
            }
        }

        public void Build()
        {
            VertexArray = new BeePoint[54];
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
            ColorArray = new BeePoint[9 * 2 * 3];
            for(int i = 0; i < ColorArray.Length; i++)
            {
                ColorArray[i] = new BeePoint((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
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
