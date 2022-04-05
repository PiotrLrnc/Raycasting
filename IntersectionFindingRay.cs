using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class IntersectionFindingRay:Ray
    {        
        public IntersectionFindingRay(Player player, float dir, ProjectionPlane projectionPlane):base(player, dir, projectionPlane)
        {

        }

        // A segment line is given by a convex combination (1-t)*A+t*B for 0<=t<=1. If we allow t>=0 we'll get a ray starting at the point A.
        // We need to find an intersection of this ray with a segment line given by (1-u)*C+u*D. We're looking for a solution with t>=0 and 0<=u<=1. 
        // Let A=(x1,y1), B=(x2,y2), C=(x3,y3), D=(x4,y4). We need to solve a set of equations (after rearranging terms)
        // t*(x2-x1)-u*(x4-x3)=x3-x1
        // t*(y2-y1)-u*(y4-y3)=y3-y1
        // We can use the method of determinants. 
        protected Intersection findIntersection(Boundary bound)
        {
            float dist = 0, dirOfWall = 0, radiusWallAngle = 0;
            int intersectX = 0, intersectY = 0, distToEnd = 0;

            int x1 = this.x, y1 = this.y, d = 100;
            float x2 = x1+d*(float)Math.Cos(this.dir), y2 = y1+d*(float)Math.Sin(this.dir);
            
            int x3 = bound.x1, y3= bound.y1;
            int x4 = bound.x2, y4 = bound.y2;
            

            float denom = (x2-x1)*(y3-y4)-(y2-y1)*(x3-x4);
            if (denom != 0)
            {
                float t = ((x3 - x1) * (y3 - y4) - (y3 - y1) * (x3 - x4)) / denom;
                float u = ((x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x)) / denom;
                if (t > 0 && u > 0 && u<1)
                {
                    intersectX = Convert.ToInt32((1-t)*x1 + t*x2);
                    intersectY = Convert.ToInt32((1-t)*y1 + t*y2);
                    dist = (float)Math.Pow((intersectX - this.x) * (intersectX - this.x) + (intersectY - this.y) * (intersectY - this.y), 0.5); //Liczimy odległość do punktu przecięcia
                    dirOfWall = bound.dir; // kąt będzie służył do cieniowania. 
                    radiusWallAngle = (this.player.direction - dirOfWall) % PI;
                    // Obliczamy współrzędne y krańców ściany w obróconym układzie współrzędnych (o taki kąt jaki ma gracz)
                    // po to, żeby znaleźć kraniec ściany, który jest bardziej na lewo i od niego teksturować ścianę. 
                    Vector a = new Vector(bound.x1, bound.y1);
                    Vector b = new Vector(bound.x2, bound.y2);
                    Vector aPrim = a.findRotatedCoordinates(this.player.direction);
                    Vector bPrim = b.findRotatedCoordinates(this.player.direction);

                    Vector intersect = new Vector(intersectX, intersectY);

                    if (aPrim.y > bPrim.y)
                        distToEnd = (int)a.findDistance(intersect);
                    else
                        distToEnd = (int)b.findDistance(intersect);
                    Intersection result = new Intersection(bound, intersectX, intersectY, dist, dirOfWall, radiusWallAngle, distToEnd);
                    return result;
                }
                else
                    return null;

            }
            else
                return null;

        } // findIntersection


        protected Intersection findIntersection2(Boundary bound)
        {
            float dist = 0, dirOfWall = 0, radiusWallAngle = 0;
            int intersectX = 0, intersectY = 0, distToEnd = 0;
            int x1 = bound.x1, y1 = bound.y1, x2 = bound.x2, y2 = bound.y2;

            int d = 100;
            int x3 = this.x;
            int y3 = this.y;
            float x4 = this.x + d * (float)Math.Cos(this.dir);
            float y4 = this.y + d * (float)Math.Sin(this.dir);

            float denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (denom != 0)
            {
                float t = (float)((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denom;
                float u = (float)((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / denom;
                if (t >= 0 && t <= 1 && u >= 0)
                {
                    intersectX = Convert.ToInt32(x1 + t * (x2 - x1));
                    intersectY = Convert.ToInt32(y1 + t * (y2 - y1));
                    dist = (float)Math.Pow((intersectX - this.x) * (intersectX - this.x) + (intersectY - this.y) * (intersectY - this.y), 0.5); //Liczimy odległość do punktu przecięcia
                    dirOfWall = bound.dir; // kąt będzie służył do cieniowania. 
                    radiusWallAngle = (this.player.direction - dirOfWall) % PI;
                    // Obliczamy współrzędne y krańców ściany w obróconym układzie współrzędnych (o taki kąt jaki ma gracz)
                    // po to, żeby znaleźć kraniec ściany, który jest bardziej na lewo i od niego teksturować ścianę. 
                    Vector a = new Vector(bound.x1, bound.y1);
                    Vector b = new Vector(bound.x2, bound.y2);
                    Vector aPrim = a.findRotatedCoordinates(this.player.direction);
                    Vector bPrim = b.findRotatedCoordinates(this.player.direction);

                    Vector intersect = new Vector(intersectX, intersectY);

                    if (aPrim.y > bPrim.y)
                        distToEnd = (int)a.findDistance(intersect);
                    else
                        distToEnd = (int)b.findDistance(intersect);
                    Intersection result = new Intersection(bound, intersectX, intersectY, dist, dirOfWall, radiusWallAngle, distToEnd);
                    return result;
                }
                else
                    return null;

            }
            else
                return null;

        } // findIntersection

        // This overloaded function finds the closest intersection for an array of walls. 
        protected Intersection findIntersection()
        {
            float dist = 0, dirOfWall = 0, radiusWallAngle = 0;
            int intersectX = 0, intersectY = 0, distToEnd = 0;
            Boundary boundary = null;
            Intersection intersect;
            int i = 0; //Liczba znalezionych przecięć. 
            foreach (Boundary b in bounds)
            {
                intersect = findIntersection(b);
                if (intersect != null)
                {
                    if (i == 0 || (i != 0 && intersect.distToPlayer < dist))  // To się wykona przy pierwszym znalezionym przecięciu lub następnym o ile odległość do przecięcia będzie mniejsza. 
                    {
                        boundary = b;
                        intersectX = intersect.x;
                        intersectY = intersect.y;
                        dist = intersect.distToPlayer;
                        dirOfWall = intersect.dirOfWall;
                        radiusWallAngle = intersect.radiusWallAngle;
                        distToEnd = (Int32)intersect.distToEnd;
                    }
                    i++;
                }


            }

            if (i != 0)
            {
                Intersection result = new Intersection(boundary, intersectX, intersectY, dist, dirOfWall, radiusWallAngle, distToEnd);
                return result;
            }
            else
                return null;
        }
              
        

        public void draw2D(IntPtr renderer, byte[] color)
        {

            int x2, y2, d = 20;
            x2 = x + Convert.ToInt32(d * Math.Cos(this.dir));
            y2 = y + Convert.ToInt32(d * Math.Sin(this.dir));
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL.SDL_RenderDrawLine(renderer, this.x, this.y, x2, y2);

            Intersection intersect = this.findIntersection();
            if (intersect != null)
                this.drawRay(renderer, Convert.ToInt32(intersect.x), Convert.ToInt32(intersect.y), color);
        }


    } //WallFindingRay
}
