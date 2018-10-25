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
            ControlType.SelectedIndex = 0;
            
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            g.ScaleTransform(1, -1);
            List<Figure> view = scene.Select(f => new Figure(f)).ToList();   
            foreach (Figure f in view) {
                switch (comboBox1.Text)
                {
                    case "Central":
                        f.project_cental();
                        break;
                    case "Isometric":
                        f.project_isometric();
                        break;
                    case "Ortographic(XY)":
                        f.project_orthogZ();
                        break;
                    case "Ortographic(XZ)":
                        f.project_orthogY();
                        break;
                    case "Ortographic(YZ)":
                        f.project_orthogX();
                        break;
                    default:
                        break;
                }
                foreach (Side s in f.sides) {
                    g.DrawLines(s.drawing_pen, s.points.Select(i => new PointF(f.points[i].x, f.points[i].y)).ToArray());
                    g.DrawLine(s.drawing_pen, new PointF(f.points[s.points.First()].x, f.points[s.points.First()].y), new PointF(f.points[s.points.Last()].x, f.points[s.points.Last()].y));
                }
                 
            }
        }

        private void rotatefigure(Figure f, float ang, string type) {
            switch (type)
            {
                case "CenterX":
                    f.rotate_around(ang, "CX");
                    break;
                case "CenterY":
                    f.rotate_around(ang, "CY");
                    break;
                case "CenterZ":
                    f.rotate_around(ang, "CZ");
                    break;
                case "X axis":
                    f.rotate_around(ang, "X");
                    break;
                case "Y axis":
                    f.rotate_around(ang, "Y");
                    break;
                case "Z asix":
                    f.rotate_around(ang, "Z");
                    break;
                case "Custom Line":
                    f.line_rotate(ang, new Point3D((float)ControlCustom1X.Value, (float)ControlCustom1Y.Value, (float)ControlCustom1Z.Value),
                                        new Point3D((float)ControlCustom2X.Value, (float)ControlCustom2Y.Value, (float)ControlCustom2Z.Value));
                    break;
                default:
                    break;
            }

        }

        private void apply_transforms() {
            float ox = (float)ControlOffsetX.Value;
            float oy = (float)ControlOffsetY.Value;
            float oz = (float)ControlOffsetZ.Value;
            float sx = (float)ControlScaleX.Value/10;
            float sy = (float)ControlScaleY.Value / 10;
            float sz = (float)ControlScaleZ.Value / 10;
            float an = (float)ControlAngle.Value;
            Figure f = scene[2];


            rotatefigure(f, an, ControlType.Text);
            scalefigure(f, sx, sy, sz, ControlType.Text);   
                f.offset(ox, oy, oz);
            
            
        }

        private void scalefigure(Figure f, float sx, float sy, float sz, string type) {
            switch (type)
            {
                case "CenterX":
                case "CenterY":
                case "CenterZ":
                    f.scale_around_center(sx,sy,sz);
                    break;
                case "X axis":
                case "Y axis":                
                case "Z asix":
                    f.scale_axis(sx, sy, sz);
                    break;
                default:
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            apply_transforms();
            pictureBox1.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            scene.Clear();
            scene.Add(Figure.get_Coordinates());
            scene.Add(gen_line());
            switch (comboBox2.Text)
            {
                case "Cube":
                    scene.Add(Figure.get_Hexahedron(100));
                    break;
                case "Tetrahedron":
                    scene.Add(Figure.get_Tetrahedron(100));
                    break;
                case "Octahedron":
                    scene.Add(Figure.get_Octahedron(100));
                    break;
                case "Icosahedron":
                    scene.Add(Figure.get_Icosahedron(100));
                    break;
                default:
                    break;
              
            }
            pictureBox1.Invalidate();
            reset_controls();

        }

        private void reset_controls() {
            ControlOffsetX.Value = 0;
            ControlOffsetY.Value = 0;
            ControlOffsetZ.Value = 0;
            ControlScaleX.Value = 10;
            ControlScaleY.Value = 10;
            ControlScaleZ.Value = 10;
            ControlAngle.Value = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reset_controls();
        }

        private Figure gen_line() {
            Figure res = new Figure();
            res.points.Add(new Point3D((float)ControlCustom1X.Value, (float)ControlCustom1Y.Value, (float)ControlCustom1Z.Value));
            res.points.Add(new Point3D((float)ControlCustom2X.Value, (float)ControlCustom2Y.Value, (float)ControlCustom2Z.Value));
            res.sides.Add(new Side(res));
            res.sides.First().points =new List<int> {0,1};

            return res;
        }

        private void ControlCustom1X_ValueChanged(object sender, EventArgs e)
        {
            scene[1] = gen_line();
            pictureBox1.Invalidate();
        }

        private Figure parse_figure(string filename)
        {
            Figure res = new Figure();
            List<string> lines = System.IO.File.ReadLines(filename).ToList();
            int count_points = Int32.Parse(lines[0]);
            Dictionary<string, int> pnts = new Dictionary<string, int>();

            for (int i = 0; i < count_points; ++i)
            {
                string[] str = lines[i + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                res.points.Add(new Point3D(float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3])));
                pnts.Add(str[0], i);
            }

            int count_sides = Int32.Parse(lines[count_points + 1]);
            for(int i = count_points+2; i < lines.Count(); ++i)
            {
                Side s = new Side(res);
                List<string> str = lines[i + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach(var id in str)
                   s.points.Add(pnts[id]);
                res.sides.Add(s); 
            }

            return res;
        }

        private void save_figure(Figure fig, string filename)
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
        public List<int> points = new List<int>();
        public Pen drawing_pen = new Pen(Color.Black);

        public Side(Figure h = null) {
            host = h;
        }
        public Side(Side s) {
            points = new List<int>(s.points);
            host = s.host;
            drawing_pen = s.drawing_pen.Clone() as Pen;
        }
        public Point3D get_point(int ind) {
            if (host != null)
                return host.points[points[ind]];
            return null;
        }


    }

    public class Edge
    {
        public Figure host = null;
        public int ind_p1, ind_p2;
        public Color clr = Color.Black;

        public Edge(int i1, int i2, Figure h = null) {
            ind_p1 = i1;
            ind_p2 = i2;
            host = h;
        }

        public Edge(Edge e) {
            ind_p1 = e.ind_p1;
            ind_p2 = e.ind_p2;
            host = e.host;
            clr = e.clr; 
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


        ///
        /// ----------------------------- TRANSFORMS METHODS --------------------------------
        ///


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
                points[i].x = matrix[i, 0]/matrix[i,3];
                points[i].y = matrix[i, 1] / matrix[i, 3];
                points[i].z = matrix[i, 2] / matrix[i, 3];
                
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


        ///
        /// ----------------------------- APHINE METHODS --------------------------------
        ///

        public void rotate_around(float angle,string type) {
            float[,] mt = get_matrix();
            Point3D center = get_center();
            switch (type)
            {
                case "CX":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_X(mt, angle * (float)Math.PI / 180);
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "CY":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_Y(mt, angle * (float)Math.PI / 180);
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "CZ":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_Z(mt, angle * (float)Math.PI / 180);
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "X":
                    mt = apply_rotation_X(mt, angle * (float)Math.PI / 180);                    
                    break;
                case "Y":
                    mt = apply_rotation_Y(mt, angle * (float)Math.PI / 180);
                    break;
                case "Z":
                    mt = apply_rotation_Z(mt, angle * (float)Math.PI / 180);
                    break;                 
                default:
                    break;
            }
            apply_matrix(mt);
        }
        public void scale_axis(float xs,float ys, float zs) {
            float[,] pnts = get_matrix();
            pnts = apply_scale(pnts, xs, ys, zs);
            apply_matrix(pnts);
        }
        public void offset(float xs, float ys, float zs) {
            apply_matrix(apply_offset(get_matrix(),xs,ys,zs));
        }
        public void scale_around_center(float xs, float ys, float zs) {
            float[,] pnts = get_matrix();
            Point3D p = get_center();
            pnts = apply_offset(pnts, -p.x, -p.y, -p.z);
            pnts = apply_scale(pnts, xs, ys, zs);
            pnts = apply_offset(pnts, p.x, p.y, p.z);
            apply_matrix(pnts);
        }
        public void line_rotate(float ang, Point3D p1, Point3D p2) {
            ang = ang * (float)Math.PI / 180;
            p2 = new Point3D(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
            p2 = Point3D.norm(p2);

            float[,] mt = get_matrix();
            apply_matrix(rotate_around_line(mt, p1, p2, ang));
        }

        ///
        /// ----------------------------- PROJECTIONS METHODS --------------------------------
        ///

        public void project_orthogX() {
            apply_matrix(orthographic_projection_X(get_matrix()));
        }
        public void project_orthogY()
        {
            apply_matrix(orthographic_projection_Y(get_matrix()));
        }
        public void project_orthogZ()
        {
            apply_matrix(orthographic_projection_Z(get_matrix()));
        }
        public void project_isometric()
        {
            apply_matrix(isometric_projection(get_matrix()));
        }
        public void project_cental()
        {
            apply_matrix(perspective_projection(get_matrix()));
        }


        ///
        /// ----------------------------- STATIC BACKEND FOR TRANSFROMS --------------------------------
        ///

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
            return apply_offset(multiply_matrix(apply_offset(transform_matrix, -start.x, -start.y, -start.z), rotateMatrix), start.x, start.y, start.z);
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
        private static float[,] perspective_projection(float[,] transform_matrix)
        {
            float center = 200;
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, -1 / center }, { 0, 0, 0, 1 } };
            float[,] res_mt = multiply_matrix(transform_matrix, projMatrix);
            return res_mt;
        }
        private static float[,] orthographic_projection_X(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 0, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            float[,] res_mt = multiply_matrix(transform_matrix, projMatrix);
            for (int i = 0; i < res_mt.GetLength(0); ++i)
            {
                res_mt[i, 0] = res_mt[i, 2];
                res_mt[i, 2] = 0;
            }
            return res_mt;
        }
        private static float[,] orthographic_projection_Y(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            float[,] res_mt = multiply_matrix(transform_matrix, projMatrix);
            for (int i = 0; i < res_mt.GetLength(0); ++i)
            {
                res_mt[i, 1] = res_mt[i, 2];
                res_mt[i, 2] = 0;
            }
            return res_mt;
        }
        private static float[,] orthographic_projection_Z(float[,] transform_matrix)
        {
            float[,] projMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 } };
            return multiply_matrix(transform_matrix, projMatrix);
        }
        private static float[,] isometric_projection(float[,] transform_matrix)
        {
            float a = (float)Math.Asin(Math.Tan(30 * Math.PI / 180));
            float b = 45 * (float)Math.PI / 180;
            float[,] transposeRotationMatrixY = new float[,] { { (float)Math.Cos(b), 0, (float)Math.Sin(b), 0 }, { 0, 1, 0, 0 }, { -(float)Math.Sin(b), 0, (float)Math.Cos(b), 0 }, { 0, 0, 0, 1 } };
            float[,] transposeRotationMatrixX = new float[,] { { 1, 0, 0, 0 }, { 0, (float)Math.Cos(a), -(float)Math.Sin(a), 0 }, { 0, (float)Math.Sin(a), (float)Math.Cos(a), 0 }, { 0, 0, 0, 1 } };
            float[,] ortMatrix = new float[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 1 } };

            float[,] mt1 = multiply_matrix(transform_matrix, transposeRotationMatrixY);
            float[,] mt2 = multiply_matrix(mt1, transposeRotationMatrixX);
            return multiply_matrix(mt2, ortMatrix);
        }

        ///
        /// ---------------------------------------------------------------------------------------
        ///

        ///
        /// ------------------------STATIC READY FIGURES-----------------------------
        ///

        static public Figure get_Hexahedron(float sz)
        {
            Figure res = new Figure();
            res.points.Add(new Point3D(sz / 2, sz / 2, sz / 2)); // 0 
            res.points.Add(new Point3D(-sz / 2, sz / 2, sz / 2)); // 1
            res.points.Add(new Point3D(-sz / 2, sz / 2, -sz / 2)); // 2
            res.points.Add(new Point3D(sz / 2, sz / 2, -sz / 2)); //3

            res.points.Add(new Point3D(sz / 2, -sz / 2, sz / 2)); // 4
            res.points.Add(new Point3D(-sz / 2, -sz / 2, sz / 2)); //5
            res.points.Add(new Point3D(-sz / 2, -sz / 2, -sz / 2)); // 6
            res.points.Add(new Point3D(sz / 2, -sz / 2, -sz / 2)); // 7
   
            

            Side s = new Side(res);
            s.points.AddRange(new int[] { 0, 1 , 2, 3});
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 4, 5, 6, 7});
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] {1,5,6,2});
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0,4,7,3});
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0,4,5,1});
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 2,3,7,6});
            res.sides.Add(s);

            return res;
        }

        static public Figure get_Coordinates() {
            Figure res = new Figure();
            res.points.Add(new Point3D(0,0,0));

            res.points.Add(new Point3D(0, 100, 0));
            res.points.Add(new Point3D(100, 0, 0));
            res.points.Add(new Point3D(0, 0, 100));

            res.sides.Add(new Side(res));
            res.sides.Last().points = new List<int> { 0, 1 };
            res.sides.Last().drawing_pen.Color = Color.Green;
            res.sides.Add(new Side(res));
            res.sides.Last().points = new List<int> { 0, 2 };
            res.sides.Last().drawing_pen.Color = Color.Red;
            res.sides.Add(new Side(res));
            res.sides.Last().points = new List<int> { 0, 3 };
            res.sides.Last().drawing_pen.Color = Color.Blue;

            return res;
        }


       static public Figure get_Octahedron(float sz)
        {
            Figure res = new Figure();
            res.points.Add(new Point3D(sz / 2,0,0)); //0
            res.points.Add(new Point3D(-sz / 2, 0, 0)); //1
            res.points.Add(new Point3D(0, sz / 2, 0)); //2
            res.points.Add(new Point3D(0, -sz / 2, 0));//3
            res.points.Add(new Point3D(0, 0, sz / 2));//4
            res.points.Add(new Point3D(0, 0, -sz / 2));//5

            res.edges.Add(new Edge(0, 2, res));
            res.edges.Add(new Edge(0, 3, res));
            res.edges.Add(new Edge(0, 4, res));
            res.edges.Add(new Edge(0, 5, res));


            res.edges.Add(new Edge(1, 2, res));
            res.edges.Add(new Edge(1, 3, res));
            res.edges.Add(new Edge(1, 4, res));
            res.edges.Add(new Edge(1, 5, res));


            res.edges.Add(new Edge(2, 4, res));
            res.edges.Add(new Edge(4, 3, res));
            res.edges.Add(new Edge(3, 5, res));
            res.edges.Add(new Edge(5, 2, res));
            return res;
        }

        static public Figure get_Tetrahedron(float sz)
        {
            Figure res = new Figure();
            sz = sz / 2;
            res.points.Add(new Point3D(sz, sz, sz));
            res.points.Add(new Point3D(-sz, -sz, sz));
            res.points.Add(new Point3D(sz, -sz, -sz));
            res.points.Add(new Point3D(-sz, sz, -sz));
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 0, 1, 2 });
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 1, 2,3 });
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 0, 2, 3 });
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 0, 1, 3 });
            return res;
        }

        static public Figure get_Icosahedron(float sz) {
            Figure res = new Figure();
            float ang = 36 * (float)Math.PI / 180;

            bool is_upper = true;
            int ind = 0;
            for (float a = 0; a <= (float) 2*Math.PI; a += ang) {
                res.points.Add(new Point3D((float)Math.Cos((float)a), (float)Math.Sin((float)a), is_upper ? (float)0.5 : (float)-0.5));
                if(ind > 0)
                    res.edges.Add(new Edge(ind, ind - 1, res));
                is_upper = !is_upper;
                ind++;
            }
            res.edges.Add(new Edge(ind-1, 0, res));
            res.points.Add(new Point3D(0, 0, (float)Math.Sqrt(5) / 2)); // ind
            res.points.Add(new Point3D(0, 0, -(float)Math.Sqrt(5) / 2)); // ind+1
            for(int i = 0; i< ind; i++)
            {
                var next = i + 2;
                if (next < ind)
                    res.edges.Add(new Edge(i, next, res));
                else
                    res.edges.Add(new Edge(i, next % ind, res));
            }

            for (int i = 0; i < ind; i++)
            {
                if (i % 2 == 0)
                    res.edges.Add(new Edge(i, ind, res));
                else
                    res.edges.Add(new Edge(i, ind+1, res));
            }

            res.scale_around_center(sz, sz, sz);

            return res;
        }

        ///
        /// ---------------------------------------------------------------------------------------
        ///

    }






}
