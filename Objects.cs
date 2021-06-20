using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using static RTTest1.Transformation;
using static RTTest1.Utils;

namespace RTTest1
{
    public static class Objects
    {
        /// <summary>
        /// Сохранить полигональный объект в  .obj
        /// </summary>
        /// <param name="mesh"></param>
        public static void SaveMesh(Mesh mesh)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "obj files (*.obj)|*.obj";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream Stream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                using (StreamWriter writer = new StreamWriter(Stream, Encoding.UTF8))
                {
                    writer.WriteLine("o MyMesh");
                    foreach (Point3D p in mesh.points)
                    {
                        string pX = p.X.ToString(CultureInfo.InvariantCulture);
                        string pY = p.Y.ToString(CultureInfo.InvariantCulture);
                        string pZ = p.Z.ToString(CultureInfo.InvariantCulture);
                        writer.WriteLine("v " + pX + " " + pY + " " + pZ);
                    }
                    foreach (Polygon p in mesh.faces)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("f ");
                        int counter = p.points.Count();
                        foreach (Point3D p3 in p.points)
                        {
                            counter--;
                            if (counter == 0)
                                sb.Append((p3.index + 1).ToString());
                            else
                                sb.Append((p3.index + 1).ToString() + " ");
                        }
                        writer.WriteLine(sb);
                    }
                }
            }
        }

        /// <summary>
        /// Загрузить полигональный объект
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadMesh(ref Mesh mesh)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();


            openFileDialog1.Filter = "obj files (*.obj)|*.obj";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string ans = "object";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                string[] text = File.ReadAllLines(filename);
                mesh = new Mesh();
                int cnt = 0;
                int cntN = 0;
                List<Point3D> normals = new List<Point3D>();
                foreach (string x in text)
                {
                    if (x.StartsWith("o "))
                    {
                        ans = x.Remove(0, 2);
                    }
                    if (x.StartsWith("v "))
                    {
                        string[] s = x.Remove(0, 2).Split(' ');
                        Point3D point = new Point3D();
                        point.X = Double.Parse(s[0], new CultureInfo("en-us"));
                        point.Y = Double.Parse(s[1], new CultureInfo("en-us"));
                        point.Z = Double.Parse(s[2], new CultureInfo("en-us"));
                        point.index = cnt;
                        mesh.points.Add(point);
                        cnt++;
                    }

                    if (x.StartsWith("f "))
                    {
                        string[] s = x.Remove(0, 2).Split(' ');
                        List<Point3D> l = new List<Point3D>();
                        foreach (string s1 in s)
                        {
                            var t = s1.Split('/');
                            l.Add(mesh.points[int.Parse(t[0]) - 1]);
                        }
                        Point3D norm = new Point3D(int.Parse(s[0].Split('/')[0]) - 1, 0, 0);
                        Polygon poly = new Polygon(l, norm);
                        mesh.faces.Add(poly);

                    }
                    if (x.StartsWith("vn "))
                    {
                        string[] s = x.Remove(0, 3).Split(' ');
                        Point3D norm = new Point3D(
                            Convert.ToDouble(s[0]),
                            Convert.ToDouble(s[1]),
                            Convert.ToDouble(s[2]));
                        normals.Add(norm);
                        cntN++;
                    }
                }

                foreach (Polygon pol in mesh.faces)
                {
                    int ind = (int)pol.normal.X;
                    pol.normal = normals[ind];
                }
            }
            return ans;
        }

        /// <summary>
        /// Загрузить полигональный объект по заданному адресу
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadMeshByPath(ref Mesh mesh, string path)
        {
            string ans = "object";

            string filename = path;
            string[] text = File.ReadAllLines(filename);
            mesh = new Mesh();
            int cnt = 0;
            int cntN = 0;
            List<Point3D> normals = new List<Point3D>();
            foreach (string x in text)
            {
                if (x.StartsWith("o "))
                {
                    ans = x.Remove(0, 2);
                }
                if (x.StartsWith("v "))
                {
                    string[] s = x.Remove(0, 2).Split(' ');
                    Point3D point = new Point3D();
                    point.X = Double.Parse(s[0], new CultureInfo("en-us"));
                    point.Y = Double.Parse(s[1], new CultureInfo("en-us"));
                    point.Z = Double.Parse(s[2], new CultureInfo("en-us"));
                    point.index = cnt;
                    mesh.points.Add(point);
                    cnt++;
                }

                if (x.StartsWith("f "))
                {
                    string[] s = x.Remove(0, 2).Split(' ');
                    List<Point3D> l = new List<Point3D>();
                    foreach (string s1 in s)
                    {
                        l.Add(mesh.points[int.Parse(s1.Split('/')[0]) - 1]);
                    }
                    Point3D norm = new Point3D(int.Parse(s[0].Split('/')[2]) - 1, 0, 0);
                    Polygon poly = new Polygon(l, norm);
                    mesh.faces.Add(poly);

                }
                if (x.StartsWith("vn "))
                {
                    string[] s = x.Remove(0, 3).Split(' ');
                    Point3D norm = new Point3D(
                        Convert.ToDouble(s[0], CultureInfo.InvariantCulture),
                        Convert.ToDouble(s[1], CultureInfo.InvariantCulture),
                        Convert.ToDouble(s[2], CultureInfo.InvariantCulture));
                    normals.Add(norm);
                    cntN++;
                }
            }
            foreach (Polygon pol in mesh.faces)
            {
                int ind = (int)pol.normal.X;
                pol.normal = normals[ind];
            }
            return ans;
        }

        /// <summary>
        /// Точка в пространстве
        /// </summary>
        public class Point3D : IComparable<Point3D>
        {
            public double X;
            public double Y;
            public double Z;
            public int index;

            public Point3D(double x, double y, double z, int ind)
            {
                X = x;
                Y = y;
                Z = z;
                index = ind;
            }
            public Point3D(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
                index = 0;
            }
            public Point3D()
            {
                X = 0;
                Y = 0;
                Z = 0;
                index = 0;
            }
            public Point3D(Point3D p)
            {
                X = p.X;
                Y = p.Y;
                Z = p.Z;
                index = p.index;
            }


            public static double operator *(Point3D p1, Point3D p2)
            {
                return VectorDotProduct(p1, p2);
            }
            public static Point3D operator +(Point3D p1, Point3D p2)
            {
                return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
            }
            public static Point3D operator -(Point3D p1, Point3D p2)
            {
                return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
            }
            public static Point3D operator *(Point3D p1, double d)
            {
                return new Point3D(p1.X * d, p1.Y * d, p1.Z * d);
            }

            public void ApplyMatrix(double[,] m)
            {

                double[,] m1 = new double[1, 4] { { X, Y, Z, 1 } };
                double[,] m2 = MultMatrix(m1, m);
                X = (float)m2[0, 0];
                Y = (float)m2[0, 1];
                Z = (float)m2[0, 2];
            }

            public int CompareTo(Point3D other)
            {
                if (X < other.X || Y < other.Y) return -1;
                if (X == other.X && Y == other.Y) return 0;
                return 1;
            }

            public double Length()
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        /// <summary>
        /// Грань
        /// </summary>
        public class Edge
        {
            public Point3D p1;
            public Point3D p2;

            public Edge(Point3D _p1, Point3D _p2)
            {
                p1 = _p1;
                p2 = _p2;
            }
            public Edge()
            {
                p1 = new Point3D();
                p2 = new Point3D();
            }
        }
        
        /// <summary>
        /// Полигон
        /// </summary>
        public class Polygon
        {
            public List<Point3D> points;
            public Point3D normal;
            public Bitmap lightmap;

            public Polygon()
            {
                points = new List<Point3D>();
            }
            public Polygon(List<Point3D> l, bool clockwise = true)
            {
                points = new List<Point3D>();
                foreach (Point3D p in l)
                {
                    Point3D t = new Point3D(p);
                    points.Add(t);
                }
                normal = GetNormal(this, clockwise);
            }
            public Polygon(List<Point3D> l, Point3D norm)
            {
                points = new List<Point3D>();
                foreach (Point3D p in l)
                {
                    Point3D t = new Point3D(p);
                    points.Add(t);
                }
                normal = new Point3D(norm);
            }

            public bool Ray_intersection(Point3D ray1, Point3D ray2, ref Point3D P, ref double dist)
            {
                double d = Distance(this, new Point3D(0, 0, 0));
                double dot1 = ray1 * normal;
                double dot2 = ray2 * normal;
                if (Math.Abs(dot2) < 0.000001) return false;

                dist = (-d - dot1) / dot2;
                if (dist < 0) return false;

                P = ray1 + ray2 * dist;

                double anglesum = CalcAngleSum(P, this);
                if (Math.Abs(anglesum - Math.PI * 2) > 0.0001)
                    return false;
                return true;
            }
        }
        
        /// <summary>
        /// Полигональная фигура
        /// </summary>
        public class Mesh
        {
            public List<Point3D> points;
            public SortedDictionary<int, List<int>> connections;
            public List<Edge> edges;
            public List<Polygon> faces;

            public Material material;
            public Mesh()
            {
                points = new List<Point3D>();
                connections = new SortedDictionary<int, List<int>>();
                edges = new List<Edge>();
                faces = new List<Polygon>();
                material = new Material();
            }
            public Mesh(List<Point3D> l, SortedDictionary<int, List<int>> sd, List<Edge> le, List<Polygon> lf)
            {
                points = new List<Point3D>();
                connections = new SortedDictionary<int, List<int>>();
                edges = new List<Edge>();
                faces = new List<Polygon>();
                material = new Material();
                foreach (Point3D p in l)
                {
                    Point3D p3D = new Point3D(p);
                    points.Add(p3D);
                    List<int> temp = new List<int>();
                    if (sd.ContainsKey(p.index))
                        foreach (int pp in sd[p.index])
                        {
                            temp.Add(pp);
                        }
                    connections.Add(p.index, temp);
                }
                if (le.Count() == 0)
                {
                    int countP = points.Count();
                    bool[,] flags = new bool[countP, countP];

                    foreach (Point3D p1 in points)
                    {
                        int p1ind = p1.index;
                        foreach (int p2ind in connections[p1ind])
                        {
                            if (!flags[p1ind, p2ind])
                            {
                                flags[p1ind, p2ind] = true;
                                flags[p2ind, p1ind] = true;
                                Point3D t1 = new Point3D(p1);
                                Point3D t2 = new Point3D(points[p2ind]);
                                edges.Add(new Edge(t1, t2));
                            }
                        }
                    }
                }
                else
                {
                    foreach (Edge e in le)
                    {
                        Point3D t1 = new Point3D(e.p1);
                        Point3D t2 = new Point3D(e.p2);
                        edges.Add(new Edge(t1, t2));
                    }
                }
                if (lf.Count != 0)
                {
                    foreach (Polygon p in lf) faces.Add(new Polygon(p.points));
                }

            }
            public Mesh(Mesh other)
            {
                var l = other.points;
                var sd = other.connections;
                var le = other.edges;
                var lf = other.faces;
                points = new List<Point3D>();
                connections = new SortedDictionary<int, List<int>>();
                edges = new List<Edge>();
                faces = new List<Polygon>();
                material = new Material();

                foreach (Point3D p in l)
                {
                    Point3D p3D = new Point3D(p);
                    points.Add(p3D);
                    List<int> temp = new List<int>();
                    if (sd.ContainsKey(p.index))
                        foreach (int pp in sd[p.index])
                        {
                            temp.Add(pp);
                        }
                    connections.Add(p.index, temp);
                }
                if (le.Count() == 0)
                {
                    int countP = points.Count();
                    bool[,] flags = new bool[countP, countP];
                    foreach (Point3D p1 in points)
                    {
                        int p1ind = p1.index;
                        foreach (int p2ind in connections[p1ind])
                        {
                            if (!flags[p1ind, p2ind])
                            {
                                flags[p1ind, p2ind] = true;
                                flags[p2ind, p1ind] = true;
                                Point3D t1 = new Point3D(p1);
                                Point3D t2 = new Point3D(points[p2ind]);
                                edges.Add(new Edge(t1, t2));
                            }
                        }
                    }
                }
                else
                {
                    foreach (Edge e in le)
                    {
                        Point3D t1 = new Point3D(e.p1);
                        Point3D t2 = new Point3D(e.p2);
                        edges.Add(new Edge(t1, t2));
                    }
                }
                if (lf.Count != 0)
                {
                    foreach (Polygon p in lf) faces.Add(new Polygon(p.points));
                }
            }

            public Point3D GetCenterPoint() => points.Aggregate(new Point3D(0, 0, 0), (x, y) => x + y) * (1.0 / points.Count);

            public void Clear()
            {
                points.Clear();
                connections.Clear();
                edges.Clear();
                faces.Clear();
            }
        }

        /// <summary>
        /// Сфера
        /// </summary>
        public class Sphere
        {
            public Point3D pos;
            public double radius;
            public Material material;
            public Bitmap lightmap;

            public Sphere(Point3D l, double r)
            {
                pos = new Point3D(l);
                radius = r;
                material = new Material();
            }
            public Sphere()
            {
                pos = new Point3D();
                radius = 0;
                material = new Material();
            }

            public bool ray_intersection(Point3D rayL, Point3D rayV, ref double dist)
            {
                Point3D L = new Point3D(pos.X - rayL.X, pos.Y - rayL.Y, pos.Z - rayL.Z);
                double t = VectorDotProduct(L, rayV);
                double d2 = VectorDotProduct(L, L) - t * t;
                if (d2 > radius * radius) return false;

                double thc = Math.Sqrt(radius * radius - d2);
                dist = t - thc;
                double t1 = t + thc;
                if (dist < 0)
                    dist = t1;
                if (dist < 0)
                    return false;
                return true;
            }
        }
    }
}
