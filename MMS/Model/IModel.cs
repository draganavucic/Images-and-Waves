using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Model
{
    public interface IModel
    {
        void setImage(Bitmap image);
        Bitmap getImage();

        void setChannels(Bitmap[] rgbc);
        Bitmap[] getChannels();

        void setHistograms(Bitmap[] rgbh);
        Bitmap[] getHistograms();

        void ResetUndoBuffer();

        void push(Bitmap b);
        Bitmap pop();

        void pushRedo(Bitmap b);
        Bitmap popRedo();

        void setSound(string filename);
        string getSound();
    }
}
