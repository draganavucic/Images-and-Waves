using MetroFramework;
using MetroFramework.Forms;
using MMS.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMS.View
{
    public partial class FormView : MetroForm, IView
    {
        IController controller;
        bool FlagOneImage;

        public FormView()
        {
            InitializeComponent();
            ShowOneImage();
            StartMode();
            myTabControl.SelectedIndex = 0;
            myTabControl.SelectedIndexChanged += new EventHandler(Tabs_SelectedIndexChanged);
        }

        //mvc functions
        public void AddListener(IController c)
        {
            this.controller = c;
        }

        public void showPictures(Bitmap b, Bitmap[] rgb)
        {
            if (FlagOneImage)
                pictureBoxSingle.Image = b;
            else
                pictureBoxMain.Image = b;

            if (rgb != null)
            {
                this.pictureBoxR.Image = rgb[0];
                this.pictureBoxG.Image = rgb[1];
                this.pictureBoxB.Image = rgb[2];
            }
        }

        //my help functions
        void StartMode()
        {
            panelFile.Enabled = true;
            panelFilters.Enabled = false;
            panelOptions.Enabled = false;
            labelFilename.Visible = true;
            labelSoundFilename.Visible = false;
        }

        void ShowOneImage()
        {
            FlagOneImage = true;
            pictureBoxSingle.Visible = true;
            pictureBoxR.Visible = false;
            pictureBoxG.Visible = false;
            pictureBoxB.Visible = false;
            pictureBoxMain.Visible = false;

        }

        void ShowFourImages()
        {
            FlagOneImage = false;
            pictureBoxSingle.Visible = false;
            pictureBoxR.Visible = true;
            pictureBoxG.Visible = true;
            pictureBoxB.Visible = true;
            pictureBoxMain.Visible = true;
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch(myTabControl.SelectedIndex)
            {
                case 0:
                    if (pictureBoxSingle.Image != null)
                        ImageMode(true);
                    else
                        StartMode();
                    break;
                case 1:
                    ImageMode(false);
                    labelFilename.Visible = true;
                    labelSoundFilename.Visible = false;
                    break;
                case 2:
                    ImageMode(false);
                    break;
                default:
                    ImageMode(false);
                    labelFilename.Visible = false;
                    labelSoundFilename.Visible = false;
                    break;

            }

            
        }

        void ImageMode(bool b)
        {
            panelFile.Enabled = b;
            panelOptions.Enabled = b;
            panelFilters.Enabled = b;
            labelFilename.Visible = b;
            labelSoundFilename.Visible = !b;
        }

        //load image, save image, exit
        private void tileLoadImage_Click(object sender, EventArgs e)
        {
            ShowOneImage();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images (*.jpg,*.jpeg,*.png,*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            ofd.FilterIndex = 5;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    Bitmap bmp = (Bitmap)Bitmap.FromFile(ofd.FileName);
                    labelFilename.Text = ofd.FileName;
                    //poziva kontroler
                    controller.onLoadImage(bmp);

                    textboxPictureInfo.Text = "Name: " + Path.GetFileName(ofd.FileName) + Environment.NewLine;
                    textboxPictureInfo.Text += "Height: " + bmp.Height + Environment.NewLine;
                    textboxPictureInfo.Text += "Width: " + bmp.Width + Environment.NewLine;
                    textboxPictureInfo.Text += "Pixel Format: " + bmp.PixelFormat + Environment.NewLine;

                    ImageMode(true);

                    if (bmp.PixelFormat.ToString() != "Format24bppRgb")
                    {
                        tile256colors.Enabled = false;
                        textboxConversion.Text = "Conversion to 8bit bitmap format is NOT possible." + Environment.NewLine +
                            "Only 24bit bitmaps are allowed.";
                    }
                    else
                    {
                        textboxConversion.Text = "Conversion to 8bit bitmap format is possible.";
                    }
                }
                catch (ApplicationException ex)
                {
                    MetroMessageBox.Show(this, "Exception has trown in reading image from disk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void tileSaveImage_Click(object sender, EventArgs e)
        {
            if (FlagOneImage)
            {
                try
                {
                    if (pictureBoxSingle.Image == null)
                    {
                        MetroMessageBox.Show(this, "There is no image to save. Please load image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SaveFileDialog SaveDialog = new SaveFileDialog();
                        SaveDialog.Filter = "Bitmap Image|*.bmp|JPG Image|*.jpg |JPEG Image|*.jpeg |PNG Image|*.png";
                        SaveDialog.Title = "Save Edited Image";
                        if (SaveDialog.ShowDialog() == DialogResult.OK)
                        {
                            pictureBoxSingle.Image.Save(SaveDialog.FileName);
                            MetroMessageBox.Show(this, "Image Successfully Saved.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (ApplicationException ex)
                {
                    MetroMessageBox.Show(this, "Exception has trown in saving image to disk: " + ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                DialogResult d = MetroMessageBox.Show(this, "Do you want to save all 4 images at once?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (d == DialogResult.Yes)
                {
                    pictureBoxMain.Image.Save(Path.GetDirectoryName(labelFilename.Text.ToString()) + "\\" + Path.GetFileNameWithoutExtension(labelFilename.Text.ToString()) + "MAIN.bmp");
                    pictureBoxR.Image.Save(Path.GetDirectoryName(labelFilename.Text.ToString()) + "\\" + Path.GetFileNameWithoutExtension(labelFilename.Text.ToString()) + "RED.bmp");
                    pictureBoxG.Image.Save(Path.GetDirectoryName(labelFilename.Text.ToString()) + "\\" + Path.GetFileNameWithoutExtension(labelFilename.Text.ToString()) + "GREEN.bmp");
                    pictureBoxB.Image.Save(Path.GetDirectoryName(labelFilename.Text.ToString()) + "\\" + Path.GetFileNameWithoutExtension(labelFilename.Text.ToString()) + "BLUE.bmp");

                    MetroMessageBox.Show(this, "Image Successfully Saved.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void tileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //basic filters - RGB, Gamma, Sharpen, 256colors bitmap
        private void filterRGBChannel_Click(object sender, EventArgs e)
        {
            ShowFourImages();

            if(checkboxMaxMin.Checked==true)
            {
                byte max = Byte.Parse(textboxMax.Text);
                byte min = Byte.Parse(textboxMin.Text);

                controller.onHistogram(true, max, min);
            }

            if (chechboxHistogram.Checked == false)
            {
                if (toggleUnsafeCode.Checked == false)
                {
                    controller.onSafeFilterColor();
                }
                else
                {
                    controller.onUnsafeFilterColor();
                }
            }
            else
            {
                controller.onHistogram(false, 0, 0);
            }

        }

        private void filterGamma_Click(object sender, EventArgs e)
        {
            int[] rgb = new int[3];

                rgb[0] = Int32.Parse(textboxRed.Text.ToString());
                rgb[1] = Int32.Parse(textboxGreen.Text.ToString());
                rgb[2] = Int32.Parse(textboxBlue.Text.ToString());
            

                if (toggleUnsafeCode.Checked == false)
                {
                    controller.onSafeGammaFilter(FlagOneImage, rgb[0], rgb[1], rgb[2]);

                }
                else
                {
                    controller.onUnsafeGammaFilter(FlagOneImage, rgb[0], rgb[1], rgb[2]);

                }

        }

        private void filterSharpen_Click(object sender, EventArgs e)
        {

            if (checkboxSharpen.Checked == false)
            {
                if (toggleUnsafeCode.Checked == false)
                {
                    controller.onSafeSharpenFilter(FlagOneImage);

                }
                else
                {
                    controller.onUnsafeSharpenFilter(FlagOneImage);
                }
            }
            else
            {
                ShowFourImages();
                controller.onMatrixSharpen();
            }

        }

        private void tile256colors_Click(object sender, EventArgs e)
        {
            if (pictureBoxSingle.Image == null)
            {
                MetroMessageBox.Show(this, "There is no image to convert. Please load image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (FlagOneImage)
                    controller.IndexedColorsConvert();
                else
                    MetroMessageBox.Show(this, "This option can be only applied on single picture." + Environment.NewLine +
                        "Pixel format of RGB channels aren't 24 bit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //special filters - Pixelate, Edge Enhance
        private void buttonPixelate_Click(object sender, EventArgs e)
        {
            controller.onPixelate(FlagOneImage, Int16.Parse(textboxPixelSize.Text.ToString()), (checkboxGrid.Checked == true) ? true : false);
        }

        private void buttonTreshold_Click(object sender, EventArgs e)
        {
            controller.onEdgeEnhance(FlagOneImage, Byte.Parse(textboxTreshold.Text.ToString()));
        }

        //undo, redo, reset
        private void buttonReset_Click(object sender, EventArgs e)
        {
            ShowOneImage();
            Bitmap bmp = (Bitmap)Bitmap.FromFile(labelFilename.Text.ToString());
            controller.onLoadImage(bmp);
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            if (!controller.onUndo())
            {
                MetroMessageBox.Show(this, "Undo operations are not allowed." + Environment.NewLine +
                    "Note: If it's full, press -Reset- button in order to clean buffer.", "Undo buffer is full or is empty.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            if (!controller.onRedo())
            {
                MetroMessageBox.Show(this, "Redo operations are not allowed." + Environment.NewLine +
                    "Note: If it's full, press -Reset- button in order to clean buffer.", "Redo buffer is full or is empty.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //downsampling and compression
        private void tileDownsample_Click(object sender, EventArgs e)
        {
            ShowFourImages();
            controller.onDownsample();
        }

        private void tileSaveCompressed_Click(object sender, EventArgs e)
        {
            tileDownsample_Click(sender, e);

            if (radiobuttonSaveR.Checked == true)
                controller.onDownsample(1);
            else if (radiobuttonSaveG.Checked == true)
                controller.onDownsample(2);
            else if (radiobuttonSaveB.Checked == true)
                controller.onDownsample(3);
            else
            {
                MetroMessageBox.Show(this, "You have not chosen any picture for saving." + Environment.NewLine +
                       "Please chose R, G or B channel to proceed.", "No channels selected for saving.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tileLoadCompressed_Click(object sender, EventArgs e)
        {
            ShowOneImage();
            pictureBoxSingle.Image = Downsample.openCompressedFile();
            
        }

        //sound processing
        public void PlaySound(string filename)
        {
            labelSoundFilename.Text = filename;
            wmp.URL = filename;
            wmp.Ctlcontrols.play();
        }

        private void tileLoadWav_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();

            of.Filter = "WAV file (*.wav)|*.wav";

            if (of.ShowDialog() == DialogResult.OK && of.FileName != null)
            {
                controller.onLoadSound(of.FileName);
            }
        }

        private void tileSampling_Click(object sender, EventArgs e)
        {
            controller.onNSampling(Int32.Parse(textboxNSamples.Text.ToString()));
        }

        private void tileConcatenate_Click(object sender, EventArgs e)
        {
            int n = 0;
            if (radio2.Checked == true) n = 2;
            if (radio3.Checked == true) n = 3;
            if (radio4.Checked == true) n = 4;

            if (n != 0)
                controller.onConcatenate(n);
        }

        //links
        private void linkElfak_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.elfak.ni.ac.rs");
        }

        private void linkGaga_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://cs.elfak.ni.ac.rs/nastava/user/profile.php?id=2316");
        }

    }


}