using System;
using SDL2;
using System.IO; // To jest potrzebne do wczytywania plików
using System.Linq; // A to jest potrzebne, żeby .Select działało. 
using System.Threading;

namespace Raycasting
{
    
    class Program
    {

        public const float PI = (float)Math.PI;
        public static IntPtr renderer, renderer0;
        public static byte[] YELLOW = { 255, 255, 0, 255 }, GREY = { 125, 125, 125, 255 }; //, WHITE = { 255, 255, 255, 255 };
        public static byte[] RED = { 255, 0, 0, 255 }, BLUE = { 0, 0, 255, 255 };
        public static int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;
        public static int rangeOfViewDeg = 90; // Ta liczba musi być parzysta. 
        public static float rangeOfViewRad = (float)rangeOfViewDeg/180*PI;
        public static int raysPerDeg = 2;
        public static int numberOfRays = raysPerDeg * rangeOfViewDeg;
        public static float deltaAngle = rangeOfViewRad / (numberOfRays-1); //rangeOfViewRad / rangeOfViewDeg/raysPerDeg;
        
        public static Boundary[] bounds = new Boundary[7];
        public static bool[] keys = new bool[6];
        public static MyTimeClock fpsClock = new MyTimeClock();        
        public static Texture texture;

        public static Creature Czazasty = new Creature(200, 200, PI);
        public static int[] depthBuffer, bottomYBuffer, topYBuffer;

        public static int n = raysPerDeg * rangeOfViewDeg + 1;
        public static Ray[] rays = new Ray[n];
        public static Floor[] floors = new Floor[3];
        public static Ceiling[] ceilings = new Ceiling[3];
        public static Thread thread = new Thread(renderWalls);

        static void Main(string[] args)
        {
            
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);            

