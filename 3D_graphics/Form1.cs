using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3D_graphics
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private float[,] multiply_matrix(float[,] m1, float[,] m2)
        {
            float[,] res = new float[m1.GetLength(0), m2.GetLength(1)];
            for (int i = 0; i < m1.GetLength(0); i++)
            {
                for (int j = 0; j < m2.GetLength(1); j++)
                {
                    for (int k = 0; k < m2.GetLength(0); k++)
                    {
                        res[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return res;
           
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    class Point3D
    {
        public float x, y, z;
        public Point3D() {
            x = 0;
            y = 0;
            z = 0;

        }
        public Point3D(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;

        }
    }

    class Segment3D {
        public Point3D p1, p2;
        public Segment3D(Point3D _p1, Point3D _p2) {
            p1 = _p1;
            p2 = _p2;
        }
    }
}
