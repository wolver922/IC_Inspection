using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace _2015_1_7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Image myImage;
        Image<Gray, byte> ooldImg;
        private const int Height = 480, Width = 640;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                myImage = Image.FromFile(file.FileName);
                pictureBox1.Image = myImage;
                ooldImg = new Image<Gray, byte>((Bitmap)myImage);
            }
        }
        class LineData
        {
            public double lineColor;
            public int Y;
            public LineData(int linecolor,int y)
            {
                lineColor = linecolor;
                Y = y;
            }
            
        };
        int flag = 5;
        private void button2_Click(object sender, EventArgs e)
        {
            Image<Bgr, Byte> oldImg = new Image<Bgr, Byte>(ooldImg.Bitmap);
            Image<Gray, Byte> detImg = new Image<Gray, Byte>(Width, Height);
            //Image<Gray, double> cvImageIntegral = new Image<Gray,double>(16,12);
            //Image<Gray, Byte> arrMap = null;
            Bitmap arrMap = new Bitmap(Width, Height);
            Bitmap arrMap2 = new Bitmap(Width, Height);
            detImg.Bitmap = oldImg.Bitmap;
            List<LineData> myLine = new List<LineData>();
            List<LineData> myLine2 = new List<LineData>();
            //LineData lineData = new LineData();
            for (int i = 0; i < 20;i++ )
                detImg = detImg.SmoothGaussian(3);
            double addlinecolor;
            double addlinecolor2;
           double[] lineColor = new double[480];
           double[] lineColor2 = new double[480];
           
            //橫向加總
            for (int y = 0; y < Height ; y++)
            {
                addlinecolor = 0;
                addlinecolor2 = 0;
                for (int x = 0; x < Width ; x++)
                {
                    if (x > Width / 2)
                    {
                        addlinecolor2 += Emgu.CV.CvInvoke.cvGetReal2D(detImg, y, x);
                        if (x == Width - 1)
                        {
                            lineColor2[y] = (addlinecolor2 / 320);
                            LineData thisline2 = new LineData((int)(addlinecolor / 320), y);
                            myLine2.Add(thisline2);
                            int gray = (int)(addlinecolor / 320);
                            arrMap2.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                        }
                    }
                    else
                    {
                        addlinecolor += Emgu.CV.CvInvoke.cvGetReal2D(detImg, y, x);
                        if (x == Width / 2)
                        {
                            lineColor[y] = (addlinecolor / 320);
                            LineData thisline = new LineData((int)(addlinecolor / 320), y);
                            myLine.Add(thisline);
                            int gray = (int)(addlinecolor / 320);
                            arrMap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                        }
                    }
                }
                /*lineColor[y] = (addlinecolor / 320);
                LineData thisline = new LineData((int)(addlinecolor / 320),y);
                myLine.Add(thisline);
                int gray = (int)(addlinecolor / 320);*/
                /*for (int i = 0; i < Width; i++)
                    arrMap.SetPixel(i, y, Color.FromArgb(gray, gray, gray));*/
            }
            myLine.Sort((x, y) => { return -x.lineColor.CompareTo(y.lineColor); });
            myLine2.Sort((x, y) => { return -x.lineColor.CompareTo(y.lineColor); });
            double add = 0;
            double topall = 0;
            double alldown = 0;
            int count = 0;
            foreach (LineData aline in myLine)
            {
                count++;
                if (count == myLine.Count / 2)
                {
                    add /= 240;
                    topall = add;
                    add = 0;
                }
                if (count == myLine.Count)
                {
                    add /= 240;
                    alldown = add;
                    add = 0;
                }
                add += aline.lineColor;
                
            }
            double add2 = 0;
            double topall2 = 0;
            double alldown2 = 0;
            foreach (LineData aline in myLine2)
            {
                count++;
                if (count == myLine2.Count / 2)
                {
                    add2 /= 240;
                    topall2 = add2;
                    add2 = 0;
                }
                if (count == myLine2.Count)
                {
                    add2 /= 240;
                    alldown2 = add2;
                    add2 = 0;
                }
                add2 += aline.lineColor;

            }
            int rand1 = 5;
            for (int y = 0 + rand1; y < Height - rand1; y++)
            {
                if(Math.Abs(lineColor[y] -alldown) > 10)
                    for (int x = 0; x < Width; x++)
                    {
                        PointF point = new PointF(x, y);
                        PointF point2 = new PointF(x, y);
                        LineSegment2D line = new LineSegment2D(Point.Round(point), Point.Round(point2));
                        oldImg.Draw(line, new Bgr(0, 0, 255), 2);
                    }
               /* if (Math.Abs(lineColor2[y] - alldown2) > flag)
                    for (int x = (Width /2)+1; x < Width; x++)
                    {
                        PointF point = new PointF(x, y);
                        PointF point2 = new PointF(x, y);
                        LineSegment2D line = new LineSegment2D(Point.Round(point), Point.Round(point2));
                        oldImg.Draw(line, new Bgr(0, 0, 255), 2);
                    }*/
            }
            for (int y = 0 + rand1; y < Height - rand1; y++)
            {
                for (int x = 0 + rand1; x < Width - rand1; x++)
                {
                    if (x > Width / 2)
                    {
                        if (Math.Abs(lineColor2[y] - Emgu.CV.CvInvoke.cvGetReal2D(detImg, y, x)) > flag)
                        {

                            PointF point = new PointF(x, y);
                            PointF point2 = new PointF(x, y);
                            LineSegment2D line = new LineSegment2D(Point.Round(point), Point.Round(point2));
                            oldImg.Draw(line, new Bgr(0, 0, 255), 2);

                        }
                    }
                    else
                    {
                        if (Math.Abs(lineColor[y] - Emgu.CV.CvInvoke.cvGetReal2D(detImg, y, x)) > flag)
                        {

                            PointF point = new PointF(x, y);
                            PointF point2 = new PointF(x, y);
                            LineSegment2D line = new LineSegment2D(Point.Round(point), Point.Round(point2));
                            oldImg.Draw(line, new Bgr(0, 0, 255), 2);

                        }
                    }
                }
            }
            
            
            pictureBox2.Image = oldImg.Bitmap;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            flag = trackBar1.Value;
            label1.Text = ""+trackBar1.Value;
            //pictureBox2.Image = oldImg.Bitmap;
        }
    }
}
