using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTTest1
{
    /// <summary>
    /// Материал
    /// </summary>
    public class Material
    {
        public Color color;
        public double specularHighlight;
        public double refractionIndex;
        public List<double> parameters;

        public Material()
        {
            color = Color.Gray;
            specularHighlight = 0;
            parameters = new List<double> { 1, 0, 0 };
            refractionIndex = 1;
        }

        public Material(Color c, double spec, List<double> par, double r)
        {
            color = c;
            specularHighlight = spec;
            parameters = new List<double> { par[0], par[1], par[2], par[3] };
            refractionIndex = r;
        }
    }

}
