using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.DoubleNumerics;
using System.Windows.Forms;
using static RTTest1.Objects;
using static RTTest1.Utils;
using static RTTest1.Transformation;
using System.Diagnostics;
using System.IO;

namespace RTTest1
{
    public class Camera
    {
        public FormUpdater updateDelegate = null;
        public Point3D pos;
        public Point[] Screencoords;
        public Stopwatch stopwatch = new Stopwatch();
        Color AmbientColor = Color.DarkGray;
        public bool[] AARequired;


        public bool scatteredRaysActive = false;
        public bool forwardTracingActive = false;
        public bool edgesAntiAliasingActive = false;
        public bool postProcessAntiAliasingActive = false;


        public int textureResolution = 512;
        public int spotSize = 2;
        public int interpolationSize = 4;
        public int forwardShootingResolution = 16;
        public int depth = 3;
        public int postAATreshold = 15;
        public int supersamplingResolution = 5;

        public Camera(Point3D pos, Point lu, Point br)
        {
            this.pos = pos;
            Screencoords = new Point[] { lu, br };
        }

        public Point3D SetCamVector(int _x, int _y, int w, int h)
        {
            int x = _x - w / 2;
            int y = _y - h / 2;
            int z = (int)pos.Z;
            Point3D cameraVector = new Point3D(x, y, -z);
            double distance = Distance(cameraVector, new Point3D(0, 0, 0));
            cameraVector.X = cameraVector.X / distance;
            cameraVector.Y = cameraVector.Y / distance;
            cameraVector.Z = cameraVector.Z / distance;
            return cameraVector;
        }

        public Point3D SetCamNodeVector(int _x, int _y, int w, int h)
        {
            double x = _x - 0.5 - w / 2;
            double y = _y - 0.5 - h / 2;
            int z = (int)pos.Z;
            Point3D cameraVector = new Point3D(x, y, -z);
            double distance = Distance(cameraVector, new Point3D(0, 0, 0));
            cameraVector.X = cameraVector.X / distance;
            cameraVector.Y = cameraVector.Y / distance;
            cameraVector.Z = cameraVector.Z / distance;
            return cameraVector;
        }

        public List<Point3D> SetCamSupersamplingVectors(int _x, int _y, int w, int h, int dim)
        {
            List<Point3D> res = new List<Point3D>();
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                {
                    double x = _x + ((dim < 2) ? 0 : -0.5 + i * (1.0 / (dim-1))) - w / 2;
                    double y = _y + ((dim < 2) ? 0 : -0.5 + j * (1.0 / (dim-1))) - h / 2;
                    int z = (int)pos.Z;
                    Point3D cameraVector = new Point3D(x, y, -z);
                    double distance = Distance(cameraVector, new Point3D(0, 0, 0));
                    cameraVector.X = cameraVector.X / distance;
                    cameraVector.Y = cameraVector.Y / distance;
                    cameraVector.Z = cameraVector.Z / distance;
                    res.Add(cameraVector);
                }
            return res;
        }

        bool colorDifferenceExceedsThreshold(Color c1, Color c2, int treshold)
        {
            return
                Math.Abs(c1.R - c2.R) > treshold ||
                Math.Abs(c1.G - c2.G) > treshold ||
                Math.Abs(c1.B - c2.B) > treshold;
        }

