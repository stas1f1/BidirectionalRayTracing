using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.DoubleNumerics;
using System.Text;
using System.Threading.Tasks;
using static RTTest1.Objects;

namespace RTTest1
{
    public class Light
    {
        public Point3D pos;
        public double intensity;
        public Color color;

        public Light()
        {
            pos = new Point3D();
            intensity = 0;
            color = Color.White;
        }

        public Light(Point3D p, double i, Color c)
        {
            pos = new Point3D(p);
            intensity = i;
            color = c;
        }
    }
}
