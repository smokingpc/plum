using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace plum_csharp_test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void WorkThread()
        {
            throw new Exception("Thread exception");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new Exception("UI exception");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread exception_thread = new Thread(WorkThread);
            exception_thread.Start();
        }
    }
}