        public void Render(Bitmap bm, PictureBox pb)
        {
            Graphics g = null;
            g = Graphics.FromImage(bm);
            g.Clear(Color.Transparent);

            Point3D cameraLocation = new Point3D(pos);

            int l = Math.Max(0, Screencoords[0].X);
            int r = Math.Min(bm.Width, Screencoords[1].X);

            int u = Math.Max(0, Screencoords[0].Y);
            int d = Math.Min(bm.Height, Screencoords[1].Y);

            int w = r - l;
            int h = d - u;
            int total = w * h;
            stopwatch.Reset();
            stopwatch.Start();
            int pDone = 0;

            var stamp1 = stopwatch.Elapsed;

            //Antialiasing preprocess variant
            if (edgesAntiAliasingActive)
            {
                AARequired = new bool[w * h];

                int[] hitIndData = new int[(w + 1)* (h + 1) * 3];

                int shapeType, ind, polInd;

                for (int i = l; i <= r; i++)
                    for (int j = u; j <= d; j++)
                    {
                        shapeType = -1; ind = -1; polInd = -1;
                        bool found = CollisionCheckRayCast(cameraLocation, SetCamNodeVector(i, j, bm.Width, bm.Height),
                            ref shapeType, ref ind, ref polInd);

                        if (found)
                        {
                            int index = ((i - l) * (h + 1) + (j - u)) * 3;
                            hitIndData[index] = shapeType;
                            hitIndData[index+1] = ind;
                            hitIndData[index+2] = polInd;

                        }
                    }

                for (int i = l; i < r; i++)
                    for (int j = u; j < d; j++)
                    {
                        int _i = i - l;
                        int _j = j - u;

                        int AAind = _i * h + _j;

                        int index1 = ((_i) * (h + 1) + (_j)) * 3;
                        int index2 = ((_i) * (h + 1) + (_j + 1)) * 3;
                        int index3 = ((_i + 1) * (h + 1) + (_j)) * 3;
                        int index4 = ((_i + 1) * (h + 1) + (_j + 1)) * 3;

                        //Check for shape types
                        bool shapeCheck =
                            hitIndData[index1] == hitIndData[index2] &&
                            hitIndData[index2] == hitIndData[index3] &&
                            hitIndData[index3] == hitIndData[index4];

                        if (shapeCheck)
                        {

                            //Check for shape indices
                            bool indCheck =
                                hitIndData[index1 + 1] == hitIndData[index2 + 1] &&
                                hitIndData[index2 + 1] == hitIndData[index3 + 1] &&
                                hitIndData[index3 + 1] == hitIndData[index4 + 1];

                            if (indCheck)
                            {
                                //if sphere - no further check 
                                if (hitIndData[index1] == 1)
                                {
                                    AARequired[AAind] = false;
                                }
                                else
                                {
                                    //Check for shape indices
                                    bool polIndCheck =
                                        hitIndData[index1 + 2] == hitIndData[index2 + 2] &&
                                        hitIndData[index2 + 2] == hitIndData[index3 + 2] &&
                                        hitIndData[index3 + 2] == hitIndData[index4 + 2];
                                    if (polIndCheck)
                                        AARequired[AAind] = false;
                                    else
                                    {
                                        AARequired[AAind] = true;
                                    }
                                }
                            }
                            else
                            {
                               AARequired[AAind] = true;
                            }
                        }
                        else
                        {
                           AARequired[AAind] = true; 
                        }
                    }
            }

            var stamp2 = stopwatch.Elapsed;

            //Forward tracing for caustics on lightmaps
            if (forwardTracingActive)
            {
                SetLightmaps(textureResolution);
                foreach (Light light in lights)
                {

                    Color rayColor = Color.FromArgb(
                        (int)(light.color.R * light.intensity),
                        (int)(light.color.G * light.intensity),
                        (int)(light.color.B * light.intensity));

                    for (int mi = 0; mi < meshes.Count(); mi++)
                    {
                        if (meshes[mi].material.parameters[2] > 0 || meshes[mi].material.parameters[3] > 0)
                        {
                            Point3D centerPoint = meshes[mi].GetCenterPoint();
                            Point3D pos = light.pos;
                            Point3D dir = centerPoint - pos;
                            Point3D targetNormal = Normalize(pos + centerPoint);

                            double cos = meshes[mi].points.Select(x => CosBetween(dir, x - pos)).Min();
                            double radius = dir.Length() * Math.Sqrt(1 - cos * cos) / cos;
                            double D =
                                -targetNormal.X * centerPoint.X
                                - targetNormal.Y * centerPoint.Y
                                - targetNormal.Z * centerPoint.Z;

                            double z = -(targetNormal.X + targetNormal.Y + D) / targetNormal.Z;
                            Point3D xpoint = new Point3D(1, 1, z);

                            Point3D xvector = Normalize(xpoint - centerPoint) * (radius / forwardShootingResolution);
                            Point3D yvector = Normalize(VectorCrossProduct(targetNormal, xvector)) * (radius / forwardShootingResolution);

                            int halfres = forwardShootingResolution / 2;

                            for (int i = -halfres; i <= halfres; i++)
                            {
                                for (int j = -halfres; j <= halfres; j++)
                                {
                                    Point3D target = centerPoint + xvector * i + yvector * j;
                                    ForwardRayCast(pos, Normalize(target - pos), rayColor, depth, 0, 0, mi);
                                }
                            }
                        }
                    }

                    for (int si = 0; si < spheres.Count; si++)
                    {
                        if (spheres[si].material.parameters[3] > 0) //Only glass spheres
                        {
                            Point3D centerPoint = spheres[si].pos;
                            Point3D pos = light.pos;
                            Point3D dir = centerPoint - pos;
                            Point3D targetNormal = Normalize(pos + centerPoint);

                            double radius = spheres[si].radius;
                            double D =
                                -targetNormal.X * centerPoint.X
                                - targetNormal.Y * centerPoint.Y
                                - targetNormal.Z * centerPoint.Z;

                            double z = -(targetNormal.X + targetNormal.Y + D) / targetNormal.Z;
                            Point3D xpoint = new Point3D(1, 1, z);

                            Point3D xvector = Normalize(xpoint - centerPoint) * (radius / forwardShootingResolution);
                            Point3D yvector = Normalize(VectorCrossProduct(targetNormal, xvector)) * (radius / forwardShootingResolution);

                            int halfres = forwardShootingResolution / 2;

                            for (int i = -halfres; i <= halfres; i++)
                            {
                                for (int j = -halfres; j <= halfres; j++)
                                {
                                    Point3D target = centerPoint + xvector * i + yvector * j;
                                    ForwardRayCast(pos, Normalize(target - pos), rayColor, depth, 0, 1, si);
                                }
                            }

                        }
                    }
                }
            }

            var stamp3 = stopwatch.Elapsed;

            for (int i = l; i < r; i++)
                for (int j = u; j < d; j++)
                {

                    if (edgesAntiAliasingActive && AARequired[(i - l) * h + (j - u)])
                    {
                        List<Point3D> ssVectors = SetCamSupersamplingVectors(i, j, bm.Width, bm.Height, supersamplingResolution);
                        List<Color> ssColors = ssVectors
                            .Select(x => RayCast(cameraLocation, x, depth)).ToList();

                        bool borderlandsShader = false;

                        int ssR = 0, ssG = 0, ssB = 0;

                        //fun
                        if (!borderlandsShader)
                        {
                            foreach (Color c in ssColors)
                            {
                                ssR += c.R;
                                ssG += c.G;
                                ssB += c.B;
                            }
                        }
                        else
                        {
                            ssR = ssColors[0].R + ssColors[1].R + ssColors[2].R + ssColors[3].R;
                            ssG = ssColors[0].G + ssColors[1].G + ssColors[2].G + ssColors[3].G;
                            ssB = ssColors[0].B + ssColors[1].B + ssColors[2].B + ssColors[3].B;
                        }

                        int ssq = supersamplingResolution * supersamplingResolution;

                        ssR /= ssq; ssG /= ssq; ssB /= ssq;

                        bm.SetPixel(i, j, Color.FromArgb(ssR, ssG, ssB));
                    }
                    else
                    {
                        bm.SetPixel(i, j, RayCast(cameraLocation, SetCamVector(i, j, bm.Width, bm.Height), depth));
                    }

                    pDone++;
                    //updateDelegate(((double)pDone) / total, stopwatch.Elapsed);
                }

            var stamp4 = stopwatch.Elapsed;

            if (postProcessAntiAliasingActive)
            {
                AARequired = new bool[w * h];

                for (int i = l; i < r - 1; i++)
                    for (int j = u; j < d - 1; j++)
                    {
                        Color c = bm.GetPixel(i, j);
                        Color c1;
                        c1 = bm.GetPixel(i, j + 1);
                        if (colorDifferenceExceedsThreshold(c, c1, postAATreshold))
                        {
                            AARequired[(i - l) * h + (j - u)] = true;
                            AARequired[(i - l) * h + (j + 1 - u)] = true;
                        }
                        c1 = bm.GetPixel(i + 1, j);
                        if (colorDifferenceExceedsThreshold(c, c1, postAATreshold))
                        {
                            AARequired[(i - l) * h + (j - u)] = true;
                            AARequired[(i + 1 - l) * h + (j - u)] = true;
                        }
                        c1 = bm.GetPixel(i + 1, j + 1);
                        if (colorDifferenceExceedsThreshold(c, c1, postAATreshold))
                        {
                            AARequired[(i - l) * h + (j - u)] = true;
                            AARequired[(i + 1 - l) * h + (j + 1 - u)] = true;
                        }

                    }

                pDone = 0;

                for (int i = l; i < r; i++)
                    for (int j = u; j < d; j++)
                    {
                        if (AARequired[(i - l) * h + (j - u)])
                        {
                            List<Point3D> ssVectors = SetCamSupersamplingVectors(i, j, bm.Width, bm.Height, supersamplingResolution);
                            List<Color> ssColors = ssVectors
                                .Select(x => RayCast(cameraLocation, x, depth)).ToList();

                            bool borderlandsShader = false;

                            int ssR = 0, ssG = 0, ssB = 0;

                            //fun
                            if (!borderlandsShader)
                            {
                                foreach (Color c in ssColors)
                                {
                                    ssR += c.R;
                                    ssG += c.G;
                                    ssB += c.B;
                                }
                            }
                            else
                            {
                                ssR = ssColors[0].R + ssColors[1].R + ssColors[2].R + ssColors[3].R;
                                ssG = ssColors[0].G + ssColors[1].G + ssColors[2].G + ssColors[3].G;
                                ssB = ssColors[0].B + ssColors[1].B + ssColors[2].B + ssColors[3].B;
                            }

                            int ssq = supersamplingResolution * supersamplingResolution;

                            ssR /= ssq; ssG /= ssq; ssB /= ssq;

                            bm.SetPixel(i, j, Color.FromArgb(ssR, ssG, ssB));
                        }

                        pDone++;
                        updateDelegate(((double)pDone) / total, stopwatch.Elapsed);
                    }

            }

            var stamp5 = stopwatch.Elapsed;

            stopwatch.Stop();


            File.WriteAllText( "time.txt",
                "edgespreprocess: " + (stamp2 - stamp1).ToString() +
                "\nforwardTracing: " + (stamp3 - stamp2).ToString() +
                "\nbackwardTracing: " + (stamp4 - stamp3).ToString() +
                "\npostProcess: " + (stamp5 - stamp4).ToString() +
                "\nTotal: " + (stamp5).ToString()
                );

            pb.Refresh();
        }

