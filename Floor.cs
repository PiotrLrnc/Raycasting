using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Floor:Visplane
    {
        public Floor(int rectX, int rectY, int width, int height, Texture texture, Player player, ProjectionPlane projPlane, int pixelWidth, int pixelHeight):base(
            rectX, rectY, width, height, 0, texture, player, projPlane, pixelWidth, pixelHeight)
        {

        }        

        public override void render(IntPtr renderer)
        {
            int x = rectX, y;
            while (x < rectX+width)
            {
                if (yBuffer[x] != -1)
                    y = Math.Max(yBuffer[x], rectY);
                else
                    y = Math.Max(projPlane.height / 2 + 10, rectY);
                int length = rectY+height - y;
                if (length >0)
                    textureVerticalSlice(renderer, x, y, length);              

                x += pixelWidth;
            }
        }

    } // Floor
}
