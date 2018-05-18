using MMS.Model;
using MMS.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MMS.Controller
{
    public class Controller : IController
    {
        IModel model;
        IView view;

        //mvc functions
        public Controller(IModel model, IView view)
        {
            this.model = model;
            this.view = view;
            this.view.AddListener(this);
        }

        public void onLoadImage(Bitmap bmp)
        {
            model.ResetUndoBuffer();
            model.setImage(bmp);
            model.push(bmp);
            model.setChannels(null);
            model.setHistograms(null);

            view.showPictures(model.getImage(), null);
        }

        //safe and unsafe RGB channels
        public void onUnsafeFilterColor()
        {
            model.setChannels(Filters.UnsafeColorChannels(model.getImage()));
            view.showPictures(model.getImage(), model.getChannels());
            
        }

        public void onSafeFilterColor()
        {
            model.setChannels(Filters.SafeColorChannel(model.getImage()));
            view.showPictures(model.getImage(), model.getChannels());
        }

        //safe and unsafe gamma filter
        public void onSafeGammaFilter(bool f, int r,int g,int b)
        {
            model.setImage(Filters.SafeGammaFilter(model.getImage(), r, g, b));

            if (f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
            else
            {
                Bitmap[] arr = new Bitmap[3];
                arr[0] = Filters.SafeGammaFilter(model.getChannels()[0], r, g, b);
                arr[1] = Filters.SafeGammaFilter(model.getChannels()[1], r, g, b);
                arr[2] = Filters.SafeGammaFilter(model.getChannels()[2], r, g, b);

                model.setChannels(arr);
                view.showPictures(model.getImage(), model.getChannels());
            }
        }

        public void onUnsafeGammaFilter(bool f, int r, int g, int b)
        {
            model.setImage(Filters.UnsafeGammaFilter(model.getImage(), r, g, b));

            if (f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
            else
            {
                Bitmap[] arr = new Bitmap[3];
                arr[0] = Filters.SafeGammaFilter(model.getChannels()[0], r, g, b);
                arr[1] = Filters.SafeGammaFilter(model.getChannels()[1], r, g, b);
                arr[2] = Filters.SafeGammaFilter(model.getChannels()[2], r, g, b);

                model.setChannels(arr);
                view.showPictures(model.getImage(), model.getChannels());
            }
        }

        //safe and unsafe filter sharpen
        public void onSafeSharpenFilter(bool f)
        {
            model.setImage(Filters.SafeFilterSharpen(model.getImage()));
            if(f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
            else
            {
                Bitmap[] arr = new Bitmap[3];
                arr[0] = Filters.SafeFilterSharpen(model.getChannels()[0]);
                arr[1] = Filters.SafeFilterSharpen(model.getChannels()[1]);
                arr[2] = Filters.SafeFilterSharpen(model.getChannels()[2]);
                model.setChannels(arr);
                view.showPictures(model.getImage(), model.getChannels());
            }
        }

        public void onUnsafeSharpenFilter(bool f)
        {
            model.setImage(Sharpen.FilterSharpen3(model.getImage()));
            if(f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
            else
            {
                Bitmap[] arr = new Bitmap[3];
                arr[0] = Sharpen.FilterSharpen3(model.getChannels()[0]);
                arr[1] = Sharpen.FilterSharpen3(model.getChannels()[1]);
                arr[2] = Sharpen.FilterSharpen3(model.getChannels()[2]);
                model.setChannels(arr);
                view.showPictures(model.getImage(), model.getChannels());
            }
        }

        //256 colors bitmap
        public void IndexedColorsConvert()
        {
            Image8bit c = new Image8bit();

            model.setImage(c.ConvertTo8bppFormat(model.getImage()));

            view.showPictures(model.getImage(), null);

            model.push(model.getImage());
        }

        //pixelate
        public void onPixelate(bool f, short v, bool g)
        {
            model.setImage(Filters.Pixelate(model.getImage(), v, g));

            if(f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
            else
            {
                Bitmap[] arr = new Bitmap[3];
                arr[0] = Filters.Pixelate(model.getChannels()[0], v, g);
                arr[1] = Filters.Pixelate(model.getChannels()[1], v, g);
                arr[2] = Filters.Pixelate(model.getChannels()[2], v, g);
                model.setChannels(arr);
                view.showPictures(model.getImage(), model.getChannels());
            }
        }

        //edge enhance
        public void onEdgeEnhance(bool f, byte t)
        {
            model.setImage(Filters.EdgeEnhance(model.getImage(), t));

            if (f)
            {
                view.showPictures(model.getImage(), null);
                model.push(model.getImage());
            }
        }

        //histograms
        public void onHistogram(bool maxmin, byte x, byte y)
        {
            if (model.getChannels() != null)
            {
                
                if (maxmin)
                {
                    model.setImage(Histogram.FilterForHistogramWithLimits(model.getImage(), x, y));
                }
                else
                {
                    model.setHistograms(Histogram.Histograms(model.getImage(), 255, 100, 200));
                }

                view.showPictures(model.getImage(), model.getHistograms());
            }
            else
            {
                onUnsafeFilterColor();
                onHistogram(maxmin, x, y);
            }
        }

        //undo redo
        public bool onUndo()
        {
            Bitmap s = model.pop();

            if (s == null)
                return false;
            else
            {
                view.showPictures(s, null);
                return true;
            }
        }

        public bool onRedo()
        {
            Bitmap s = model.popRedo();

            if (s == null)
                return false;
            else
            {
                view.showPictures(s, null);
                return true;
            }
        }

        //sharpen with 3x3, 5x5 and 7x7
        public void onMatrixSharpen()
        {
            Bitmap tmp1, tmp2, tmp3;

            Bitmap[] arr = new Bitmap[3];

            tmp1 = (Bitmap) model.getImage().Clone();
            tmp2 = (Bitmap)model.getImage().Clone();
            tmp3 = (Bitmap)model.getImage().Clone();


            arr[0] = Sharpen.FilterSharpen3(tmp1);

            arr[1] = Sharpen.FilterSharpen5(tmp2);

            arr[2] = Sharpen.FilterSharpen7(tmp3);

            model.setChannels(arr);
            view.showPictures(model.getImage(), model.getChannels());
        }

        //downsampling and compression
        public void onDownsample()
        {
            model.setChannels(Downsample.Downsamples(model.getImage()));
            view.showPictures(model.getImage(), model.getChannels());
        }

        public void onDownsample(int k)
        {
            if (k == 1)
                Downsample.saveR(model.getChannels()[0]);
            else if(k==2)
                Downsample.saveG(model.getChannels()[1]);
            else if(k==3)
                Downsample.saveB(model.getChannels()[2]);

        }

        //sound processing
        public void onLoadSound(string filename)
        {
            model.setSound(filename);
            view.PlaySound(model.getSound());
        }

        public void onNSampling(int n)
        {
            if (model.getSound() != null)
            {
                model.setSound(WAV.NSampling(model.getSound(), n));
                view.PlaySound(model.getSound());
            }
            else
                MessageBox.Show("In order to procced with sampling, please first load wave file.");
        }

        public void onConcatenate(int c)
        {
            model.setSound(WAV.Concatenation(c));
            if (model.getSound() != null)
                view.PlaySound(model.getSound());
        }
    }
}
