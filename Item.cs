using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Item:GameObject
    {
        public Item(int x, int y):base(x, y)
        {

        }
        virtual public void use()
        {
            animation.startPlaying();
        }

    }

}
