using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Raycasting
{
    // Visplanes are floors and ceilings. Visplane comes from Doom. I don't know what vis- stands for (maybe visible?) but I like it.
    internal class Visplane
    {
        private const float PI= (float)Math.PI;
        // Takie alphaMax odpowiada zasięgowi wzroku 5000 m dla człowieka o wzroście 1,8 m. 
        // Takie alphaMin odpowiada odległości 0,5 m w przypadku osoby o wzroście 1,8 m. 
        //public float alphaMin = Math.Atan(0.5 / 1.8), alphaMax = Math.Atan(5000.0 / 1.8); 
        //public float phiMin=-PI/4, phiMax = PI/4; 
        public int rectX, rectY, height, width;
        private int altitude; 
        public int pieceWidth = 5, pieceHeight = 5;
        private static float meterPerUnit = 600;
        private static float texturePerMeter = 0.5F;
        private static float texturePerUnit = texturePerMeter * meterPerUnit;
        //private double playerHeightInUnits;
        private Player player;
        private Texture texture;
        protected ProjectionPlane projPlane;
        protected int pixelWidth, pixelHeight;
        private float[] phiTable = new float[Program.SCREEN_WIDTH];
        private float[] cosPhiTable= new float[Program.SCREEN_WIDTH]; 
        public int[] depthBuffer, yBuffer;     
        
        public Visplane(int rectX, int rectY, int width, int height, Texture texture, Player player, ProjectionPlane projPlane):this(
            rectX, rectY, width, height, 0, texture, player, projPlane, 2, 2)
        {

        }

        public Visplane(int rectX, int rectY, int width, int height, int altitude, Texture texture, Player player, ProjectionPlane projectionPlane, int pixelWidth, int pixelHeight)
        {
            this.rectX = rectX;
            this.rectY = rectY;
            this.width = width;
            this.height = height;
            this.altitude = altitude;
            this.texture = texture;
            this.player = player;
            this.projPlane = projectionPlane;
            //this.height = Math.Abs(y2 - y1);
            //this.width = Math.Abs(x2 - x1);
            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;

            //this.phiBuffer = new float[height, (int)Math.Ceiling((float)width/pixelWidth)];
            //this.distBuffer = new float[height, (int)Math.Ceiling((float)width/pixelWidth)];
            //this.localXBuffer = new float[height, (int)Math.Ceiling((float)width / pixelWidth)];
            //this.localYBuffer = new float[height, (int)Math.Ceiling((float)width / pixelWidth)];
            fillPhiTable();
            fillCosPhiTable();
            
        }        

        private float findPhi(int x, int y)
        {
            float t = x-Program.SCREEN_WIDTH/2;
            float phi = MathF.Atan(t / projPlane.distToCamera);
            //Console.WriteLine("phi="+phi);
            return phi;
        }

        public void fillPhiTable()
        {
            int n = phiTable.GetLength(0);
            for (int x = 0; x<n; x++)
                phiTable[x] = findPhi(x,rectY);
        }

        public void fillCosPhiTable()
        {
            int n = cosPhiTable.GetLength(0);
            for (int x = 0; x < n; x++)
                cosPhiTable[x] = MathF.Cos(phiTable[x]);

        }

        private float findHeightOfProjection(int x, int y)
        {
            //float t = (float)(y-Program.SCREEN_HEIGHT/2)/(Program.SCREEN_HEIGHT/2);
            //float height = (1 - t) * projectionPlane.heightOfCenter + t * (projectionPlane.heightOfCenter-Program.SCREEN_HEIGHT/2); // convex combination 
            float height = projPlane.heightOfCenter - (y - Program.SCREEN_HEIGHT / 2);
            return height; 
        }

        private int findYRelativeToMiddleOfScreen(int x, int y)
        {
            int relativeY = y-projPlane.height/2;
            return relativeY; 
        }

        private float findPerpendicularDistance(int x, int y)
        {
            int relativeY = findYRelativeToMiddleOfScreen(x, y);
            float dist = (float)projPlane.distToCamera/relativeY*(projPlane.heightOfCenter-this.altitude);
            return dist;
        }

        private float findPerpendicularDistanceOld(int x, int y)
        {
            float height = findHeightOfProjection(x, y);
            float dist = (float)projPlane.heightOfCenter * projPlane.distToCamera / (projPlane.heightOfCenter - height);
            //Console.WriteLine("Perpendicular distance = " + dist);
            return dist; 
        }
       
        // wynik zwracany tutaj jest w jednostkach 
        private float findDistance(int x, int y)
        {            
            float cosPhi = cosPhiTable[x];
            //float phi = findPhi(x, y);
            float perpendicularDistance = findPerpendicularDistance(x, y);
            float dist = perpendicularDistance /cosPhi;
            return dist;
        }

        //private float findLengthOfPieceOfPlane(int x, int y)
        //{
        //    int x1 = x, y1 = y;
        //    int x2 = x1, y2 = y1 + pieceHeight;

        //    float length = findDistance(x1, y1) - findDistance(x2, y2);
        //    return length;
        //}

        //private float findWidthtOfPieceOfPlane(int x, int y)
        //{
        //    int x1 = x, y1 = y;
        //    int x2 = x1 + pieceWidth, y2 = y1;
        //    float phi1 = findPhi(x1, y1);
        //    float phi2 = findPhi(x2, y2);
        //    float deltaPhi = phi2 - phi1;

        //    float dist = findDistance(x1, y1);
        //    float w = deltaPhi * dist;
        //    return w;
        //}
    
        private Vector findPolarCoordsRelativeToPlayer(int x, int y)
        {
            float r = findDistance(x,y);
            float phi = phiTable[x];
            return new Vector(r, phi);
        }

        private Vector findCartesianCoordsRelativeToPlayer(int x, int y)
        {           
            Vector vec = findPolarCoordsRelativeToPlayer(x, y).findCartesianCoordinates();            
            return vec;
        }

        private Vector findAbsoluteCoordinates(int x, int y)
        {            
            Vector vec = findCartesianCoordsRelativeToPlayer(x, y).findRotatedCoordinates(-player.direction);
            Vector translation = new Vector(-player.x, -player.y);
            vec = vec.findShiftedCoordinates(translation);
            return vec;
        }

        private Vector findPointOnTexture(int x, int y)
        {
            Vector coords = findAbsoluteCoordinates(x, y);
            int picX = (int)Math.Abs((coords.x % texture.width)); 
            int picY = (int)Math.Abs((coords.y % texture.height));
            return new Vector(picX, picY);
        }

        //private float findAngleToRotateTexture()
        //{
        //    float angle= player.direction-3*PI/2; // Jak gracz jest zwrócony na północ (direction = 3PI/2) to nie musimy obracać tekstury. 
        //    if (angle < 0)
        //        angle += 2 * PI; 
        //    if (angle >2*PI)
        //        angle -= 2 * PI;
        //    return angle;
        //}

        // Finds the image of a point from texture on the preparedTexture
        private Vector findPointOnPreparedTexture(int picX, int picY)
        {
            int x = picX + texture.width;
            int y = picY + texture.height;
            return new Vector(x, y);
        }

        // Rendering floor using pieces of texture, not just pixels, is very fast, unfortunatly looks very bad when you look at some directions. 
        // We would need to rotate the texture what is very slow, moreover it seems we would need to rotate the texture in diferent directions
        // depending on the point in space. I don't even know how to determine the directions. 
        //private void texturePieceOfPlane(IntPtr renderer, int x, int y)
        //{
        //    Vector pointOnTexture = findPointOnTexture(x, y);
        //    int picX = (int)pointOnTexture.x;
        //    int picY = (int)pointOnTexture.y;

        //    Vector point = findPointOnPreparedTexture(picX, picY);
        //    int length = (int)findLengthOfPieceOfPlane(x, y);
        //    int width = (int)findWidthtOfPieceOfPlane(x, y);

        //    SDL.SDL_Rect sourceRect;
        //    sourceRect.x = (int)point.x;
        //    sourceRect.y = (int)point.y;
        //    sourceRect.w = width;
        //    sourceRect.h = length;

        //    SDL.SDL_Rect destRect;
        //    destRect.x = x;
        //    destRect.y = y;
        //    destRect.w = pieceWidth;
        //    destRect.h = pieceHeight;

        //    SDL.SDL_RenderCopy(renderer, texture.preparedTexture, ref sourceRect, ref destRect);            

        //}    

        private void showPoint(IntPtr renderer, int x, int y)
        {
            Vector point = findAbsoluteCoordinates(x, y);
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL.SDL_RenderDrawPoint(renderer, (int)point.x, (int)point.y);
        }

        public void showLine(IntPtr renderer, int y)
        {
            for (int x = 0; x < Program.SCREEN_WIDTH; x++)
                showPoint(renderer, x, y);
        }

        protected void texturePixel(IntPtr renderer, int x, int y, int pixelWidth, int pixelHeight)
        {
            Vector point = findPointOnTexture(x, y);
            int picX = (int)point.x;
            int picY = (int)point.y;

            SDL.SDL_Rect sourceRect;
            sourceRect.x = picX; 
            sourceRect.y = picY;
            sourceRect.w = 1; 
            sourceRect.h = 1;

            SDL.SDL_Rect targetRect; 
            targetRect.x = x;
            targetRect.y = y;
            targetRect.w = pixelWidth;
            targetRect.h = pixelHeight;

            SDL.SDL_RenderCopy(renderer, texture.image, ref sourceRect, ref targetRect);
        }

        protected void textureVerticalSlice(IntPtr renderer, int rectX, int rectY, int length)
        {

            while (length > 0)
            {
                texturePixel(renderer, rectX, rectY, pixelWidth, pixelHeight);
                rectY += pixelHeight;
                length -= pixelHeight;
            }

        }

        public virtual void render(IntPtr renderer)
        {

        }


    }
}