            var window = SDL.SDL_CreateWindow("Wolfenstein 3D", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            //var window0 = SDL.SDL_CreateWindow("Wolfenstein 3D", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            //renderer0 = SDL.SDL_CreateRenderer(window0, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            // Włączamy obsługę parametru alfa. 
            if (SDL.SDL_SetRenderDrawBlendMode(renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND) < 0)
                Console.WriteLine($"There was an issue {SDL.SDL_GetError()}");

            // We initialize SDL_ttf:
            if (SDL_ttf.TTF_Init() == -1)
            {
                Console.WriteLine($"There was an issue initilizing SDL_ttf. {SDL.SDL_GetError()}");
                //Console.WriteLine($"There was an issue initilizing SDL_ttf {SDL_ttf.TTF_GetError()}");
                //printf("TTF_Init: %s\n", TTF_GetError());
                //exit(2);
            }

            var Sans = SDL_ttf.TTF_OpenFont("OpenSans-Bold.ttf", 30);
            //if (SDL.SDL_GetError() != null)
            if (Sans == IntPtr.Zero) //Sprwdzamy, czy nie wystąpił błąd w trakcie wczytywania czcionki. 
            {
                Console.WriteLine($"There was an issue with the font:{SDL.SDL_GetError()}");
                SDL.SDL_ClearError();
            }
            
            SDL.SDL_Color WHITE; 
            WHITE.r = 255;
            WHITE.g = 255;
            WHITE.b = 255;
            WHITE.a = 255;

            // Za pomocą tej funkcji tworzę planszę z napisem:
            var SurfaceMessage = SDL_ttf.TTF_RenderText_Solid(Sans, "Jakis tekst.", WHITE);
            //if (SDL.SDL_GetError() != null)
            if (SurfaceMessage == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue with creating SurfaceMessage:{SDL.SDL_GetError()}");
                SDL.SDL_ClearError();
            }


            // Żeby mieć działające polskie znaki muszę zamiast poprzedniej funkcji urzyć tej:
            var SurfaceMessage2 = SDL_ttf.TTF_RenderUTF8_Solid(Sans, "Jakiś tekst ąęśćżźół.", WHITE);
            //if (SDL.SDL_GetError() != null)
            if (SurfaceMessage2 == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue with creating SurfaceMessage2:{SDL.SDL_GetError()}");
                SDL.SDL_ClearError();
            }


            var Message = SDL.SDL_CreateTextureFromSurface(renderer0, SurfaceMessage2);
            //if (SDL.SDL_GetError() != null)
            if (Message == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue with creating Message:{SDL.SDL_GetError()}");
                SDL.SDL_ClearError();
            }

            SDL.SDL_Rect MessageRect;
            MessageRect.x = 100;
            MessageRect.y = 0;
            MessageRect.h = 40;
            MessageRect.w = 200;


            Console.WriteLine("Hello World!");

            float heigthOfCamera = 600; 
            ProjectionPlane projectionPlane = new ProjectionPlane(Program.SCREEN_WIDTH, Program.SCREEN_HEIGHT, Program.rangeOfViewRad, (int)heigthOfCamera);

            Player player = new Player(300, 300, 3*PI/2, projectionPlane);

            bounds[0] = new Boundary(100, 100, 6000, 100);
            bounds[1] = new Boundary(6000, 100, 8000, 2000);
            bounds[2] = new Boundary(2000, 100, 2000, 6000);
            bounds[3] = new Boundary(100, 100, 100, 6000);
            bounds[4] = new Boundary(8000, 2000, 8000, 6000);
            bounds[5] = new Boundary(4000, 100, 4000, 6000);
            bounds[6] = new Boundary(1000, 3000, 3000, 3000);
            //bounds[4] = new Boundary(200, 200, 300, 200); // to chyba ta środkowa ściana. 
            //bounds[5] = new Boundary(300, 200, 400, 300);
            //bounds[6] = new Boundary(400, 300, 300, 400);
            //bounds[7] = new Boundary(300, 400, 400, 400);
            //bounds[8] = new Boundary(400, 400, 200, 400);

           
            //const int raysPerDeg = 4; // Tutaj najczęściej miałem 2
            
            
            int iMax = numberOfRays / 2;
            
            float[] angles = new float[n];

            //for (int i = -iMax; i <= iMax; i++)            
            //angles[i + iMax] = i * rangeOfViewRad / rangeOfViewDeg/raysPerDeg;

            angles[0] = -PI / 4;
            for (int i = 1; i < numberOfRays; i++)
                angles[i] = angles[i - 1] + deltaAngle;
            

            //CompleteRay[] rays = new CompleteRay[n];
            for (int i= 0; i<n; i++)
                rays[i] = new CompleteRay(player, player.direction+angles[i], projectionPlane);

            foreach (Ray r in rays)
                r.setBoundaries(bounds);





            //byte[] textureRow = File.ReadAllLines("C:\\Users\\Piotr\\Desktop\\Bricks.txt").Select(byte.Parse).ToArray();
            var currentDirectory = Directory.GetCurrentDirectory();            
            var parentDirectory = Directory.GetParent(currentDirectory);
            var pathToRaycasting = parentDirectory.Parent.Parent.ToString();
            //Console.WriteLine("Path to Raycasting= "+pathToRaycasting);
            //Console.WriteLine("Path to Raycasting="+parentDirectory.Parent.Parent.ToString());            
            
            
            var textureOfWall = SDL_image.IMG_LoadTexture(renderer, pathToRaycasting+"\\Pictures\\Bricks.png");  
            const int picWidth = 251, picHeight = 239;
            texture = new Texture(renderer, picWidth, picHeight, textureOfWall);

            int numberOfFrames = 15;
            string[] paths = new string[numberOfFrames];
            for (var i = 0; i < paths.Length; i++)
                paths[i] = pathToRaycasting+"\\Pictures\\Katana\\Katana" + (i + 1) + ".png";
            IntPtr[] katanaAnim = new IntPtr[numberOfFrames];
            for (int i = 0;i < katanaAnim.Length; i++)
                katanaAnim[i] = SDL_image.IMG_LoadTexture(renderer, paths[i]);
            Animation animationOfKatana = new Animation(renderer, katanaAnim);

            SDL.SDL_Rect targetRect;
            targetRect.x = 100;
            targetRect.y = 100;
            targetRect.w = 1000;
            targetRect.h = 600;
            animationOfKatana.targetRect = targetRect;

            Item katana = new Item(10, 10);
            katana.animation = animationOfKatana;

            
            player.addItem(katana);
            player.switchWeapon(katana);            

            depthBuffer = new int[SCREEN_WIDTH];
            bottomYBuffer = new int[SCREEN_WIDTH];
            topYBuffer = new int[SCREEN_WIDTH];

            foreach (CompleteRay ray in rays)
            {
                ray.depthBuffer = depthBuffer;
                ray.bottomYBuffer = bottomYBuffer;
                ray.topYBuffer = topYBuffer;
            }          
                

            IntPtr imageOfFloor = SDL_image.IMG_LoadTexture(renderer, pathToRaycasting+"\\Pictures\\Floor.jpg");
            Texture textureOfFloor = new Texture(renderer, 280, 280, imageOfFloor);
            IntPtr imageOfCeiling = SDL_image.IMG_LoadTexture(renderer, pathToRaycasting+"\\Pictures\\Ceiling.png");
            Texture textureOfCeiling = new Texture(renderer, 410, 320, imageOfCeiling);
            floors[0] = new Floor(0, SCREEN_HEIGHT-50, SCREEN_WIDTH, 50, textureOfFloor, player, projectionPlane, 2, 2);
            floors[1] = new Floor(0, SCREEN_HEIGHT-100, SCREEN_WIDTH, 50, textureOfFloor, player, projectionPlane, 3, 3);
            floors[2] = new Floor(0, SCREEN_HEIGHT/2, SCREEN_WIDTH, SCREEN_HEIGHT / 2 - 100, textureOfFloor, player, projectionPlane, 4, 4);

            ceilings[0] = new Ceiling(0, 0, SCREEN_WIDTH, 50, textureOfCeiling, player, projectionPlane, 2, 2);
            ceilings[1] = new Ceiling(0, 50, SCREEN_WIDTH, 50, textureOfCeiling, player, projectionPlane, 3, 3);
            ceilings[2] = new Ceiling(0, 100, SCREEN_WIDTH, SCREEN_HEIGHT / 2 - 100, textureOfCeiling, player, projectionPlane, 4, 4);


            floors[0].yBuffer = bottomYBuffer;
            floors[1].yBuffer= bottomYBuffer;
            floors[2].yBuffer = bottomYBuffer;

            ceilings[0].yBuffer= topYBuffer;
            ceilings[1].yBuffer= topYBuffer;
            ceilings[2].yBuffer= topYBuffer;


            var running = true;
            // Main loop for the program
            while (running)
            {
                // Check to see if there are any events and continue to do so until the queue is empty.
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            running = false;

                            break;
                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            switch (e.window.windowEvent)
                            {
                                case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                    running = false;
                                    break;
                            }

                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            SDL.SDL_Keycode keyCode = e.key.keysym.sym;
                            switch (keyCode)
                            {
                                case SDL.SDL_Keycode.SDLK_UP:
                                    keys[0] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    keys[1] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    keys[2] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    keys[3] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_ESCAPE:
                                    break;
                                case SDL.SDL_Keycode.SDLK_w:
                                    keys[0] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_s:
                                    keys[1] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_a:
                                    keys[2] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_d:
                                    keys[3] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_u:
                                    keys[4] = true;
                                    break;
                                case SDL.SDL_Keycode.SDLK_i:
                                    keys[5] = true;
                                    break;
                                

                            }
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            SDL.SDL_Keycode keyCode2 = e.key.keysym.sym;
                            switch (keyCode2)
                            {
                                case SDL.SDL_Keycode.SDLK_UP:
                                    keys[0] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_DOWN:
                                    keys[1] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_LEFT:
                                    keys[2] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_RIGHT:
                                    keys[3] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_ESCAPE:
                                    break;
                                case SDL.SDL_Keycode.SDLK_w:
                                    keys[0] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_s:
                                    keys[1] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_a:
                                    keys[2] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_d:
                                    keys[3] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_u:
                                    keys[4] = false;
                                    break;
                                case SDL.SDL_Keycode.SDLK_i:
                                    keys[5] = false;
                                    break;
                            }
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            player.weapon.use();
                            break;
                    }
                }

