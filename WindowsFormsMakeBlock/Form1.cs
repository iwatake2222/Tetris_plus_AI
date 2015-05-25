using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsMakeBlock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int penWidth = 2;
            int size = 30;
            Brush[] brushCol = new Brush[]{
                Brushes.Gray,
                Brushes.Red,
                Brushes.Green,
                Brushes.Blue,
                Brushes.Yellow,
                Brushes.Purple,
                Brushes.Olive,
                Brushes.Orange,
                Brushes.DeepPink,
            };

            Pen penEdge = new Pen(Color.Silver, penWidth);

            Bitmap bmp = new Bitmap(size*brushCol.Count(), size);
            Graphics g = Graphics.FromImage(bmp);

            for (int i = 0; i < brushCol.Count(); i++) {
                g.DrawRectangle(penEdge, penWidth / 2 + size * i, penWidth / 2, size - penWidth, size - penWidth);
                g.FillRectangle(brushCol[i], penWidth / 2 + penWidth / 2 + size * i, penWidth / 2 + penWidth / 2, size - penWidth*2, size - penWidth*2);
            }

            g.Dispose();
            pictureBox1.Image = bmp;
            bmp.Save("test.png");




        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
