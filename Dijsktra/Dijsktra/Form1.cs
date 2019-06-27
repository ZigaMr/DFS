using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Dijsktra
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);


        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);
        Dictionary<int, List<int>> AdjacencyList = new Dictionary<int, List<int>>();
        public static ManualResetEvent mre = new ManualResetEvent(true);

        Point zadnjaLokacija;   //hrani zadnjo lokacijo miške
        bool risem;             //stikalo za risanje
        List<Point> Coords = new List<Point>();

        public static int steviloVozlisc;

        public Form1()
        {
            InitializeComponent();
            risem = false;

            // Povežemo dogodke z metodami

            Load += novoOzadje;
            platno.MouseClick += klikMiske;
            platno.MouseDoubleClick += novoOzadje;
        }

        private void novoOzadje(object sender, EventArgs e)
        {
            platno.Image = new Bitmap(platno.Size.Width, platno.Size.Height);
        }

        private void klikMiske(object sender, MouseEventArgs e)
        {
            foreach(var point in Coords)
            {
                if (((point.X - e.X) * (point.X - e.X) + (e.Y - point.Y) * (e.Y - point.Y)) < 500*500)
                {
                    return;
                }
            }
            Point clickPoint = e.Location;
        

            //Counter++;
            int x = e.X;
            int y = e.Y;
            Coords.Add(new Point(e.X, e.Y));

            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);

            DrawCircle(arg, x, y, 10, 10, Color.Red);
            platno.Invalidate();
        }

        private void DrawCircle(PaintEventArgs e, int x, int y, int width, int height, Color clr)
        {
            Pen pen = new Pen(Color.Red, 3);
            e.Graphics.DrawEllipse(pen, x - width / 2, y - height / 2, width, height);
            SolidBrush solidBrush = new SolidBrush(clr);
            e.Graphics.FillEllipse(solidBrush, x - width / 2, y - height / 2, width, height);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string st_vozlisc;
            var formPopup = new PopupForm();
            formPopup.Show(this); // if you need non-modal window
            
        }

        private bool check_coords(int X, int Y, decimal n)
        {
            foreach (var point in Coords)
            {
                if (((point.X - X) * (point.X - X) + (Y - point.Y) * (Y - point.Y)) < n * n)
                {
                    return false;
                }
            }
            return true;
        }

        public void generate_graph(int stVozlisc)//, int stPovezav)
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            Random rnd = new Random();

            int X;
            int Y;
            int counter = 0;//counter-uporabimo, ker vcasih zmanjka prostora in se ustvari neskoncna zanka, 
                            //ki nikoli ne najde ustreznih vozlišč(ni prostora na canvas)
            for (int i=0; i < steviloVozlisc; i++)
            {
                X = rnd.Next(100, 500);
                Y = rnd.Next(100, 400);
                counter = 1;
                decimal n = 200;
                while (!check_coords(X, Y, n/counter) & counter < 10)
                {
                    X = rnd.Next(100, 500);
                    Y = rnd.Next(100, 400);
                    counter++;
                }
                
                DrawCircle(arg, X, Y, 10, 10, Color.Red);
                Coords.Add(new Point(X, Y));

                g.DrawString(i.ToString(), new Font("Papyrus", 12), Brushes.Black, new Point(X, Y));
                platno.Invalidate();
            }

        }
        private void draw_edges()
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            for (int i=0; i < Coords.Count(); i++)
            {
                AdjacencyList[i] = new List<int>();
            }
            for (int i = 0; i < Coords.Count(); i++)
            {
                List<int> possible = Enumerable.Range(0,Coords.Count()).ToList();
                possible.RemoveAt(i);

                Random rnd = new Random();
                int r1 = rnd.Next(0,possible.Count);
                int r2 = 0;

                AdjacencyList[i].Add(possible[r1]);

                int nxt = rnd.Next(0, 2);
                if (nxt == 1)
                {
                    r2 = rnd.Next(0, possible.Count);
                }
                AdjacencyList[i].Add(possible[r2]);




                //System.Drawing.Drawing2D.AdjustableArrowCap bigArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5);

                Pen pen = new Pen(Color.Green, 2);
                //pen.CustomEndCap = bigArrow;



                foreach (var x in AdjacencyList[i])
                {
                    if (!AdjacencyList[x].Contains(i))
                    {
                        AdjacencyList[x].Add(i);
                    }

                }
                var xcor = Coords[i].X;
                var ycor = Coords[i].Y;
                foreach (var x in AdjacencyList[i])
                {
                    var xcor2 = Coords[x].X;
                    var ycor2 = Coords[x].Y;

                    arg.Graphics.DrawLine(pen, xcor, ycor, xcor2, ycor2);
                    platno.Invalidate();
                }


            }
        }

        public void DFS(Object stateInfo)
        {
            int start = 0;
            this.Invoke((Action)delegate
            {
                listBox1.Items.Clear();
            });
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            Pen pen = new Pen(Color.Yellow, 5);

            var visited = new List<int>();

            if (!AdjacencyList.ContainsKey(start))
                return;

            var stack = new Stack<int>();
            stack.Push(start);
            List<int> helper = new List<int>();
            while (stack.Count > 0)
            {
                var vertex = stack.Pop();
                if (visited.Contains(vertex))
                    continue;
                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Yellow);
                platno.Invalidate();

                mre.Reset();
                //Pocakamo na klik gumba
                mre.WaitOne();

                foreach (var h in helper)
                {
                    DrawCircle(arg, Coords[h].X, Coords[h].Y, 10, 10, Color.Red);
                }

                helper.Clear();


                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Orange);
                platno.Invalidate();


                //Poklicemo event handler zunaj naše niti
                this.Invoke((Action)delegate
                {
                    listBox1.Items.Add(vertex.ToString());
                });

                visited.Add(vertex);

                this.Invoke((Action)delegate
                {
                    listBox2.Items.Clear();
                });
                foreach (var neighbor in AdjacencyList[vertex])
                {
                    
                    if (!visited.Contains(neighbor))
                    {
                        helper.Add(neighbor);
                        DrawCircle(arg, Coords[neighbor].X, Coords[neighbor].Y, 10, 10, Color.DarkGreen);
                        platno.Invalidate();
                        stack.Push(neighbor);
                        this.Invoke((Action)delegate
                        {
                            listBox2.Items.Add(neighbor.ToString());
                        });
                    }

                }
                mre.Reset();
                //Pocakamo na gumb
                mre.WaitOne();

                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Blue);



            }

            arg.Graphics.Clear(Color.White);
            this.Invalidate();
            AdjacencyList.Clear();
            Coords.Clear();
            this.Invoke((Action)delegate
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                var formPopup = new PopupForm2();
                formPopup.Show(this); // if you need non-modal window
            });
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mre.Set();
        }
        private void deleteGraph(object sender, EventArgs e)
        {
            this.Invalidate();
        }

         
        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            arg.Graphics.Clear(Color.White);
            this.Invalidate();
            AdjacencyList.Clear();
            Coords.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            //foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            //{
            //    IntPtr ptrThread = OpenThread(1, false, (uint)pt.Id);
            //    string ax=pt.GetType().ToString();
            //    if ((AppDomain.GetCurrentThreadId() != pt.Id))// & pt.CurrentPriority < 10)
            //    {
            //        try
            //        {
            //            TerminateThread(ptrThread, 1);
            //        }
            //        catch (Exception exc)
            //        {
            //            continue;
            //        }
            //    }

            //}


            this.generate_graph(steviloVozlisc);//, steviloPovezav);
            this.draw_edges();

            //ThreadPool.QueueUserWorkItem(DFS);
            Thread nit = new Thread(DFS);
            nit.Start();
            //nit.Abort();


        }

        private void pritiskMiske(object sender, MouseEventArgs e)
        {
            risem = true;
            zadnjaLokacija = e.Location;
        }
    }
}
