using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using static RTTest1.Objects;
using static RTTest1.Transformation;

namespace RTTest1
{

    public static class Utils
    {
        //Scene objects storage
        public static List<Mesh> meshes;
        public static List<Sphere> spheres;
        public static List<Light> lights;

        /// <summary>
        /// Get normal vectors for mesh object
        /// </summary>
        /// <param name="m"></param>
        /// <param name="clockwise"></param>
        /// <returns></returns>
        public static List<Point3D> GetVertexNormals(Mesh m, bool clockwise)
        {
            List<Point3D> ans = new List<Point3D>();
            foreach (Point3D point in m.points)
            {
                List<Point3D> faceNormals = new List<Point3D>();
                foreach (Polygon face in m.faces)
                {
                    if (face.points.Where((x) => x.index == point.index).Count() > 0) continue;

                    Point3D fnorm = GetNormal(face, clockwise);
                    faceNormals.Add(fnorm);
                }
                if (faceNormals.Count > 0)
                {
                    Point3D norm = new Point3D(0, 0, 0, point.index);
                    foreach (Point3D p in faceNormals)
                    {
                        norm.X += p.X;
                        norm.Y += p.Y;
                        norm.Z += p.Z;
                    }
                    double len = Distance(new Point3D(0, 0, 0), norm);
                    norm.X /= len;
                    norm.Y /= len;
                    norm.Z /= len;
                    ans.Add(norm);
                }
            }
            return ans;
        }

        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public static double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public static double Distance(Point3D p1, Point3D p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) + Math.Pow(p1.Z - p2.Z, 2));
        }
        public static double Distance(Polygon pol, Point3D p)
        {
            Point3D p1 = pol.points[0];
            Point3D p2 = pol.points[1];
            Point3D p3 = pol.points[2];

            double x1 = p1.X, y1 = p1.Y, z1 = p1.Z;
            double x2 = p2.X, y2 = p2.Y, z2 = p2.Z;
            double x3 = p3.X, y3 = p3.Y, z3 = p3.Z;

            double a = (y2 - y1) * (z3 - z1) - (z2 - z1) * (y3 - y1);
            double b = (z2 - z1) * (x3 - x1) - (x2 - x1) * (z3 - z1);
            double c = (x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1);
            double d = (-1) * (a * x1 + b * y1 + c * z1);

            return Math.Abs(a * p.X + b * p.Y + c * p.Z + d) / Math.Sqrt(a * a + b * b + c * c);
        }

        /// <summary>
        /// Dot product for two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double VectorDotProduct(Point3D p1, Point3D p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
        }

        /// <summary>
        /// Cross product for two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3D VectorCrossProduct(Point3D p1, Point3D p2)
        {
            return new Point3D(
                p1.Y * p2.Z - p1.Z * p2.Y,
                p1.Z * p2.X - p1.X * p2.Z,
                p1.X * p2.Y - p1.Y * p2.X);
        }

        /// <summary>
        /// Normalize vectors
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point3D Normalize(Point3D p)
        {
            double d = Distance(new Point3D(0, 0, 0), p);
            return new Point3D(p.X / d, p.Y / d, p.Z / d);
        }

        /// <summary>
        /// Angle between two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="inRadians">true => radians, false => degrees</param>
        /// <returns></returns>
        public static double AngleBetween(Point3D p1, Point3D p2, bool inRadians = false)
        {
            double a = Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z) * Math.Sqrt(p2.X * p2.X + p2.Y * p2.Y + p2.Z * p2.Z);
            double b = p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z;
            double c = b / a;

            if (!inRadians)
                return Math.Acos(c) * 180 / Math.PI;
            else
                return Math.Acos(c);
        }

        /// <summary>
        /// Cos of angle between two vectors
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double CosBetween(Point3D p1, Point3D p2)
        {
            double p1X = p1.X;
            double p1Y = p1.Y;
            double p1Z = p1.Z;
            double p2X = p2.X;
            double p2Y = p2.Y;
            double p2Z = p2.Z;

            if (Math.Abs(p1.X) < 0.0001) p1X = 0;
            if (Math.Abs(p1.Y) < 0.0001) p1Y = 0;
            if (Math.Abs(p1.Z) < 0.0001) p1Z = 0;
            if (Math.Abs(p2.X) < 0.0001) p2X = 0;
            if (Math.Abs(p2.Y) < 0.0001) p2Y = 0;
            if (Math.Abs(p2.Z) < 0.0001) p2Z = 0;

            return (p1X * p2X + p1Y * p2Y + p1Z * p2Z) / (p1.Length() * p2.Length());

            //return (p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z) / (p1.Length() * p2.Length());
        }

        /// <summary>
        /// Reflect vector using normal of surface
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Point3D RayReflect(Point3D vec, Point3D normal)
        {
            double dot1 = VectorDotProduct(vec, normal);
            return new Point3D(vec.X - normal.X * 2.0 * dot1, vec.Y - normal.Y * 2.0 * dot1, vec.Z - normal.Z * 2.0 * dot1);
        }

        /// <summary>
        /// Refract vector using surface normal and refraction index
        /// </summary>
        /// <param name="rayV"></param>
        /// <param name="normal"></param>
        /// <param name="refractionIndex"></param>
        /// <returns></returns>
        public static Point3D RayRefract(Point3D rayV, Point3D normal, double refractionIndex)
        {

            double oldInd = 1;
            double newInd = refractionIndex;

            double dp = -1 * (rayV * normal);

            Point3D n = normal;

            //normal vector facing wrong dir
            if (dp < 0)
            {

                n *= -1;
                dp *= -1;

                double t = oldInd;
                oldInd = newInd;
                newInd = t;
            }
            double eta = oldInd / newInd;

            double k = 1 - (1 - dp * dp) * eta * eta ;

            //Does reflect?
            if (k < 0)
                return new Point3D(0, 0, 0);
            else
                return rayV * eta + n * (eta * dp - Math.Sqrt(k));
        }
        
        /// <summary>
        /// Get polygon normal directed outwards
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="clockwise">false to direct inwards</param>
        /// <returns></returns>
        public static Point3D GetNormal(Polygon polygon, bool clockwise)
        {
            Point3D normalVec = new Point3D();
            Point3D vec1 = new Point3D(polygon.points[0].X - polygon.points[1].X, polygon.points[0].Y - polygon.points[1].Y, polygon.points[0].Z - polygon.points[1].Z);
            Point3D vec2 = new Point3D(polygon.points[2].X - polygon.points[1].X, polygon.points[2].Y - polygon.points[1].Y, polygon.points[2].Z - polygon.points[1].Z);

            if (clockwise)
            {
                normalVec = new Point3D(vec1.Z * vec2.Y - vec1.Y * vec2.Z, vec1.X * vec2.Z - vec1.Z * vec2.X, vec1.Y * vec2.X - vec1.X * vec2.Y);
            }
            else
            {
                normalVec = new Point3D(vec1.Y * vec2.Z - vec1.Z * vec2.Y, vec1.Z * vec2.X - vec1.X * vec2.Z, vec1.X * vec2.Y - vec1.Y * vec2.X);
            }

            double dist = Distance(new Point3D(0, 0, 0), normalVec);
            return new Point3D(normalVec.X / dist, normalVec.Y / dist, normalVec.Z / dist);
        }

        public static double CalcAngleSum(Point3D q, Polygon p)
        {
            int n = p.points.Count;

            double m1, m2;

            double anglesum = 0, costheta = 0;
            double epsilon = 0.0000001;
            double twopi = Math.PI * 2;
            Point3D p1 = new Point3D();
            Point3D p2 = new Point3D();

            for (int i = 0; i < n; i++)
            {
                p1.X = p.points[i].X - q.X;
                p1.Y = p.points[i].Y - q.Y;
                p1.Z = p.points[i].Z - q.Z;

                p2.X = p.points[(i + 1) % n].X - q.X;
                p2.Y = p.points[(i + 1) % n].Y - q.Y;
                p2.Z = p.points[(i + 1) % n].Z - q.Z;

                m1 = Math.Sqrt(p1.X * p1.X + p1.Y * p1.Y + p1.Z * p1.Z);
                m2 = Math.Sqrt(p2.X * p2.X + p2.Y * p2.Y + p2.Z * p2.Z);
                if (m1 * m2 <= epsilon)
                    return (twopi);
                else
                    costheta = (p1.X * p2.X + p1.Y * p2.Y + p1.Z * p2.Z) / (m1 * m2);

                anglesum += Math.Acos(costheta);
            }
            return (anglesum);
        }

        /// <summary>
        /// Find intersection between ray and all of the scene objects
        /// </summary>
        /// <param name="origin">origin point</param>
        /// <param name="dir">directional vector</param>
        /// <param name="intersectionPoint">intersection point</param>
        /// <param name="normal">plane normal</param>
        /// <param name="material"></param>
        /// <returns></returns>
        public static bool FindIntersection(Point3D origin, Point3D dir, ref Point3D intersectionPoint,
            ref Point3D normal, ref Material material)
        {
            double dist = float.MaxValue;
            foreach (Sphere s in spheres)
            {
                double dist_i = 0;
                if (s.ray_intersection(origin, dir, ref dist_i) && dist_i < dist)
                {
                    dist = dist_i;
                    intersectionPoint = origin + dir * dist_i;
                    normal = Normalize(intersectionPoint - s.pos);
                    material = s.material;
                }
            }
            foreach (Mesh msh in meshes)
            {
                foreach (Polygon pol in msh.faces)
                {
                    Point3D p = new Point3D();
                    double dist_i = 0;
                    if (pol.Ray_intersection(origin, dir, ref p, ref dist_i) && dist_i < dist)
                    {
                        dist = dist_i;
                        intersectionPoint = origin + dir * dist_i;
                        normal = pol.normal;
                        material = msh.material;
                    }
                }
            }
            return dist < 1000000;
        }

        public static double LightFalloff(double dist, double maxDist)
        {
            double d = 1 - Math.Min(1, dist / maxDist);
            return d;
        }

        public static Point3D HitToCoords(Point3D hit, Polygon p)
        {
            Point3D side, dbase;
            double cos, x, y;

            if (p.points.Count == 3)
            {
                Point3D top = p.points[2] - p.points[0];
                side = hit - p.points[0];
                dbase = p.points[1] - p.points[0];
                cos = CosBetween(side, dbase);
                double cos2 = CosBetween(top, dbase);

                x = cos * side.Length() / dbase.Length();
                y = Math.Sqrt(1 - cos * cos) / Math.Sqrt(1 - cos2 * cos2);

                return new Point3D(x, y, 0);
            }
            else 
            {
                side = hit - p.points[0];
                dbase = p.points[1] - p.points[0];
                cos = CosBetween(side, dbase);
                x = cos * side.Length() / dbase.Length();

                side = hit - p.points[1];
                dbase = p.points[2] - p.points[1];
                cos = CosBetween(side, dbase);
                y = cos * side.Length() / dbase.Length();

                return new Point3D(x, y, 0);
            }
        }

        public static Point3D HitToCoords(Point3D hit, Sphere p)
        {
            //TODO
                return new Point3D(0, 0, 0);
        }

        public static void SetLightmaps(int resolution)
        { 
            foreach (Mesh m in meshes)
            {
                foreach (Polygon p in m.faces)
                {
                    p.lightmap = new Bitmap(resolution, resolution);
                }
            }

            /*foreach (Sphere s in Sphere)
            {
                {
                    s.lightmap = new Bitmap(resolution, resolution);
                }
            }*/

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        /// <param name="radius">0 = point</param>
        public static void MakeSpot(ref Bitmap map, int x, int y, Color c, int radius)
        {
            int maxX = map.Width;
            int maxY = map.Height;



            for (int i = 0; i < 2 * radius + 1; i++)
                for (int j = 0; j < 2 * i + 1; j++)
                {
                    int cx = x - radius + i;
                    int cy = y - i + j;
                    if (cx < maxX && cy < maxY && cx > -1 && cy > -1)
                    {
                        Color curr = map.GetPixel(cx, cy);
                        int resR = curr.R + c.R;
                        int resG = curr.G + c.G;
                        int resB = curr.B + c.B;
                        if (resR > 255) resR = 255;
                        if (resG > 255) resG = 255;
                        if (resB > 255) resB = 255;
                        Color res = Color.FromArgb(resR, resG, resB);
                        map.SetPixel(cx, cy,res);
                    }
                }
        }

        public static Color GetColorInterpolated (Bitmap map, int x, int y, int radius)
        {
            int maxX = map.Width;
            int maxY = map.Height;

            double sumR = 0;
            double sumG = 0;
            double sumB = 0;
            int count = 0;
            for (int cx = x - radius; cx <= x + radius; cx++)
                for (int cy = y - radius; cy <= y + radius; cy++)
                {
                    if (cx < maxX && cy < maxY && cx > -1 && cy > -1)
                    {
                        Color c = map.GetPixel(cx, cy);
                        sumR += c.R;
                        sumG += c.G;
                        sumB += c.B;
                        count++;
                    }
                }
            return Color.FromArgb((int)(sumR / count), (int)(sumG / count), (int)(sumB / count));
        }

        public static double[,] RotateAroundLineMatrix(Point3D p1, Point3D p2, double angle)
        {
            double l = (p2.X - p1.X) / Distance(p1, p2);
            double m = (p2.Y - p1.Y) / Distance(p1, p2);
            double n = (p2.Z - p1.Z) / Distance(p1, p2);

            return new double[4, 4]
                {{ l*l + Math.Cos(angle)*(1 - l*l), l*(1-Math.Cos(angle))*m + n*Math.Sin(angle), l*(1 - Math.Cos(angle))*n - m*Math.Sin(angle), 0 },
                { l*(1 - Math.Cos(angle))*m - n*Math.Sin(angle), m*m + Math.Cos(angle)*(1 - m*m), m*(1 - Math.Cos(angle))*n + l*Math.Sin(angle), 0 },
                { l*(1 - Math.Cos(angle))*n + m*Math.Sin(angle), m*(1 - Math.Cos(angle))*n - l*Math.Sin(angle), n*n + Math.Cos(angle)*(1 - n*n), 0 },
                { 0, 0, 0, 1}
                };
        }

        /// <summary>
        /// Generate scattered diffuse rays
        /// </summary>
        /// <param name="p"></param>
        /// <param name="Norm"></param>
        /// <param name="ringsCount"></param>
        /// <param name="ParalCount"></param>
        /// <returns></returns>
        public static List<Point3D> MakeDiffuseRays(Point3D p, Point3D Norm, int ringsCount, int ParalCount)
        {
            List<Point3D> rays = new List<Point3D>();
            rays.Add(Norm);

            double ringAngle = 90 / (ringsCount + 1);
            double ParalAngle = 360 / ParalCount;
            Point3D curRot = new Point3D(Norm);
            for (int i = 0; i < ringsCount; i++)
            {
                curRot = TransformPoint(curRot, Rotate(ringAngle, 'x'));
                for (int j = 0; j < ParalCount; j++)
                {
                    rays.Add(curRot);
                    curRot = TransformPoint(curRot, RotateAroundLineMatrix(p, p + Norm, ParalAngle));
                }
            }
            return rays;
        }
    }
}
