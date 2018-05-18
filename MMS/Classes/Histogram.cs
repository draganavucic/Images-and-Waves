using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace MMS
{
    public class Histogram
    {
        //basic histograms
        public static Bitmap[] Histograms(Bitmap b, int brPodX, int brPodY, int nijansaSive)
        {
            Bitmap[] arr = new Bitmap[3];

            int[] rH = new int[brPodX];
            int[] gH = new int[brPodX];
            int[] bH = new int[brPodX];

            int max = 0;

            arr[0] = new Bitmap(brPodX, brPodY);
            arr[1] = new Bitmap(brPodX, brPodY);
            arr[2] = new Bitmap(brPodX, brPodY);
            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmDataR = arr[0].LockBits(new Rectangle(0, 0, arr[0].Width, arr[0].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmDataG = arr[1].LockBits(new Rectangle(0, 0, arr[1].Width, arr[1].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmDataB = arr[2].LockBits(new Rectangle(0, 0, arr[2].Width, arr[2].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr ScanX = bmDataR.Scan0;
            System.IntPtr ScanY = bmDataG.Scan0;
            System.IntPtr ScanZ = bmDataB.Scan0;
            int stride = bmData.Stride;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;

                for (int i = 0; i < b.Height; ++i)
                {
                    for (int j = 0; j < nWidth; j += 3)
                    {
                        //Odvajanje Red komponete
                        double R = p[2];
                        R = (R * brPodX) / 256;
                        rH[(int)R]++;
                        max = (rH[(int)R] > max) ? rH[(int)R] : max;

                        //Odvajanje Green komponete
                        double G = p[1];
                        G = (G * brPodX) / 256;
                        gH[(int)G]++;
                        max = (gH[(int)G] > max) ? gH[(int)G] : max;

                        //Odvajanje Blue komponete
                        double B = p[0];
                        B = (B * brPodX) / 256;
                        bH[(int)B]++;
                        max = (bH[(int)B] > max) ? bH[(int)B] : max;
                        p += 3;
                    }
                    p += nOffset;
                }
                stride = bmDataR.Stride;
                nOffset = stride - arr[0].Width * 3;
                nWidth = arr[0].Width * 3;

                byte* x = (byte*)(void*)ScanX;
                byte* y = (byte*)(void*)ScanY;
                byte* z = (byte*)(void*)ScanZ;

                for (int i = 0; i < brPodY; ++i)
                {
                    for (int j = 0; j < nWidth; j = j + 3)
                    {

                        x[0] = ((int)(((double)rH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        x[1] = ((int)(((double)rH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        x[2] = ((int)(((double)rH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)255 : (byte)nijansaSive;
                        x += 3;


                        y[0] = ((int)(((double)gH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        y[1] = ((int)(((double)gH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)255 : (byte)nijansaSive;
                        y[2] = ((int)(((double)gH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        y += 3;


                        z[0] = ((int)(((double)bH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)255 : (byte)nijansaSive;
                        z[1] = ((int)(((double)bH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        z[2] = ((int)(((double)bH[j / 3] / (double)max) * (double)brPodY) >= (brPodY - i)) ? (byte)0 : (byte)nijansaSive;
                        z += 3;
                    }
                    x += nOffset;
                    y += nOffset;
                    z += nOffset;
                }
            }

            b.UnlockBits(bmData);
            arr[0].UnlockBits(bmDataR);
            arr[1].UnlockBits(bmDataG);
            arr[2].UnlockBits(bmDataB);

            return arr;
        }

        //filter for histogram with limits
        public static Bitmap FilterForHistogramWithLimits(Bitmap b, byte max, byte min)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        p[2] = p[2] < min ? min : p[2] > max ? max : p[2];
                        p[1] = p[1] < min ? min : p[1] > max ? max : p[1];
                        p[0] = p[0] < min ? min : p[0] > max ? max : p[0];

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
            return b;
        }
    
    }
}
