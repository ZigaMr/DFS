using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DFS
{
    public partial class Form1 : Form
    {
        Dictionary<int, List<int>> SlovarSosedov = new Dictionary<int, List<int>>();//Slovar vozlišč in soležnih sosedov
        List<Point> Coords = new List<Point>();
        Stack<int> sklad = new Stack<int>();
        Stack<int> CoordStack = new Stack<int>();

        int start;
        List<int> obiskani = new List<int>();
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

            // Povežemo dogodke z metodami
            dataGridView1.EnableHeadersVisualStyles = false;
            Load += novoOzadje;
        }

        private void novoOzadje(object sender, EventArgs e)
        {
            platno.Image = new Bitmap(platno.Size.Width, platno.Size.Height);
        }

        private void posodobi_matriko(int visina, int sirina)
        {
            dataGridView1.ColumnCount = visina;

            for (int r = 0; r < visina; r++)
            {
                dataGridView1.Columns[r].Width = 25;
                dataGridView1.Columns[r].Name = r.ToString();
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);

                for (int c = 0; c < sirina; c++)
                {
                    if (SlovarSosedov[r].Contains(c))
                    {
                        row.Cells[c].Value = 1;

                    }
                    
                }
                
                dataGridView1.Rows.Add(row);
                dataGridView1.Rows[r].Height = 25;
                dataGridView1.Rows[r].HeaderCell.Value = r.ToString();
            }
            DataGridViewElementStates states = DataGridViewElementStates.None;
            dataGridView1.ScrollBars = ScrollBars.None;
            var totalHeight = dataGridView1.Rows.GetRowsHeight(states) + dataGridView1.ColumnHeadersHeight;
            totalHeight += dataGridView1.Rows.Count * 4;  // a correction I need
            var totalWidth = dataGridView1.Columns.GetColumnsWidth(states) + dataGridView1.RowHeadersWidth;
            dataGridView1.ClientSize = new Size(totalWidth, totalHeight);
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

            for (int i=0; i < stVozlisc; i++)
            {

                X = 100 + (int)(200*(1+Math.Cos(2 * Math.PI * i / stVozlisc)));
                Y = 100 + (int)(150*(1+Math.Sin(2 * Math.PI * i / stVozlisc)));
                
                DrawCircle(arg, X, Y, 20, 20, Color.Red);
                Coords.Add(new Point(X, Y));

                g.DrawString(i.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(X-5, Y-5));
                platno.Invalidate();
            }

        }

        public int draw_edges()
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);

            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);
            for (int i = 0; i < Coords.Count(); i++)
            {
                SlovarSosedov[i] = new List<int>();
            }
            for (int i = 0; i < Coords.Count(); i++)
            {
                List<int> possible = Enumerable.Range(0,Coords.Count()).ToList();
                possible.RemoveAt(i);

                Random rnd = new Random();
                int r1 = rnd.Next(0,possible.Count);
                int r2 = 0;

                if (!SlovarSosedov[i].Contains(r1))
                {
                    SlovarSosedov[i].Add(possible[r1]);
                }

                int nxt = rnd.Next(0, 2);
                if (nxt == 1)
                {
                    r2 = rnd.Next(0, possible.Count);
                }

                if (!SlovarSosedov[i].Contains(r2))
                {
                    SlovarSosedov[i].Add(possible[r2]);
                }

                Pen pen = new Pen(Color.Green, 2);

                foreach (var x in SlovarSosedov[i])
                {
                    if (!SlovarSosedov[x].Contains(i))
                    {
                        SlovarSosedov[x].Add(i);
                    }

                }
                var xcor = Coords[i].X;
                var ycor = Coords[i].Y;
                foreach (var x in SlovarSosedov[i])
                {
                    var xcor2 = Coords[x].X;
                    var ycor2 = Coords[x].Y;
                    c = (float)Math.Sqrt(Math.Pow(xcor2 - xcor, 2) + Math.Pow(ycor2 - ycor, 2));
                    x_delta = 10 * (xcor - xcor2) / c;
                    y_delta = 10 * (ycor - ycor2) / c;
                    arg.Graphics.DrawLine(pen, xcor - x_delta, ycor - y_delta, xcor2 + x_delta, ycor2 + y_delta);
                    platno.Invalidate();
                }


            }
            return Coords.Count; 
        }

        //Next gumb
        private void button3_Click(object sender, EventArgs e)
        {
            Graphics g;
            g = Graphics.FromImage(platno.Image);
            Rectangle rectangle = new Rectangle();
            PaintEventArgs arg = new PaintEventArgs(g, rectangle);

            if (steviloVozlisc < 2)
            {
                g.DrawString("Premalo vozlišč", new Font("Arial", 32, FontStyle.Bold), Brushes.Black, new Point(100, 300));
                platno.Invalidate();
                return;
            }

            dataGridView1.SuspendLayout();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.HeaderCell.Style.BackColor = Color.Empty;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if(cell.Style.BackColor != Color.LightSkyBlue)
                        cell.Style.BackColor = Color.Empty;
                }
            }
            dataGridView1.ResumeLayout();

            if (SlovarSosedov.Count == 0)
            {
                arg.Graphics.Clear(Color.White);
                g.DrawString("Ni grafa", new Font("Arial", 32, FontStyle.Bold), Brushes.Black, new Point(200,300));
                platno.Invalidate();
                return;
            }

            if (counter == 0 & sklad.Count > 0)
            {

                neobiskani_sosedje.Clear();
                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.LightSkyBlue);
                g.DrawString(vertex.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[vertex].X -5, Coords[vertex].Y - 5));
                xcor = Coords[vertex].X;
                ycor = Coords[vertex].Y;
                CoordStack.Push(vertex);
                vertex = sklad.Pop();
                listBox3.Items.RemoveAt(listBox3.Items.Count - 1);
                richTextBox2.Text = "Naslednje vozlišče v skladu je " + vertex.ToString();
                platno.Invalidate();


                if (obiskani.Contains(vertex))
                    return;

                CoordStack.Push(vertex);
                xcor2 = Coords[vertex].X;
                ycor2 = Coords[vertex].Y;
                Pen pen = new Pen(Color.Yellow, 5);
                int i=0;
                while (CoordStack.Count > 0)
                {
                    i = CoordStack.Pop();
                    if (SlovarSosedov[vertex].Contains(i))
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
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.Rows[vertex].HeaderCell.Style.BackColor = Color.Red;
                dataGridView1.Rows[i].Cells[vertex].Style.BackColor = Color.LightSkyBlue;
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

                Pen pen = new Pen(Color.Yellow, 5);

                c = (float)Math.Sqrt(Math.Pow(xcor2 - xcor, 2) + Math.Pow(ycor2 - ycor, 2));
                x_delta = 10 * (xcor - xcor2) / c;
                y_delta = 10 * (ycor - ycor2) / c;
                arg.Graphics.DrawLine(pen, xcor - x_delta, ycor - y_delta, Coords[vertex].X + x_delta, Coords[vertex].Y + y_delta);

                DrawCircle(arg, Coords[vertex].X, Coords[vertex].Y, 20, 20, Color.Orange);
                g.DrawString(vertex.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[vertex].X - 5, Coords[vertex].Y - 5));
                platno.Invalidate();


                listBox1.Items.Add(vertex.ToString());

                obiskani.Add(vertex);
                neobiskani_sosedje.Clear();
                dataGridView1.Rows[vertex].HeaderCell.Style.BackColor = Color.Red;
                foreach (var neighbor in SlovarSosedov[vertex])
                {
                    
                    if (!obiskani.Contains(neighbor))
                    {
                        helper.Add(neighbor);
                        DrawCircle(arg, Coords[neighbor].X, Coords[neighbor].Y, 20, 20, Color.LightSeaGreen);
                        dataGridView1.Rows[vertex].Cells[neighbor].Style.BackColor = Color.LightSeaGreen;
                        g.DrawString(neighbor.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(Coords[neighbor].X - 5, Coords[neighbor].Y - 5));
                        platno.Invalidate();
                        sklad.Push(neighbor);
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
                SlovarSosedov.Clear();
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
            SlovarSosedov.Clear();
            Coords.Clear();
            listBox1.Items.Clear();

            start = 0;
            listBox1.Items.Clear();
            Pen pen = new Pen(Color.Yellow, 5);

            obiskani.Clear();
            sklad.Clear();
            this.Invalidate();
        }

        //Generiraj gumb
        public void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
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
            SlovarSosedov.Clear();
            Coords.Clear();
            listBox1.Items.Clear();
            listBox3.Items.Clear();

            start = 0;
            listBox1.Items.Clear();
            Pen pen = new Pen(Color.Yellow, 5);
            vertex = 0;
            obiskani.Clear();
            sklad.Clear();
            sklad.Push(start);
            listBox3.Items.Add(start);
            helper.Clear();
            counter = 0;
            CoordStack.Clear();

            this.generate_graph(steviloVozlisc);
            int sirina = this.draw_edges();
            posodobi_matriko(sirina, sirina);
            //dataGridView1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;



            platno.Invalidate();
        }

        private void pritiskMiske(object sender, MouseEventArgs e)
        {
        }
    }
}
