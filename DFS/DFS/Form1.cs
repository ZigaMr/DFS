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

namespace DFS
{
    public partial class Form1 : Form
    {

        Dictionary<int, List<int>> AdjacencyList = new Dictionary<int, List<int>>();
        public static ManualResetEvent mre = new ManualResetEvent(true);

        Point zadnjaLokacija;   //hrani zadnjo lokacijo miške
        bool risem;             //stikalo za risanje
        List<Point> Coords = new List<Point>();
        Stack<int> stack = new Stack<int>();
        Stack<int> CoordStack = new Stack<int>();

        int start;
        List<int> visited = new List<int>();
        List<int> neobiskani_sosedje = new List<int>();
        int xcor2, ycor2, ycor, xcor;
        int vertex;
        int counter;
        List<int> helper = new List<int>();
        float x_delta, y_delta, c;

        public static int steviloVozlisc;

        public Form1()
        {
            InitializeComponent();
            risem = false;

            // Povežemo dogodke z metodami

            Load += novoOzadje;
        }

        private void novoOzadje(object sender, EventArgs e)
        {
            platno.Image = new Bitmap(platno.Size.Width, platno.Size.Height);
        }

        private void DrawCircle(PaintEventArgs e, int x, int y, int width, int height, Color clr)
        {
            Pen pen = new Pen(clr, 3);
            e.Graphics.DrawEllipse(pen, x - width / 2, y - height / 2, width, height);
            SolidBrush solidBrush = new SolidBrush(clr);
            e.Graphics.FillEllipse(solidBrush, x - width / 2, y - height / 2, width, height);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //Začetek gumb
        private void button1_Click(object sender, EventArgs e)
        {
            string st_vozlisc;
            var formPopup = new PopupForm();
            this.Hide();
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

        public void generate_graph(int stVozlisc)
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

                X = 100 + (int)(200*(1+Math.Cos(2 * Math.PI * i / steviloVozlisc)));
                Y = 100 + (int)(150*(1+Math.Sin(2 * Math.PI * i / steviloVozlisc)));

                counter = 1;
                
                DrawCircle(arg, X, Y, 20, 20, Color.Red);
                Coords.Add(new Point(X, Y));

                g.DrawString(i.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(X-5, Y-5));
                platno.Invalidate();
            }

        }
        public void draw_edges()
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            for (int i = 0; i < Coords.Count(); i++)
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

                if (!AdjacencyList[i].Contains(r1))
                {
                    AdjacencyList[i].Add(possible[r1]);
                }

                int nxt = rnd.Next(0, 2);
                if (nxt == 1)
                {
                    r2 = rnd.Next(0, possible.Count);
                }

                if (!AdjacencyList[i].Contains(r2))
                {
                    AdjacencyList[i].Add(possible[r2]);
                }

                Pen pen = new Pen(Color.Green, 2);

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

           
        //Next gumb
        private void button3_Click(object sender, EventArgs e)
        {

            Graphics g;
            g = Graphics.FromImage(platno.Image);
            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            if (AdjacencyList.Count == 0)
            {
                arg.Graphics.Clear(Color.White);
                g.DrawString("Ni grafa", new Font("Arial", 32, FontStyle.Bold), Brushes.Black, new Point(200,300));
                platno.Invalidate();
                return;
            }

            if (counter == 0 & stack.Count > 0)
            {
                neobiskani_sosedje.Clear();
                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.LightSkyBlue);
                g.DrawString(vertex.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[vertex].X -5, Coords[vertex].Y - 5));
                xcor = Coords[vertex].X;
                ycor = Coords[vertex].Y;
                CoordStack.Push(vertex);
                vertex = stack.Pop();
                listBox3.Items.RemoveAt(listBox3.Items.Count - 1);
                richTextBox2.Text = "Naslednje vozlišče v skladu je " + vertex.ToString();
                platno.Invalidate();


                if (visited.Contains(vertex))
                    return;

