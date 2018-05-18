using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Controller
{
    public interface IController
    {
        void onLoadImage(Bitmap bmp);

        //rgb channels
        void onSafeFilterColor();
        void onUnsafeFilterColor();

        //gamma filter
        void onSafeGammaFilter(bool f,  int r, int g, int b);
        void onUnsafeGammaFilter(bool f, int r, int g, int b);

        //sharpen
        void onSafeSharpenFilter(bool f);
        void onUnsafeSharpenFilter(bool f);

        //8 bit image
        void IndexedColorsConvert();

        //pixelate
        void onPixelate(bool f, short v, bool g);

        //edge enhance
        void onEdgeEnhance(bool f, byte t);

        //histogram
        void onHistogram(bool maxmin, byte x, byte y);

        //undo
        bool onUndo();
        bool onRedo();

        //sharpen 3x3 5x5 7x7
        void onMatrixSharpen();

        //downsample
        void onDownsample();
        void onDownsample(int k);

        //load sound
        void onLoadSound(string filename);
        void onNSampling(int n);
        void onConcatenate(int c);
    }
}
