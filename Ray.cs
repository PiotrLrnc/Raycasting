using System;
using System.Collections; // To jest potrzebne do ArrayList
using System.Collections.Generic; // To jest do list. 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Ray
    {
        protected const float PI = (float)Math.PI;
        public static int counter = 0; // licznik promieni
        protected int x, y, no;
        protected float dir, angleBetweenRayAndPlayer;
        protected Player player;
        protected Boundary[] bounds;
        protected ProjectionPlane projPlane;
        

        public Ray(Player player, float dir, ProjectionPlane projectionPlane)
        {
            this.x = player.x;
            this.y = player.y;
            this.dir = dir;
            this.player = player;
            this.projPlane = projectionPlane;
            this.angleBetweenRayAndPlayer = this.dir - this.player.direction;
            Ray.counter += 1;
            this.no = Ray.counter;

        }

        public void setBoundaries(Boundary[] bounds)
        {
            this.bounds = bounds;
        }    
        

        protected void drawRay(IntPtr renderer, int destX, int destY, byte[] color)
        {
            SDL.SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            SDL.SDL_RenderDrawLine(renderer, this.x, this.y, destX, destY);
        }


        public void update()
        {
            this.x = player.x;
            this.y = player.y;
            this.dir = player.direction + angleBetweenRayAndPlayer;
        }
        
        
    }
}
