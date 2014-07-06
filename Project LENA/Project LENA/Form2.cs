﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using BitMiracle.LibTiff.Classic;
using System.IO;

namespace Project_LENA
{
    public partial class Form2 : Form
    {
        string cleanimage;
        string noisyimage;

        public Form2(string clean, string noisy)
        {
            InitializeComponent();           

            cleanimage = clean;
            noisyimage = noisy;

            // read bytes of an image
            byte[] buffer = File.ReadAllBytes(clean);

            // create a memory streams out of the bytes read
            MemoryStream ms = new MemoryStream(buffer);

            //open a tiff stored in the memory stream!
            pictureBox1.Image = Image.FromStream(ms);

            label1.Text = Path.GetFileName(clean);

            // read bytes of an image
            byte[] buffer2 = File.ReadAllBytes(noisy);

            // create a memory streams out of the bytes read
            MemoryStream ms2 = new MemoryStream(buffer2);

            //open a tiff stored in the memory stream!
            pictureBox2.Image = Image.FromStream(ms2);

            label2.Text = Path.GetFileName(noisy);

            int maxwidth = pictureBox1.Width + pictureBox2.Width + 40;
            int maxheight = pictureBox1.Height + 138;

            this.MaximumSize = new Size(maxwidth, maxheight);

            panel1.Width = (this.Width) / 2 - 20;
            panel2.Width = (this.Width) / 2 - 20;
            panel2.Left = (panel1.Width + 12);
            label2.Left = panel2.Left;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            panel1.Width = (this.Width) / 2 - 20;
            panel2.Width = (this.Width) / 2 - 20;
            panel2.Left = (panel1.Width + 12);
            label2.Left = panel2.Left;
        }

        private Point PanStartPoint;

