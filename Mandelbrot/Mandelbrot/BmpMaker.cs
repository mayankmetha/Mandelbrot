using System;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Mandelbrot
{
    class BmpMaker
    {
        const int headerSize = 54;
        readonly byte[] buffer;

        public BmpMaker(int width, int height)
        {
            Width = width;
            Height = height;

            int numPixels = Width * Height;
            int numPixelBytes = 4 * numPixels;
            int fileSize = headerSize + numPixelBytes;
            buffer = new byte[fileSize];
            
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream, Encoding.UTF8))
                {
                    writer.Write(new char[] { 'B', 'M' });  
                    writer.Write(fileSize);                 
                    writer.Write((short)0);                 
                    writer.Write((short)0);                 
                    writer.Write(headerSize);               
                    
                    writer.Write(40);                       
                    writer.Write(Width);                    
                    writer.Write(Height);                   
                    writer.Write((short)1);                 
                    writer.Write((short)32);                
                    writer.Write(0);                        
                    writer.Write(numPixelBytes);            
                    writer.Write(0);                        
                    writer.Write(0);                        
                    writer.Write(0);                        
                    writer.Write(0);                        
                }
            }
        }

        public int Width
        {
            private set;
            get;
        }

        public int Height
        {
            private set;
            get;
        }

        public void SetPixel(int row, int col, Color color)
        {
            SetPixel(row, col, (int)(255 * color.R),
                               (int)(255 * color.G),
                               (int)(255 * color.B),
                               (int)(255 * color.A));
        }

        public void SetPixel(int row, int col, int r, int g, int b, int a = 255)
        {
            int index = (row * Width + col) * 4 + headerSize;
            buffer[index + 0] = (byte)b;
            buffer[index + 1] = (byte)g;
            buffer[index + 2] = (byte)r;
            buffer[index + 3] = (byte)a;
        }

        public ImageSource Generate()
        {
            MemoryStream memoryStream = new MemoryStream(buffer);
            
            ImageSource imageSource = ImageSource.FromStream(() =>
            {
                return memoryStream;
            });
            return imageSource;
        }
    }
}
