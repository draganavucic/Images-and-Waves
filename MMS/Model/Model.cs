using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MMS.Model
{
    public class Model : IModel
    {
        //image, channels, histograms
        Bitmap image;
        Bitmap[] rgb;
        Bitmap[] hrgb;

        public Bitmap getImage()
        {
            return image;
        }

        public void setImage(Bitmap image)
        {
            this.image = image;
        }

        public void setChannels(Bitmap[] rgbc)
        {
            this.rgb = rgbc;
        }

        public Bitmap[] getChannels()
        {
            return rgb;
        }

        public void setHistograms(Bitmap[] rgbh)
        {
            this.hrgb = rgbh;
        }

        public Bitmap[] getHistograms()
        {
            return hrgb;
        }

        //undo and redo buffers
        const int size = 20;

        Bitmap[] bufferUndo;
        Bitmap[] bufferRedo;
        int i, k;
        int u, r;

        public void ResetUndoBuffer()
        {
            i = 0;
            u = 0;
            k = 0;
            r = 0;
            bufferUndo = null;
            bufferRedo = null;
            bufferUndo = new Bitmap[size];
            bufferRedo = new Bitmap[size];
        }

        public void pushRedo(Bitmap b)
        {
            if (b != null)
            {
                if (k <= size - 2)
                {
                    if (k != 0)
                    {
                        for (int j = k + 1; j > 0; j--)
                            bufferRedo[j] = bufferRedo[j - 1];

                    }
                }
                else if (k >= size - 1)
                {
                    for (int j = size - 1; j > 0; j--)
                        bufferRedo[j] = bufferRedo[j - 1];
                }

                bufferRedo[0] = new Bitmap(b);
                k++;
            }
        }

        public Bitmap popRedo()
        {
            if (r < size - 2)
            {
                r++;
                push(bufferRedo[r]);
                return bufferRedo[r];
            }
            else
            {
                return null;
            }
        }

        public Bitmap pop()
        {
            if (u < size-2)
            {
                u++;
                pushRedo(bufferUndo[u]);
                return bufferUndo[u];
            }
            else
            {
                return null;
            }
        }

        public void push(Bitmap b)
        {
            if (b != null)
            {
                if (i <= size - 2)
                {
                    if (i != 0)
                    {
                        for (int j = i + 1; j > 0; j--)
                            bufferUndo[j] = bufferUndo[j - 1];

                    }
                }
                else if (i >= size - 1)
                {
                    for (int j = size - 1; j > 0; j--)
                        bufferUndo[j] = bufferUndo[j - 1];
                }

                bufferUndo[0] = new Bitmap(b);
                i++;
            }
        }

        //sound functions
        string soundFilename;

        public void setSound(string filename)
        {
            soundFilename = filename;
        }

        public string getSound()
        {
            return soundFilename;
        }
    }
}
