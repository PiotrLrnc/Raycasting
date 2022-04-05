using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycasting
{
    internal class Character:Creature
    {        
        private List<Item> equipment = new List<Item>();
        public Item weapon; 

        public Character(int x, int y, float direction):base(x, y, direction)
        {            
            
        }        

        public void addItem(Item item)
        {
            equipment.Add(item);
        }

        public void removeItem(Item item)
        {
            equipment.Remove(item);
            item.x = this.x;
            item.y = this.y;
        }

        public void switchWeapon(Item item)
        {
            if (equipment.Contains(item))
                this.weapon = item;
        }

    }
}
