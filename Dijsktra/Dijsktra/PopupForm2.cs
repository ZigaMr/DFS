using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dijsktra
{
    public partial class PopupForm2 : Form
    {
        public PopupForm2()
        {
            InitializeComponent();
        }
        public PopupForm2(Form callingForm)
        {
            //mainForm = callingForm as Form1;
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
