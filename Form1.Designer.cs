namespace RTTest1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.elapsedTimeLabel = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.lightInterpolationUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.forwardShootUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lightSpotUpDown = new System.Windows.Forms.NumericUpDown();
            this.lightmapUpDown = new System.Windows.Forms.NumericUpDown();
            this.depthUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.supersamplingUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightInterpolationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardShootUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightSpotUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightmapUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.supersamplingUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(649, 612);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 630);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 62);
            this.button1.TabIndex = 1;
            this.button1.Text = "Render";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(136, 630);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 62);
            this.button2.TabIndex = 2;
            this.button2.Text = "Save image";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SaveRender);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(213, 630);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(445, 22);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 705);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "diffuse reflection";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // elapsedTimeLabel
            // 
            this.elapsedTimeLabel.AutoSize = true;
            this.elapsedTimeLabel.Location = new System.Drawing.Point(210, 655);
            this.elapsedTimeLabel.Name = "elapsedTimeLabel";
            this.elapsedTimeLabel.Size = new System.Drawing.Size(43, 13);
            this.elapsedTimeLabel.TabIndex = 13;
            this.elapsedTimeLabel.Text = "Время:";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 731);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(96, 17);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "forward tracing";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // lightInterpolationUpDown
            // 
            this.lightInterpolationUpDown.Location = new System.Drawing.Point(559, 679);
            this.lightInterpolationUpDown.Name = "lightInterpolationUpDown";
            this.lightInterpolationUpDown.Size = new System.Drawing.Size(96, 20);
            this.lightInterpolationUpDown.TabIndex = 15;
            this.lightInterpolationUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.lightInterpolationUpDown.ValueChanged += new System.EventHandler(this.lightInterpolationUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(432, 681);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Light interpolation size";
            // 
            // forwardShootUpDown
            // 
            this.forwardShootUpDown.Location = new System.Drawing.Point(559, 705);
            this.forwardShootUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.forwardShootUpDown.Name = "forwardShootUpDown";
            this.forwardShootUpDown.Size = new System.Drawing.Size(96, 20);
            this.forwardShootUpDown.TabIndex = 17;
            this.forwardShootUpDown.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.forwardShootUpDown.ValueChanged += new System.EventHandler(this.forwardShootUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(432, 707);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Forward shoot resolution";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 707);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Light spot size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 681);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Lightmap resolution";
            // 
            // lightSpotUpDown
            // 
            this.lightSpotUpDown.Location = new System.Drawing.Point(320, 705);
            this.lightSpotUpDown.Name = "lightSpotUpDown";
            this.lightSpotUpDown.Size = new System.Drawing.Size(96, 20);
            this.lightSpotUpDown.TabIndex = 24;
            this.lightSpotUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.lightSpotUpDown.ValueChanged += new System.EventHandler(this.lightSpotUpDown_ValueChanged);
            // 
            // lightmapUpDown
            // 
            this.lightmapUpDown.Location = new System.Drawing.Point(320, 679);
            this.lightmapUpDown.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.lightmapUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.lightmapUpDown.Name = "lightmapUpDown";
            this.lightmapUpDown.Size = new System.Drawing.Size(96, 20);
            this.lightmapUpDown.TabIndex = 23;
            this.lightmapUpDown.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.lightmapUpDown.ValueChanged += new System.EventHandler(this.lightmapUpDown_ValueChanged);
            // 
            // depthUpDown
            // 
            this.depthUpDown.Location = new System.Drawing.Point(320, 731);
            this.depthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.depthUpDown.Name = "depthUpDown";
            this.depthUpDown.Size = new System.Drawing.Size(96, 20);
            this.depthUpDown.TabIndex = 26;
            this.depthUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.depthUpDown.ValueChanged += new System.EventHandler(this.depthUpDown_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 733);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Tracing depth";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(131, 705);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(58, 17);
            this.checkBox3.TabIndex = 27;
            this.checkBox3.Text = "pre-AA";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(131, 731);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(63, 17);
            this.checkBox4.TabIndex = 28;
            this.checkBox4.Text = "post-AA";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(432, 733);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Supersampling level";
            // 
            // supersamplingUpDown
            // 
            this.supersamplingUpDown.Location = new System.Drawing.Point(559, 731);
            this.supersamplingUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.supersamplingUpDown.Name = "supersamplingUpDown";
            this.supersamplingUpDown.Size = new System.Drawing.Size(96, 20);
            this.supersamplingUpDown.TabIndex = 29;
            this.supersamplingUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.supersamplingUpDown.ValueChanged += new System.EventHandler(this.supersamplingUpDown_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 769);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.supersamplingUpDown);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.depthUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lightSpotUpDown);
            this.Controls.Add(this.lightmapUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.forwardShootUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lightInterpolationUpDown);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.elapsedTimeLabel);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightInterpolationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.forwardShootUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightSpotUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightmapUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.supersamplingUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label elapsedTimeLabel;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.NumericUpDown lightInterpolationUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown forwardShootUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown lightSpotUpDown;
        private System.Windows.Forms.NumericUpDown lightmapUpDown;
        private System.Windows.Forms.NumericUpDown depthUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown supersamplingUpDown;
    }
}

