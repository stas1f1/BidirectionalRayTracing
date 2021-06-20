using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.DoubleNumerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum RayType {Reflection, Diffuse, Transparent, Specular}

namespace RTTest1
{
    public class Ray
    {
        /// <summary>
        /// Ray position
        /// </summary>
        public Vector3 pos;

        /// <summary>
        /// Ray direction
        /// </summary>
        public Vector3 dir;

        /// <summary>
        /// Depth of ray in tracing tree hierarchy
        /// </summary>
        public int iter;

        /// <summary>
        /// Type of ray (Reflection, Diffuse, Transparent, Specular)
        /// </summary>
        public RayType type;

        /// <summary>
        /// Constructs ray
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="iter"></param>
        public Ray(Vector3 pos, Vector3 dir, int iter = 0)
        {
            this.pos = pos;
            this.dir = dir;
            this.iter = iter;
        }
        

        //Hit checking methods

        /// <summary>
        /// Finds whether ray hits SceneObject or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Hits(ISceneObject o)
        {
            switch (o)
            {
                case Box b:
                    return Hits(b);
                case Sphere s:
                    return Hits(s);
                case Plane p:
                    return Hits(p);

                default:
                    throw new ArgumentException("Wrong Type");
            }
        }

        /// <summary>
        /// Finds whether ray hits box or not
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Hits(Box b)
        {
            return b.planes.Any(Hits);
        }

        /// <summary>
        /// Finds whether ray hits plane or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Hits(Plane p)
        {
            //Calculates divisor for intersection point equasion 
            double divisor = p.A * dir.X + p.B * dir.Y + p.C * dir.Z;

            //Divisor = 0 means ray is parallel or within surface
            if (divisor == 0) return false;

            //Calculates lambda-parameter for intersection point equasion
            double lambda = - (p.A * pos.X + p.B * pos.Y + p.C * pos.Z + p.D) / divisor;

            //Lambda = 0 means ray starts at surface, lambda < 0 means ray
            //intersects behind starting point
            if (lambda <= 0) return false;

            Vector3 intersectionPoint = pos + dir * lambda;

            //Checking whether intersecting point is located on the plane
            //Due to values being floating point, uses accuracy comparison
            if (
                (
                    Math.Abs(p.ul.X - intersectionPoint.X) < Config.Eps ||
                    Math.Abs(p.lr.X - intersectionPoint.X) < Config.Eps ||
                    (p.ul.X - intersectionPoint.X) * (p.lr.X - intersectionPoint.X) <= 0
                )
                &&
                (
                    Math.Abs(p.ul.Y - intersectionPoint.Y) < Config.Eps ||
                    Math.Abs(p.lr.Y - intersectionPoint.Y) < Config.Eps ||
                    (p.ul.Y - intersectionPoint.Y) * (p.lr.Y - intersectionPoint.Y) <= 0
                )
                &&
                (
                    Math.Abs(p.ul.Z - intersectionPoint.Z) < Config.Eps ||
                    Math.Abs(p.lr.Z - intersectionPoint.Z) < Config.Eps ||
                    (p.ul.Z - intersectionPoint.Z) * (p.lr.Z - intersectionPoint.Z) <= 0
                )
               )
                return true;
            else return false;
        }

        /// <summary>
        /// Finds whether ray hits sphere or not
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool Hits(Sphere s)
        {
            double lambdaH = 
                (dir.X * (s.pos.X - pos.X) +
                dir.Y * (s.pos.Y - pos.Y) +
                dir.Z * (s.pos.Z - pos.Z)) /
                (Math.Pow(dir.X, 2) + Math.Pow(dir.Y, 2) + Math.Pow(dir.Z, 2));

            Vector3 basePoint = pos + lambdaH * dir;

            return (basePoint - s.pos).Length() < s.radius;
        }

        /// <summary>
        /// Finds whether ray hits light source or not
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public bool Hits(LightSource l)
        {
            return dir == Vector3.Normalize(l.pos - pos);
        }


        //Ray intersecton methods

        /// <summary>
        /// Get SceneObject intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 Intersection(ISceneObject o)
        {
            switch (o)
            {
                case Box b:
                    return Intersection(b);
                case Sphere s:
                    return Intersection(s);
                case Plane p:
                    return Intersection(p);

                default:
                    throw new ArgumentException("Wrong Type");
            }
        }

