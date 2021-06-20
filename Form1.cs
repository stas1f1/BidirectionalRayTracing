using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.DoubleNumerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static RTTest1.Objects;
using static RTTest1.Utils;
using static RTTest1.Transformation;
using System.IO;

namespace RTTest1
{
    public delegate void FormUpdater(double progress, TimeSpan time);

    public partial class Form1 : Form
    {
        Bitmap pic;
        public Camera cam;
        
        public Form1()
        {
            InitializeComponent();
        }

        public void UpdateLearningInfo(double progress, TimeSpan elapsedTime)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new FormUpdater(UpdateLearningInfo), new Object[] { progress, elapsedTime });
                return;
            }
            int prgs = (int)Math.Round(progress * 100);
            prgs = Math.Min(100, Math.Max(0, prgs));
            elapsedTimeLabel.Text = "Затраченное время : " + elapsedTime.Duration().ToString(@"hh\:mm\:ss\:ff");
            progressBar1.Value = prgs;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            cam = new Camera(new Point3D(0, 0, -500, 0),
                new Point(0, 0),
                new Point(pic.Width, pic.Height));

            cam.updateDelegate = UpdateLearningInfo;
            pictureBox1.Image = pic;



            ResetObjects();

            LoadScene2();
        }

        


        private void button1_Click(object sender, EventArgs e)
        {
            cam.Render(pic, pictureBox1);
        }

        private void SaveRender(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "jpeg files (*.jpg)|.jpg";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (pic)
                {
                    ImageCodecInfo jpgEncoder;
                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
                    jpgEncoder = codecs[0];
                    foreach (ImageCodecInfo codec in codecs)
                    {

                        if (codec.FormatID == ImageFormat.Jpeg.Guid)
                        {
                            jpgEncoder = codec;
                            break;
                        }
                    }

                    // Create an Encoder object based on the GUID  
                    // for the Quality parameter category.  
                    System.Drawing.Imaging.Encoder myEncoder =
                        System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  
                    // An EncoderParameters object has an array of EncoderParameter  
                    // objects. In this case, there is only one  
                    // EncoderParameter object in the array.  
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter;
                    /*
                    myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    pic.Save("TestPhotoQualityFifty.jpg", jpgEncoder, myEncoderParameters);

                    myEncoderParameter = new EncoderParameter(myEncoder, 255L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    pic.Save("TestPhotoQualityHundred.jpg", jpgEncoder, myEncoderParameters);
                    */

                    // Save the bitmap as a JPG file with zero quality level compression.  
                    myEncoderParameter = new EncoderParameter(myEncoder, 255L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    pic.Save(saveFileDialog1.FileName, jpgEncoder, myEncoderParameters);
                }
            }

            
        }

        private void ResetObjects()
        {
            meshes = new List<Mesh>();
            spheres = new List<Sphere>();
            lights = new List<Light>();
        }

        private void LoadScene1()
        {
            LoadRoom();
            // Diffuse, Specular, Reflection, Refraction
            var t1 = new Mesh();
            LoadMeshByPath(ref t1, "../../models/octa.obj");
            t1.material = new Material();
            t1.material.color = Color.Purple;
            t1.material.parameters = new List<double> { 0, 0, 0, 0 };
            ApplyTransform(ref t1, Transformation.Scale(1.5, 1.5, 1.5));
            ApplyTransform(ref t1, Transformation.Move(-100, -200, 250));
            meshes.Add(t1);

            var t2 = new Mesh();
            LoadMeshByPath(ref t2, "../../models/cube.obj");
            t2.material = new Material();
            t2.material.color = Color.Green;
            t2.material.specularHighlight = 5;
            t2.material.parameters = new List<double> { 0, 0.2, 0, 0 };
            ApplyTransform(ref t1, Transformation.Scale(0.75, 0.75, 0.75));
            ApplyTransform(ref t2, Rotate(-30.0 / 360.0 * (2.0 * Math.PI), 'y'), true);
            ApplyTransform(ref t2, Transformation.Move(115, 75, 300));
            meshes.Add(t2);

            var t3 = new Sphere();
            t3.material = new Material();
            t3.material.color = Color.Yellow;
            t3.material.specularHighlight = 300;
            t3.material.parameters = new List<double> { 0, 5, 0.5, 0 };
            t3.pos = new Point3D(-160, 100, 200);
            t3.radius = 50;
            spheres.Add(t3);

            var t4 = new Mesh();
            LoadMeshByPath(ref t4, "../../models/plane.obj");
            t4.material = new Material();
            t4.material.color = Color.Gray;
            t4.material.parameters = new List<double> { 0, 0, 0, 0 };
            //AtheneTransform(ref t4, AtheneScale(0.75, 0.75, 0.75));
            ApplyTransform(ref t4, Rotate((180 + 75.0) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref t4, Transformation.Move(220, 50, 50));
            meshes.Add(t4);

            var t5 = new Mesh();
            LoadMeshByPath(ref t5, "../../models/plane.obj");
            t5.material = new Material();
            t5.material.color = Color.Gray;
            t5.material.parameters = new List<double> { 0, 0, 0.9, 0 };
            ApplyTransform(ref t5, Transformation.Scale(0.95, 0.95, 0.95));
            ApplyTransform(ref t5, Rotate((180 + 75.0) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref t5, Transformation.Move(219, 50, 50));
            meshes.Add(t5);

            //var t6 = new Sphere();
            //t6.mat = new Material();
            //t6.mat.color = Color.Yellow;
            //t6.mat.specularity = 30;
            //t6.mat.albedo = new List<double> { 1, 0, 0.5, 0 };
            //t6.location = new Point3D(180, -100, 250);
            //t6.radius = 100;
            //spheres.Add(t6);

            var t6 = new Sphere();
            t6.material = new Material();
            t6.material.color = Color.Yellow;
            t6.material.specularHighlight = 30;
            t6.material.refractionIndex = 1.5;
            t6.material.parameters = new List<double> { 0, 0, 0, 1 };
            t6.pos = new Point3D(-40, 90, 60);
            t6.radius = 60;
            spheres.Add(t6);

            var t7 = new Sphere();
            t7.material = new Material();
            t7.material.color = Color.Yellow;
            t7.material.specularHighlight = 30;
            t7.material.refractionIndex = 1.5;
            t7.material.parameters = new List<double> { 0, 0, 0, 0 };
            t7.pos = new Point3D(-180, 110, -40);
            t7.radius = 40;
            spheres.Add(t7);

            var l1 = new Light();
            l1.pos = new Point3D(150, 0, 0);
            l1.intensity = 0.7;
            l1.color = Color.Orange;
            lights.Add(l1);

            var l2 = new Light();
            l2.pos = new Point3D(-150, 0, 0);
            l2.intensity = 0.7;
            l2.color = Color.Cyan;
            lights.Add(l2);
        }

        private void LoadScene2()
        {
            LoadRoom();
            // Diffuse, Specular, Reflection, Refraction
            
            var t1 = new Mesh();
            LoadMeshByPath(ref t1, "../../models/cube.obj");
            t1.material = new Material();
            t1.material.color = Color.Black;
            t1.material.parameters = new List<double> { 0, 0, 1, 0 };
            //Transform(ref t1, Transformation.Scale(1.5, 1.5, 1.5));
            ApplyTransform(ref t1, Rotate(-130.0 / 360.0 * (2.0 * Math.PI), 'y'), true);
            ApplyTransform(ref t1, Transformation.Move(-80, 75, 400));
            meshes.Add(t1);

            var t3 = new Sphere();
            t3.material = new Material();
            t3.material.color = Color.Black;
            t3.material.specularHighlight = 100;
            t3.material.parameters = new List<double> { 0.1, 5, 0.8, 0};
            t3.pos = new Point3D(100, 100, 200);
            t3.radius = 50;
            spheres.Add(t3);

            var t6 = new Sphere();
            t6.material = new Material();
            t6.material.color = Color.Black;
            t6.material.specularHighlight = 30;
            t6.material.refractionIndex = 1.5;
            t6.material.parameters = new List<double> { 0, 0.5, 0.2, 0.8 };
            t6.pos = new Point3D(-140, 90, 0);
            t6.radius = 60;
            spheres.Add(t6);

            var t2 = new Mesh();
            LoadMeshByPath(ref t2, "../../models/cube.obj");
            t2.material = new Material();
            t2.material.color = Color.Red;
            t2.material.specularHighlight = 30;
            t2.material.refractionIndex = 1.5;
            t2.material.parameters = new List<double> { 0.15, 1, 0, 0.8 };
            ApplyTransform(ref t2, Transformation.Scale(0.5, 0.5, 0.5));
            ApplyTransform(ref t2, Rotate((-25.0) / 360.0 * (2.0 * Math.PI), 'y'), true);
            ApplyTransform(ref t2, Transformation.Move(150, 100, 0));
            meshes.Add(t2);

            /*
            var t2 = new Mesh();
            LoadMeshByPath(ref t2, "../../models/cube.obj");
            t2.material = new Material();
            t2.material.color = Color.Green;
            t2.material.specularHighlight = 5;
            t2.material.parameters = new List<double> { 0, 0.2, 0, 0 };
            Transform(ref t2, Transformation.Scale(0.75, 0.75, 0.75));
            Transform(ref t2, Rotate(-30.0 / 360.0 * (2.0 * Math.PI), 'y'), true);
            Transform(ref t2, Transformation.Move(115, 75, 300));
            meshes.Add(t2);
            
            var t3 = new Sphere();
            t3.mat = new Material();
            t3.mat.color = Color.Yellow;
            t3.mat.specularHighlight = 300;
            t3.mat.parameters = new List<double> { 0, 5, 0.5, 0 };
            t3.pos = new Point3D(-160, 100, 200);
            t3.radius = 50;
            spheres.Add(t3);

            var t4 = new Mesh();
            LoadMeshByPath(ref t4, "../../models/plane.obj");
            t4.material = new Material();
            t4.material.color = Color.Gray;
            t4.material.parameters = new List<double> { 0, 0, 0, 0 };
            //AtheneTransform(ref t4, AtheneScale(0.75, 0.75, 0.75));
            Transform(ref t4, Rotate((180 + 75.0) / 360.0 * (2.0 * Math.PI), 'z'), true);
            Transform(ref t4, Transformation.Move(220, 50, 50));
            meshes.Add(t4);

            /*
            var t5 = new Mesh();
            LoadMeshByPath(ref t5, "../../models/plane.obj");
            t5.material = new Material();
            t5.material.color = Color.Gray;
            t5.material.parameters = new List<double> { 0, 0, 0.9, 0 };
            Transform(ref t5, Transformation.Scale(0.95, 0.95, 0.95));
            Transform(ref t5, Rotate((180 + 75.0) / 360.0 * (2.0 * Math.PI), 'z'), true);
            Transform(ref t5, Transformation.Move(219, 50, 50));
            meshes.Add(t5);
            */

            //var t6 = new Sphere();
            //t6.mat = new Material();
            //t6.mat.color = Color.Yellow;
            //t6.mat.specularity = 30;
            //t6.mat.albedo = new List<double> { 1, 0, 0.5, 0 };
            //t6.location = new Point3D(180, -100, 250);
            //t6.radius = 100;
            //spheres.Add(t6);

            /*
            var t6 = new Sphere();
            t6.mat = new Material();
            t6.mat.color = Color.Yellow;
            t6.mat.specularHighlight = 30;
            t6.mat.refractionIndex = 1.5;
            t6.mat.parameters = new List<double> { 0, 0, 0, 1 };
            t6.pos = new Point3D(-40, 90, 60);
            t6.radius = 60;
            spheres.Add(t6);

            var t7 = new Sphere();
            t7.mat = new Material();
            t7.mat.color = Color.Yellow;
            t7.mat.specularHighlight = 30;
            t7.mat.refractionIndex = 1.5;
            t7.mat.parameters = new List<double> { 0, 0, 0, 0 };
            t7.pos = new Point3D(-180, 110, -40);
            t7.radius = 40;
            spheres.Add(t7);
            */

            var l1 = new Light();
            l1.pos = new Point3D(0, -150, -300);
            l1.intensity = 1;
            l1.color = Color.White;
            lights.Add(l1);

            var l2 = new Light();
            l2.pos = new Point3D(-150, 0, -430);
            l2.intensity = 0.7;
            l2.color = Color.Cyan;
            //lights.Add(l2); 

            /*
            var l1 = new Light();
            l1.pos = new Point3D(150, 0, 0);
            l1.intensity = 0.7;
            l1.color = Color.Orange;
            lights.Add(l1);

            var l2 = new Light();
            l2.pos = new Point3D(-150, 0, 0);
            l2.intensity = 0.7;
            l2.color = Color.Cyan;
            lights.Add(l2);*/
        }
        
        private void LoadRoom()
        {
            var floor = new Mesh();
            LoadMeshByPath(ref floor, "../../models/plane.obj");
            floor.material = new Material();
            floor.material.color = Color.White;
            floor.material.parameters = new List<double> { 0.2, 0, 0, 0 };
            ApplyTransform(ref floor, Rotate((180) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref floor, Transformation.Scale(2.5, 2.5, 5));
            ApplyTransform(ref floor, Transformation.Move(0, -100 + 250, -1));
            meshes.Add(floor);

            var ceiling = new Mesh();
            LoadMeshByPath(ref ceiling, "../../models/plane.obj");
            ceiling.material = new Material();
            ceiling.material.color = Color.White;
            ceiling.material.parameters = new List<double> { 0.2, 0, 0, 0 };
            //AtheneTransform(ref ceiling, AtheneRotate((180) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref ceiling, Transformation.Scale(2.5, 2.5, 5));
            ApplyTransform(ref ceiling, Transformation.Move(0, -100 - 250, -1));
            meshes.Add(ceiling);

            var wall1 = new Mesh();
            LoadMeshByPath(ref wall1, "../../models/plane.obj");
            wall1.material = new Material();
            wall1.material.color = Color.Green;
            wall1.material.parameters = new List<double> { 0, 0, 0, 0 };
            ApplyTransform(ref wall1, Transformation.Scale(2.5, 2.5, 5));
            ApplyTransform(ref wall1, Rotate((90) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref wall1, Transformation.Move(-250, -100, -1));
            meshes.Add(wall1);

            var wall2 = new Mesh();
            LoadMeshByPath(ref wall2, "../../models/plane.obj");
            wall2.material = new Material();
            wall2.material.color = Color.LightGray;
            wall2.material.parameters = new List<double> { 0, 0, 0, 0 };
            ApplyTransform(ref wall2, Transformation.Scale(2.5, 2.5, 2.5));
            ApplyTransform(ref wall2, Rotate((90) / 360.0 * (2.0 * Math.PI), 'x'), true);
            ApplyTransform(ref wall2, Transformation.Move(0, -100, -1 + 500));
            meshes.Add(wall2);

            var wall3 = new Mesh();
            LoadMeshByPath(ref wall3, "../../models/plane.obj");
            wall3.material = new Material();
            wall3.material.color = Color.Red;
            wall3.material.parameters = new List<double> { 0, 0, 0, 0 };
            ApplyTransform(ref wall3, Transformation.Scale(2.5, 2.5, 5));
            ApplyTransform(ref wall3, Rotate((-90) / 360.0 * (2.0 * Math.PI), 'z'), true);
            ApplyTransform(ref wall3, Transformation.Move(250, -100, -1));
            meshes.Add(wall3);

            var wall4 = new Mesh();
            LoadMeshByPath(ref wall4, "../../models/plane.obj");
            wall4.material = new Material();
            wall4.material.color = Color.White;
            wall4.material.parameters = new List<double> { 0, 0, 0, 0 };
            ApplyTransform(ref wall4, Transformation.Scale(2.5, 2.5, 2.5));
            ApplyTransform(ref wall4, Rotate((-90) / 360.0 * (2.0 * Math.PI), 'x'), true);
            ApplyTransform(ref wall4, Transformation.Move(0, -100, -500 - 1));
            meshes.Add(wall4);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            cam.scatteredRaysActive = !cam.scatteredRaysActive;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            cam.forwardTracingActive = !cam.forwardTracingActive;
        }

        private void depthUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.depth = (int)depthUpDown.Value;
        }

        private void lightmapUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.textureResolution = (int)lightmapUpDown.Value;
        }

        private void lightSpotUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.spotSize = (int)lightSpotUpDown.Value;
        }

        private void lightInterpolationUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.interpolationSize = (int)lightInterpolationUpDown.Value;
        }

        private void forwardShootUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.forwardShootingResolution = (int)forwardShootUpDown.Value;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            cam.edgesAntiAliasingActive = !cam.edgesAntiAliasingActive;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            cam.postProcessAntiAliasingActive = !cam.postProcessAntiAliasingActive;
        }

        private void supersamplingUpDown_ValueChanged(object sender, EventArgs e)
        {
            cam.supersamplingResolution = (int)supersamplingUpDown.Value;
        }
    }
}