                CoordStack.Push(vertex);
                xcor2 = Coords[vertex].X;
                ycor2 = Coords[vertex].Y;
                Pen pen = new Pen(Color.Yellow, 5);
                int i=0;
                while (CoordStack.Count > 0)
                {
                    i = CoordStack.Pop();
                    if (AdjacencyList[vertex].Contains(i))
                    {
                        break;
                    }
                }
                xcor = Coords[i].X;
                ycor = Coords[i].Y;
                CoordStack.Push(i);
                CoordStack.Push(vertex);
                c = (float)Math.Sqrt(Math.Pow(xcor2 - xcor, 2) + Math.Pow(ycor2 - ycor, 2));
                x_delta = 10 * (xcor - xcor2)/c;
                y_delta = 10 * (ycor - ycor2)/c;
                arg.Graphics.DrawLine(pen, xcor-x_delta, ycor-y_delta, xcor2+x_delta, ycor2+y_delta);
                //DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Yellow);
                g.DrawString(vertex.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[vertex].X - 5, Coords[vertex].Y - 5));
                platno.Invalidate();
                counter = 1;
            }
               else if (counter == 1)
            {

                foreach (var h in helper)
                {
                    DrawCircle(arg, Coords[h].X, Coords[h].Y, 20, 20, Color.Red);
                    g.DrawString(h.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[h].X - 5, Coords[h].Y - 5));
                }

                helper.Clear();

                //var xcor2 = Coords[vertex].X;
                //var ycor2 = Coords[vertex].Y;
                Pen pen = new Pen(Color.Yellow, 5);

                c = (float)Math.Sqrt(Math.Pow(xcor2 - xcor, 2) + Math.Pow(ycor2 - ycor, 2));
                x_delta = 10 * (xcor - xcor2) / c;
                y_delta = 10 * (ycor - ycor2) / c;
                arg.Graphics.DrawLine(pen, xcor - x_delta, ycor - y_delta, Coords[vertex].X + x_delta, Coords[vertex].Y + y_delta);
                //arg.Graphics.DrawLine(pen, xcor, ycor, Coords[vertex].X, Coords[vertex].Y);

                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Orange);
                g.DrawString(vertex.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[vertex].X - 5, Coords[vertex].Y - 5));
                platno.Invalidate();


                listBox1.Items.Add(vertex.ToString());

                visited.Add(vertex);
                neobiskani_sosedje.Clear();
                foreach (var neighbor in AdjacencyList[vertex])
                {
                    
                    if (!visited.Contains(neighbor))
                    {
                        helper.Add(neighbor);
                        DrawCircle(arg, Coords[neighbor].X, Coords[neighbor].Y, 20, 20, Color.LightSeaGreen);
                        g.DrawString(neighbor.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[neighbor].X - 5, Coords[neighbor].Y - 5));
                        platno.Invalidate();
                        stack.Push(neighbor);
                        listBox3.Items.Add(neighbor.ToString());
                        neobiskani_sosedje.Add(neighbor);
                    }

                }
                if(neobiskani_sosedje.Count == 0)
                {
                    richTextBox2.Text = "Vozlišče " + vertex.ToString() + " nima neobiskanih sosedov. \nNadaljujemo pri zadnjem dodanem vozlišču v skladu";
                }
                else if(neobiskani_sosedje.Count == 1)
                {
                    richTextBox2.Text += "\nNeobiskan sosed je " + string.Join(", ", neobiskani_sosedje.ToArray());
                }
                else
                {
                    richTextBox2.Text += "\nNeobiskani sosedje so " + string.Join(", ", neobiskani_sosedje.ToArray());

                }
                counter = 0;
            }

            else
            {
                arg.Graphics.Clear(Color.White);
                this.Invalidate();
                AdjacencyList.Clear();
                Coords.Clear();
                listBox1.Items.Clear();
                var formPopup = new PopupForm2();
                this.Hide();
                formPopup.ShowDialog(this); 
            }

            return;
        }

        public void deleteGraph()
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

            start = 0;
            listBox1.Items.Clear();
            Pen pen = new Pen(Color.Yellow, 5);

            visited.Clear();
            stack.Clear();
            this.Invalidate();
        }

        //Generiraj gumb
        public void button2_Click(object sender, EventArgs e)
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            arg.Graphics.Clear(Color.White);
            if (steviloVozlisc < 2)
            {
                g.DrawString("Premalo vozlišč", new Font("Arial", 32, FontStyle.Bold), Brushes.Black, new Point(100, 300));
                platno.Invalidate();
                return;
            }
            this.Invalidate();
            AdjacencyList.Clear();
            Coords.Clear();
            listBox1.Items.Clear();
            listBox3.Items.Clear();

            start = 0;
            listBox1.Items.Clear();
            Pen pen = new Pen(Color.Yellow, 5);
            vertex = 0;
            visited.Clear();
            stack.Clear();
            stack.Push(start);
            listBox3.Items.Add(start);
            helper.Clear();
            counter = 0;
            CoordStack.Clear();

            this.generate_graph(steviloVozlisc);
            this.draw_edges();

            platno.Invalidate();
        }

        private void pritiskMiske(object sender, MouseEventArgs e)
        {
            risem = true;
            zadnjaLokacija = e.Location;
        }
    }
}
