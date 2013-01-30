using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ploter_ramie
{
    public partial class Form1 : Form
    {
        private pltFile _file = new pltFile();
        private arm ramie= new arm();

        public Form1()
        {
            InitializeComponent();
            ramie.Bmp_Refresh += new arm.BmpRefresh(ramie_Bmp_Refresh);
            ramie.log += new arm.Log(ramie_log);       
        }

        void ramie_log(string text)
        {
           textBox1.AppendText(text+Environment.NewLine);
        }

        void ramie_Bmp_Refresh(Bitmap arm, Bitmap picture, int progress)
        {
            pictureBox1.Image = arm;
            pictureBox2.Image = picture;
            try
            {
                progressBar1.Value = progress;
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ramie.loadSettings();
            _file.LoadFile();
            this.Text = _file.fileName;
            button2.Enabled = true;
            button3.Enabled = true;

        }

      

        private void button2_Click(object sender, EventArgs e)
        {
            ramie.Disconnect();
            ramie.Draw(_file.fileContent, 100 + trackBar1.Value, trackBar2.Value, trackBar3.Value);

            
        }

      



        private void Form1_Load(object sender, EventArgs e)
        {
            foreach( string a in Arduino.Arduino.PortsList())
            {
                comboBox1.Items.Add(a);
            }

            try
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }catch
            {
            }

        }

       

        private void button4_Click(object sender, EventArgs e)
        {
            ramie.Disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ramie.ConnectToArduino(comboBox1.Text);

            Thread.Sleep(1000);

            ramie.Draw(_file.fileContent, 100 + trackBar1.Value, trackBar2.Value, trackBar3.Value);

        }

      

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ramie.ConnectToArduino(comboBox1.Text );
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox1.AppendText(trackBar3.Value  + Environment.NewLine);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.AppendText(trackBar1.Value + Environment.NewLine);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox1.AppendText(trackBar2.Value + Environment.NewLine);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ramie.SetPosition(e.X, 400 - e.Y);

            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if(checkBox1.Checked) ramie.SetServos((byte)numericUpDown1.Value ,(byte)numericUpDown2.Value ,(byte)numericUpDown3.Value );
        }
    }
}