        /// <summary>
        /// Get plane intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 Intersection(Plane p)
        {
            double divisor = p.A * dir.X + p.B * dir.Y + p.C * dir.Z;
            double lambda = - (p.A * pos.X + p.B * pos.Y + p.C * pos.Z + p.D) / divisor;
            return pos + dir * lambda;
        }

        /// <summary>
        /// Get plane intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector3 Intersection(Box b)
        {
            return Intersection(BoxIntersectionPlane(b));
        }

        /// <summary>
        /// Get sphere intersection point (assuming ray hits sphere)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector3 Intersection(Sphere s)
        {
            //solving quadratic equasion to find lambda for intersecton point
             double a = dir.LengthSquared();
             double b = 2 * (
                 dir.X * (pos.X - s.pos.X) +
                 dir.Y * (pos.Y - s.pos.Y) +
                 dir.Z * (pos.Z - s.pos.Z));
             double c =
                 Math.Pow(pos.X - s.pos.X, 2) +
                 Math.Pow(pos.Y - s.pos.Y, 2) +
                 Math.Pow(pos.Z - s.pos.Z, 2) -
                 Math.Pow(s.radius, 2);

             double disc = b*b - 4 * a * c;
             double lambda = (-b - Math.Sqrt(disc)) / (2 * a);

            //Console.WriteLine(Vector3.Distance(pos + lambda * dir, pos));
             return pos + lambda * dir;

            /*Vector3 sv = s.pos - pos;

            double ds = 
                dir.X * sv.X +
                dir.Y * sv.Y +
                dir.Z * sv.Z;

            double disc = ds * ds -
                (dir.LengthSquared() * (sv.LengthSquared() - s.radius * s.radius));



            double lambda = (ds - Math.Sqrt(disc)) / dir.LengthSquared();

            return pos + lambda * dir;*/
        }

        /// <summary>
        /// Get light source intersection point (assuming ray hits light source)
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public Vector3 Intersection(LightSource l)
        {
            return l.pos;
        }


        //Ray furthest intersecton methods

        /// <summary>
        /// Get SceneObject furthest intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 FurthestIntersection(ISceneObject o)
        {
            switch (o)
            {
                case Box b:
                    return FurthestIntersection(b);
                case Sphere s:
                    return FurthestIntersection(s);

                default:
                    throw new ArgumentException("Wrong Type");
            }
        }

        /// <summary>
        /// Get sphere furthest intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 FurthestIntersection(Sphere s)
        {
            //solving quadratic equasion to find lambda for intersecton point
            double a = dir.LengthSquared();
            double b = 2 * (
                dir.X * (pos.X - s.pos.X) +
                dir.Y * (pos.Y - s.pos.Y) +
                dir.Z * (pos.Z - s.pos.Z));
            double c = -2 * (
                pos.X * s.pos.X +
                pos.Y * s.pos.Y +
                pos.Z * s.pos.Z
                ) - Math.Pow(s.radius, 2);

            double disc = b * b - 4 * a * c;
            double lambda = (-b + Math.Sqrt(disc)) / (2 * a);

            return pos + lambda * dir;
        }

        /// <summary>
        /// Get box furthest intersection point (assuming ray hits plane)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector3 FurthestIntersection(Box b)
        {
            return Intersection(BoxFurthestIntersectionPlane(b));
        }


        //Reflection ray methods

        /// <summary>
        /// Get SceneObject reflection ray (assuming ray hits plane)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public Ray ReflectionRay(ISceneObject o)
        {
            switch (o)
            {
                case Box b:
                    return ReflectionRay(b);
                case Sphere s:
                    return ReflectionRay(s);
                case Plane p:
                    return ReflectionRay(p);

                default:
                    throw new ArgumentException("Wrong Type");
            }
        }

        /// <summary>
        /// Get plane reflection ray (assuming ray hits plane)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Ray ReflectionRay(Plane p)
        {
            Vector3 intersectionPoint = Intersection(p);
            Vector3 normal = p.FacingNormalVector(pos);

            double lambdaH = (
                normal.X * (pos.X - intersectionPoint.X) +
                normal.Y * (pos.Y - intersectionPoint.Y) +
                normal.Z * (pos.Z - intersectionPoint.Z)) /
                (Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));

