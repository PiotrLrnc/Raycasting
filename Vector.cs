using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Vector
    {
        public float x, y;
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void normalize()
        {
            float module = (float)Math.Sqrt(x * x + y * y);
            if (module == 0)
                Console.WriteLine("Moduł wektora musi być większy od 0, żeby można było podzielić przez niego");
            else
            {
                this.x = this.x / module;
                this.y = this.y / module;
            }

        }

        // inner product of two vectors:
        public float dotProduct(Vector a)
        {
            return this.x * a.x + this.y * a.y;
        }

        // Finds the coordinates in a system rotated by a given angle
        public Vector findRotatedCoordinates(float angle)
        {
            float x = this.x * (float)Math.Cos(angle) + this.y * (float)Math.Sin(angle);
            float y = this.y * (float)Math.Cos(angle) - this.x * (float)Math.Sin(angle);
            Vector a = new Vector(x, y);
            return a;
        }

        public Vector findCartesianCoordinates()
        {
            // Przyjmujemy konwencję, że pierwsza współrzędna wektora to r, druga - to phi. 
            float v1 = x*(float)Math.Cos(y);
            float v2 = x*(float)Math.Sin(y);
            return new Vector(v1, v2);
        }

        public Vector findShiftedCoordinates(Vector translation)
        {
            float v1 = x - translation.x; 
            float v2 = y - translation.y;
            return new Vector(v1, v2);
        }

        // Calculates the distance to a given vector 
        public float findDistance(Vector vec)
        {
            float x1 = this.x, y1 = this.y, x2 = vec.x, y2 = vec.y;
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public float findCosBetween(Vector vec)
        {
            // Tutaj trzeba dopisac zabezpieczenie na wypadek, gdyby któryś wektor był zerowy. 
            Vector a = new Vector(x, y);
            a.normalize();
            vec.normalize();
            return a.dotProduct(vec); 
        }

        public static Vector operator -(Vector a) => new Vector(-a.x, -a.y);


    }
}
