using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharp.Script;

namespace CSScriptDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            int errorcode = CSCompiler.Compile(textBox1.Text, "test.dll", out message);
            if (errorcode != 0)
            {
                textBox2.Text = message;
            }

            //object obj1 = CSScript.CSScript.CreateObject("test.dll", "test.testdata", null);
            //object obj2 = CSScript.CSScript.CreateObject("test.dll", "test.CTObservableCollection", null);

            //string message = string.Empty;
            int error = 0;
            object obj = CSCompiler.Compile(textBox1.Text, "test.testdata", null, out error, out message);
            if (errorcode != 0)
            {
                textBox2.Text = message;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int error = 0;
            string message = string.Empty;
            var cpr = CSCompiler.Compile(textBox1.Text, "aaa.dll", out error, out message);
            if (error != 0)
            {
                textBox2.Text = message;
            }

            object obj1 = CSCompiler.CreateObject(cpr, "WpfApplication1.NS", null, 0);
            //object obj2 = CSCompiler.CreateObject(cpr, "test.CTObservableCollection", null, 0);

        }
    }
}