                if (keys[0])
                    player.goForward();
                if (keys[1])
                    player.goBackward();
                if (keys[2])
                    player.turnLeft();
                if (keys[3])
                    player.turnRight();
                if (keys[4])
                    player.moveUp();
                if (keys[5])
                    player.moveDown();

                //// Rysujemy planszę 2D i promienie: 
                //SDL.SDL_SetRenderDrawColor(renderer0, 0, 125, 255, 255);

                //SDL.SDL_RenderClear(renderer0);

                //foreach (CompleteRay r in rays)
                //    r.draw2D(renderer0, YELLOW);

                //foreach (Boundary b in bounds)
                //    b.draw(renderer0, RED);                

                //// Wyświetlamy napis:             

                //if (SDL.SDL_RenderCopy(renderer0, Message, IntPtr.Zero, ref MessageRect) < 0)
                //    Console.WriteLine($"There was an error:{SDL.SDL_GetError()}");

                //SDL.SDL_RenderPresent(renderer0);
                //#####################################################


                // Updatujemy promienie: 
                
                foreach (Ray ray in rays)
                    ray.update();

                resetBuffers();
                //##########################################################3
                

                //Renderujemy grafikę 3D
                SDL.SDL_SetRenderDrawColor(renderer, 125, 125, 125, 255); 

                SDL.SDL_RenderClear(renderer);

                // Ściany:
                renderWalls();
                //thread.Start();
                
                // Podłoga:                 
                renderFloor();

                // Sufit: 
                renderCeiling();

                player.drawCrosshairs(renderer, SCREEN_WIDTH/2, 300);
                player.weapon.playAnimation();
                player.weapon.display();


                SDL.SDL_RenderPresent(renderer);
                //#############################################################3

                

                // Na koniec tyknięcie zegara
                fpsClock.tick(60);

            }

            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyWindow(window);
            //SDL.SDL_DestroyRenderer(renderer0);
            //SDL.SDL_DestroyWindow(window0);
            SDL.SDL_Quit();
            // Environment.Exit(0); // To kończy działanie aplikacji 



        } // Main

        protected static void resetBuffers()
        {
            int n = depthBuffer.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                depthBuffer[i] = -1;
                bottomYBuffer[i] = -1;
                topYBuffer[i] = -1;
            }

        } // resetBuffers

        protected static void renderWalls()
        {            
            foreach (CompleteRay r in rays)
                r.render3D(renderer);
        }

        protected static void renderFloor()
        {
            foreach (Floor f in floors)
                f.render(renderer);
        }

        protected static void renderCeiling()
        {
            foreach (Ceiling c in ceilings)
                c.render(renderer);
        }

    } // Program 
}
