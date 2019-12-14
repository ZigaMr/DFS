using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DFS
{
    public partial class PopupForm : Form
    {
        public PopupForm()
        {
            InitializeComponent();
        }
        //private static Form1 mainForm = null;
        public PopupForm(Form callingForm)
        {
            //mainForm = callingForm as Form1;
            InitializeComponent();
            
        }
        private void PopupForm_Load(object sender, EventArgs e)
        {

        }



        public void button1_Click(object sender, EventArgs e)
        {
            //this.mainForm
            try
            {
                Form1.steviloVozlisc = int.Parse(textBox1.Text);
            }
            catch
            {
                Form1.steviloVozlisc = 0;
            }
            //Form1.steviloPovezav = int.Parse(textBox1.Text);
            //Form1.generate_graph(Form1.steviloVozlisc, Form1.steviloPovezav);
            this.Close();
            (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).Show();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
