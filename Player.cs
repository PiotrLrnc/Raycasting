using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Player:Character
    {
        //public float height;
        public ProjectionPlane projectionPlane;


        public Player(int x, int y, float direction, ProjectionPlane proPlane):base(x, y, direction)
        {
            this.step = 80;
            this.projectionPlane = proPlane;
            //this.height = projectionPlane.heightOfCenter;
            
        }

        public void moveUp()
        {
            projectionPlane.moveUpBy(30);
        }

        public void moveDown()
        {
            projectionPlane.moveDownBy(30);
        }
        

        public void drawCrosshairs(IntPtr renderer, int x, int y)
        {
            SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
            SDL.SDL_RenderDrawLine(renderer, x-10, y, x+10, y);
            SDL.SDL_RenderDrawLine(renderer, x, y-10, x, y+10);
        }

        
        public void renderSprite(GameObject sprite, float[] zBuffer)
        {
            SDL.SDL_Rect sourceRect;
            Vector vectorFromPlayerToSprite; 
            Vector normalVector = new Vector(this.x+40*(float)Math.Cos(this.direction), this.y+40*(float)Math.Sin(this.direction));
            
            int stripeHeight = Program.SCREEN_HEIGHT;
            int stripeWidth = Program.SCREEN_WIDTH / Ray.counter + 3;
            vectorFromPlayerToSprite = new Vector(sprite.x - this.x, sprite.y-this.y);

            float cos = normalVector.findCosBetween(vectorFromPlayerToSprite);
            float angle = (float)Math.Acos(cos);
            int rectX = Convert.ToInt32(Math.Tan(angle) / Math.Tan(0.5 * Program.rangeOfViewRad) / 1.8 * Program.SCREEN_WIDTH + Program.SCREEN_WIDTH / 2);
            int rectY;
                
            sourceRect.w = stripeWidth;
            sourceRect.h = 2324;  

        }

        public void renderSprites(GameObject[] sprites, float[] buffer)
        {
            foreach (GameObject sprite in sprites)
                renderSprite(sprite, buffer);
        }


        
        /*public void display(IntPtr renderer)
        {
            int u, w;
            u = Convert.ToInt32(this.x + 20 * Math.Cos(this.direction));
            w = Convert.ToInt32(this.y + 20 * Math.Sin(this.direction));
            Program.fillSquare(renderer, this.x, this.y, 5, Program.YELLOW);
            SDL.SDL_RenderDrawLine(renderer, this.x, this.y, u, w);
        }*/

    }
}
