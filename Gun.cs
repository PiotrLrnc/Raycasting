using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Gun:Item
    {
        private int ammo;
        private IntPtr[] image;

        public Gun(int x, int y, int ammo):base(x, y)
        {
            this.ammo = ammo;
        }

        override public void use()
        {
            if (ammo > 0)
            {
                base.playAnimation();
                ammo--;
            }
            else
            {

            }
            
                
        }

        
        override public void display()
        {

        }


    }

}
