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

        private float[,] apply_offset(float [,] transform_matrix, float offset_x, float offset_y, float offset_z)
        {
            float[,] translationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { offset_x, offset_y, offset_z, 1 } };
            return multiply_matrix(transform_matrix, translationMatrix);
        }

        private float[,] apply_rotation_X(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, (float)Math.Cos(angle), (float)Math.Sin(angle), 0 },
                { 0, -(float)Math.Sin(angle), (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private float[,] apply_rotation_Y(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), 0, -(float)Math.Sin(angle), 0 }, { 0, 1, 0, 0 },
                { (float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private float[,] apply_rotation_Z(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), (float)Math.Sin(angle), 0, 0 }, { -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0 },
                { 0, 0, 1, 0 }, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private float[,] apply_scale(float[,] transform_matrix, float scale_x, float scale_y, float scale_z)
        {
            float[,] scaleMatrix = new float[,] { { scale_x, 0, 0, 0 }, { 0, scale_y, 0, 0 }, { 0, 0, scale_z, 0}, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, scaleMatrix);
        }

        private float[,] orthographic_projection_X(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }

        private float[,] orthographic_projection_Y(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }

        private float[,] orthographic_projection_Z(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }

        private float[,] isometric_projection(float[,] transform_matrix)
        {
            float a = (float)Math.Asin(Math.Tan(30 * Math.PI / 180));
            float b = 45 * (float)Math.PI / 180;
            float[,] transposeRotationMatrixY = new float[,] { { (float)Math.Cos(b), 0, (float)Math.Sin(b) }, { 0, 1, 0 }, { -(float)Math.Sin(b), 0, (float)Math.Cos(b) } };
            float[,] transposeRotationMatrixX = new float[,] { { 1, 0, 0 }, { 0, (float)Math.Cos(a), -(float)Math.Sin(a) }, { 0, (float)Math.Sin(a), (float)Math.Cos(a) } };
            float[,] ortMatrix = new float[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

            float[,] mt1 = multiply_matrix(transform_matrix, transposeRotationMatrixY);
            float[,] mt2 = multiply_matrix(mt1, transposeRotationMatrixX);
            return multiply_matrix(mt2, ortMatrix);
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
