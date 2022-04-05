using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Ceiling:Visplane
    {
        public Ceiling(int rectX, int rectY, int width, int height, Texture texture, Player player, ProjectionPlane projPlane, int pixelWidth, int pixelHeight) : base(
            rectX, rectY, width, height, 1000, texture, player, projPlane, pixelWidth, pixelHeight)
        {

        }        

        public override void render(IntPtr renderer)
        {
            int x = rectX, y;
            while (x < rectX+width)
            {
                if (yBuffer[x] != -1)                
                    y = Math.Min(yBuffer[x], rectY+height); 
                else                
                    y = Math.Min(projPlane.height / 2 - 10, rectY+height);
                int length = y - rectY;

                if (length > 0)
                    textureVerticalSlice(renderer, x, rectY, length);

                x += pixelWidth;
            }
        }


    }
}
