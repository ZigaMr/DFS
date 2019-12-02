using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DFS
{
    public partial class PopupForm2 : Form
    {
        public PopupForm2()
        {

            //mainForm = callingForm as Form1;
            InitializeComponent();
        }
        public PopupForm2(Form callingForm)
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            InitializeComponent();

            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {

                (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).Show();
                (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).deleteGraph();
                //(System.Windows.Forms.Application.OpenForms["Form1"] as Form1).generate_graph(Form1.steviloVozlisc);
                //(System.Windows.Forms.Application.OpenForms["Form1"] as Form1).draw_edges();
            }
            this.Close();
        }
    }
}
