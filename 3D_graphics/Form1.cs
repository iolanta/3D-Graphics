using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _3D_graphics
{
    public partial class Form1 : Form
    {

        public List<Figure> scene = new List<Figure>();
        public Lighting light = new Lighting(new Point3D(0,0,1000), Color.White,Color.FromArgb(40, 40, 40));
        public OrbitCamera OrbitCam = new OrbitCamera(200, 0, (float)Math.PI/2, 0, new Point3D(0, 0, 0), (float)(65 * Math.PI / 180), (float)(65 * Math.PI / 180), 100, 300);
        private System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        public Color[,] texture;
        
       

        public Form1()
        {
            watch.Stop();

            InitializeComponent();
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                graph.FillRectangle(Brushes.White, ImageSize);
            }

            pictureBox1.Image = bmp;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            ControlType.SelectedIndex = 0;
            curveType.SelectedIndex = 0;
            texture = new Color[1, 1] { { Color.White } };



        }
        
        /// <summary>
        ///  Invokes rerender of scene to  PictureBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            watch.Restart();
            e.Graphics.TranslateTransform(0, pictureBox1.Height);
            e.Graphics.ScaleTransform(1, -1);
            e.Graphics.DrawImageUnscaled(OrbitCam.CameraRender(pictureBox1, scene,light, texture),new Point(0,0));
            watch.Stop();
            debuglabel.Text = String.Format("Camerea pos:\n AngleX:{0}\n AngleY:{1}\n Tilt:{2}\n Distance:{3} ", OrbitCam.AngleX * 180 / (float)Math.PI, OrbitCam.AngleY * 180 / (float)Math.PI, OrbitCam.AngleTilt * 180 / (float)Math.PI, OrbitCam.Distance);
            int time = (int)(1000 / watch.ElapsedMilliseconds);
            FPSlabel.Text = "FPS: " + time;
            
        }

      


        private void render() {
           
            
            pictureBox1.Invalidate();
           
        }


        /// ---- Aphine Transforms handlers -----------
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

        /// ------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
           render();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            apply_transforms();
            groupBox1.Enabled = true;
           render();
        }

        /// <summary>
        ///  view change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                default:
                case "Perspective":
                    OrbitCam.IsOrthogonal = false;
                    break;
                case "Orthogonal":
                    OrbitCam.IsOrthogonal = true;
                    break;
            }
           render();
        }



        // 
        /// <summary>
        ///  figure change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            scene.Clear();
            scene.Add(Figure.get_Coordinates());
            scene.Add(gen_line());
            groupBox2.Enabled = false;
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
                    scene.Add(Figure.get_Icosahedron(50));
                    break;
                case "Torus":
                    scene.Add(Figure.get_Torus(50));
                    break;
                case "Custom":
                    scene.Add(new Figure());
                    break;
                case "Curve":
                    scene.Add(curve());
                    groupBox2.Enabled = true;
                    break;
                default:
                    break;
              
            }
            
            reset_controls();
            render();
        }

        /// <summary>
        ///  Generates new Curve based on controls data
        /// </summary>
        /// <returns></returns>
        private Figure curve() {
            Func<float, float, float> f;
            switch (curveType.Text)
            {
                case "f(x, y) = 5 * cos(x * x + y * y + 1) / (x * x + y * y + 1) + 0.1":
                    f = (float x, float y) => (float)(5 * (float)Math.Cos(x * x + y * y + 1) / (x * x + y * y + 1) + 0.1);
                    break;
                case "f(x, y) = cos(x * x + y * y) / (x * x + y * y + 1)":
                    f = (float x, float y) => (float)(Math.Cos(x * x + y * y) / (x * x + y * y + 1));
                    break;
                case "f(x, y) = sin(x) * cos(y)":
                    f = (float x, float y) => (float)(Math.Sin(x) * Math.Cos(y));
                    break;
                case "f(x, y) = sin(x) + cos(y)":
                    f = (float x, float y) => (float)(Math.Sin(x) + Math.Cos(y));
                    break;
                default:
                case "f(x, y) = x * x + y * y*":
                    f = (float x, float y) => x * x + y * y;
                    break;
            }

            float x0 = (float)curveX0.Value;
            float x1 = (float)curveX1.Value;
            float y0 = (float)curveY0.Value;
            float y1 = (float)curveY1.Value;

            if (x0 == x1 || y0 == y1)
                return new Figure();

            if(x1 < x0)
            {
                float t = x1;
                x1 = x0;
                x0 = t;
            }

            if (y1 < y0)
            {
                float t = y1;
                y1 = y0;
                y0 = t;
            }

            int n_x = (int)curveNX.Value;
            int n_y = (int)curveNY.Value;

            return Figure.get_curve(x0, x1, y0, y1, n_x, n_y, f);
        }

        /// <summary>
        /// Reset controls for Aphine transforms
        /// </summary>
        private void reset_controls() {
            ControlOffsetX.Value = 0;
            ControlOffsetY.Value = 0;
            ControlOffsetZ.Value = 0;
            ControlScaleX.Value = 10;
            ControlScaleY.Value = 10;
            ControlScaleZ.Value = 10;
            ControlAngle.Value = 0;
        }

        private void AphineResetButton_Click(object sender, EventArgs e)
        {
            reset_controls();
        }


        /// <summary>
        ///  Generates line figure, reads params from controls
        /// </summary>
        /// <returns></returns>
        private Figure gen_line() {
            Figure res = new Figure();
            res.points.Add(new Point3D((float)ControlCustom1X.Value, (float)ControlCustom1Y.Value, (float)ControlCustom1Z.Value));
            res.points.Add(new Point3D((float)ControlCustom2X.Value, (float)ControlCustom2Y.Value, (float)ControlCustom2Z.Value));
            res.sides.Add(new Side(res));
            res.sides.First().points =new List<int> {0,1};

            return res;
        }

        /// <summary>
        ///  handles changes of custom line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlCustom1X_ValueChanged(object sender, EventArgs e)
        {
            scene[1] = gen_line();
           render();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 5;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string filename = openFileDialog1.FileName; 
            if (!System.IO.File.Exists(filename))
                return;
            scene[2] = Figure.parse_figure(filename);
           render();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string filename = saveFileDialog1.FileName;
            // Figure.save_figure(scene[2], filename);
            Figure.save_figure_obj(scene[2], filename);
        }

        /// <summary>
        ///  Handles changes in Curve Paramters, loads new curve, locks/unlcoks groupbox, resets aphine transforms controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void curve_params_change(object sender, EventArgs e)
        {
            if (comboBox2.Text != "Curve")
                return;
            groupBox2.Enabled = false;
            scene[2] = curve();
            reset_controls();
           render();
            groupBox2.Enabled = true;
        }

        /// <summary>
        /// Handles keyboard controls for camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // return if camera mode is not on

            // camera movemetns
            switch (e.KeyCode)
            {
                case Keys.A: // move left by 1d
                    OrbitCam.MoveLeftRight((float)(Math.PI / 180));
                    break;
                case Keys.D: // move right by 1d
                    OrbitCam.MoveLeftRight(-(float)(Math.PI / 180));
                    break;
                case Keys.W: //move up by 1d
                    OrbitCam.MoveUpDown((float)(Math.PI / 180));
                    break;
                case Keys.S: // move down by 1d
                    OrbitCam.MoveUpDown(-(float)(Math.PI / 180));
                    break;
                case Keys.Q: // rotate left
                    OrbitCam.TiltLeftRight((float)(Math.PI / 180));
                    break;
                case Keys.E: // rotate right
                    OrbitCam.TiltLeftRight(-(float)(Math.PI / 180));
                    break;
                case Keys.R: //move closer 
                    OrbitCam.MoveFarNear(-1);
    
                    break;
                case Keys.F: // move further
                    OrbitCam.MoveFarNear(1); ;
                    break;
                case Keys.L: // 
                    light.position = OrbitCam.Position;
                    break;
                default:
                    break;


            }
            //e.Handled = true ;
            // e.SuppressKeyPress = true;
            //debuglabel.Text = String.Format("Camerea pos:\n AngleX:{0}\n AngleY:{1}\n Tilt:{2}\n Distance:{3} ",OrbitCam.AngleX*180/(float)Math.PI, OrbitCam.AngleY * 180 / (float)Math.PI, OrbitCam.AngleTilt * 180 / (float)Math.PI, OrbitCam.Distance);
            render();
        }

        private void CamReset_Click(object sender, EventArgs e)
        {
            OrbitCam.Reset();
            debuglabel.Text = String.Format("Camerea pos:\n AngleX:{0}\n AngleY:{1}\n Tilt:{2}\n Distance:{3} ", OrbitCam.AngleX * 180 / (float)Math.PI, OrbitCam.AngleY * 180 / (float)Math.PI, OrbitCam.AngleTilt * 180 / (float)Math.PI, OrbitCam.Distance);
            render();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float fov = (float)((sender as TrackBar).Value * Math.PI / 180);
            OrbitCam.SetFov(fov, fov);
            render();
        }

        private void NearPlaneBar_Scroll(object sender, EventArgs e)
        {
            float val = (float)((sender as TrackBar)).Value;
            OrbitCam.MinDistance = val;
            render();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            float val = (float)((sender as TrackBar)).Value;
            OrbitCam.MaxDistance = val;
            render();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string filename = openFileDialog1.FileName;
            if (!System.IO.File.Exists(filename))
                return;

            texture = CameraView.TextureToColors(filename);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            OrbitCam.istexture = checkBox1.Checked;
            render();
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
            if (p == null)
                return;
            x = p.x;
            y = p.y;
            z = p.z;
        }

        public override string ToString()
        {
            return String.Format("X:{0:f1} Y:{1:f1} Z:{2:f1}", x,y,z);
        }
        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);

        }

        public static float scalar(Point3D p1, Point3D p2) {
            return p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
        }

        public static Point3D norm(Point3D p)
        {
            float z = (float)Math.Sqrt((float)(p.x * p.x + p.y * p.y + p.z * p.z));
            return new Point3D(p.x / z, p.y / z, p.z / z);
        }

        public static Point3D operator*(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.y*p2.z - p1.z*p2.y, p1.z*p2.x-p1.x*p2.z, p1.x*p2.y - p1.y*p2.x);
        }



    }


    public class Side {
        public Figure host = null;
        public List<int> points = new List<int>();
        public Pen drawing_pen = new Pen(Color.Black);
        public Point3D Normal;
        public bool IsVisible = false;

        public Side(Figure h = null) {
            host = h;
        }
        public Side(Side s) {
            points = new List<int>(s.points);
            host = s.host;
            drawing_pen = s.drawing_pen.Clone() as Pen;
            Normal = new Point3D(s.Normal);
            IsVisible = s.IsVisible;
        }
        public Point3D get_point(int ind) {
            if (host != null)
                return host.points[points[ind]];
            return null;
        }

        public static Point3D norm(Side S) {
            if (S.points.Count() < 3)
                return new Point3D(0,0,0);
            Point3D U = S.get_point(1) - S.get_point(0);
            Point3D V = S.get_point(S.points.Count-1) - S.get_point(0);
            Point3D normal = V * U;
            return Point3D.norm(normal);
        }

        public void CalculateSideNormal() {
            Normal = norm(this);
        }

        public void CalculateVisibilty(Point3D cntr) {
            if (Normal == null)
                IsVisible = true;
            else 
                IsVisible =  Point3D.scalar(cntr - get_point(0), Normal) < 0;

        }


    }

 


    public class Figure
    {

        public List<Point3D> points = new List<Point3D>(); // точки 
        public List<Side> sides = new List<Side>(); // стороны
        public float[] lighting;
        private Point3D[] point_normals;
        public Figure() {
        }

        // redo for new members
        public Figure(Figure f) {
            foreach (Point3D p in f.points) {
                points.Add(new Point3D(p));
            }
            foreach (Side s in f.sides)
            {
                sides.Add(new Side(s));
                sides.Last().host = this;
            }
            if (f.lighting != null)
            lighting = f.lighting.ToArray();
        }




        /// <summary>
        ///  Calculate visibility of each side and lighting intensifyer of every visible vertex
        /// </summary>
        /// <param name="eye_pos"> Postion of Camera</param>
        /// <param name="light_pos">Position of ligthing</param>
        public void CalculateVisibiltyAndLighting(Point3D eye_pos, Point3D light_pos) {
            lighting = new float[points.Count];
            List<Side>[] point_sides = new List<Side>[points.Count];
            bool[] point_visible = new bool[points.Count];
            point_normals = new Point3D[points.Count];
            foreach (Side s in sides)
            {
                s.CalculateSideNormal();
                s.CalculateVisibilty(eye_pos);
                foreach (int ind in s.points)
                    if (point_sides[ind] == null)
                        point_sides[ind] = new List<Side>() {s };
                    else
                        point_sides[ind].Add(s);
                
            }

            for (int i = 0; i < points.Count; i++)
            {
                point_visible[i] = point_sides[i].Any(s => s.IsVisible);
                if (point_visible[i])
                {
                    Point3D t = point_sides[i].Aggregate(new Point3D(0, 0, 0), (Point3D n, Side s) => { n.x += s.Normal.x; n.y += s.Normal.y; n.z += s.Normal.z; return n; });
                    t.x /= point_sides[i].Count;
                    t.y /= point_sides[i].Count;
                    t.z /= point_sides[i].Count;
                    point_normals[i] = t;

                    lighting[i] = (Point3D.scalar(t, Point3D.norm(  points[i]-light_pos)));
                    if (lighting[i] <= 0)
                        lighting[i] = 0;


                }
                

            }

            

        }

        private void CalculateVertexNormals()
        {
            point_normals = new Point3D[points.Count];
            int[] count_sidesPerPoint = new int[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                point_normals[i] = new Point3D(0, 0, 0);
                count_sidesPerPoint[i] = 0;

            }

            foreach(Side s in sides)
            {
                s.CalculateSideNormal();
                // sorry in this project normals are calculated inverted pls fix
                Point3D Normal = new Point3D(s.Normal.x * -1, s.Normal.y * -1, s.Normal.z * -1); 
                for (int i = 0; i < s.points.Count; i++)
                {
                    int ind = s.points[i];
                    point_normals[ind].x += Normal.x;
                    point_normals[ind].y += Normal.y;
                    point_normals[ind].z += Normal.z;
                    count_sidesPerPoint[ind] += 1;

                }


            }
            for (int i = 0; i < points.Count;  i++)
            {
                if(count_sidesPerPoint[i] != 0)
                {
                    point_normals[i].x /= count_sidesPerPoint[i];
                    point_normals[i].y /= count_sidesPerPoint[i];
                    point_normals[i].z /= count_sidesPerPoint[i];

                }
            }


        }


        ///
        /// ----------------------------- TRANSFORMS  SUPPORT METHODS --------------------------------
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
                points[i].x = matrix[i, 0] / matrix[i, 3];
                points[i].y = matrix[i, 1] / matrix[i, 3];
                points[i].z = matrix[i, 2] / matrix[i, 3];

            }
        }
        private Point3D get_center() {
            Point3D res = new Point3D(0, 0, 0);
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
        /// ----------------------------- APHINE TRANSFORMS METHODS --------------------------------
        ///

        public void rotate_around_rad(float rangle, string type) {
            float[,] mt = get_matrix();
            Point3D center = get_center();
            switch (type)
            {
                case "CX":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_X(mt, rangle);
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "CY":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_Y(mt, rangle );
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "CZ":
                    mt = apply_offset(mt, -center.x, -center.y, -center.z);
                    mt = apply_rotation_Z(mt, rangle );
                    mt = apply_offset(mt, center.x, center.y, center.z);
                    break;
                case "X":
                    mt = apply_rotation_X(mt, rangle );
                    break;
                case "Y":
                    mt = apply_rotation_Y(mt, rangle );
                    break;
                case "Z":
                    mt = apply_rotation_Z(mt, rangle);
                    break;
                default:
                    break;
            }
            apply_matrix(mt);
        }
        public void rotate_around(float angle, string type)
        {
            rotate_around_rad(angle * (float)Math.PI / 180, type);
        }
        public void scale_axis(float xs, float ys, float zs) {
            float[,] pnts = get_matrix();
            pnts = apply_scale(pnts, xs, ys, zs);
            apply_matrix(pnts);
        }
        public void offset(float xs, float ys, float zs) {
            apply_matrix(apply_offset(get_matrix(), xs, ys, zs));
        }

        public void set_pen(Pen dw)
        {
            foreach (Side s in sides)
                s.drawing_pen = dw;

        }
        public void set_rand_color() {
            Random r = new Random();
            foreach (Side s in sides) {
                
                Color c = Color.FromArgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
                s.drawing_pen = new Pen(c);
            }
                
        }


        public void scale_around_center(float xs, float ys, float zs) {
            float[,] pnts = get_matrix();
            Point3D p = get_center();
            pnts = apply_offset(pnts, -p.x, -p.y, -p.z);
            pnts = apply_scale(pnts, xs, ys, zs);
            pnts = apply_offset(pnts, p.x, p.y, p.z);
            apply_matrix(pnts);
        }
        public void line_rotate_rad(float rang, Point3D p1, Point3D p2)
        {
            
            p2 = new Point3D(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
            p2 = Point3D.norm(p2);

            float[,] mt = get_matrix();
            apply_matrix(rotate_around_line(mt, p1, p2, rang));
        }
        /// <summary>
        /// rotate figure line
        /// </summary>
        /// <param name="ang">angle in degrees</param>
        /// <param name="p1">line start</param>
        /// <param name="p2">line end</param>
        public void line_rotate(float ang, Point3D p1, Point3D p2) {
            ang = ang * (float)Math.PI / 180;
            line_rotate_rad(ang, p1, p2);
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

                res_mt[i, 0] = res_mt[i, 1];
                res_mt[i, 1] = res_mt[i, 2];
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
        /// --------------------SAVE/LOAD METHODS------------------------------------------
        ///

        public static Figure parse_figure(string filename)
        {
            Figure res = new Figure();
            List<string> lines = System.IO.File.ReadLines(filename).ToList();
            var st = lines[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (st[0] == "rotation")
                return parse_rotation(lines);
            else
            {
                int count_points = Int32.Parse(st[0]);
                Dictionary<string, int> pnts = new Dictionary<string, int>();

                for (int i = 0; i < count_points; ++i)
                {
                    string[] str = lines[i + 1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    res.points.Add(new Point3D(float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3])));
                    pnts.Add(str[0], i);
                }

                int count_sides = Int32.Parse(lines[count_points + 1]);
                for (int i = count_points + 2; i < lines.Count(); ++i)
                {
                    Side s = new Side(res);
                    List<string> str = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var id in str)
                        s.points.Add(pnts[id]);
                    res.sides.Add(s);
                }

                res.set_pen(new Pen(Color.Red));
                return res;
            }
        }

        public static Figure parse_rotation(List<string> lines)
        {
            
            string[] cnt = lines[1].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int count_points = Int32.Parse(cnt[0]);
            int count_divs = Int32.Parse(cnt[1]);

            if (count_points < 1 || count_divs < 1)
                return new Figure();

            List<Point3D> pnts = new List<Point3D>();
            for (int i = 2; i < count_points + 2; ++i)
            {
                string[] s = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                pnts.Add(new Point3D(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3])));
            }

            string[] str = lines[count_points + 2].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            Point3D axis1 = new Point3D(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));
            str = lines[count_points + 3].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            Point3D axis2 = new Point3D(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));

            return get_Rotation( pnts, axis1,  axis2, count_divs);
        }

        public static void save_figure(Figure fig, string filename)
        {
            List<string> lines = new List<string>();
            Dictionary<int, string> pnts = new Dictionary<int, string>();
            lines.Add(fig.points.Count().ToString());
            for (int i = 0; i < fig.points.Count(); ++i)
            {
                string ind = "p" + i.ToString();
                pnts.Add(i, ind);
                lines.Add(ind + ' ' + fig.points[i].x.ToString() + ' ' + fig.points[i].y.ToString() + ' ' + fig.points[i].z.ToString());
            }
            lines.Add(fig.sides.Count().ToString());
            for (int i = 0; i < fig.sides.Count(); ++i)
            {
                string side_points = "";
                foreach (int s in fig.sides[i].points)
                {
                    side_points += pnts[s] + ' ';
                }
                lines.Add(side_points);
            }
            System.IO.File.WriteAllLines(filename, lines);
        }

        public static void save_figure_obj(Figure fig, string filename)
        {
            List<string> lines = new List<string>();
            List<string> normals = new List<string>();
            List<string> f = new List<string>();

            Dictionary<int, string> pnts = new Dictionary<int, string>();
            fig.CalculateVertexNormals();

            for (int i = 0; i < fig.points.Count(); ++i)
            {
                string ind = "p" + i.ToString();
                pnts.Add(i, ind);
                lines.Add("v " + fig.points[i].x.ToString() + ' ' + fig.points[i].y.ToString() + ' ' + fig.points[i].z.ToString());
                normals.Add("vn " + fig.point_normals[i].x.ToString() + ' ' + fig.point_normals[i].y.ToString() + ' ' + fig.point_normals[i].z.ToString());
            }

            foreach (Side s in fig.sides)
            {
                string str = "f";
                foreach (int p in s.points)
                    str += " " + (p + 1).ToString() + "//" + (p + 1).ToString() + " ";

                f.Add(str);
            }

            lines.AddRange(normals);
            lines.AddRange(f);
            lines.Concat(f);

            System.IO.File.WriteAllLines(filename, lines);
        }

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
            s.points.AddRange(new int[] { 3 , 2, 1 , 0});
            s.drawing_pen = new Pen(Color.Aquamarine); 
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 4, 5, 6, 7});
            s.drawing_pen = new Pen(Color.Green);
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] {2,6,5,1 });
            s.drawing_pen = new Pen(Color.Red);
            res.sides.Add(s);

            s = new Side(res);
            s.drawing_pen = new Pen(Color.Yellow);
            s.points.AddRange(new int[] { 0,4,7,3});
            res.sides.Add(s);

            s = new Side(res);
            s.drawing_pen = new Pen(Color.Blue);
            s.points.AddRange(new int[] { 1,5,4,0}); 
            
            res.sides.Add(s);

            s = new Side(res);
            s.drawing_pen = new Pen(Color.Brown);
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

            Side s = new Side(res);
            s.points.AddRange(new int[] {0,4,3 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0, 2,4 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 1, 4, 2 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 1, 3, 4 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0, 5, 2 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 1, 2, 5 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 0, 3, 5 });
            res.sides.Add(s);

            s = new Side(res);
            s.points.AddRange(new int[] { 1, 5, 3 });
            res.sides.Add(s);

            res.set_rand_color();

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
            res.sides.Last().points.AddRange(new List<int> { 1,3,2 });
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 0, 2, 3 });
            res.sides.Add(new Side(res));
            res.sides.Last().points.AddRange(new List<int> { 0, 3, 1 });
            res.set_rand_color();
            return res;
        }

        static public Figure get_Torus(float sz, int d = 100) {

            sz /= 2;
            List<Point3D> crcl = new List<Point3D>(); 
            float ang = 0;
            float a =(float)( 2*Math.PI/d); 
            for (int i = 0; i <= d; ++i) {
                crcl.Add(new Point3D((float)Math.Cos(ang)* sz, 0, (float)Math.Sin(ang) *sz));
                ang += a;
            }
            
            Figure res = get_Rotation(crcl, new Point3D(-(float)(sz * 2.5), 0, 0), new Point3D(-(float)(sz * 2.5), 0, 1), d);
            res.offset((float)(sz*2.5), 0, 0);
            res.set_pen(new Pen(Color.Tomato));
            return res;
        }


        static public Figure get_Icosahedron(float sz) {
            Figure res = new Figure();
            float ang = (float)(Math.PI / 5);

            bool is_upper = true;
            int ind = 0;
            float a = 0;
            for (int i = 0; i<10;++i ) {
                res.points.Add(new Point3D((float)Math.Cos((float)a), (float)Math.Sin((float)a), is_upper ? (float)0.5 : (float)-0.5));
                is_upper = !is_upper;
                ind++;
                a += ang;
            }
            Side s;
            for (int i = 0; i < ind; i++)
            {
                s = new Side(res);
                if (i % 2 == 0)
                {
                    s.points.AddRange(new int[] { i, (i + 1) % ind, (i + 2) % ind });
                  //  s.drawing_pen = new Pen(Color.Green);
                }
                else
                {
                    s.points.AddRange(new int[] { (i + 2) % ind, (i + 1) % ind, i });
                 //   s.drawing_pen = new Pen(Color.Red);
                }
            
                res.sides.Add(s);
            }



            
            res.points.Add(new Point3D(0, 0, (float)Math.Sqrt(5) / 2)); // ind
            res.points.Add(new Point3D(0, 0, -(float)Math.Sqrt(5) / 2)); // ind+1
            for(int i = 0; i< ind; i+=2)
            {
                s = new Side(res);
                s.points.AddRange(new int[] { i, ind, (i + 2) % ind });
                s.points.Reverse();
                
                res.sides.Add(s);
            }

            for (int i = 1; i < ind; i += 2)
            {
                s = new Side(res);
                s.points.AddRange(new int[] { i, (i + 2) % ind , ind +1 });
                s.points.Reverse();
                res.sides.Add(s);
            }

            res.scale_around_center(sz, sz, sz);

            res.set_rand_color();
            return res;
        }

        public static Figure get_curve(float x0, float x1, float y0, float y1, int n_x, int n_y, Func<float, float, float> f)
        {
            float step_x = (x1 - x0) / n_x;
            float step_y = (y1 - y0) / n_y;
            Figure res = new Figure();

            float x = x0;
            float y = y0;

            for (int i = 0; i <= n_x; ++i)
            {
                y = y0;
                for (int j = 0; j <= n_y; ++j)
                {
                    res.points.Add(new Point3D(x, y, f(x, y)));
                    y += step_y;
                }
                x += step_x;
            }

            for(int i = 0; i < res.points.Count; ++i)
            {
                if ((i + 1) % (n_y + 1) == 0)
                    continue;
                if (i / (n_y + 1) == n_x)
                    break;

                Side s = new Side(res);
                s.points.AddRange(new int[] { i, i + 1, i + n_y + 2, i + n_y + 1 });
                s.points.Reverse();
                res.sides.Add(s);
            }
            res.set_rand_color();
            return res;
        }

       
        public static Figure get_Rotation(List<Point3D> pnts, Point3D axis1, Point3D axis2, int divs) {
            Figure res = new Figure();
            Figure edge = new Figure();
            int cnt_pnt = pnts.Count;
            edge.points = pnts.Select(x => new Point3D(x)).ToList();
            res.points = pnts.Select(x => new Point3D(x)).ToList();
            int cur_ind = res.points.Count;
            float ang = (float)360/divs;
            for (int i = 0; i < divs; i++) {
                edge.line_rotate(ang, axis1, axis2);
                cur_ind = res.points.Count;
                for (int j = 0; j<cnt_pnt; j++)
                {
                    res.points.Add(new Point3D(edge.points[j]));

                }

                for (int j = cur_ind; j < res.points.Count-1; j++)
                {
                    Side s = new Side(res);
                    s.points.AddRange(new int[] { j, j + 1, j + 1 - cnt_pnt, j-cnt_pnt});
                    res.sides.Add(s);

                }


            }



      
            res.set_pen(new Pen(Color.Magenta));
                return res;
        }
        ///
        /// ---------------------------------------------------------------------------------------
        ///

    }


    public class CameraView
    {
        private Point3D view_target;
        private Point3D eye_postion;
        private Point3D up;
        private float fovx;
        private float fovy;
        private float max_distance;
        private float min_distance;
        private float cam_width;
        private float cam_height;
        private float[,] view_matrix;
        private float[,] perspective_projection_matrix;
        private float[,] orthoganal_projection_matrix;
        private float[,] complete_matrix_perspective;
        private float[,] complete_matrix_orthoganal;
        private bool isorthg = false;
        public bool istexture = false;

        /// <summary>
        /// Basic Camera object
        /// </summary>
        /// <param name="p">Postion of Camera</param>
        /// <param name="t">Target to look at</param>
        /// <param name="u">Vector to be vertical in camera</param>
        /// <param name="fvx">FOVx in radians</param>
        /// <param name="fvy">FOVy in radians</param>
        /// <param name="mind">distance to near plane</param>
        /// <param name="maxd">distacne to far plane</param>
        public CameraView(Point3D p, Point3D t, Point3D u, float fvx, float fvy, float mind, float maxd)
        {
            view_target = new Point3D(t);
            eye_postion = new Point3D(p);
            up = new Point3D(u);
            fovx = fvx;
            fovy = fvy;
            max_distance = maxd;
            min_distance = mind;
            cam_width = 100;
            cam_height = 100;
            update_view_matrix();
            update_proj_matrix();
            update_full_matrix();
        }


        /// <summary>
        ///  Set all params at once and recount matrixes only ones
        /// </summary>
        /// <param name="p"></param>
        /// <param name="t"></param>
        /// <param name="u"></param>
        /// <param name="fvx"></param>
        /// <param name="fvy"></param>
        /// <param name="mind"></param>
        /// <param name="maxd"></param>
        protected void set_params_at_once(Point3D p, Point3D t, Point3D u, float fvx, float fvy, float mind, float maxd)
        {
            view_target = new Point3D(t);
            eye_postion = new Point3D(p);
            up = new Point3D(u);
            fovx = fvx;
            fovy = fvy;
            max_distance = maxd;
            min_distance = mind;
            cam_width = 100;
            cam_height = 100;
            update_view_matrix();
            update_proj_matrix();
            update_full_matrix();
        }


        /// <summary>
        /// Invokes render
        /// </summary>
        /// <param name="g">Graphics objects from Paint event</param>
        /// <param name="rend_obj"> PictureBox to rednder to</param>
        /// <param name="scene"> list of objects to render, is copied</param>
        /// <param name="light"> Light position and color </param>
        public Bitmap CameraRender(PictureBox rend_obj, List<Figure> scene,Lighting light, Color[,] tex)
        {
            Point3D lightamb = new Point3D(light.ambient_color.R / 255.0f, light.ambient_color.G / 255.0f, light.ambient_color.B / 255.0f);
            Point3D lightpos = light.position;



            Point3D calculate_color(float val, Point3D obj_c, Point3D lig_c,Point3D amb) {
                return new Point3D(Math.Min( Math.Max(val , amb.x) * obj_c.x * lig_c.x, 1),
                                  Math.Min(Math.Max(val, amb.y) * obj_c.y * lig_c.y, 1),
                                   Math.Min(Math.Max(val, amb.z) * obj_c.z * lig_c.z, 1));
            }
            point3 ViewPortTranform(Point3D p,float l)
            {
                return new point3((int)((1 + p.x) * rend_obj.Width / 2), 
                                  (int)((1 + p.y) * rend_obj.Height / 2),
                                  (int)(1 / p.z * 100000000),l);
            }

            List<Figure> view = scene.Select(f => new Figure(f)).ToList();
            int h = rend_obj.Height;
            int w = rend_obj.Width;

            int[,] zbuffer = new int[h, w];
            Color[,] cbuffer = new Color[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    zbuffer[i, j] = 0;
                    cbuffer[i, j] = Color.LightSkyBlue;
                }


            foreach (Figure f in view)
            {

                f.CalculateVisibiltyAndLighting(Position,lightpos);

                if (isorthg)
                {

                   // f.apply_matrix(multiply_matrix(f.get_matrix(), complete_matrix_orthoganal));
                }
                else
                {

                    f.apply_matrix(multiply_matrix(multiply_matrix(f.get_matrix(), view_matrix), perspective_projection_matrix));
                }

                
                foreach (Side s in f.sides.Where(s => s.IsVisible))
                {
                    Color[,] ObjectTexture;


                    if (istexture && tex != null)
                    {
                        ObjectTexture = tex;
                    }
                    else {
                        ObjectTexture = new Color[1, 1] { { s.drawing_pen.Color } };
                    }

                   point3[] pl;
                   switch (s.points.Count)
                    {
                        case 1:
                            point3 p0 = ViewPortTranform(s.get_point(0), f.lighting[s.points[0]]);
                            if (p0.z > zbuffer[p0.y, p0.x])
                            {
                                zbuffer[p0.y, p0.x] = p0.z;
                                cbuffer[p0.y, p0.x] = ObjectTexture[0,0];
                            }
                            break;
                        case 2:
                            pl = s.points.Select(i => ViewPortTranform(s.host.points[i],s.host.lighting[i]) ).OrderBy(p => p.y).ToArray();
                            FillTrinagle(pl[0], pl[1], pl[1], w, h, zbuffer, cbuffer, ObjectTexture, light.color, light.ambient_color);
                            break;

                        case 3:
                            pl = s.points.Select(i => ViewPortTranform(s.host.points[i], s.host.lighting[i])).OrderBy(p => p.y).ToArray();
                            FillTrinagle(pl[0], pl[1], pl[2], w, h, zbuffer, cbuffer, ObjectTexture, light.color, light.ambient_color);
                            break;
                        default:
                        case 4:
                           pl = s.points.Select(i => ViewPortTranform(s.host.points[i], s.host.lighting[i])).ToArray();
                           FillQuad(pl, w, h, zbuffer, cbuffer, ObjectTexture,light.color,light.ambient_color);     
                           break;           
                    }
                   





                }


            }

            Bitmap bmp = new Bitmap(rend_obj.Width,rend_obj.Height);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int strd = Math.Abs(bmpData.Stride);
            int bytes = strd * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int ind = strd * i + j * 4;

                    rgbValues[ind] = cbuffer[i, j].B; //B
                    rgbValues[ind + 1] = cbuffer[i, j].G;//G
                    rgbValues[ind + 2] = cbuffer[i, j].R; // R
                    rgbValues[ind + 3] = cbuffer[i, j].A; //A
                }


            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);


            return bmp;

        }


        /// 
        ///  Camera params setters and getters invoking recounting of matrixes 
        /// 
        private void update_view_matrix()
        {
            Point3D f = Point3D.norm(eye_postion - view_target);
            Point3D s = Point3D.norm(f * up);
            Point3D v = s * f;
            view_matrix = new float[,] { { s.x, v.x, f.x, 0 }, { s.y, v.y, f.y, 0 }, { s.z, v.z, f.z, 0 }, { -Point3D.scalar(s, eye_postion), -Point3D.scalar(v, eye_postion), -Point3D.scalar(f, eye_postion), 1 } };
        }
        private void update_proj_matrix()
        {
            float w = (float)(1 / Math.Tan(fovx / 2));
            float h = (float)(1 / Math.Tan(fovy / 2));
            perspective_projection_matrix = new float[,] { { w, 0, 0, 0 },
                                                           { 0, h, 0, 0 },
                                                           { 0, 0,max_distance / (min_distance-max_distance), -1 },
                                                           { 0, 0,max_distance*min_distance/(min_distance - max_distance), 0 } };
            orthoganal_projection_matrix = new float[,] { { 2/cam_width,0,0,0},
                                                          {0,2/cam_height,0,0},
                                                          {0,0,1 / (min_distance-max_distance),-1 },
                                                          {0,0,min_distance/(min_distance - max_distance),0} };


        }
        private void update_full_matrix()
        {
            complete_matrix_perspective = multiply_matrix(view_matrix, perspective_projection_matrix);
            complete_matrix_orthoganal = multiply_matrix(view_matrix, orthoganal_projection_matrix);

        }
        public Point3D Up
        {
            get { return new Point3D(up); }
            set { up = value; update_view_matrix(); update_full_matrix(); }
        }
        public Point3D Position
        {
            get { return new Point3D(eye_postion); }
            set { eye_postion = value; update_view_matrix(); update_full_matrix(); }
        }
        public Point3D Target
        {
            get { return new Point3D(view_target); }
            set { view_target = value; update_view_matrix(); update_full_matrix(); }
        }
        public float FovX
        {
            get { return fovx; }
            set { fovx = value; update_proj_matrix(); update_full_matrix(); }
        }
        public float FovY
        {
            get { return fovy; }
            set { fovy = value; update_proj_matrix(); update_full_matrix(); }
        }
        public float MaxDistance
        {
            get { return max_distance; }
            set { max_distance = value; update_proj_matrix(); update_full_matrix(); }
        }
        public float MinDistance
        {
            get { return min_distance; }
            set { min_distance = value; update_proj_matrix(); update_full_matrix(); }
        }
        public float CamWidth
        {
            get { return cam_width; }
            set { cam_width = value; update_proj_matrix(); update_full_matrix(); }
        }
        public float CamHeight
        {
            get { return cam_height; }
            set { cam_height = value; update_proj_matrix(); update_full_matrix(); }
        }

        public bool IsOrthogonal
        {
            get { return isorthg; }
            set { isorthg = value; }

        }

        public void SetMimMaxPlane(float mn, float mx)
        {
            min_distance = mn;
            max_distance = mx;
            update_proj_matrix();
            update_full_matrix();
        }

        public void SetFov(float fx, float vy)
        {
            fovx = fx;
            fovy = vy;
            update_proj_matrix();
            update_full_matrix();
        }
        /// 
        /// ----------------------------------------------
        ///

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

        private static int[] Interpolate(int i0, int d0, int i1, int d1)
        {
            if (i0 == i1)
            {
                return new int[] { d0 };
            }
            int[] res;
            float a = (float)(d1 - d0) / (i1 - i0);
            float val = d0;
            res = new int[i1 - i0 + 1];
            d1 = 0;
            for (int i = i0; i <= i1; i++)
            {
                res[d1] = d0;
                val += a;
                d0 = (int)val;
                ++d1;
            }


            return res;

        }

        private static float[] FInterpolate(int i0, float d0, int i1, float d1)
        {
            if (i0 == i1)
            {
                return new float[] { d0 };
            }
            float[] res;
            float a = (d1 - d0) / (i1 - i0);
            float val = d0;
            res = new float[i1 - i0 + 1];
            int ind = 0;
            for (int i = i0; i <= i1; i++)
            {
                res[ind] = val;
                val += a;
                ++ind;
            }

            return res;
        }


        private static void FillQuad(point3[] pl, int w, int h, int[,] zbuffer, Color[,] cbuffer, Color[,] texture, Color light, Color Ambient)
        {
            int cur = 0;
            int[] indexes = pl.Select(p => cur++).OrderBy(_i => pl[_i].y).ToArray();
            List<int> wayCW = new List<int> { indexes.First() };
            List<int> wayCCW = new List<int> { indexes.First() };


            int minx = pl.Min(p => p.x), maxx = pl.Max(p => p.x);
            int ly = Math.Max(pl[indexes.First()].y, 0);
            int i = ly - pl[indexes.First()].y;
            int uy = Math.Min(pl[indexes.Last()].y, h - 1);
            if (ly > uy || Math.Max(0, minx) > Math.Min(w - 1, maxx))
                return;


            float TextureScaleX, TextureScaleY;
            if (maxx != minx)
                TextureScaleX = (float)(texture.GetLength(1) - 1) / (maxx - minx);
            else
                TextureScaleX = 0;

            if (pl[indexes.First()].y != pl[indexes.Last()].y)
                TextureScaleY = (float)(texture.GetLength(0) - 1) / (pl[indexes.Last()].y - pl[indexes.First()].y);
            else
                TextureScaleY = 0;

        
            Point ToTextureCoords(int x,int y) {
                if (x < minx)
                    x = minx;
                return new Point((int)(TextureScaleX * (x - minx)), (int)(TextureScaleY * (y - pl[indexes.First()].y)));
            }

            cur = indexes.First();
            while(cur != indexes.Last())
            {
                cur = (cur + 1) % pl.Count();
                wayCW.Add(cur);
            }

            cur = indexes.First();
            while (cur != indexes.Last())
            {
                cur = (cur + pl.Count() - 1) % pl.Count();
                wayCCW.Add(cur);
            }


            

            int[] xWayCW = Interpolate(pl[wayCW[0]].y, pl[wayCW[0]].x, pl[wayCW[1]].y, pl[wayCW[1]].x);
            int[] hWayCW = Interpolate(pl[wayCW[0]].y, pl[wayCW[0]].z, pl[wayCW[1]].y, pl[wayCW[1]].z);
            float[] fWayCW = FInterpolate(pl[wayCW[0]].y, pl[wayCW[0]].l, pl[wayCW[1]].y, pl[wayCW[1]].l);
            for (int k = 1; k < wayCW.Count-1; k++)
            {

                xWayCW = xWayCW.Take(xWayCW.Count() - 1).Concat(Interpolate(pl[wayCW[k]].y, pl[wayCW[k]].x, pl[wayCW[k+1]].y, pl[wayCW[k+1]].x)).ToArray();
                hWayCW = hWayCW.Take(hWayCW.Count() - 1).Concat(Interpolate(pl[wayCW[k]].y, pl[wayCW[k]].z, pl[wayCW[k + 1]].y, pl[wayCW[k + 1]].z)).ToArray();
                fWayCW = fWayCW.Take(fWayCW.Count() - 1).Concat(FInterpolate(pl[wayCW[k]].y, pl[wayCW[k]].l, pl[wayCW[k + 1]].y, pl[wayCW[k + 1]].l)).ToArray();

            }






            int[] xWayCCW = Interpolate(pl[wayCCW[0]].y, pl[wayCCW[0]].x, pl[wayCCW[1]].y, pl[wayCCW[1]].x);
            int[] hWayCCW = Interpolate(pl[wayCCW[0]].y, pl[wayCCW[0]].z, pl[wayCCW[1]].y, pl[wayCCW[1]].z);
            float[] fWayCCW = FInterpolate(pl[wayCCW[0]].y, pl[wayCCW[0]].l, pl[wayCCW[1]].y, pl[wayCCW[1]].l);
            for (int k = 1; k < wayCCW.Count - 1; k++)
            {
                xWayCCW = xWayCCW.Take(xWayCCW.Count() - 1).Concat(Interpolate(pl[wayCCW[k]].y, pl[wayCCW[k]].x, pl[wayCCW[k + 1]].y, pl[wayCCW[k + 1]].x)).ToArray();
                hWayCCW = hWayCCW.Take(hWayCCW.Count() - 1).Concat(Interpolate(pl[wayCCW[k]].y, pl[wayCCW[k]].z, pl[wayCCW[k + 1]].y, pl[wayCCW[k + 1]].z)).ToArray();
                fWayCCW = fWayCCW.Take(fWayCCW.Count() - 1).Concat(FInterpolate(pl[wayCCW[k]].y, pl[wayCCW[k]].l, pl[wayCCW[k + 1]].y, pl[wayCCW[k + 1]].l)).ToArray();
            }


            int[] xleft, xright, hleft, hright;
            float[] fleft, fright;

            int m = xWayCW.Length / 2;
            if (xWayCW[m] < xWayCCW[m]) {
                // CW way is left
                // CCW way is right
                xleft = xWayCW;
                xright = xWayCCW;
                hleft = hWayCW;
                hright = hWayCCW;
                fleft = fWayCW;
                fright = fWayCCW;
                
            }
            else {
                // CCW way is left
                // CW way is right
                xleft = xWayCCW;
                xright = xWayCW;
                hleft = hWayCCW;
                hright = hWayCW;
                fleft = fWayCCW;
                fright = fWayCW;
            }


            for (int y = ly; y <= uy; y++)
            {
                int x_l = xleft[i];
                int x_r = xright[i];

                int lx = Math.Max(x_l, 0);
                int j = lx - x_l;
                int ux = Math.Min(x_r, w - 1);

                if (x_l > x_r || lx > ux)
                {
                    ++i;
                    continue;
                }

                int[] h_segment = Interpolate(x_l, hleft[i], x_r, hright[i]);
                float[] f_segment = FInterpolate(x_l, fleft[i], x_r, fright[i]);

                for (int x = lx; x <= ux; x++)
                {
                    int z = h_segment[j];
                    if (z > zbuffer[y, x])
                    {
                        zbuffer[y, x] = z;
                        Point t = ToTextureCoords(x, y);
                        cbuffer[y, x] = CompileColor(texture[t.Y, t.X], f_segment[j], light, Ambient);
                    }
                    j++;
                }
                i++;
            }


        }


        private static Color CompileColor(Color tex, float fval, Color light, Color ambient) {
            int fuse(int t, int l,int a)
            {
                return  (int)((1 - (255-fval*l)*(255-a)/255/255)*t);

            }
            
            /*return Color.FromArgb(255,(int)(fval*tex.R*light.R*ambient.R/Lighting.color_norm),
                                      (int)(fval * tex.G * light.G * ambient.G / Lighting.color_norm),
                                      (int)(fval * tex.B * light.B * ambient.B / Lighting.color_norm));*/

            return Color.FromArgb(tex.A, fuse(tex.R, light.R,ambient.R), fuse(tex.G, light.G, ambient.G),fuse(tex.B, light.B,ambient.B));
        }


        private static void FillTrinagle(point3 p0, point3 p1, point3 p2, int w, int h, int[,] zbuffer, Color[,] cbuffer, Color[,] texture, Color light, Color Ambient)
        {
            // p0.y <=p1.y <= p2.y

            var pl = new point3[]{ p0, p1, p2 };
            int minx = pl.Min(p => p.x), maxx = pl.Max(p => p.x);

            int ly = Math.Max(p0.y, 0);
            int i = ly - p0.y;
            int uy = Math.Min(p2.y, h - 1);
            if (ly > uy || Math.Max(0, minx) > Math.Min(w - 1, maxx))
                return;



            float TextureScaleX, TextureScaleY;
            if (maxx != minx)
                TextureScaleX = (float)(texture.GetLength(1) - 1) / (maxx - minx);
            else
                TextureScaleX = 0;

            if (p0.y != p2.y)
                TextureScaleY = (float)(texture.GetLength(0) - 1) / (p2.y - p0.y);
            else
                TextureScaleY = 0;

            Point ToTextureCoords(int x, int y)
            {
                if (x < minx)
                    x = minx;
                return new Point((int)(TextureScaleX * (x - minx)), (int)(TextureScaleY * (y - p0.y)));
            }



            int[] x012 = Interpolate(p0.y, p0.x, p1.y, p1.x);
            x012 = x012.Take(x012.Length - 1).Concat(Interpolate(p1.y, p1.x, p2.y, p2.x)).ToArray();

            int[] h012 = Interpolate(p0.y, p0.z, p1.y, p1.z);
            h012 = h012.Take(h012.Length - 1).Concat(Interpolate(p1.y, p1.z, p2.y, p2.z)).ToArray();

            float[] f012 = FInterpolate(p0.y, p0.l, p1.y, p1.l);
            f012 = f012.Take(f012.Length - 1).Concat(FInterpolate(p1.y, p1.l, p2.y, p2.l)).ToArray();

            int[] x02 = Interpolate(p0.y, p0.x, p2.y, p2.x);
            int[] h02 = Interpolate(p0.y, p0.z, p2.y, p2.z);
            float[] f02 = FInterpolate(p0.y, p0.l, p2.y, p2.l);



            int[] x_left, x_right, h_left, h_right;
            float[] f_left,f_right;

            int m = x012.Length / 2;
            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;

                h_left = h02;
                h_right = h012;

                f_left = f02;
                f_right = f012;
            }
            else
            {
                x_left = x012;
                x_right = x02;

                h_left = h012;
                h_right = h02;

                f_left = f012;
                f_right = f02;
            }

            for (int y = ly; y <= uy; y++)
            {

                int x_l = x_left[i];
                int x_r = x_right[i];

                int lx = Math.Max(x_l, 0);
                int j = lx - x_l;
                int ux = Math.Min(x_r, w - 1);

                if (lx > ux) {
                    i++;
                    continue;
                }

                int[] h_segment;
                float[] f_segment;
                if (x_l > x_r)
                    break;
                h_segment = Interpolate(x_l, h_left[i], x_r, h_right[i]);
                f_segment = FInterpolate(x_l, f_left[i], x_r, f_right[i]);


                for (int x = lx; x <= ux; x++)
                {
                    int z = h_segment[j];
                    if (z > zbuffer[y, x])
                    {
                        zbuffer[y, x] = z;
                        Point t = ToTextureCoords(x, y);
                        cbuffer[y, x] = CompileColor(texture[t.Y, t.X], f_segment[j], light, Ambient);
                    }
                    j++;
                }
                i++;
            }

        }

       
        public static Color[,] TextureToColors(string filepath) {
            Bitmap tex = new Bitmap(filepath);
            Color[,] res = new Color[tex.Height, tex.Width];

          
            Rectangle rect = new Rectangle(0, 0, tex.Width, tex.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                tex.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                tex.PixelFormat);


            IntPtr ptr = bmpData.Scan0;
            int strd = Math.Abs(bmpData.Stride);
            int bytes = strd * tex.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            tex.UnlockBits(bmpData);
            
            for (int i = 0; i < tex.Height; i++)
            {
                for (int j = 0; j < tex.Width; j++)
                {
                    int ind = strd * i + j * 4;

                    res[i, j] = Color.FromArgb(rgbValues[ind + 3], rgbValues[ind + 2], rgbValues[ind + 1], rgbValues[ind]);
                }
            }
            return res;
        }
    }    

    
  

    public class OrbitCamera : CameraView {
        private float cam_angx;
        private float cam_angy;
        private float cam_distance;
        private float cam_tilt;
        private Figure cam;
        private float angX;
        private float angY;
        private float angT;
        private float dist;





        public OrbitCamera(float distance, float ini_anglx, float init_angly, float init_tiltang, Point3D t, float fvx, float fvy, float mind, float maxd)
            : base(new Point3D(0,0,0), t, new Point3D(0,0,0), fvx, fvy, mind, maxd) {

            cam_distance = distance;
            cam_angy = init_angly;
            cam_angx = ini_anglx;
            cam_tilt = init_tiltang;

            dist = distance;
            angX = ini_anglx;
            angY = init_angly;
            angT = init_tiltang;

            cam = Figure.get_Coordinates();
            for (int i = 1; i < 4; i++)
                cam.points[i] = Point3D.norm(cam.points[i]);

            cam.offset(-cam_distance, 0, 0);
            cam.line_rotate_rad(cam_angx, new Point3D(0, 0, 0), cam.points[3] - cam.points[0]);
            cam.line_rotate_rad(cam_angy, new Point3D(0, 0, 0), cam.points[1] - cam.points[0]);
            cam.line_rotate_rad(cam_tilt, new Point3D(0, 0, 0), cam.points[2] - cam.points[0]);

            set_cam();


        }

        private void set_cam() {
            set_params_at_once(cam.points[0], Target, cam.points[3] - cam.points[0], FovX, FovY, MinDistance, MaxDistance);
             
        }

        public void MoveUpDown(float rad_ang) {
            cam.line_rotate_rad(rad_ang, new Point3D(0, 0, 0), cam.points[1] - cam.points[0]);
            angY += rad_ang;       
            set_cam();
            if (angY >= Math.PI * 2) angY -= (float)Math.PI * 2;
            else if (angY <= -Math.PI * 2) angY += (float)Math.PI * 2;
        }

        public void MoveLeftRight(float rad_ang)
        {
            cam.line_rotate_rad(rad_ang, new Point3D(0, 0, 0), cam.points[3] - cam.points[0]);
            angX += rad_ang;
            set_cam();
            if (angX >= Math.PI * 2) angX -= (float)Math.PI * 2;
            else if (angX <= -Math.PI * 2) angX += (float)Math.PI * 2;
        }

        public void TiltLeftRight(float rad_ang) {
            cam.line_rotate_rad(rad_ang, new Point3D(0, 0, 0), cam.points[2] - cam.points[0]);
            angT += rad_ang;        
            set_cam();
            if (angT >= Math.PI * 2) angT -= (float)Math.PI * 2;
            else if (angT <= -Math.PI * 2) angT += (float)Math.PI * 2;
        }

        public void MoveFarNear(float d) {
            Point3D ofst = cam.points[0] - cam.points[2];
            cam.offset(d * ofst.x, d * ofst.y, d * ofst.z);
            dist += d;
            set_cam();
        }

        public float AngleX
        {
            get { return angX; }
            set { MoveLeftRight(value - angX);}
        }

        public float AngleY
        {
            get { return angY; }
            set { MoveUpDown(value - angY); }
        }

        public float AngleTilt
        {
            get { return angT; }
            set { TiltLeftRight(value - angT); }
        }

        public float Distance
        {
            get { return dist; }
            set { MoveFarNear(value - dist); }
        }

        ///something weird with resetting to angles, went the hard way
        public void Reset() {
            dist = cam_distance;
            angX = cam_angx;
            angY = cam_angy;
            angT = cam_tilt;

            cam = Figure.get_Coordinates();
            for (int i = 1; i < 4; i++)
                cam.points[i] = Point3D.norm(cam.points[i]);

            cam.offset(-cam_distance, 0, 0);
            cam.line_rotate_rad(cam_angx, new Point3D(0, 0, 0), cam.points[3] - cam.points[0]);
            cam.line_rotate_rad(cam_angy, new Point3D(0, 0, 0), cam.points[1] - cam.points[0]);
            cam.line_rotate_rad(cam_tilt, new Point3D(0, 0, 0), cam.points[2] - cam.points[0]);

            set_cam();
        }


    }


    public class Lighting
    {
        public Point3D position;
        public Color color;
        public Color ambient_color;

        public Lighting(Point3D p, Color c,Color a)
        {
            position = new Point3D(p);
            color = c;
            ambient_color = a;

        }

        public static int color_norm = 255 * 255 * 255;
    }

    public struct point3
    {
        public int x;
        public int y;
        public int z;
        public float l;
     

        public point3(int _x, int _y, int _z,float _l) {
            x = _x;
            y = _y;
            z = _z;
            l = _l;
        }




    }
}
