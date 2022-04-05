using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Animation
    {
        public IntPtr[] animation;
        private int ind = 0; // indeks liczący klatki głównej pętli gry 
        private int animIndex; // indeks przebiegający klatki animacji 
        public IntPtr renderer;  
        public SDL.SDL_Rect targetRect;
        public bool playing = false;

        public Animation(IntPtr renderer, IntPtr[] animation)
        {
            this.renderer = renderer;
            this.animation = animation;
        }

        private void increaseIndices()
        {
            if (playing == true && ind % 15 == 0)
            {
                if (animIndex == animation.Length - 1)                
                    stop(); 
                    
                else
                    animIndex++;
            }
                
            
        }

        public void startPlaying()
        {
            playing = true;
        }
        
        public void play()
        {
            //if (animIndex == 0)
                //playing = true;
            if (playing == true)
            {
                SDL.SDL_RenderCopy(renderer, animation[animIndex], IntPtr.Zero, ref targetRect);
                increaseIndices();
            }
            
        }

        public void stop()
        {
            playing = false;
            animIndex = 0;
            ind = 0;
        }


    }
}
