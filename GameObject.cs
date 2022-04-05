using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class GameObject
    {
        public const float PI = (float)Math.PI;
        public int x, y;
        public Animation animation; 
        public IntPtr image;
        
        public GameObject(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public void playAnimation()
        {
            animation.play();
        }

        virtual public void display()
        {
            if (animation.playing == false) 
                SDL.SDL_RenderCopy(animation.renderer, animation.animation[0], IntPtr.Zero, ref animation.targetRect);
        }

        

    }
}
