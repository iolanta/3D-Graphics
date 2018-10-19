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
        public List<Figure> scene = new List<Figure>();

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            scene.Add(Figure.get_Hexahedron(100));
            
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
            var g = e.Graphics;
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            g.ScaleTransform(1, -1);

            List<Figure> view = new List<Figure>(scene);

            foreach (Figure f in view) {
                f.apply_matrix(orthographic_projection_X(f.get_matrix()));
                foreach (Edge ed in f.edges)
                    g.DrawLine(new Pen(Color.Black), new PointF(ed.p1.y, ed.p1.z), new PointF(ed.p2.y, ed.p2.z));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }
    }

    public class Point3D
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

        public Point3D(Point3D p)
        {
            x = p.x;
            y = p.y;
            z = p.z;
        }


        public static Point3D norm(Point3D p)
        {
            float z = (float)Math.Sqrt((float)(p.x * p.x + p.y * p.y + p.z * p.z));
            return new Point3D(p.x / z, p.y / z, p.z / z);
        }

    }

    public class Segment3D {
        public Point3D p1, p2;
        public Segment3D(Point3D _p1, Point3D _p2) {
            p1 = _p1;
            p2 = _p2;
        }
        public Segment3D(Segment3D s)
        {
            p1 = s.p1;
            p2 = s.p2;

        }
    }

    public class Side {
        public Figure host = null;
        public List<int> edges = new List<int>();

        public Side(Figure host = null) {
        }
        public Side(Side s) {
            edges = new List<int>(edges);
            host = s.host;
        }



        public Edge get_edge(int i) {
            if (host == null)
                return null;
            return host.edges[i];
        }

        public void add_edge(Edge e) {
            if (host == null)
                return;
            int res = host.edges.FindIndex(x => e == x);
            if(res>=0)
                edges.Add(res);
        }


    }

    public class Edge
    {
        public Figure host = null;
        public int ind_p1, ind_p2;

        public Edge(int i1, int i2, Figure h = null) {
            ind_p1 = i1;
            ind_p2 = i2;
            host = h;
        }

        public Edge(Edge e) {
            ind_p1 = e.ind_p1;
            ind_p2 = e.ind_p1;
            host = e.host;
        }

        public Point3D p1
        {
            get
            {
                if(host != null)
                return new Point3D(host.points[ind_p1]);
                return null;
            }
            set
            {
                if (host !=null)
                {
                    host.points[ind_p1] = value;
                }
            }
        }

        public Point3D p2
        {
            get
            {
                if (host != null)
                    return new Point3D(host.points[ind_p2]);
                return null;
            }
            set
            {
                if (host != null)
                {
                    host.points[ind_p2] = value;
                }
            }
        }

   
    }


    public class Figure
    {
        
        public List<Point3D> points = new List<Point3D>(); // точки 
        public List<Edge> edges = new List<Edge>(); // ребра 
        public List<Side> sides = new List<Side>(); // стороны

        public Figure() { }

        public Figure(Figure f) {
            foreach (Point3D p in f.points) {
                points.Add(new Point3D(p));
            }
            foreach(Edge e in f.edges)
            {
                edges.Add(new Edge(e));
                edges.Last().host = this;
            }
            foreach (Side s in f.sides)
            {
                sides.Add(new Side(s));
                sides.Last().host = this;
            }

        }

        public float[,] get_matrix()
        {
            var res = new float[points.Count, 4];
            for (int i = 0; i < points.Count; i++)
            {
                res[i, 0] = points[i].x;
                res[i, 1] = points[i].y;
                res[i, 2] = points[i].z;
                res[i, 3] = 1;
            }
            return res;
        }

        public void apply_matrix(float[,] matrix) {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].x=matrix[i, 0];
                points[i].y = matrix[i, 1];
                points[i].z = matrix[i, 2];
                
            }
        }


        private Point3D get_center() {
            Point3D res = new Point3D(0,0,0);
            foreach (Point3D p in points)
            {
                res.x += p.x;
                res.y += p.y;
                res.z += p.z;

            }
            res.x /= points.Count();
            res.y /= points.Count();
            res.z /= points.Count();
            return res;
        }



        private void scale_around_center(float xs, float ys, float zs) {
            float[,] pnts = get_matrix();
            Point3D p = get_center();
            pnts = apply_offset(pnts, -p.x, -p.y, -p.z);
            pnts = apply_scale(pnts, xs, ys, zs);
            pnts = apply_offset(pnts, p.x, p.y, p.z);
            apply_matrix(pnts);
        }


        

        /// <summary>
        ///  rotate this figure around line(beg,end) by angle 
        /// </summary>
        /// <param name="beg"></param>
        /// <param name="end"></param>
        /// <param name="angle"> in degrees</param>
        private void rotate_around(Point3D beg, Point3D end, float angle) {
            end = Point3D.norm(end);
            angle = angle * (float)Math.PI / 180;     
            apply_matrix(rotate_around_line(get_matrix(), beg, end, angle));
        }


        /// <summary>
        ///  rotating around line 
        /// </summary>
        /// <param name="start">X Y Z</param>
        /// <param name="dir"> l m n</param>
        /// <param name="angle"> in radians</param>
        private static float[,] rotate_around_line(float[,] transform_matrix, Point3D start, Point3D dir, float angle) {
            float cos_angle = (float)Math.Cos(angle);
            float sin_angle = (float)Math.Sin(angle);
            float val00 = dir.x * dir.x + cos_angle * (1 - dir.x * dir.x);
            float val01 = dir.x * (1 - cos_angle) * dir.y + dir.z * sin_angle;
            float val02 = dir.x * (1 - cos_angle) * dir.z - dir.y * sin_angle;
            float val10 = dir.x * (1 - cos_angle) * dir.y - dir.z * sin_angle;
            float val11 = dir.y * dir.y + cos_angle * (1 - dir.y * dir.y);
            float val12 = dir.y * (1 - cos_angle) * dir.z + dir.x * sin_angle;
            float val20 = dir.x * (1 - cos_angle) * dir.z + dir.y * sin_angle;
            float val21 = dir.y * (1 - cos_angle) * dir.z - dir.x * sin_angle;
            float val22 = dir.z * dir.z + cos_angle * (1 - dir.z * dir.z);
            float[,] rotateMatrix = new float[,] { { val00, val01, val02, 0 }, { val10, val11, val12, 0 }, { val20, val21, val22, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(rotateMatrix, transform_matrix);
        }



        private static float[,] multiply_matrix(float[,] m1, float[,] m2)
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

        private static float[,] apply_offset(float[,] transform_matrix, float offset_x, float offset_y, float offset_z)
        {
            float[,] translationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { offset_x, offset_y, offset_z, 1 } };
            return multiply_matrix(transform_matrix, translationMatrix);
        }

        private static float[,] apply_rotation_X(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, (float)Math.Cos(angle), (float)Math.Sin(angle), 0 },
                { 0, -(float)Math.Sin(angle), (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private static float[,] apply_rotation_Y(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), 0, -(float)Math.Sin(angle), 0 }, { 0, 1, 0, 0 },
                { (float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0}, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private static float[,] apply_rotation_Z(float[,] transform_matrix, float angle)
        {
            float[,] rotationMatrix = new float[,] { { (float)Math.Cos(angle), (float)Math.Sin(angle), 0, 0 }, { -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0 },
                { 0, 0, 1, 0 }, { 0, 0, 0, 1} };
            return multiply_matrix(transform_matrix, rotationMatrix);
        }

        private static float[,] apply_scale(float[,] transform_matrix, float scale_x, float scale_y, float scale_z)
        {
            float[,] scaleMatrix = new float[,] { { scale_x, 0, 0, 0 }, { 0, scale_y, 0, 0 }, { 0, 0, scale_z, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, scaleMatrix);
        }

        private static float[,] orthographic_projection_X(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }

        private static float[,] orthographic_projection_Y(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }

        private static float[,] orthographic_projection_Z(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }


        static public Figure get_Hexahedron(float sz)
        {
            Figure res = new Figure();
            res.points.Add(new Point3D(sz / 2, sz / 2, sz / 2)); // 0 
            res.edges.Add(new Edge(0, 1, res));
            res.points.Add(new Point3D(sz / 2, -sz / 2, sz / 2)); // 1
            res.edges.Add(new Edge(0, 2, res));
            res.points.Add(new Point3D(-sz / 2, sz / 2, sz / 2)); // 2
            res.edges.Add(new Edge(0, 3, res));
            res.points.Add(new Point3D(sz / 2, sz / 2, -sz / 2)); //3

            res.points.Add(new Point3D(-sz / 2, -sz / 2, -sz / 2)); // 4
            res.edges.Add(new Edge(4, 5, res));
            res.points.Add(new Point3D(-sz / 2, sz / 2, -sz / 2)); //5
            res.edges.Add(new Edge(4, 6, res));
            res.points.Add(new Point3D(sz / 2, -sz / 2, -sz / 2)); //6
            res.edges.Add(new Edge(4, 7, res));
            res.points.Add(new Point3D(-sz / 2, -sz / 2, sz / 2)); //7

            res.edges.Add(new Edge(2, 5, res)); // 
            res.edges.Add(new Edge(5, 3, res)); // 
            res.edges.Add(new Edge(3, 6, res)); // 
            res.edges.Add(new Edge(6, 1, res)); // 
            res.edges.Add(new Edge(7, 1, res)); // 
            res.edges.Add(new Edge(7, 2, res)); // 

            
            
            return res;
        }

    }



}
