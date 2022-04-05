using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Boundary
    {
        const float PI = (float)Math.PI;
        public int x1, y1, x2, y2;
        public float dir;
        public static int height = 1000;
        public Boundary(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            double r = Math.Pow((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1), 0.5);
            double x = x2 - x1, y = y2 - y1;
            if (y > 0)
                this.dir = (float)Math.Acos(x / r);
            else
                this.dir = (float)Math.Acos(x / r) + PI;
        }

        public void draw(IntPtr renderer, byte[] color)
        {
            SDL.SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL.SDL_RenderDrawLine(renderer, this.x1, this.y1, this.x2, this.y2);
        }
    }
}
