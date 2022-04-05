using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class Texture
    {
        public const float PI = (float)Math.PI;
        public int width, height;
        public IntPtr image, canvas, preparedTexture;
        public int widthOfCanvas, heightOfCanvas;
        private int maxWidthOfCanvas, maxHeightOfCanvas;



        public Texture(IntPtr renderer, int width, int height, IntPtr image)
        {

            this.width = width;
            this.height = height;
            this.image = image;
            this.maxWidthOfCanvas = 3 * width;
            this.maxHeightOfCanvas = 3*height;

            this.preparedTexture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                maxWidthOfCanvas, maxHeightOfCanvas);

            this.canvas = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                maxWidthOfCanvas, maxHeightOfCanvas);

            drawTextureOnCanvas(renderer, this.image, preparedTexture, maxWidthOfCanvas, maxHeightOfCanvas, 0, 0); // Przygotowujemy teksturę do renderowania 

        }      


        public void textureRectangle(IntPtr renderer, int x, int y, int rectWidth, int rectHeight, float scale, int picX)
        {
            if (scale <= 0)
                Console.WriteLine("The scale must be greater than 0");
            else
            {
                int H = rectHeight;
                SDL.SDL_Rect sRect;
                sRect.x = picX;
                sRect.y = 0;
                sRect.w = Math.Min(this.width - picX, width);


                SDL.SDL_Rect tRect;
                tRect.x = x;
                tRect.y = y;
                tRect.w = sRect.w;

                // Najpierw rysujemy tę przesuniętą warstwę tekstur:
                while (H > 0)
                {
                    if (H / scale >= this.height)
                    {
                        sRect.h = this.height;
                        tRect.h = (Int32)(this.height * scale);
                        H -= tRect.h;
                    }
                    else
                    {
                        sRect.h = (Int32)(H / scale);
                        tRect.h = H;
                        H = 0;
                    }

                    SDL.SDL_RenderCopy(renderer, this.image, ref sRect, ref tRect);
                    tRect.y += (Int32)(this.height * scale); // Przesuwamy się w kierunku pionowym. 

                }
                H = rectHeight; // Resetujemy wysokość
                rectWidth -= tRect.w;
                tRect.x += tRect.w; // Przesuwamy się w prawo
                tRect.y = y; // Resetujemy położenie w kierunku pionowym 
                sRect.x = 0;
                sRect.w = Math.Min(this.width, width);
                tRect.w = sRect.w;

                // Teraz rysujemy całą resztę tekstur
                while (rectWidth > 0)
                {

                    while (H > 0)
                    {
                        if (H / scale >= this.height)
                        {
                            sRect.h = this.height;
                            tRect.h = (Int32)(this.height * scale);
                            H -= (Int32)(this.height * scale);
                        }
                        else
                        {
                            sRect.h = (Int32)(H / scale);
                            tRect.h = H;
                            H = 0;
                        }

                        SDL.SDL_RenderCopy(renderer, this.image, ref sRect, ref tRect);
                        tRect.y += (Int32)(this.height * scale); // Przesuwamy się w kierunku pionowym. 

                    }
                    H = rectHeight; // Resetujemy wysokość
                    rectWidth -= tRect.w;
                    tRect.x += this.width; // Przesuwamy się w prawo
                    tRect.y = y; // Resetujemy położenie w kierunku pionowym 
                    sRect.w = Math.Min(this.width, width);
                    tRect.w = sRect.w;
                }
            }



        } // textureRectangle
        

        private void drawTexture(IntPtr renderer, IntPtr texture, int rectWidth, int rectHeight, int picX, int picY)
        {
            SDL.SDL_Rect sourceRect;
            sourceRect.x = 0;
            sourceRect.y = 0;
            sourceRect.w = this.width;
            sourceRect.h = this.height; 

            SDL.SDL_Rect targetRect;
            targetRect.x = -picX;
            targetRect.y = -picY;
            targetRect.w = sourceRect.w;
            targetRect.h = sourceRect.h;

            int W = rectWidth+picX, H = rectHeight+picY;
            while (W > 0)
            {                
                while (H > 0)
                {
                    SDL.SDL_RenderCopy(renderer, texture, ref sourceRect, ref targetRect);
                    H -= sourceRect.h;
                    targetRect.y += sourceRect.h; // Przesywamy się w dół. 
                }
                W -= sourceRect.w;
                H = rectHeight+picY;
                targetRect.y = -picY; // Resetujemy wysokość
                targetRect.x += sourceRect.w; // Przesuwamy się w prawo 
            }
                
        }


                
        private void drawTextureOnCanvas(IntPtr renderer, IntPtr texture, IntPtr canvas, int rectWidth, int rectHeight, int picX, int picY)
        {
            SDL.SDL_SetRenderTarget(renderer, canvas); // Ustawiamy renderowanie do wewnętrznej tekstury 
            drawTexture(renderer, texture, rectWidth, rectHeight, picX, picY);

            SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero); // Ustawiamy renderowanie spowrotem do okna  
        }

        public void prepareTexture(IntPtr renderer, int rectWidth, int rectHeight, int picX, int picY)
        {
            if (rectWidth > maxWidthOfCanvas)
                rectWidth = maxWidthOfCanvas;
            if (rectHeight > maxHeightOfCanvas)
                rectHeight = maxHeightOfCanvas;
            drawTextureOnCanvas(renderer, this.image, this.canvas, rectWidth, rectHeight, picX,picY);
            widthOfCanvas = rectWidth;
            heightOfCanvas = rectHeight;

        } // getPreparedRectangle    

        public void rotatePreparedTexture(IntPtr renderer, float angleInRad)
        {
            SDL.SDL_SetRenderTarget(renderer, canvas);
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(renderer); // Why renderer, instead of canvas since I set rendering to the canvas?
            if (SDL.SDL_RenderCopyEx(renderer, preparedTexture, IntPtr.Zero, IntPtr.Zero, angleInRad * 360 / 2 / PI, IntPtr.Zero,
                SDL.SDL_RendererFlip.SDL_FLIP_NONE) < 0)
                Console.WriteLine("I failed to rotate the texture.");
            SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero);
        }


        public void drawPreparedTexture(IntPtr renderer, int x, int y, int width, int heigth)
        {
            SDL.SDL_Rect sourceRect;
            sourceRect.x = 0;
            sourceRect.y = 0;
            sourceRect.w = widthOfCanvas;
            sourceRect.h = heightOfCanvas;

            SDL.SDL_Rect targetRect;
            targetRect.x = x;
            targetRect.y = y;
            targetRect.w = width;
            targetRect.h = heigth;
            if (SDL.SDL_RenderCopy(renderer, canvas, ref sourceRect, ref targetRect) < 0)
                Console.WriteLine("Failed to render canvas");
        }


    } // Texture


}