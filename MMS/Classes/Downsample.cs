using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMS
{
    public class Downsample
    {
        //downsampling
        public static void YCBCRTORGB(byte[] ycbcr, byte[] rgb)
        {
            int temp;

            temp = (int)(ycbcr[0] + 1.4 * (float)(ycbcr[2] - 128));

            if (temp > 255)
                temp = 255;
            else if (temp < 0)
                temp = 0;
            rgb[0] = (byte)temp;

            temp = (int)(ycbcr[0] - 0.343 * (float)(ycbcr[1] - 128) - 0.711 * (float)(ycbcr[2] - 128));
            if (temp > 255)
                temp = 255;
            else if (temp < 0)
                temp = 0;
            rgb[1] = (byte)temp;

            temp = (int)(ycbcr[0] + 1.765 * (float)(ycbcr[1] - 128));
            if (temp > 255)
                temp = 255;
            else if (temp < 0)
                temp = 0;
            rgb[2] = (byte)temp;
        }

        public static Bitmap[] Downsamples(Bitmap m_Bitmap)
        {
            Bitmap[] temp = new Bitmap[3];

            temp[0] = (Bitmap)m_Bitmap.Clone();
            temp[1] = (Bitmap)m_Bitmap.Clone();
            temp[2] = (Bitmap)m_Bitmap.Clone();

            byte Y, cb, cr;
            Y = cb = cr = 0;
            byte[] y1 = new byte[3];

            //Glavna slika
            BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //povratna vrednost
            BitmapData[] tempData = new BitmapData[3];

            tempData[0] = temp[0].LockBits(new Rectangle(0, 0, temp[0].Width, temp[0].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            tempData[1] = temp[1].LockBits(new Rectangle(0, 0, temp[1].Width, temp[1].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            tempData[2] = temp[2].LockBits(new Rectangle(0, 0, temp[2].Width, temp[2].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //Instance glavne slike
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            //instance za povratne slike
            int[] tempStrade = new int[3];
            tempStrade[0] = tempData[0].Stride;
            tempStrade[1] = tempData[1].Stride;
            tempStrade[2] = tempData[2].Stride;

            System.IntPtr[] tempScan0 = new System.IntPtr[3];
            tempScan0[0] = tempData[0].Scan0;
            tempScan0[1] = tempData[1].Scan0;
            tempScan0[2] = tempData[2].Scan0;

            unsafe
            {
                //Orginalna slika
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - m_Bitmap.Width * 3;

                byte* temp0 = (byte*)(void*)tempScan0[0];
                byte* temp1 = (byte*)(void*)tempScan0[1];
                byte* temp2 = (byte*)(void*)tempScan0[2];

                int tempOffset0 = tempStrade[0] - temp[0].Width * 3;
                int tempOffset1 = tempStrade[1] - temp[1].Width * 3;
                int tempOffset2 = tempStrade[2] - temp[2].Width * 3;

                byte[] y = new byte[3];

                for (int i = 0; i < m_Bitmap.Height; ++i)
                {
                    for (int j = 0; j < m_Bitmap.Width; ++j)
                    {

                        y[0] = (byte)((0.299 * (float)p[2] + 0.587 * (float)p[1] + 0.114 * (float)p[0]));
                        y[1] = (byte)(128 + (byte)((-0.16874 * (float)p[2] - 0.33126 * (float)p[1] + 0.5 * (float)p[0])));
                        y[2] = (byte)(128 + (byte)((0.5 * (float)p[2] - 0.41869 * (float)p[1] - 0.08131 * (float)p[0])));

                        if (j % 2 == 0)
                        {
                            Y = y[0];
                            cb = y[1];
                            cr = y[2];
                        }

                        y1[0] = y[0];
                        y1[1] = cb;
                        y1[2] = cr;

                        byte[] rgb = new byte[3];

                        YCBCRTORGB(y1, rgb);
                        temp0[0] = rgb[2];
                        temp0[1] = rgb[1];
                        temp0[2] = rgb[0];

                        y1[0] = Y;
                        y1[1] = y[1];
                        y1[2] = cr;

                        YCBCRTORGB(y1, rgb);
                        temp1[0] = rgb[2];
                        temp1[1] = rgb[1];
                        temp1[2] = rgb[0];

                        y1[0] = Y;
                        y1[1] = cb;
                        y1[2] = y[2];

                        YCBCRTORGB(y1, rgb);
                        temp2[0] = rgb[2];
                        temp2[1] = rgb[1];
                        temp2[2] = rgb[0];

                        p += 3;

                        temp0 += 3;
                        temp1 += 3;
                        temp2 += 3;
                    }

                    p += nOffset;

                    temp0 += tempOffset0;
                    temp1 += tempOffset1;
                    temp2 += tempOffset2;
                }
            }

            m_Bitmap.UnlockBits(bmData);

            temp[0].UnlockBits(tempData[0]);
            temp[1].UnlockBits(tempData[1]);
            temp[2].UnlockBits(tempData[2]);


            return temp;
        }

        //saving compressed channels
        public static void saveR(Bitmap bitmap)
        {
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb); // PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            byte[] izlaz = new byte[(bitmap.Width * 2 * bitmap.Height) + ((bitmap.Width % 2 == 1) ? 2 * bitmap.Height : 0)];
            int n = 0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;

                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        byte[] nizR = { (byte)p[2], (byte)p[5] };
                        byte[] nizG = { (byte)p[1], (byte)p[4] };
                        byte[] nizB = { (byte)p[0], (byte)p[3] };
                        p += 6;

                        byte G = (byte)((nizG[0] + nizG[1]) / 2);
                        byte B = (byte)((nizB[0] + nizB[1]) / 2);

                        izlaz[n] = nizR[0];
                        izlaz[n + 1] = nizR[1];
                        izlaz[n + 2] = G;
                        izlaz[n + 3] = B;
                        n += 4;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bmData);
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "Compressed files (*.r)|*.r";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                saveCompressedFile(izlaz, saveFileDialog.FileName, bitmap);
            }
        }

        public static void saveG(Bitmap bitmap)
        {
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb); // PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            byte[] izlaz = new byte[(bitmap.Width * 2 * bitmap.Height) + ((bitmap.Width % 2 == 1) ? 2 * bitmap.Height : 0)];
            int n = 0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;

                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        //pretvaranje iz RGB u XYZ
                        byte[] nizR = { (byte)p[2], (byte)p[5] };
                        byte[] nizG = { (byte)p[1], (byte)p[4] };
                        byte[] nizB = { (byte)p[0], (byte)p[3] };
                        p += 6;

                        byte R = (byte)((nizR[0] + nizR[1]) / 2);
                        byte B = (byte)((nizB[0] + nizB[1]) / 2);

                        izlaz[n] = R;
                        izlaz[n + 1] = nizG[0];
                        izlaz[n + 2] = nizG[1];
                        izlaz[n + 3] = B;
                        n += 4;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bmData);
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "Compressed files (*.g)|*.g";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                saveCompressedFile(izlaz, saveFileDialog.FileName, bitmap);
            }
        }

        public static void saveB(Bitmap bitmap)
        {
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb); // PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            byte[] izlaz = new byte[(bitmap.Width * 2 * bitmap.Height) + ((bitmap.Width % 2 == 1) ? 2 * bitmap.Height : 0)];
            int n = 0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;

                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        //pretvaranje iz RGB u XYZ
                        byte[] nizR = { (byte)p[2], (byte)p[5] };
                        byte[] nizG = { (byte)p[1], (byte)p[4] };
                        byte[] nizB = { (byte)p[0], (byte)p[3] };
                        p += 6;

                        byte R = (byte)((nizR[0] + nizR[1]) / 2);
                        byte G = (byte)((nizG[0] + nizG[1]) / 2);

                        izlaz[n] = R;
                        izlaz[n + 1] = G;
                        izlaz[n + 2] = nizB[0];
                        izlaz[n + 3] = nizB[1];
                        n += 4;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(bmData);
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            //saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "Compressed files (*.b)|*.b";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                saveCompressedFile(izlaz, saveFileDialog.FileName, bitmap);
            }
        }

        public static void saveCompressedFile(byte[] izlaz, string fileName, Bitmap bitmap)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(bitmap.Width);
            sw.WriteLine(bitmap.Height);
            byte[] outByteArray = new byte[izlaz.Length];
            int len = ShannonFano.Compress(izlaz, outByteArray, (uint)izlaz.Length);
            sw.WriteLine(len);
            sw.Close();

            FileStream fs = new FileStream(fileName, FileMode.Append);
            fs.Write(outByteArray, 0, len);
            fs.Close();

        }

        //opening compressed files
        public static Bitmap openCompressedFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Compressed files(*.r, *.g, *.b)|*.r;*.g;*.b";

            if (DialogResult.OK == o.ShowDialog() && o.FileName != null)
            {

                StreamReader sr = new StreamReader(o.FileName);
                string channel = Path.GetExtension(o.FileName);
                int w = Int32.Parse(sr.ReadLine());
                int h = Int32.Parse(sr.ReadLine());
                int len = Int32.Parse(sr.ReadLine());
                sr.Close();
                FileStream fs = File.OpenRead(o.FileName);
                fs.Seek(3 * sizeof(int) + 5, SeekOrigin.Begin);
                byte[] inA = new byte[len];
                fs.Read(inA, 0, len);
                fs.Close();
                byte[] byteArray = new byte[w * h * 2 + ((w % 2 == 1) ? 2 * h : 0)];
                ShannonFano.Decompress(inA, byteArray, (uint)len, (uint)(w * h * 2 + ((w % 2 == 1) ? 2 * h : 0)));
                Bitmap outB = new Bitmap(w, h);
                switch (channel)
                {
                    case ".r":
                        outB = openR(w, h, byteArray);
                        break;
                    case ".g":
                        outB = openG(w, h, byteArray);
                        break;
                    case ".b":
                        outB = openB(w, h, byteArray);
                        break;
                }
                return (Bitmap)outB.Clone();
            }
            else
            {
                return null;
            }
        }

        private static Bitmap openR(int w, int h, byte[] byteArray)
        {
            Bitmap rChannel = new Bitmap(w, h);
            BitmapData bmDataR = rChannel.LockBits(new Rectangle(0, 0, rChannel.Width, rChannel.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr ScanR = bmDataR.Scan0;
            unsafe
            {
                int stride = bmDataR.Stride;
                byte* r = (byte*)(void*)ScanR;
                int nOffset = stride - w * 3;
                int nWidth = w * 3;
                int n = 0;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        byte[] nizR = { byteArray[n], byteArray[n + 1] };
                        byte G = byteArray[n + 2], B = byteArray[n + 3];
                        //prikazivanje R - normal
                        r[2] = nizR[0];
                        r[5] = nizR[1];
                        r[1] = r[4] = G;
                        r[0] = r[3] = B;
                        r += 6;
                        n += 4;
                    }
                    r += nOffset;
                }
                rChannel.UnlockBits(bmDataR);
            }
            return (Bitmap)rChannel.Clone();
        }

        private static Bitmap openG(int w, int h, byte[] byteArray)
        {
            Bitmap gChannel = new Bitmap(w, h);
            BitmapData bmDataG = gChannel.LockBits(new Rectangle(0, 0, gChannel.Width, gChannel.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr ScanG = bmDataG.Scan0;
            unsafe
            {
                int stride = bmDataG.Stride;
                byte* g = (byte*)(void*)ScanG;
                int nOffset = stride - w * 3;
                int nWidth = w * 3;
                int n = 0;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        byte[] nizG = { byteArray[n + 1], byteArray[n + 2] };
                        byte R = byteArray[n], B = byteArray[n + 3];
                        //prikazivanje R - normal
                        g[2] = g[5] = R;
                        g[1] = nizG[0];
                        g[4] = nizG[1];
                        g[0] = g[3] = B;
                        g += 6;
                        n += 4;
                    }
                    g += nOffset;
                }
                gChannel.UnlockBits(bmDataG);
            }
            return (Bitmap)gChannel.Clone();
        }

        private static Bitmap openB(int w, int h, byte[] byteArray)
        {
            Bitmap bChannel = new Bitmap(w, h);
            BitmapData bmDataB = bChannel.LockBits(new Rectangle(0, 0, bChannel.Width, bChannel.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr ScanB = bmDataB.Scan0;
            unsafe
            {
                int stride = bmDataB.Stride;
                byte* b = (byte*)(void*)ScanB;
                int nOffset = stride - w * 3;
                int nWidth = w * 3;
                int n = 0;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < nWidth; j += 6)
                    {
                        byte[] nizB = { byteArray[n + 2], byteArray[n + 3] };
                        byte R = byteArray[n], G = byteArray[n + 1];
                        //prikazivanje R - normal
                        b[2] = b[5] = R;
                        b[1] = b[4] = G;
                        b[0] = nizB[0];
                        b[3] = nizB[1];
                        b += 6;
                        n += 4;
                    }
                    b += nOffset;
                }
                bChannel.UnlockBits(bmDataB);
            }
            return (Bitmap)bChannel.Clone();
        }
    }
}
