using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    internal class MyTimeClock
    {
        // Przed konstruktorem musimy mieć zadeklarowane pola klasy/obiektu. 
        // Obiekt klasy MyTimeClock musi pamiętać czasy z końca i początku pętli. 
        private UInt64 StartTime, EndTime, tickTime = 0;

        public MyTimeClock() //Konstruktor obiektu klasy 
        {
            this.StartTime = 0;
            this.EndTime = 0;
            //this.i = 1;
        }
        public void tick(int fps) // Procedura spowalniajaca program. 
        {
            this.EndTime = SDL.SDL_GetPerformanceCounter();
            // Po podzieleniu przez 10.000 dostajemy liczbę milisekund:
            float elapsedTime = (this.EndTime - this.StartTime) / 10000; // Przy pierwszym wykonaniu elapsedTime będzie bardzo dużą liczbą, bo startTime będzie równe 0.

            float msPerFrame = 1000 / fps; // Obliczamy ile czasu musi trwać jedna klatka programu dla ustalonej liczby FPS.
            // Problem jest taki, że np. 1000/60 = 16,66 ms, a nie da się SDL czekać 16,66 ms. Można kazać mu czekać albo 16, albo 17 ms. 
            if (msPerFrame - elapsedTime > 0)
            {
                UInt32 delay = Convert.ToUInt32(Math.Round(msPerFrame - elapsedTime, 0));
                SDL.SDL_Delay(delay);
            }
            this.StartTime = this.EndTime;
            //if (this.i % fps == 0) // To się wykonuje co {fps} klatek. 
            if ((this.EndTime - tickTime) / 10000 >= 1000) //To się będzie wykonywało co sekundę.
            {
                //Console.WriteLine("Zegar tyknął.");
                //Console.WriteLine($"Współrzędne Słońca: x= {Program.SystemCenterX}, y={Program.SystemCenterY}");
                tickTime = this.EndTime; //Notujemy nowy czas "tyknięcia" zegara. 

            }


        }

    }
}