            Vector3 halfPoint = intersectionPoint + normal * lambdaH;
            Vector3 refDir = 2 * halfPoint - pos;
            
            return new Ray(intersectionPoint, Vector3.Normalize(refDir - intersectionPoint), iter + 1);
        }
        
        /// <summary>
        /// Get box reflection ray (assuming ray hits box)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Ray ReflectionRay(Box b)
        {
            return ReflectionRay(BoxIntersectionPlane(b));
        }

        /// <summary>
        /// Get sphere reflection ray (assuming ray hits sphere)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Ray ReflectionRay(Sphere s)
        {
            Vector3 intersectionPoint = Intersection(s);
            Vector3 normal = intersectionPoint - s.pos;
            
            double lambdaH = (
                normal.X * (pos.X - intersectionPoint.X) +
                normal.Y * (pos.Y - intersectionPoint.Y) +
                normal.Z * (pos.Z - intersectionPoint.Z)) /
                (Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            
            Vector3 halfPoint = intersectionPoint + 2 * lambdaH * normal;
            Vector3 refDir = halfPoint + dir;

            return new Ray(intersectionPoint, Vector3.Normalize(refDir - intersectionPoint), iter + 1);
        }


        //Refraction ray methods

        /// <summary>
        /// Get SceneObject refraction ray (assuming ray hits plane)
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public Ray RefractionRay(ISceneObject o)
        {
            switch (o)
            {
                case Box b:
                    return RefractionRay(b);
                case Sphere s:
                    return RefractionRay(s);

                default:
                    throw new ArgumentException("Wrong Type");
            }
        }

        /// <summary>
        /// Get refracted ray coming out of the box (assuming ray hits box)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Ray RefractionRay(Box b)
        {
            //Vector3.Normalize(dir);
            Plane p = BoxIntersectionPlane(b);
            Vector3 intersectionPoint = Intersection(p);

            Vector3 normal = p.FacingNormalVector(pos);

            //calculating sin for the falling ray
            double fallingCos = -(dir.X * normal.X + dir.Y * normal.Y + dir.Z * normal.Z) / (dir.Length() * normal.Length());
            double fallingSin = Math.Sqrt(1 - fallingCos * fallingCos);
            double fallingTg = fallingSin / fallingCos;

            double refSin = fallingSin / Config.GlassRefractionCoefficient;
            double refCos = Math.Sqrt(1 - refSin * refSin);
            double refTg = refSin / refCos;

            double lambdaH = (
                normal.X * (pos.X - intersectionPoint.X) +
                normal.Y * (pos.Y - intersectionPoint.Y) +
                normal.Z * (pos.Z - intersectionPoint.Z)) /
                (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            Vector3 basePoint = intersectionPoint + lambdaH * normal;

            //Finding refraction ray direction
            double IPCoef = refTg / fallingTg;
            Vector3 refDir = pos * (IPCoef) + basePoint * (1 - IPCoef);

            //Refracted ray inside the object
            Ray refRayIn = new Ray(intersectionPoint, Vector3.Normalize(intersectionPoint - refDir));
            refRayIn.type = RayType.Transparent;
            //return refRayIn;
            while (true)
            {
                List<Plane> intPlanes = b.planes.Where(refRayIn.Hits).ToList();
                if (intPlanes.Count == 0)
                    break;


                Plane intersectionPlane = intPlanes.First();

                intersectionPoint = refRayIn.Intersection(intersectionPlane);
                normal = intersectionPlane.FacingNormalVector(refRayIn.pos);

                //calculating sin for the falling ray
                fallingCos = -(refRayIn.dir.X * normal.X + refRayIn.dir.Y * normal.Y + refRayIn.dir.Z * normal.Z) / (refRayIn.dir.Length() * normal.Length());
                fallingSin = Math.Sqrt(1 - fallingCos * fallingCos);

                if (fallingSin >= 1.0 / Config.GlassRefractionCoefficient)
                {
                    double lambdaR = (
                        normal.X * (refRayIn.pos.X - intersectionPoint.X) +
                        normal.Y * (refRayIn.pos.Y - intersectionPoint.Y) +
                        normal.Z * (refRayIn.pos.Z - intersectionPoint.Z)) /
                        (Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));

                    Vector3 halfPoint = intersectionPoint + lambdaR * normal;
                    Vector3 RDir = 2 * halfPoint - refRayIn.pos;

                    refRayIn.pos = intersectionPoint + normal * Config.Eps;
                    refRayIn.dir = Vector3.Normalize(RDir - intersectionPoint);
                    refRayIn.type = RayType.Reflection;

                    /*
                    intPlanes = b.planes.Where(refRayIn.Hits).ToList();


                    intersectionPlane = intPlanes.First();
                    */
                }
                else
                {
                    refSin = fallingSin * Config.GlassRefractionCoefficient;
                    refRayIn.type = RayType.Transparent;
                    break;
                }
            }

            fallingTg = fallingSin / fallingCos;
            refCos = Math.Sqrt(1 - refSin * refSin);
            refTg = refSin / refCos;

            lambdaH = (
                normal.X * (refRayIn.pos.X - intersectionPoint.X) +
                normal.Y * (refRayIn.pos.Y - intersectionPoint.Y) +
                normal.Z * (refRayIn.pos.Z - intersectionPoint.Z)) /
                (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            basePoint = intersectionPoint + lambdaH * normal;

            //Finding refraction ray direction
            IPCoef = refTg / fallingTg;
            refDir = refRayIn.pos * (IPCoef) + basePoint * (1 - IPCoef);
            //Vector3.Normalize(refDir);

            //TODO: NEW IP?
            Ray outRay = new Ray(intersectionPoint, intersectionPoint - refDir);
            return outRay;

        }

        /// <summary>
        /// Get refracted ray coming out of the sphere (assuming ray hits sphere)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Ray RefractionRay(Sphere s)
        {
            Vector3 intersectionPoint = Intersection(s);

            Vector3 normal = intersectionPoint - s.pos;

            //calculating sin for the falling ray
            double fallingCos = -(dir.X * normal.X + dir.Y * normal.Y + dir.Z * normal.Z) / (dir.Length() * normal.Length());
            double fallingSin = Math.Sqrt(1 - fallingCos * fallingCos);
            double fallingTg = fallingSin / fallingCos;

            double refSin = fallingSin / Config.GlassRefractionCoefficient;
            double refCos = Math.Sqrt(1 - refSin * refSin);
            double refTg = refSin / refCos;

            double lambdaH = (
                normal.X * (pos.X - intersectionPoint.X) +
                normal.Y * (pos.Y - intersectionPoint.Y) +
                normal.Z * (pos.Z - intersectionPoint.Z)) /
                (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            Vector3 basePoint = intersectionPoint + lambdaH * normal;

            //Finding refraction ray direction
            double IPCoef = refTg / fallingTg;
            Vector3 refDir = pos * (IPCoef) + basePoint * (1 - IPCoef);

            //Refracted ray inside the object
            Ray refRayIn = new Ray(intersectionPoint, Vector3.Normalize(intersectionPoint - refDir));
            refRayIn.type = RayType.Transparent;
            //return refRayIn;
            while (true)
            {
                intersectionPoint = refRayIn.FurthestIntersection(s);
                normal = s.pos - intersectionPoint;

                //calculating sin for the falling ray
                fallingCos = -(refRayIn.dir.X * normal.X + refRayIn.dir.Y * normal.Y + refRayIn.dir.Z * normal.Z) / (refRayIn.dir.Length() * normal.Length());
                fallingSin = Math.Sqrt(1 - fallingCos * fallingCos);

                if (fallingSin >= 1.0 / Config.GlassRefractionCoefficient)
                {
                    double lambdaR = (
                        normal.X * (refRayIn.pos.X - intersectionPoint.X) +
                        normal.Y * (refRayIn.pos.Y - intersectionPoint.Y) +
                        normal.Z * (refRayIn.pos.Z - intersectionPoint.Z)) /
                        (Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));

                    Vector3 halfPoint = intersectionPoint + lambdaR * normal;
                    Vector3 RDir = 2 * halfPoint - refRayIn.pos;

                    refRayIn.pos = intersectionPoint + normal * Config.Eps;
                    refRayIn.dir = Vector3.Normalize(RDir - intersectionPoint);
                    refRayIn.type = RayType.Reflection;

                    /*
                    intPlanes = b.planes.Where(refRayIn.Hits).ToList();


                    intersectionPlane = intPlanes.First();
                    */
                }
                else
                {
                    refSin = fallingSin * Config.GlassRefractionCoefficient;
                    refRayIn.type = RayType.Transparent;
                    break;
                }
            }

            fallingTg = fallingSin / fallingCos;
            refCos = Math.Sqrt(1 - refSin * refSin);
            refTg = refSin / refCos;

            lambdaH = (
                normal.X * (refRayIn.pos.X - intersectionPoint.X) +
                normal.Y * (refRayIn.pos.Y - intersectionPoint.Y) +
                normal.Z * (refRayIn.pos.Z - intersectionPoint.Z)) /
                (normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

            basePoint = intersectionPoint + lambdaH * normal;

            //Finding refraction ray direction
            IPCoef = refTg / fallingTg;
            refDir = refRayIn.pos * (IPCoef) + basePoint * (1 - IPCoef);
            //Vector3.Normalize(refDir);

            //TODO: NEW IP?
            Ray outRay = new Ray(intersectionPoint, refDir - intersectionPoint); //?
            return outRay;

        }

        //Helper methods

        /// <summary>
        /// Finds the plane ray intersects first
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Plane BoxIntersectionPlane(Box b)
        {
            List<Plane> intersectingPlanes = b.planes.Where(Hits).ToList();
            //There will always be only 1 (ray casted inside box) or 2 planes (ouside)
            if (intersectingPlanes.Count == 1) return intersectingPlanes[0];
            double minLen = Vector3.Distance(pos, Intersection(intersectingPlanes[0]));
            Plane res = intersectingPlanes[0];
            foreach (Plane p in intersectingPlanes.Skip(1))
            {
                double len = Vector3.Distance(pos, Intersection(p));
                if (len < minLen)
                {
                    minLen = len;
                    res = p;
                }
            }
            return res;
        }

        /// <summary>
        /// Finds the plane ray intersects last
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Plane BoxFurthestIntersectionPlane(Box b)
        {
            List<Plane> intersectingPlanes = b.planes.Where(Hits).ToList();
            //There will always be only 1 (ray casted inside box) or 2 planes (ouside)
            if (intersectingPlanes.Count == 1) return intersectingPlanes[0];
            double maxLen = Vector3.Distance(pos, Intersection(intersectingPlanes[0]));
            Plane res = intersectingPlanes[0];
            foreach (Plane p in intersectingPlanes.Skip(1))
            {
                double len = Vector3.Distance(pos, Intersection(p));
                if (len > maxLen)
                {
                    maxLen = len;
                    res = p;
                }
            }
            return res;
        }
        
        /// <summary>
        /// Return first object ray intersects with
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public ISceneObject FirstIntersectingObject(List<ISceneObject> objects)
        {
            List<ISceneObject> hitObjects = objects
                .Where(Hits)
                .Where(o => (Vector3.Distance(Intersection(o), pos)) > Config.Eps).ToList();
            ISceneObject res = hitObjects.First();
            double minDist = Vector3.Distance(Intersection(res), pos);
            foreach (ISceneObject b in hitObjects.Skip(1))
            {
                double dist = Vector3.Distance(Intersection(b), pos);
                if (dist > Config.Eps && dist < minDist)
                {
                    minDist = dist;
                    res = b;
                }
            }
            return res;
        }

        //Color methods

        /// <summary>
        /// Sums two colors
        /// </summary>
        /// <param name="a">First color</param>
        /// <param name="b">Second color</param>
        /// <returns></returns>
        Color SumColor(Color a, Color b)
        {
            return Color.FromArgb(Math.Min(254, a.R + b.R),
                Math.Min(254, a.G + b.G),
                Math.Min(254, a.B + b.B));
        }

        /// <summary>
        /// Multiplies color by a coefficient
        /// </summary>
        /// <param name="a">Color to be multipled</param>
        /// <param name="l">Coefficient, [0,1]</param>
        /// <returns></returns>
        Color MulColor(Color a, double l)
        {
            return Color.FromArgb((int)Math.Min(254, (int)a.R * l),
                (int)Math.Min(254, (int)a.G * l),
                (int)Math.Min(254, (int)a.B * l));
        }
        
        //Tracing

        /// <summary>
        /// Returns computed color of ray node
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public Color Trace(List<ISceneObject> objects, LightSource source)
        {
            if (Hits(source)) return source.color;
            if (objects.Any(Hits))
            {
                ISceneObject intObj = FirstIntersectingObject(objects);
                switch (intObj)
                {
                    case Box b:
                        {
                            Plane intSurface = BoxIntersectionPlane(b);
                            Vector3 intPoint = Intersection(intSurface);
                            Vector3 normal = intSurface.FacingNormalVector(pos);
                            Color a = Color.Black;
                            if (intSurface.diffuse > 0)
                            {
                                Ray diffRay = new Ray(intPoint, source.pos - intPoint);
                                double sourceLen = (source.pos - diffRay.pos).Length();
                                //var hitObjects = objects.Where(diffRay.Hits);

                                var hitObjects = objects.Where(diffRay.Hits).Where((o) => o != b);
                                List<ISceneObject> box = new List<ISceneObject> { b };
                                bool RayVisible = !
                                    hitObjects.Select(diffRay.FurthestIntersection)
                                    //.Union(hitObjects.Select(diffRay.Intersection))
                                    .Select((Vector3 ip) => Vector3.Distance(diffRay.pos, ip))
                                    .Where((double val) => val > Config.Eps)
                                    .Any((double val) => val < sourceLen);
                                if (RayVisible)
                                {
                                    Color diffuse = MulColor(source.color, Math.Abs(intSurface.diffuse *
                                        Vector3.Dot(normal, diffRay.dir) /
                                        (normal.Length() * diffRay.dir.Length())));
                                    a = SumColor(a, diffuse);
                                }
                            }
                            if (intSurface.specular > 0)
                            {

                            }
                            //(Recursive) Object reflected lighting
                            if (intSurface.reflection > 0 && iter < Config.Reflections)
                            {
                                Ray refRay = ReflectionRay(intSurface);
                                a = SumColor(a, MulColor(refRay.Trace(objects, source), intSurface.reflection));
                            }
                            //(Recursive) Transparent lighting, refracted and multiplied by transparency component 
                            if (intSurface.transparency > 0)
                            {
                                Ray refrRay = RefractionRay(intObj);
                                a = SumColor(a, MulColor(refrRay.Trace(objects, source), intSurface.transparency));
                                //return refrRay.Trace(objects, source);
                            }
                            return SumColor(a, MulColor(intSurface.ambient, 0.35f));
                        }
                   case Sphere s:
                        {
                            Vector3 intPoint = Intersection(s);
                            Vector3 normal = intPoint - s.pos;
                            Color a = Color.Black;
                            if (s.diffuse > 0)
                            {
                                Ray diffRay = new Ray(intPoint, source.pos - intPoint);
                                double sourceLen = (source.pos - diffRay.pos).Length();
                                var hitObjects = objects.Where(diffRay.Hits);
                                bool RayVisible = 
                                    hitObjects.Select(diffRay.Intersection)
                                    //.Union(hitObjects.Select(diffRay.FurthestIntersection))
                                    .Select((Vector3 ip) => Vector3.Distance(diffRay.pos, ip))
                                    .Where((double val) => val > Config.Eps)
                                    .Any((double val) => val < sourceLen);

                                if (RayVisible)
                                {
                                    //a = Color.Red;
                                   Color diffuse = MulColor(source.color, Math.Abs(s.diffuse *
                                        (Vector3.Dot(normal, diffRay.dir) /
                                        (normal.Length() * diffRay.dir.Length()))));
                                    a = SumColor(a, diffuse);
                                }
                                else
                                {
                                }
                            }
                            if (s.specular > 0)
                            {

                            }
                            //(Recursive) Object reflected lighting
                            if (s.reflection > 0 && iter < 3)
                            {
                                Ray refRay = ReflectionRay(s);
                                a = SumColor(a, MulColor(refRay.Trace(objects, source), s.reflection));
                            }
                            //(Recursive) Transparent lighting, refracted and multiplied by transparency component 
                            if (s.transparency > 0)
                            {
                                Ray refrRay = RefractionRay(intObj);
                                a = SumColor(a, MulColor(refrRay.Trace(objects, source), s.transparency));
                                //return refrRay.Trace(objects, source);
                            }
                            return SumColor(a, MulColor(s.ambient, 0.2f));
                        }
                    default:
                        throw new ArgumentException("Wrong Type");
                }
            }
            else
            return Config.AmbientColor;
        }
    }
}
