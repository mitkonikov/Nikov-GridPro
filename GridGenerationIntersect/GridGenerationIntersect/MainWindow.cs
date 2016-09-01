using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NikovsAdvancedGridGenerationAlgorithm
{
    public partial class MainWindow : Form
    {
        Bitmap resultImage = new Bitmap(1920, 1080);
        Graphics resultGraphics;

        Bitmap bmpColorPickerStroke = new Bitmap(20, 20);
        Bitmap bmpColorPickerGlow = new Bitmap(20, 20);
        Bitmap bmpColorPickerCircle = new Bitmap(20, 20);
        Bitmap bmpColorPickerBG = new Bitmap(20, 20);
        Graphics gColorPickerStroke;
        Graphics gColorPickerGlow;
        Graphics gColorPickerCircle;
        Graphics gColorPickerBG;
        Color strokeColor;
        Color circleColor;
        Color glowColor;
        Color bgColor;

        BackgroundWorker workerX;
        BackgroundWorker workerY;
        BackgroundWorker workerIntersect;
        bool worker1Completed = false;
        bool worker2Completed = false;
        double partCalcX;
        double partCalcY;
        double partIntersectCalc;
        int finalCalc;
        int calc;

        int offsetXValue = 0;
        int offsetYValue = 0;
        int squareSizeXValue = 0;
        int squareSizeYValue = 0;
        int circleSizeXValue = 0;
        int circleSizeYValue = 0;
        int glowSizeXValue = 0;
        int glowSizeYValue = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gColorPickerStroke = Graphics.FromImage(bmpColorPickerStroke);
            gColorPickerGlow = Graphics.FromImage(bmpColorPickerGlow);
            gColorPickerCircle = Graphics.FromImage(bmpColorPickerCircle);
            gColorPickerBG = Graphics.FromImage(bmpColorPickerBG);
            resultGraphics = Graphics.FromImage(resultImage);

            // Setting up the default colors
            strokeColor = Color.White;
            circleColor = Color.White;
            glowColor = Color.White;
            bgColor = Color.Green;
            // -----------------------------

            gColorPickerStroke.FillRectangle(new SolidBrush(strokeColor), 0, 0, 20, 20);
            gColorPickerGlow.FillRectangle(new SolidBrush(circleColor), 0, 0, 20, 20);
            gColorPickerCircle.FillRectangle(new SolidBrush(glowColor), 0, 0, 20, 20);
            gColorPickerBG.FillRectangle(new SolidBrush(bgColor), 0, 0, 20, 20);

            pictureBox2.Image = bmpColorPickerStroke; // Stroke
            pictureBox3.Image = bmpColorPickerGlow; // Glow
            pictureBox4.Image = bmpColorPickerCircle; // Circle
            pictureBox5.Image = bmpColorPickerBG; // Background

            partCalcX = 100.0 / resultImage.Size.Width;
            partCalcY = 100.0 / resultImage.Size.Height;
            finalCalc = (int)Math.Round(Convert.ToDouble(100.0 / 4.0));
            partIntersectCalc = 100.0 / (resultImage.Size.Height * resultImage.Size.Width);

            // Background Workers Setup ------------------------------------------------
            workerX = new BackgroundWorker();
            workerX.WorkerReportsProgress = true;
            workerX.ProgressChanged += workerX_ProgressChanged;
            workerX.DoWork += workerX_DoWork;
            workerX.RunWorkerCompleted += workerX_RunWorkerCompleted;

            workerY = new BackgroundWorker();
            workerY.WorkerReportsProgress = true;
            workerY.ProgressChanged += workerY_ProgressChanged;
            workerY.DoWork += workerY_DoWork;
            workerY.RunWorkerCompleted += workerY_RunWorkerCompleted;

            workerIntersect = new BackgroundWorker();
            workerIntersect.WorkerReportsProgress = true;
            workerIntersect.ProgressChanged += workerIntersect_ProgressChanged;
            workerIntersect.DoWork += workerIntersect_DoWork;
            workerIntersect.RunWorkerCompleted += workerIntersect_RunWorkerCompleted;
            // ---------------------------------------------------------------------------

            saveBtn.BackColor = Color.FromArgb(255, 255, 100, 100);
            saveBtn.Enabled = false;
        }

        void workerX_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker1Completed = true;
        }

        void workerY_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker2Completed = true;
        }

        void workerIntersect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            renderBtn.Enabled = true;
            renderBtn.ForeColor = Color.FromArgb(255, 10, 10, 10);
            renderBtn.BackColor = Color.FromArgb(255, 200, 200, 200);

            saveBtn.Enabled = true;
            saveBtn.ForeColor = Color.FromArgb(255, 10, 10, 10);
            saveBtn.BackColor = Color.FromArgb(255, 200, 200, 200);
        }

        void workerX_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int x = offsetXValue; x < resultImage.Size.Width; x += (int)squareSizeX.Value)
            {
                for (int y = 0; y < resultImage.Size.Height; y++)
                {
                    for (int stroke = 0; stroke < strokeWidth.Value; stroke++)
                    {
                        if (x + stroke + offsetXValue < resultImage.Size.Width)
                        {
                            resultImage.SetPixel(x + stroke, y, strokeColor);
                        }
                    }
                }
                calc = (int)Math.Round(Convert.ToDouble(partCalcX * x));
                workerX.ReportProgress(10);
                Thread.Sleep(10);
            }
            pictureBox1.Image = resultImage;
            workerX.ReportProgress(100);
        }

        void workerY_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int y = offsetYValue; y < resultImage.Size.Height; y += (int)squareSizeY.Value)
            {
                for (int x = 0; x < resultImage.Size.Width; x++)
                {
                    for (int stroke = 0; stroke < strokeWidth.Value; stroke++)
                    {
                        if (y + stroke + offsetYValue < resultImage.Size.Height)
                        {
                            resultImage.SetPixel(x, y + stroke, strokeColor);
                        }
                    }
                }
                calc = (int)Math.Round(Convert.ToDouble(partCalcY * y));
                workerY.ReportProgress(10);
                Thread.Sleep(10);
            }
            pictureBox1.Image = resultImage;
            workerY.ReportProgress(100);
        }

        void workerIntersect_DoWork(object sender, DoWorkEventArgs e)
        {
            int sizeX = (int)circleSizeX.Value;
            int sizeY = (int)circleSizeY.Value;
            for (int x = offsetXValue; x < resultImage.Size.Width; x += (int)squareSizeX.Value)
            {
                for (int y = offsetYValue; y < resultImage.Size.Height; y += (int)squareSizeY.Value)
                {
                    if (!(x == 0 || y == 0))
                    {
                        resultGraphics.FillEllipse(new SolidBrush(circleColor), x - (circleSizeXValue / 2), y - (circleSizeYValue / 2), circleSizeXValue, circleSizeYValue);
                        for (int i = 1; i < glowSizeXValue - circleSizeXValue; i++)
                        {
                            Color alphaglowcolor = Color.FromArgb(255 / (glowSizeXValue - circleSizeXValue) * (int)Math.Sqrt(i), glowColor);
                            resultGraphics.FillEllipse(new SolidBrush(alphaglowcolor), x - ((circleSizeXValue / 2) + i), y - ((circleSizeYValue / 2) + i), circleSizeXValue + (i * 2), circleSizeYValue + (i * 2));
                        }
                    }
                    calc = (int)Math.Round(Convert.ToDouble(partIntersectCalc * x * y));
                    workerIntersect.ReportProgress(10);
                }
            }
            pictureBox1.Image = resultImage;
            workerIntersect.ReportProgress(100);
        }

        void workerX_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                progressBar1.Value = 0;
                progressBar2.Value += finalCalc;
            }
            else
            {
                if (progressBar1.Value + calc < 100) progressBar1.Value += calc;
            }
        }

        void workerY_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                progressBar1.Value = 0;
                progressBar2.Value += finalCalc;
            }
            else
            {
                if (progressBar1.Value + calc < 100) progressBar1.Value += calc;
            }
        }

        void workerIntersect_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                progressBar1.Value = 0;
                progressBar2.Value += finalCalc;
            }
            else
            {
                if (progressBar1.Value + calc < 100) progressBar1.Value += calc;
            }
        }

        private void renderFunction()
        {
            // Reset --------------------------------------------
            resultImage = new Bitmap(1920, 1080);
            resultGraphics = Graphics.FromImage(resultImage);
            progressBar1.Value = 0;
            progressBar2.Value = 0;
            worker1Completed = false;
            worker2Completed = false;
            // --------------------------------------------------

            // Get values ---------------------------------------
            offsetXValue = (int)offsetX.Value;
            offsetYValue = (int)offsetY.Value;
            squareSizeXValue = (int)squareSizeX.Value;
            squareSizeYValue = (int)squareSizeY.Value;
            circleSizeXValue = (int)circleSizeX.Value;
            circleSizeYValue = (int)circleSizeY.Value;
            glowSizeXValue = (int)glowSizeX.Value;
            glowSizeYValue = (int)glowSizeY.Value;
            // --------------------------------------------------

            if (checkBoxBG.Checked == true) resultGraphics.FillRectangle(new SolidBrush(bgColor), 0, 0, 1920, 1080);
            else resultImage.MakeTransparent(Color.White);

            // Reset the graphics - you must do it - it's a bug
            resultGraphics = Graphics.FromImage(resultImage);
            // ------------------------------------------------

            // Disable the button and start the work
            renderBtn.Enabled = false;
            renderBtn.BackColor = Color.FromArgb(255, 255, 100, 100);
            toolStripStatusLabel1.Text = "Status: Working on the X Lines";
            workerX.RunWorkerAsync(); // Work for x

            waitTimer1.Start();
        }

        private void renderBtn_Click(object sender, EventArgs e)
        {
            renderFunction();
        }

        private void pictureBox2_Click(object sender, EventArgs e) // Stroke Color Picker
        {
            ColorPick(ref gColorPickerStroke, ref pictureBox2, ref bmpColorPickerStroke, ref strokeColor);
        }

        private void pictureBox3_Click(object sender, EventArgs e) // Glow Color Picker
        {
            ColorPick(ref gColorPickerGlow, ref pictureBox3, ref bmpColorPickerGlow, ref glowColor);
        }

        private void pictureBox4_Click(object sender, EventArgs e) // Circle Color Picker
        {
            ColorPick(ref gColorPickerCircle, ref pictureBox4, ref bmpColorPickerCircle, ref circleColor);
        }

        private void pictureBox5_Click(object sender, EventArgs e) // Background Guide Layer Color Picker
        {
            ColorPick(ref gColorPickerBG, ref pictureBox5, ref bmpColorPickerBG, ref bgColor);
        }

        private void ColorPick(ref Graphics g, ref PictureBox pb, ref Bitmap bmp, ref Color color)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                g.FillRectangle(new SolidBrush(cd.Color), 0, 0, 20, 20);
                color = cd.Color;
            }
            else
            {
                gColorPickerStroke.FillRectangle(new SolidBrush(Color.White), 0, 0, 20, 20);
                color = Color.White;
            }
            pb.Image = bmp;
        }

        private void waitTimer1_Tick(object sender, EventArgs e)
        {
            if (worker1Completed == true)
            {
                toolStripStatusLabel1.Text = "Status: Working on the Y Lines";
                workerY.RunWorkerAsync(); // Work for y
                waitTimer2.Start();
                waitTimer1.Stop();
            }
        }

        private void waitTimer2_Tick(object sender, EventArgs e)
        {
            if (worker2Completed == true)
            {
                toolStripStatusLabel1.Text = "Status: Working on the Circles";
                workerIntersect.RunWorkerAsync(); // Work for the circles
                waitTimer2.Stop();
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = "png";
                sfd.Filter = "PNG (*.png)|*.png";
                sfd.InitialDirectory = "C:/";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Image img = (Image)resultImage;
                    img.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}
