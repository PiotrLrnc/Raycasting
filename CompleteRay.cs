using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class CompleteRay:IntersectionFindingAndCountingRay
    {        
        public CompleteRay(Player player, float dir, ProjectionPlane projectionPlane):base(player, dir, projectionPlane)
        {

        }

        private void fillRectangle(IntPtr renderer, int x, int y, int width, int height, byte[] color)
        {
            SDL.SDL_SetRenderDrawColor(renderer, color[0], color[1], color[2], color[3]);
            for (int i = 0; i < width; i++)
                SDL.SDL_RenderDrawLine(renderer, x + i, y, x + i, y + height);

        }

        private void putShade(IntPtr renderer, int rectX, int rectY, int width, int height, byte alpha)
        {
            byte[] color = { 0, 0, 0, alpha };
            fillRectangle(renderer, rectX, rectY, width, height, color);
        }

        // Nowa funkcja renderująca
        private void renderSliceOfWall(IntPtr renderer, Intersection intersect)
        {
            float distToWall = intersect.distToPlayer;
            int perpDistToWall = (int)(distToWall *(float)Math.Cos(this.player.direction - this.dir)); //Korygujemy odległość do ściany, aby sie pozbyć efektu fish eye.

            float rectX = (float)Program.SCREEN_WIDTH / 2 - projPlane.distToCamera * (float)Math.Tan(player.direction - this.dir);
            int height, width;


            height = findHeightOfWall(perpDistToWall);

            float nextRectX = (float)Program.SCREEN_WIDTH / 2 - projPlane.distToCamera * (float)Math.Tan(player.direction - (this.dir + Program.deltaAngle));
            width = (int)(nextRectX - rectX) + 1; //Dodajemy 1 piksel szerokosci, żeby nie było przerw między paskami. 


            byte colorR;
            intersect.dirOfWall = intersect.dirOfWall % PI; // Bierzemy resztę z dzielenie przez Pi, żeby ściany o różnej orientacji, ale tym samym kierunku miały taki sam kolor. 
            colorR = Convert.ToByte(155 + 100 * intersect.dirOfWall / (2 * Math.PI));


            int rectY = findYCoordinateOfTop(perpDistToWall);

            int unitsPerTexture = 2*Program.texture.width; // Dla 40 wygląda w miarę nieźle. 
            float shade = intersect.dirOfWall / PI / 2;

            // Znajdujemy kolumnę od której zaczynamy nanoszenie tekstury:  
            var picX = Convert.ToInt32(((float)(intersect.distToEnd % unitsPerTexture) / unitsPerTexture) * Program.texture.width);

            int lengthOfTexture = (int)((float)(findLengthOfWall(intersect) * Program.texture.width) / unitsPerTexture);
            //Program.texture.prepareTexture(renderer, length, picX);
            Program.texture.prepareTexture(renderer, lengthOfTexture, 2 * Program.texture.width, picX, 0);
            Program.texture.drawPreparedTexture(renderer, (int)rectX, rectY, width, height);

            byte alpha = Convert.ToByte(intersect.dirOfWall * 255 / PI);
            putShade(renderer, (int)rectX, rectY, width, height, alpha);

            writeDepthToBuffer((int)rectX, width, (int)distToWall);
            writeTopYToBuffer((int)rectX, width, (int)rectY);
            writeBottomYToBuffer((int)rectX, width, (int)rectY+height);

        }


        public void render3D(IntPtr renderer)
        {
            Intersection intersect = this.findIntersection();
            if (intersect != null)
                if (intersect.distToPlayer > 0.1)
                    renderSliceOfWall(renderer, intersect);


        }


    } // DrawingRay
}
