using MMS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS
{
    public class ConMatrix
    {

        public int Factor { get; set; }
        public int Offset { get; set; }

        private int[,] _matrix = {  {0, 0, 0, 0, 0},
                                    {0, 0, 0, 0, 0},
                                    {0, 0, 1, 0, 0},
                                    {0, 0, 0, 0, 0},
                                    {0, 0, 0, 0, 0}
                                };

        public int[,] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;

                Factor = 0;
                for (int i = 0; i < Size; i++)
                    for (int j = 0; j < Size; j++)
                        Factor += _matrix[i, j];

                if (Factor == 0)
                    Factor = 1;
            }
        }




        private int _size = 5;

        public int Size
        {
            get { return _size; }
            set
            {
                if (value != 1 && value != 3 && value != 5 && value != 7)
                    _size = 5;
                else
                    _size = value;
            }
        }


        public ConMatrix()
        {
            Offset = 0;
            Factor = 1;
        }


    }

    public class Sharpen
    {
        private static Bitmap ImgConvolution(Bitmap image, ConMatrix fmat)
        {
            //Avoid division by 0
            if (fmat.Factor == 0)
                return null;


            Bitmap srcImage = (Bitmap)image.Clone();

            int x, y, filterx, filtery, tempx, tempy;
            int s = fmat.Size / 2;

            int r, g, b, tr, tg, tb;

            int pixelSize = GetPixelSize(image);

            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.WriteOnly, image.PixelFormat);
            BitmapData srcImageData = srcImage.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);

            // Scan0 is the memory address where pixel-array begins.
            // Stride is the width of each row of pixels.
            int stride = srcImageData.Stride;
            IntPtr scan0 = srcImageData.Scan0;

            unsafe
            {
                byte* tempPixel;

                for (y = s; y < srcImageData.Height - s; y++)
                {
                    for (x = s; x < srcImageData.Width - s; x++)
                    {
                        r = g = b = 0;

                        //Convolution
                        for (filtery = 0; filtery < fmat.Size; filtery++)
                        {
                            for (filterx = 0; filterx < fmat.Size; filterx++)
                            {
                                // Get nearby pixel's position
                                tempx = x + filterx - s;
                                tempy = y + filtery - s;

                                // Go to that pixel in pixel-array
                                tempPixel = (byte*)scan0 + (tempy * stride) + (tempx * pixelSize);

                                // The format is BGRA (1 byte each). Get em
                                tb = (int)*tempPixel;
                                tg = (int)*(tempPixel + 1);
                                tr = (int)*(tempPixel + 2);

                                r += fmat.Matrix[filtery, filterx] * tr;
                                g += fmat.Matrix[filtery, filterx] * tg;
                                b += fmat.Matrix[filtery, filterx] * tb;
                            }
                        }

                        // Remove values out of [0,255]
                        r = Math.Min(Math.Max((r / fmat.Factor) + fmat.Offset, 0), 255);
                        g = Math.Min(Math.Max((g / fmat.Factor) + fmat.Offset, 0), 255);
                        b = Math.Min(Math.Max((b / fmat.Factor) + fmat.Offset, 0), 255);

                        // Compute new pixel position (in new image) and write the pixels (BGRA format)
                        byte* newpixel = (byte*)imageData.Scan0 + (y * imageData.Stride) + (x * pixelSize);
                        *newpixel = (byte)b;
                        *(newpixel + 1) = (byte)g;
                        *(newpixel + 2) = (byte)r;
                        // If there is Alpha value
                        if (HasAlpha(image))
                            *(newpixel + 3) = 255;

                    }

                }
            }

            image.UnlockBits(imageData);
            srcImage.UnlockBits(srcImageData);

            return image;

        }

        public static Bitmap FilterSharpen3(Bitmap image)
        {

            ConMatrix matr = new ConMatrix();

            matr.Size = 3;
            matr.Matrix = new int[3, 3] {
                                            { 0 , -2 , 0 },
                                            { -2 , 11 , -2 },
                                            { 0 , -2 , 0 }
                                        };
            return Sharpen.ImgConvolution(image, matr);
        }

        public static Bitmap FilterSharpen5(Bitmap image)
        {
            ConMatrix matr = new ConMatrix();

                matr.Size = 5;

                matr.Matrix = new int[5, 5]
                {
                                    { 0, 0,-2,-2, 0},
                                    {-2, 0,-2, 0, 0},
                                    {-2,-2, 11,-2,-2},
                                    { 0, 0,-2, 0,-2},
                                    { 0,-2,-2, 0, 0}
                };
                return Sharpen.ImgConvolution(image, matr);

            }

        public static Bitmap FilterSharpen7(Bitmap image)
        {
            ConMatrix matr = new ConMatrix();

                matr.Size = 7;

                matr.Matrix = new int[7, 7]
                {
                                    { 0, 0, 0,-2,-2, 0, 0},
                                    { 0, 0, 0,-2,-2, 0, 0},
                                    {-2,-2, 0,-2, 0, 0, 0},
                                    {-2,-2,-2, 11,-2,-2,-2},
                                    { 0, 0, 0,-2, 0,-2,-2},
                                    { 0, 0,-2,-2, 0, 0, 0},
                                    { 0, 0,-2,-2, 0, 0, 0}
                };
                return Sharpen.ImgConvolution(image, matr);
         }

        private static int GetPixelSize(Bitmap image)
        {
            int pixelSize;
            switch (image.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    pixelSize = 1;
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    pixelSize = 2;
                    break;
                case PixelFormat.Format24bppRgb:
                    pixelSize = 3;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    pixelSize = 4;
                    break;
                case PixelFormat.Format48bppRgb:
                    pixelSize = 6;
                    break;
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    pixelSize = 8;
                    break;
                default:
                    pixelSize = -1;
                    break;

            }
            return pixelSize;
        }

        private static bool HasAlpha(Bitmap image)
        {
            PixelFormat pf = image.PixelFormat;
            if (pf == PixelFormat.Format16bppArgb1555 || pf == PixelFormat.Format32bppArgb || pf == PixelFormat.Format32bppPArgb ||
                pf == PixelFormat.Format64bppArgb || pf == PixelFormat.Format64bppPArgb)
                return true;
            else
                return false;

        }
    }
}
