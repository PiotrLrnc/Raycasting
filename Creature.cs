using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Creature:GameObject
    {        
        public int step = 10;
        public float direction;
        public const float deltaAngle = PI / 18;
        protected float hp;

        public Creature(int x, int y, float direction):base(x, y)
        {
            this.direction = direction;
        }

        public void goForward()
        {

            this.x += Convert.ToInt32(this.step * Math.Cos(this.direction));
            this.y += Convert.ToInt32(this.step * Math.Sin(this.direction));


        }

        public void goBackward()
        {

            this.x -= Convert.ToInt32(this.step * Math.Cos(this.direction));
            this.y -= Convert.ToInt32(this.step * Math.Sin(this.direction));


        }

        public void turnLeft()
        {
            this.direction -= deltaAngle;
            if (this.direction < 0)
                this.direction += 2 * PI;
        }

        public void turnRight()
        {

            this.direction += deltaAngle;
            if (this.direction > 2 * PI)
                this.direction -= 2 * PI;
        }

    }
}
