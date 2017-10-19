﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_ind
{

    public partial class Form1 : Form
    {

        public List<Point> points;

        public List<Point> above;

        public List<Point> below;

        Graphics g;

        Pen pen = new Pen(Color.Black);

        public bool isUp(Point a, Point b, Point c)
        {
            return ((c.Y - a.Y) / (double)(b.Y-a.Y) - (c.X-a.X)/(double)(b.X-a.X)) > 0;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(bmp);
            points = new List<Point>();
            above = new List<Point>();
            below = new List<Point>();
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            g.Clear(System.Drawing.Color.White);
            points.Clear();
            above.Clear();
            below.Clear();
            pen.Color = Color.Black;
            pictureBox1.Refresh();
        }

        //right turn is > 0, left turn if < 0, collin = 0 
        double cww(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }


        //Construct Convex Hull
        private void button2_Click(object sender, EventArgs e)
        {
            if (points.Count < 3)
                return;
            points.Sort(new Sort()); //first is left border, last is right border
            above.Add(points[0]);
            above.Add(points[1]);
            for (int i = 2; i<points.Count; i++)
            {
                above.Add(points[i]);
                while (above.Count > 2 && cww(above[above.Count - 3], above[above.Count - 2], above[above.Count - 1]) <= 0)
                {
                    above.Remove(above[above.Count - 2]); // if not left => delete middle
                }
            }

            connect_all_the_points(above);

            below.Add(points[points.Count - 1]);
            below.Add(points[points.Count - 2]);

            for (int i = points.Count - 3; i >= 0; i--)
            {
                below.Add(points[i]);
                while (below.Count > 2 && cww(below[below.Count - 3], below[below.Count - 2], below[below.Count - 1]) <= 0)
                {
                    below.Remove(below[below.Count - 2]);
                }
            }

            pen.Color = Color.Red;

            connect_all_the_points(below);
            //above.AddRange(below); //Connects two
           // pen.Color = Color.Purple;
            //connect_all_the_points(above);
        }



        void connect_all_the_points(List<Point> lp)
        {

            for (int i = 0; i < lp.Count - 1; i++)
            {
                g.DrawLine(pen, lp[i].X, lp[i].Y, lp[i + 1].X, lp[i + 1].Y);
            }
            pictureBox1.Refresh();
        }

        //draw points
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            g.DrawEllipse(pen, new Rectangle(e.X,e.Y, 1, 1));
            points.Add(new Point(e.X, e.Y));
            listBox1.Items.Add(new Point(e.X, e.Y));
            pictureBox1.Refresh();
        }
        
        //put random
        private void button1_Click(object sender, EventArgs e)
        { 
            Random random = new Random();
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                while (true)
                {
                    int rx = random.Next((int)numericUpDown2.Value, (int)(pictureBox1.Width - numericUpDown2.Value) );
                    int ry = random.Next((int)numericUpDown2.Value, (int)(pictureBox1.Height - numericUpDown2.Value) );
                    if (!points.Contains(new Point(rx, ry)))
                    {
                        points.Add(new Point(rx, ry));
                        g.DrawEllipse(pen, new Rectangle(rx, ry, 1, 1));
                        pictureBox1.Refresh();
                        break;
                    }
                }
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            g = Graphics.FromImage(bmp);
            if (!(points == null))
            foreach (Point p in points)
            {
                g.DrawEllipse(pen, new Rectangle(p.X, p.Y, 1, 1));
            }
            pictureBox1.Refresh();
        }
    }


    class FindLeft : IComparer<Point>
    {
        public int Compare(Point first, Point second)
        {
            if (first.X == second.X)
            {
                return second.Y - first.Y;
            }
            else
            {
                return first.X - second.X;
            }
        }
    }



    class FindDown : IComparer<Point>
    {
        public int Compare(Point first, Point second)
        {
            if (first.Y == second.Y)
            {
                return second.X - first.Y;
            }
            else
            {
                return second.Y - first.Y;
            }
        }
    }

    class Sort : IComparer<Point>
    {
        public int Compare(Point first, Point second)
        {
            if (first.X == second.X)
            {
                return first.Y - second.Y;
            }
            else
            {
                return first.X - second.X;
            }
        }
    }

}