        private Point RectStartPoint;
        private Rectangle Rect = new Rectangle();
        private Rectangle notRect = new Rectangle();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 72, 145, 220));
        private bool CreatedRect;

        #region pictureBox1

        // Start Rectangle
        //
        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PanStartPoint = e.Location;
            }
            if (e.Button == MouseButtons.Left)
            {
                // Determine the initial rectangle coordinates...
                RectStartPoint = e.Location;
                CreatedRect = false;
                //Invalidate();
            }

        }

        // Draw Rectangle
        //
        private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = String.Format(label1.Text + "  X: {0}; Y: {1}", e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                Cursor = Cursors.SizeAll;
                Control c = (Control)sender;

                if (c.Left + (e.X - PanStartPoint.X) < 0 && c.Left + (e.X - PanStartPoint.X) > panel1.Width - pictureBox1.Width)
                {
                    c.Left = c.Left + (e.X - PanStartPoint.X);
                }
                else if (c.Left + (e.X - PanStartPoint.X) > 0)
                {
                    c.Left = 0;
                }
                else if (c.Left + (e.X - PanStartPoint.X) < panel1.Width - pictureBox1.Width)
                {
                    c.Left = panel1.Width - pictureBox1.Width;
                }

                if (c.Top + (e.Y - PanStartPoint.Y) <= 0 && c.Top + (e.Y - PanStartPoint.Y) > panel1.Height - pictureBox1.Height)
                {
                    c.Top = (c.Top + e.Y) - PanStartPoint.Y;
                }
                else if (c.Top + (e.Y - PanStartPoint.Y) > 0)
                {
                    c.Top = 0;
                }
                else if (c.Top + (e.Y - PanStartPoint.Y) < panel1.Height - pictureBox1.Height)
                {
                    c.Top = panel1.Height - pictureBox1.Height;
                }
                c.BringToFront();

            }
            if (e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                Point tempEndPoint = e.Location;

                Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, Math.Max(tempEndPoint.X, 0)),
                    Math.Min(RectStartPoint.Y, Math.Max(tempEndPoint.Y, 0)));


                if (tempEndPoint.X <= pictureBox1.Width && tempEndPoint.X >= 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - tempEndPoint.X);
                }
                else if (tempEndPoint.X > pictureBox1.Width)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - pictureBox1.Width);
                }
                else if (tempEndPoint.X < 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - 0);
                }

                if (tempEndPoint.Y <= pictureBox1.Height && tempEndPoint.Y >= 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - tempEndPoint.Y);
                }
                else if (tempEndPoint.Y > pictureBox1.Height)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - pictureBox1.Height);
                }
                else if (tempEndPoint.Y < 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - 0);
                }

                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", Rect.Width, Rect.Height);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                
                //if (Rect.Contains(e.Location))
                //{
                Cursor = Cursors.Default;
                //}
            }
            if (e.Button == MouseButtons.Left)
            {
                Point tempEndPoint = e.Location;
                if (RectStartPoint.X == tempEndPoint.X)
                {
                    Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, tempEndPoint.X),
                    Math.Min(RectStartPoint.Y, tempEndPoint.Y));
                    Rect.Size = new Size(
                    Math.Abs(RectStartPoint.X - tempEndPoint.X),
                    Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
                    //Rect.Size = new Size(
                    //    Math.Min(Math.Abs(RectStartPoint.X - tempEndPoint.X), Math.Abs(RectStartPoint.X - pictureBox1.ClientRectangle.Width)),
                    //    Math.Min(Math.Abs(RectStartPoint.Y - tempEndPoint.Y), Math.Abs(RectStartPoint.Y - pictureBox1.ClientRectangle.Height)));

                    toolStripStatusLabel2.Text = String.Format("  ");
                    Invalidate();
                }

                CreatedRect = true;
                notRect.Location = new Point(0, 0);
                notRect.Size = pictureBox1.Size;
                Invalidate();
                //if (!Rect.Contains(e.Location))
                //{
                //    CreatedRect = true;
                //    notRect.Location = new Point(0, 0);
                //    notRect.Size = pictureBox1.Size;
                //    Invalidate();
                //    // I can add custom select code here!

                //}
                Cursor = Cursors.Default;
            }
        }

        // Draw Area
        //
        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw the rectangle...
            if (pictureBox1.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    if (CreatedRect == false)
                    {
                        using (Pen p = new Pen(Color.FromArgb(255, 51, 153, 255), 1.0F))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - 1, Rect.Height - 1);
                        }

                        e.Graphics.FillRectangle(selectionBrush, Rect);
                    }

                    if (CreatedRect == true)
                    {
                        using (Brush notselectedBrush = new SolidBrush(Color.FromArgb(128, 40, 40, 40)))
                        {
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            Rectangle InverseRect = new Rectangle(0, 0, Rect.Location.X, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X, 0, pictureBox1.Width, Rect.Location.Y);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X + Rect.Width, Rect.Location.Y, pictureBox1.Width, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X, Rect.Location.Y + Rect.Height, Rect.Width, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(255, 255, 255, 255), 1.0F))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - 1, Rect.Height - 1);
                        }
                        //using (Brush selectedBrush = new SolidBrush(Color.FromArgb(0, 72, 145, 220)))
                        //{
                        //    e.Graphics.FillRectangle(selectedBrush, Rect);
                        //}                      
                    }
                }
            }
        }       
        #endregion

        #region pictureBox2

        // Start Rectangle
        //
        private void pictureBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PanStartPoint = e.Location;
            }
            if (e.Button == MouseButtons.Left)
            {
                // Determine the initial rectangle coordinates...
                RectStartPoint = e.Location;
                CreatedRect = false;
                //Invalidate();
            }

        }

        // Draw Rectangle
        //
        private void pictureBox2_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = String.Format(label2.Text + "  X: {0}; Y: {1}", e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                Cursor = Cursors.SizeAll;
                Control c = (Control)sender;

                if (c.Left + (e.X - PanStartPoint.X) <= 0 && c.Left + (e.X - PanStartPoint.X) >= panel2.Width - pictureBox2.Width)
                {
                    c.Left = c.Left + (e.X - PanStartPoint.X);
                }
                else if (c.Left + (e.X - PanStartPoint.X) > 0)
                {
                    c.Left = 0;
                }
                else if (c.Left + (e.X - PanStartPoint.X) < panel2.Width - pictureBox2.Width)
                {
                    c.Left = panel1.Width - pictureBox1.Width;
                }

                if (c.Top + (e.Y - PanStartPoint.Y) <= 0 && c.Top + (e.Y - PanStartPoint.Y) >= panel2.Height - pictureBox2.Height)
                {
                    c.Top = (c.Top + e.Y) - PanStartPoint.Y;
                }
                else if (c.Top + (e.Y - PanStartPoint.Y) > 0)
                {
                    c.Top = 0;
                }
                else if (c.Top + (e.Y - PanStartPoint.Y) < panel2.Height - pictureBox2.Height)
                {
                    c.Top = panel2.Height - pictureBox2.Height;
                }
                c.BringToFront();
            }
            if (e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                Point tempEndPoint = e.Location;
                Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, Math.Max(tempEndPoint.X, 0)),
                    Math.Min(RectStartPoint.Y, Math.Max(tempEndPoint.Y, 0)));


                if (tempEndPoint.X <= pictureBox2.Width && tempEndPoint.X >= 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - tempEndPoint.X);
                }
                else if (tempEndPoint.X > pictureBox2.Width)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - pictureBox2.Width);
                }
                else if (tempEndPoint.X < 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - 0);
                }

                if (tempEndPoint.Y <= pictureBox2.Height && tempEndPoint.Y >= 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - tempEndPoint.Y);
                }
                else if (tempEndPoint.Y > pictureBox2.Height)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - pictureBox2.Height);
                }
                else if (tempEndPoint.Y < 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - 0);
                }
                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", Rect.Width, Rect.Height);
            }
        }

        // Draw Area
        //
        private void pictureBox2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw the rectangle...
            if (pictureBox2.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    if (CreatedRect == false)
                    {
                        using (Pen p = new Pen(Color.FromArgb(255, 51, 153, 255), 1.0F))
                        {
                            /***************** TODO: create if statement to correct left and top boundary issue *********************************************/
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - 1, Rect.Height - 1);
                        }

                        e.Graphics.FillRectangle(selectionBrush, Rect);
                    }

                    if (CreatedRect == true)
                    {
                        using (Brush notselectedBrush = new SolidBrush(Color.FromArgb(128, 40, 40, 40)))
                        {
                            Rectangle InverseRect = new Rectangle(0, 0, Rect.Location.X, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X, 0, pictureBox2.Width, Rect.Location.Y);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X + Rect.Width, Rect.Location.Y, pictureBox2.Width, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.Location.X, Rect.Location.Y + Rect.Height, Rect.Width, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(255, 255, 255, 255), 1.0F))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - 1, Rect.Height - 1);
                        }                     
                    }
                }
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //if (Rect.Contains(e.Location))
                //{
                Cursor = Cursors.Default;
                //}
            }
            if (e.Button == MouseButtons.Left)
            {
                Point tempEndPoint = e.Location;
                if (RectStartPoint.X == tempEndPoint.X)
                {
                    Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, tempEndPoint.X),
                    Math.Min(RectStartPoint.Y, tempEndPoint.Y));
                    Rect.Size = new Size(
                        Math.Min(Math.Abs(RectStartPoint.X - tempEndPoint.X), Math.Abs(RectStartPoint.X - pictureBox2.ClientRectangle.Width)),
                        Math.Min(Math.Abs(RectStartPoint.Y - tempEndPoint.Y), Math.Abs(RectStartPoint.Y - pictureBox2.ClientRectangle.Height)));

                    toolStripStatusLabel2.Text = String.Format("  ");
                    Invalidate();
                }

                CreatedRect = true;
                notRect.Location = new Point(0, 0);
                notRect.Size = pictureBox2.Size;
                Invalidate();
                //if (!Rect.Contains(e.Location))
                //{
                //    CreatedRect = true;
                //    notRect.Location = new Point(0, 0);
                //    notRect.Size = pictureBox1.Size;
                //    Invalidate();
                //    // I can add custom select code here!

                //}
                Cursor = Cursors.Default;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                #region Clean image
                Tiff Cleanimage = Tiff.Open(cleanimage, "r");

                // Obtain basic tag information of the image
                #region GetTagInfo
                int cleanwidth = Cleanimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int cleanheight = Cleanimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte cleanbits = Cleanimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte cleanpixel = Cleanimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                #endregion

                if (cleanpixel == 1)
                {
                    byte[,] clean = new byte[cleanheight, cleanwidth];

                    // store the image information in 2d byte array
                    // reserve memory for storing the size of 1 line
                    byte[] scanline = new byte[Cleanimage.ScanlineSize()];
                    // reserve memory for the size of image
                    //byte[,] im = new byte[heightc, widthc];
                    for (int i = 0; i < cleanheight; i++)
                    {
                        Cleanimage.ReadScanline(scanline, i);
                        {
                            for (int j = 0; j < cleanwidth; j++)
                            {
                                clean[i, j] = scanline[j];
                            }
                        }
                    } // end grabbing intensity values 

                    saveFileDialog1.Title = "Save Clean Fragment As";
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(cleanimage) + "_Fragment" + ".tif";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Tiff output = Tiff.Open(saveFileDialog1.FileName, "w"))
                        {
                            output.SetField(TiffTag.IMAGEWIDTH, Rect.Size.Width);
                            output.SetField(TiffTag.IMAGELENGTH, Rect.Size.Height);
                            output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                            output.SetField(TiffTag.BITSPERSAMPLE, 8);
                            output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                            output.SetField(TiffTag.ROWSPERSTRIP, Rect.Size.Height);
                            output.SetField(TiffTag.XRESOLUTION, 88.0);
                            output.SetField(TiffTag.YRESOLUTION, 88.0);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                            output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                            output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                            output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                            byte[] im = new byte[Rect.Size.Width * sizeof(byte /*can be changed depending on the format of the image*/)];

                            for (int i = 0; i < Rect.Size.Height; ++i) // change i to initial, change heightc to final
                            {
                                for (int j = 0; j < Rect.Size.Width; ++j)
                                {
                                    im[j] = clean[Rect.Location.Y + i, Rect.Location.X + j];
                                }
                                output.WriteScanline(im, i);
                            }
                            output.WriteDirectory();
                            output.Dispose();
                        }
                    }
                }
                if (cleanpixel == 3)
                {
                    byte[] scanline = new byte[Cleanimage.ScanlineSize()];

                    // Read the image into the memory buffer
                    byte[,] redc = new byte[cleanheight, cleanwidth];
                    byte[,] greenc = new byte[cleanheight, cleanwidth];
                    byte[,] bluec = new byte[cleanheight, cleanwidth];

                    //for (int i = height - 1; i != -1; i--)
                    for (int i = 0; cleanheight > i; i++)
                    {
                        Cleanimage.ReadScanline(scanline, i); // EVIL BUG HERE
                        for (int j = 0; j < cleanwidth; j++)
                        {
                            redc[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                            greenc[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                            bluec[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                        }
                    }
                }
                #endregion

                #region Noisy image
                Tiff Noisyimage = Tiff.Open(noisyimage, "r");

                // Obtain basic tag information of the image
                #region GetTagInfo
                int noisywidth = Noisyimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int noisyheight = Noisyimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte noisybits = Noisyimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte noisypixel = Noisyimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                #endregion

                if (noisypixel == 1)
                {
                    byte[,] noisy = new byte[noisyheight, noisywidth];

                    // store the image information in 2d byte array
                    // reserve memory for storing the size of 1 line
                    byte[] scanline = new byte[Noisyimage.ScanlineSize()];
                    // reserve memory for the size of image
                    //byte[,] im = new byte[heightc, widthc];
                    for (int i = 0; i < noisyheight; i++)
                    {
                        Noisyimage.ReadScanline(scanline, i);
                        {
                            for (int j = 0; j < noisywidth; j++)
                            {
                                noisy[i, j] = scanline[j];
                            }
                        }
                    } // end grabbing intensity values 

                    saveFileDialog1.Title = "Save Noisy Fragment As";
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(noisyimage) + "_Fragment" + ".tif";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Tiff output = Tiff.Open(saveFileDialog1.FileName, "w"))
                        {
                            output.SetField(TiffTag.IMAGEWIDTH, Rect.Size.Width);
                            output.SetField(TiffTag.IMAGELENGTH, Rect.Size.Height);
                            output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                            output.SetField(TiffTag.BITSPERSAMPLE, 8);
                            output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                            output.SetField(TiffTag.ROWSPERSTRIP, Rect.Size.Height);
                            output.SetField(TiffTag.XRESOLUTION, 88.0);
                            output.SetField(TiffTag.YRESOLUTION, 88.0);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                            output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                            output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                            output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                            byte[] im = new byte[Rect.Size.Width * sizeof(byte /*can be changed depending on the format of the image*/)];

                            for (int i = 0; i < Rect.Size.Height; ++i) // change i to initial, change heightc to final
                            {
                                for (int j = 0; j < Rect.Size.Width; ++j)
                                {
                                    im[j] = noisy[Rect.Location.Y + i, Rect.Location.X + j];
                                }
                                output.WriteScanline(im, i);
                            }
                            output.WriteDirectory();
                            output.Dispose();
                        }
                    }
                }
                #endregion

            }
            catch (InvalidOperationException)
            {
                Console.Write("Images must be the same resolution and color depth in order to crop images.");
            }
        }
    }
}