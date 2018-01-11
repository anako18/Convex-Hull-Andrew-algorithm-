using System;
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

        public LinkedList<Point> points;

        public LinkedList<Point> above;

        public LinkedList<Point> below;

        public Graphics g;

        public bool done = false; //If form is resized - draw the hull

        public Pen pen = new Pen(Color.Black);

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
            points = new LinkedList<Point>();
            above = new LinkedList<Point>();
            below = new LinkedList<Point>();
        }

        //if c above the line a_b
        private bool above_the_line(Point a, Point b, Point c)
        {
            double y = ((c.X - a.X) / (double) (b.X - a.X) + a.Y / (double) (b.Y - a.Y)) * (b.Y - a.Y);
            return (y>=c.Y);
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            g.Clear(System.Drawing.Color.White);
            points.Clear();
            above.Clear();
            below.Clear();
            pen.Color = Color.Black;
            listBox1.Items.Clear();
            pictureBox1.Refresh();
        }

        //right turn is > 0, left turn if < 0, collin = 0 
        private double cww(Point p1, Point p2, Point p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
        }


        //Construct Convex Hull
        private void button2_Click(object sender, EventArgs e)
        {
            done = false;
            above.Clear();
            below.Clear();
            Redraw_points();

            if (points.Count < 3)
                return;
          
            //points.Sort(new Sort()); //first is left border, last is right border
            var sorted_points = points.OrderBy(p => p.X).ThenBy(p => p.Y);

            above.AddLast(sorted_points.ElementAt(0));
            above.AddLast(sorted_points.ElementAt(1));

            for (int i = 2; i < sorted_points.Count(); i++)
            {
                if (!above_the_line(sorted_points.ElementAt(0), sorted_points.ElementAt(sorted_points.Count() - 1), sorted_points.ElementAt(i)) && i!=sorted_points.Count()-1)
                    continue;

                    above.AddLast(sorted_points.ElementAt(i));
                    while (above.Count > 2 && cww(above.ElementAt(above.Count - 3), above.ElementAt(above.Count - 2), above.ElementAt(above.Count - 1)) <= 0)
                    {
                        above.Remove(above.ElementAt(above.Count - 2)); // if not left => delete middle
                    }
            }
            connect_all_the_points(above);


            below.AddLast(sorted_points.ElementAt(sorted_points.Count() - 1));
            below.AddLast(sorted_points.ElementAt(sorted_points.Count() - 2));

            for (int i = sorted_points.Count() - 3; i >= 0; i--)
            {
                if (above_the_line(sorted_points.ElementAt(0), sorted_points.ElementAt(sorted_points.Count() - 1), sorted_points.ElementAt(i)) && i!=0)
                    continue;

                below.AddLast(sorted_points.ElementAt(i));
                while (below.Count > 2 && cww(below.ElementAt(below.Count - 3), below.ElementAt(below.Count - 2), below.ElementAt(below.Count - 1)) <= 0)
                {
                    below.Remove(below.ElementAt(below.Count - 2));
                }
            }
            
            pen.Color = Color.Red;
            connect_all_the_points(below);
            pen.Color = Color.Black;
            done = true;
        }



        private void connect_all_the_points(LinkedList<Point> lp)
        {

            for (int i = 0; i < lp.Count - 1; i++)
            {
                g.DrawLine(pen, lp.ElementAt(i).X, lp.ElementAt(i).Y, lp.ElementAt(i + 1).X, lp.ElementAt(i + 1).Y);
            }
            pictureBox1.Refresh();
        }

        //draw points
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (listBox1.Items.Contains(new Point(e.X, e.Y)))
                return;
            g.DrawEllipse(pen, new Rectangle(e.X,e.Y, 1, 1));
            points.AddLast(new Point(e.X, e.Y));
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
                        points.AddLast(new Point(rx, ry));
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

            if (done)
            {
                pen.Color = Color.Black;
                connect_all_the_points(above);
                pen.Color = Color.Red;
                connect_all_the_points(below);
            }
        }

        //removing point from the list
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int ind = this.listBox1.IndexFromPoint(e.Location);

            if (ind != ListBox.NoMatches)
            {
                points.Remove((Point)listBox1.Items[ind]);
                done = false;
                Redraw_points();
                listBox1.Items.RemoveAt(ind);
            }
        }


        void Redraw_points()
        {
            g.Clear(System.Drawing.Color.White);
            for (int i = 0; i<points.Count(); i++)
            {
                g.DrawEllipse(pen, new Rectangle(points.ElementAt(i).X, points.ElementAt(i).Y, 1, 1));
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
