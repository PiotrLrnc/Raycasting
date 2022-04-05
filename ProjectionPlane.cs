using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class ProjectionPlane
    {
        public int width, height, heightOfCenter; // Chyba lepsza nazwa to heightOfCamera
        public float rangeOfFieldOfViewRad, angleOfRotation; 
        public int distToCamera;
        public float[,] phis;

        public ProjectionPlane(int width, int height, float angleOfFieldOfView, int heightOfCenter)
        {
            this.width = width;
            this.height = height;
            this.rangeOfFieldOfViewRad = angleOfFieldOfView;
            this.angleOfRotation = 0;
            this.heightOfCenter = heightOfCenter;      
            this.distToCamera = (int)((float)(width/2)/Math.Tan(rangeOfFieldOfViewRad/2));
            this.phis = new float[height, width];

        }

        public void moveUpBy(int step)
        {
            this.heightOfCenter += step;
        }
        
        public void moveDownBy(int step)
        {
            this.heightOfCenter -= step;
        }

    } // ProjectionPlane
}