        public Color RayCast(Point3D pos, Point3D dir, int depth)
        {
            if (depth == 0) return Color.Black;
            Point3D hit = new Point3D();
            Point3D normal = new Point3D();
            Material hitMaterial = new Material();
            double minDist = double.MaxValue;
            bool found = false;
            double eps = 0.001;

            double CausticR = 0;
            double CausticG = 0;
            double CausticB = 0;


            foreach (Mesh m in meshes)
            {
                foreach (Polygon pol in m.faces)
                {
                    double t = 0;
                    Point3D P = new Point3D();
                    if (!pol.Ray_intersection(pos, dir, ref P, ref t))
                        continue;


                    if (t < minDist)
                    {
                        found = true;
                        minDist = t;
                        hit = P;
                        normal = pol.normal;
                        hitMaterial = m.material;

                        if (forwardTracingActive)
                        {
                            Point3D coords = HitToCoords(hit, pol);

                            int x = (coords.X >= 1) ? textureResolution - 1 : ((coords.X <= 0) ? 0 : (int)(textureResolution * coords.X));
                            int y = (coords.Y >= 1) ? textureResolution - 1 : ((coords.X <= 0) ? 0 : (int)(textureResolution * coords.Y));


                            Color Caustic = GetColorInterpolated(pol.lightmap, x, y, interpolationSize);

                            CausticR = Caustic.R;
                            CausticG = Caustic.G;
                            CausticB = Caustic.B;

                        }
                    }
                }
            }
            foreach (Sphere sphere in spheres)
            {
                double t = 0;
                if (sphere.ray_intersection(pos, dir, ref t))
                {
                    if (t < minDist)
                    {
                        found = true;
                        minDist = t;
                        hit = pos + dir * t;
                        normal = Normalize(hit - sphere.pos);
                        hitMaterial = sphere.material;
                    }
                }
            }

            if (found)
            {
                double difIntensitySumR = 0;
                double difIntensitySumG = 0;
                double difIntensitySumB = 0;
                double specIntensitySum = 0;

                double ReflectionR = 0;
                double ReflectionG = 0;
                double ReflectionB = 0;

                double RefractionR = 0;
                double RefractionG = 0;
                double RefractionB = 0;

                Color DiffuseReflectedCol = AmbientColor;

                if (hitMaterial.parameters[3] > 0)
                {
                    Point3D RefractRay = Normalize(RayRefract(dir, normal, hitMaterial.refractionIndex));
                    Point3D RefractLoc = RefractRay * normal < 0 ? hit - normal * eps : hit + normal * eps;
                    Color RefractCol = RayCast(RefractLoc, RefractRay, depth - 1);

                    RefractionR = RefractCol.R * hitMaterial.parameters[3];
                    RefractionG = RefractCol.G * hitMaterial.parameters[3];
                    RefractionB = RefractCol.B * hitMaterial.parameters[3];
                }

                if (hitMaterial.parameters[2] > 0)
                {
                    Point3D ReflectRay = RayReflect(dir, normal);
                    Point3D ReflectLoc = ReflectRay * normal < 0 ? hit - normal * eps : hit + normal * eps;
                    Color ReflectCol = RayCast(ReflectLoc, ReflectRay, depth - 1);

                    ReflectionR = ReflectCol.R * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]);
                    ReflectionG = ReflectCol.G * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]);
                    ReflectionB = ReflectCol.B * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]);
                }

                if (hitMaterial.parameters[0] > 0)
                    if (scatteredRaysActive)
                    {
                        List<Point3D> DiffuseRays = MakeDiffuseRays(hit, normal, 2, 4); // 4 8
                        Point3D DiffuseLoc = DiffuseRays[0] * normal < 0 ? hit - normal * eps : hit + normal * eps;
                        double[] colSum = new double[3] { 0, 0, 0 };

                        foreach (var DiffuseRay in DiffuseRays)
                        {
                            double coef = CosBetween(DiffuseRay, normal);
                            if (coef > 0)
                            {
                                Color curDifCol = RayCast(DiffuseLoc, DiffuseRay, depth - 1);
                                colSum[0] += curDifCol.R * coef;
                                colSum[1] += curDifCol.G * coef;
                                colSum[2] += curDifCol.B * coef;
                            }
                        }
                        colSum[0] /= DiffuseRays.Count;
                        colSum[1] /= DiffuseRays.Count;
                        colSum[2] /= DiffuseRays.Count;
                        DiffuseReflectedCol = Color.FromArgb((int)colSum[0], (int)colSum[1], (int)colSum[2]);
                    }
                    else
                    { }

                foreach (Light l in lights)
                {
                    Point3D lightVecFull = l.pos - hit;
                    double light_distance = lightVecFull.Length();
                    Point3D lightVec = Normalize(lightVecFull);


                    Point3D shadow_origin = lightVec * normal < 0 ? hit - normal * eps : hit + normal * eps;
                    Point3D shadow_destination = new Point3D();
                    Point3D shadow_N = new Point3D();
                    Material tmat = new Material();
                    if (FindIntersection(shadow_origin, lightVec, ref shadow_destination, ref shadow_N, ref tmat) && (shadow_destination - shadow_origin).Length() < light_distance)
                        continue;

                    double intensity = LightFalloff(light_distance, 800) * l.intensity * Math.Max(0.0, (lightVec * normal));
                    difIntensitySumR += (l.color.R / 255.0) * intensity;
                    difIntensitySumG += (l.color.G / 255.0) * intensity;
                    difIntensitySumB += (l.color.B / 255.0) * intensity;
                    specIntensitySum += Math.Pow(Math.Max(0.0, (RayReflect(lightVec, normal) * dir)), hitMaterial.specularHighlight) * l.intensity;
                }

                if (forwardTracingActive)
                {
                    difIntensitySumR += (CausticR / 255.0);
                    difIntensitySumG += (CausticG / 255.0);
                    difIntensitySumB += (CausticB / 255.0);
                }

                double lightnessR = difIntensitySumR * (1 - hitMaterial.parameters[2]) * (1 - hitMaterial.parameters[3]);
                double lightnessG = difIntensitySumG * (1 - hitMaterial.parameters[2]) * (1 - hitMaterial.parameters[3]);
                double lightnessB = difIntensitySumB * (1 - hitMaterial.parameters[2]) * (1 - hitMaterial.parameters[3]);

                double addspec = 255.0 * specIntensitySum * hitMaterial.parameters[1];

                double colR = hitMaterial.color.R * (1 - hitMaterial.parameters[0]) * lightnessR + DiffuseReflectedCol.R * hitMaterial.parameters[0];
                double colG = hitMaterial.color.G * (1 - hitMaterial.parameters[0]) * lightnessG + DiffuseReflectedCol.G * hitMaterial.parameters[0];
                double colB = hitMaterial.color.B * (1 - hitMaterial.parameters[0]) * lightnessB + DiffuseReflectedCol.B * hitMaterial.parameters[0];

                int r = (int)(colR + addspec + ReflectionR + RefractionR);
                r = r > 255 ? 255 : r;
                int g = (int)(colG + addspec + ReflectionG + RefractionG);
                g = g > 255 ? 255 : g;
                int b = (int)(colB + addspec + ReflectionB + RefractionB);
                b = b > 255 ? 255 : b;
                return Color.FromArgb(r, g, b);
            }
            return AmbientColor;
        }

        public void ForwardRayCast(Point3D pos, Point3D dir, Color c, int depth, int iter, int shapeType = 0, int ind = 0)
        {
            if (depth == 0) return;
            Point3D hit = new Point3D();
            Point3D normal = new Point3D();
            Material hitMaterial = new Material();
            double minDist = double.MaxValue;
            bool found = false;
            double eps = 0.001;
            int polInd = 0;

            if (iter == 0)
            {
                if (shapeType == 0)
                {
                    foreach (Polygon pol in meshes[ind].faces)
                    {
                        double t = 0;
                        Point3D P = new Point3D();
                        if (!pol.Ray_intersection(pos, dir, ref P, ref t))
                            continue;


                        if (t < minDist)
                        {
                            found = true;
                            minDist = t;
                            hit = P;
                            normal = pol.normal;
                            hitMaterial = meshes[ind].material;
                        }
                    }
                    if (!found) return;
                }
                else
                {
                    double t = 0;
                    if (spheres[ind].ray_intersection(pos, dir, ref t))
                    {
                        if (t < minDist)
                        {
                            found = true;
                            minDist = t;
                            hit = pos + dir * t;
                            normal = Normalize(hit - spheres[ind].pos);
                            hitMaterial = spheres[ind].material;
                        }
                    }
                    else return;
                }
            }
            else
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    for (int j = 0; j < meshes[i].faces.Count; j++)
                    {
                        double t = 0;
                        Point3D P = new Point3D();
                        if (!meshes[i].faces[j].Ray_intersection(pos, dir, ref P, ref t))
                            continue;


                        if (t < minDist)
                        {
                            found = true;
                            minDist = t;
                            hit = P;
                            normal = meshes[i].faces[j].normal;
                            hitMaterial = meshes[i].material;
                            shapeType = 0;
                            ind = i;
                            polInd = j;
                        }
                    }

                }
                for (int i = 0; i < spheres.Count; i++)
                {

                    double t = 0;
                    if (spheres[i].ray_intersection(pos, dir, ref t))
                    {
                        if (t < minDist)
                        {
                            found = true;
                            minDist = t;
                            hit = pos + dir * t;
                            normal = Normalize(hit - spheres[i].pos);
                            hitMaterial = spheres[i].material;
                            shapeType = 1;
                            ind = i;
                        }
                    }

                }
            }

            if (found)
            {
                double ReflectionR = 0;
                double ReflectionG = 0;
                double ReflectionB = 0;

                double RefractionR = 0;
                double RefractionG = 0;
                double RefractionB = 0;

                Point3D lightVecFull = pos - hit;
                double light_distance = lightVecFull.Length();
                Point3D lightVec = Normalize(lightVecFull);

                //ACCOUNT FOR LFO
                double intensity = LightFalloff(light_distance, 800);

                if (hitMaterial.parameters[3] > 0)
                {
                    Point3D RefractRay = Normalize(RayRefract(dir, normal, hitMaterial.refractionIndex));
                    Point3D RefractLoc = RefractRay * normal < 0 ? hit - normal * eps : hit + normal * eps;


                    RefractionR = c.R * hitMaterial.parameters[3] * intensity;
                    RefractionG = c.G * hitMaterial.parameters[3] * intensity;
                    RefractionB = c.B * hitMaterial.parameters[3] * intensity;

                    Color RefractionCol = Color.FromArgb((int)RefractionR, (int)RefractionG, (int)RefractionB);

                    ForwardRayCast(RefractLoc, RefractRay, RefractionCol, depth - 1, iter + 1);
                }

                if (hitMaterial.parameters[2] > 0 && hitMaterial.parameters[3] == 0 && shapeType == 0 && false) //DISABLED 
                {
                    Point3D ReflectRay = RayReflect(dir, normal);
                    Point3D ReflectLoc = ReflectRay * normal < 0 ? hit - normal * eps : hit + normal * eps;

                    ReflectionR = c.R * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]) * intensity;
                    ReflectionG = c.G * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]) * intensity;
                    ReflectionB = c.B * hitMaterial.parameters[2] * (1 - hitMaterial.parameters[3]) * intensity;

                    Color ReflectionCol = Color.FromArgb((int)ReflectionR, (int)ReflectionG, (int)ReflectionB);

                    ForwardRayCast(ReflectLoc, ReflectRay, ReflectionCol, depth - 1, iter + 1);
                }

                if (hitMaterial.parameters[2] == 0 && hitMaterial.parameters[3] == 0) //FOR DIFFUSE SURFACES - STORE CAUSTICS
                {
                    Point3D coords;

                    //ACCOUNT FOR LFO + ANGLE
                    double fallIntensity = intensity * Math.Max(0.0, (lightVec * normal));

                    Color fallColor = Color.FromArgb(
                        (int)(c.R * fallIntensity),
                        (int)(c.G * fallIntensity),
                        (int)(c.B * fallIntensity));


                    if (shapeType == 0) //lock on mesh
                    {
                        coords = HitToCoords(hit, meshes[ind].faces[polInd]);

                        int x = (coords.X >= 1) ? textureResolution - 1 : ((coords.X <= 0) ? 0 : (int)(textureResolution * coords.X));
                        int y = (coords.Y >= 1) ? textureResolution - 1 : ((coords.X <= 0) ? 0 : (int)(textureResolution * coords.Y));

                        MakeSpot(ref meshes[ind].faces[polInd].lightmap, x, y, fallColor, spotSize);
                    }
                    else // sphere - disabled
                    {
                    }

                    //double intensity = LightFalloff(light_distance, 800) * l.intensity * Math.Max(0.0, (lightVec * normal));
                }
            }
        }

        public bool CollisionCheckRayCast(Point3D pos, Point3D dir, ref int shapeType, ref int ind, ref int polInd)
        {
            double minDist = double.MaxValue;
            bool found = false;

            for (int i = 0; i < meshes.Count; i++)
            {
                for (int j = 0; j < meshes[i].faces.Count; j++)
                {
                    double t = 0;
                    Point3D P = new Point3D();
                    if (!meshes[i].faces[j].Ray_intersection(pos, dir, ref P, ref t))
                        continue;


                    if (t < minDist)
                    {
                        found = true;
                        minDist = t;
                        shapeType = 0;
                        ind = i;
                        polInd = j;
                    }
                }

            }
            for (int i = 0; i < spheres.Count; i++)
            {

                double t = 0;
                if (spheres[i].ray_intersection(pos, dir, ref t))
                {
                    if (t < minDist)
                    {
                        found = true;
                        minDist = t;
                        shapeType = 1;
                        ind = i;
                    }
                }

            }

            return found;
        }
    }
}
