using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class IntersectionFindingAndCountingRay:IntersectionFindingRay
    {
        public int[] depthBuffer, topYBuffer, bottomYBuffer;
        //public int[] bottomYBuffer;

        public IntersectionFindingAndCountingRay(Player player, float dir, ProjectionPlane projectionPlane):base(player, dir, projectionPlane)
        {

        }

        // Finds the angle between the radius and a wall needed to calculate the length of the wall between two rays
        private float findAngleBetweenRayAndWall(Intersection intersect)
        {
            Boundary bound = intersect.boundary;
            Vector a = new Vector(bound.x2 - intersect.x, bound.y2 - intersect.y); // Wektor łączący punkt przecięcia z krańcem ściany (potem trzeba sprawdziź, czy to dobry kraniec ściany).  

            Vector b = new Vector(this.x - intersect.x, this.y - intersect.y); // Wektor łączący punkt przecięcia z graczem.  

            Vector aPrim = a.findRotatedCoordinates(this.dir);
            //Vector aPrim = findRotatedCoordinates(a, this.dir);
            // Jeżeli w obróconym układzie współrzędnych wektor a jest zwrócony w lewo, to musimy wziąć przeciwny wektor do policzenia kąta pomiędzy ścianą, a promieniem. 
            // Ponieważ oś y jest zwrócona w dół (czyli odwrotnie niż zwykle), to wektor a jest zwrócony w lewo, jak jego współrzedna y-owa jest ujemna. 
            if (aPrim.y < 0)
                a = -a;

            a.normalize();
            b.normalize();
            var cosBeta = a.dotProduct(b);
            var beta = (float)Math.Acos(cosBeta);
            return beta;


        }

        // Finds the length of a visable wall between two rays. Alpha, beta and gamma are 3 angles in the triangle between the wall and two consecutive rays. 
        // Alpha is the angel between two consecutive rays. Beta is the angle between the ray and the wall. 
        // Height is height of the triangle delineated from the vertex beta. 
        protected int findLengthOfWall(Intersection intersect)
        {
            //Boundary bound = intersect.boundary;
            var alpha = Program.deltaAngle;
            var beta = findAngleBetweenRayAndWall(intersect);
            var gamma = PI - alpha - beta;
            int length;
            float height = intersect.distToPlayer * (float)Math.Sin(alpha);
            if (beta > PI / 2)
                length = (int)(height / Math.Sin(gamma));

            else if (beta < PI / 2)
                length = (int)(height / Math.Sin(PI - gamma));

            else // beta = PI/2
                length = (int)(intersect.distToPlayer * Math.Tan(alpha));

            return length;

        }
        
        private int findHeightOfProjectionOfBottom(int perpDist)
        {
            int height = (int)((float)projPlane.heightOfCenter / perpDist * (perpDist - projPlane.distToCamera));
            return height;
        }

        private int findYCoordinateOfBottom(int perpendicularDistance)
        {
            int height = findHeightOfProjectionOfBottom(perpendicularDistance);
            float t = (float)(projPlane.heightOfCenter - height) / (Program.SCREEN_HEIGHT / 2);
            int yCoord = (int)((1 - t) * Program.SCREEN_HEIGHT / 2 + t * Program.SCREEN_HEIGHT);
            return yCoord;
        }

        private int findHeightOfProjectionOfTop(int perpDist)
        {
            int height = (int)((float)(Boundary.height - projPlane.heightOfCenter) / perpDist * projPlane.distToCamera + projPlane.heightOfCenter);
            return height;
        }

        protected int findYCoordinateOfTop(int perpDist)
        {
            int height = findHeightOfProjectionOfTop(perpDist);
            float t = (float)(height - projPlane.heightOfCenter) / (Program.SCREEN_HEIGHT / 2);
            int yCoord = (int)((1 - t) * Program.SCREEN_HEIGHT / 2 + t * 0);
            return yCoord;
        }

        protected int findHeightOfWall(int perpendicularDistance)
        {
            int top = findYCoordinateOfTop(perpendicularDistance);
            int bottom = findYCoordinateOfBottom(perpendicularDistance);
            int height = bottom - top;
            return height;
        }

        protected void writeDepthToBuffer(int x, int numberOfVerticalLines, int distanceToWall)
        {
            int i=x;
            while (i!=x+numberOfVerticalLines && i != depthBuffer.GetLength(0)-1)
            {
                depthBuffer[i] = distanceToWall;
                i++;
            }
                
        }       

        protected void writeTopYToBuffer(int x, int numberOfVerticalLines, int yCoordOfTop)
        {
            int i = x;
            while (i != x + numberOfVerticalLines && i != depthBuffer.GetLength(0) - 1)
            {
                topYBuffer[i] = yCoordOfTop;
                i++;
            }
        }
        protected void writeBottomYToBuffer(int x, int numberOfVerticalLines, int yCoordOfBottom)
        {
            int i = x;
            while (i != x + numberOfVerticalLines && i != depthBuffer.GetLength(0) - 1)
            {
                bottomYBuffer[i] = yCoordOfBottom;
                i++;
            }
        }



    } //CountingRay
}
