using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Intersection
    {
        public int x, y;
        public float distToEnd, distToPlayer, dirOfWall, radiusWallAngle;
        public Boundary boundary;

        public Intersection(Boundary boundary, int x, int y, float distToPlayer, float dirOfWall, float radiusWallAngle, float distToEnd)
        {
            this.boundary = boundary;
            this.x = x;
            this.y = y;
            this.distToPlayer = distToPlayer;
            this.distToEnd = distToEnd;
            this.dirOfWall = dirOfWall;
            this.radiusWallAngle = radiusWallAngle;

        }
    }
}
