namespace Lab6_GUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxa = new System.Windows.Forms.NumericUpDown();
            this.labela = new System.Windows.Forms.Label();
            this.textBoxb = new System.Windows.Forms.NumericUpDown();
            this.labelb = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.x = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.y = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.radioParabola = new System.Windows.Forms.RadioButton();
            this.groupFuncSelect = new System.Windows.Forms.GroupBox();
            this.radioEllipse = new System.Windows.Forms.RadioButton();
            this.radioHyperbola = new System.Windows.Forms.RadioButton();
            this.groupParamSelect = new System.Windows.Forms.GroupBox();
            this.buttonCalc = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxx = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxb)).BeginInit();
            this.groupFuncSelect.SuspendLayout();
            this.groupParamSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxx)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxa
            // 
            this.textBoxa.DecimalPlaces = 8;
            this.textBoxa.Location = new System.Drawing.Point(39, 28);
            this.textBoxa.Name = "textBoxa";
            this.textBoxa.Size = new System.Drawing.Size(105, 22);
            this.textBoxa.TabIndex = 0;
            this.textBoxa.TextChanged += new System.EventHandler(this.textBoxa_TextChanged);
            // 
            // labela
            // 
            this.labela.AutoSize = true;
            this.labela.Location = new System.Drawing.Point(14, 31);
            this.labela.Name = "labela";
            this.labela.Size = new System.Drawing.Size(15, 14);
            this.labela.TabIndex = 1;
            this.labela.Text = "a";
            // 
            // textBoxb
            // 
            this.textBoxb.DecimalPlaces = 8;
            this.textBoxb.Location = new System.Drawing.Point(39, 56);
            this.textBoxb.Name = "textBoxb";
            this.textBoxb.Size = new System.Drawing.Size(105, 22);
            this.textBoxb.TabIndex = 2;
            this.textBoxb.TextChanged += new System.EventHandler(this.textBoxb_TextChanged);
            // 
            // labelb
            // 
            this.labelb.AutoSize = true;
            this.labelb.Location = new System.Drawing.Point(14, 59);
            this.labelb.Name = "labelb";
            this.labelb.Size = new System.Drawing.Size(15, 14);
            this.labelb.TabIndex = 3;
            this.labelb.Text = "b";
            // 
            // listView
            // 
            this.listView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.x,
            this.y});
            this.listView.Location = new System.Drawing.Point(16, 120);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(310, 113);
            this.listView.TabIndex = 5;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // x
            // 
            this.x.Text = "x";
            this.x.Width = 115;
            // 
            // y
            // 
            this.y.Text = "y";
            this.y.Width = 112;
            // 
            // radioParabola
            // 
            this.radioParabola.AutoSize = true;
            this.radioParabola.Location = new System.Drawing.Point(31, 19);
            this.radioParabola.Name = "radioParabola";
            this.radioParabola.Size = new System.Drawing.Size(81, 18);
            this.radioParabola.TabIndex = 6;
            this.radioParabola.TabStop = true;
            this.radioParabola.Text = "Parabola";
            this.radioParabola.UseVisualStyleBackColor = true;
            this.radioParabola.CheckedChanged += new System.EventHandler(this.radioParabola_CheckedChanged);
            // 
            // groupFuncSelect
            // 
            this.groupFuncSelect.Controls.Add(this.radioEllipse);
            this.groupFuncSelect.Controls.Add(this.radioHyperbola);
            this.groupFuncSelect.Controls.Add(this.radioParabola);
            this.groupFuncSelect.Location = new System.Drawing.Point(16, 13);
            this.groupFuncSelect.Name = "groupFuncSelect";
            this.groupFuncSelect.Size = new System.Drawing.Size(152, 98);
            this.groupFuncSelect.TabIndex = 7;
            this.groupFuncSelect.TabStop = false;
            this.groupFuncSelect.Text = "Math Function";
            // 
            // radioEllipse
            // 
            this.radioEllipse.AutoSize = true;
            this.radioEllipse.Location = new System.Drawing.Point(31, 69);
            this.radioEllipse.Name = "radioEllipse";
            this.radioEllipse.Size = new System.Drawing.Size(65, 18);
            this.radioEllipse.TabIndex = 8;
            this.radioEllipse.TabStop = true;
            this.radioEllipse.Text = "Ellipse";
            this.radioEllipse.UseVisualStyleBackColor = true;
            this.radioEllipse.CheckedChanged += new System.EventHandler(this.radioEllipse_CheckedChanged);
            // 
            // radioHyperbola
            // 
            this.radioHyperbola.AutoSize = true;
            this.radioHyperbola.Location = new System.Drawing.Point(31, 44);
            this.radioHyperbola.Name = "radioHyperbola";
            this.radioHyperbola.Size = new System.Drawing.Size(89, 18);
            this.radioHyperbola.TabIndex = 7;
            this.radioHyperbola.TabStop = true;
            this.radioHyperbola.Text = "Hyperbola";
            this.radioHyperbola.UseVisualStyleBackColor = true;
            this.radioHyperbola.CheckedChanged += new System.EventHandler(this.radioHyperbola_CheckedChanged);
            // 
            // groupParamSelect
            // 
            this.groupParamSelect.Controls.Add(this.textBoxa);
            this.groupParamSelect.Controls.Add(this.labela);
            this.groupParamSelect.Controls.Add(this.textBoxb);
            this.groupParamSelect.Controls.Add(this.labelb);
            this.groupParamSelect.Location = new System.Drawing.Point(176, 13);
            this.groupParamSelect.Name = "groupParamSelect";
            this.groupParamSelect.Size = new System.Drawing.Size(152, 98);
            this.groupParamSelect.TabIndex = 8;
            this.groupParamSelect.TabStop = false;
            this.groupParamSelect.Text = "Parameters";
            // 
            // buttonCalc
            // 
            this.buttonCalc.Location = new System.Drawing.Point(16, 237);
            this.buttonCalc.Name = "buttonCalc";
            this.buttonCalc.Size = new System.Drawing.Size(152, 25);
            this.buttonCalc.TabIndex = 9;
            this.buttonCalc.Text = "Calculate for";
            this.buttonCalc.UseVisualStyleBackColor = true;
            this.buttonCalc.Click += new System.EventHandler(this.buttonCalc_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(16, 269);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(312, 25);
            this.buttonClear.TabIndex = 10;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxx
            // 
            this.textBoxx.DecimalPlaces = 8;
            this.textBoxx.Location = new System.Drawing.Point(176, 239);
            this.textBoxx.Name = "textBoxx";
            this.textBoxx.Size = new System.Drawing.Size(152, 22);
            this.textBoxx.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 307);
            this.Controls.Add(this.textBoxx);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonCalc);
            this.Controls.Add(this.groupParamSelect);
            this.Controls.Add(this.groupFuncSelect);
            this.Controls.Add(this.listView);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Math Functions";
            ((System.ComponentModel.ISupportInitialize)(this.textBoxa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxb)).EndInit();
            this.groupFuncSelect.ResumeLayout(false);
            this.groupFuncSelect.PerformLayout();
            this.groupParamSelect.ResumeLayout(false);
            this.groupParamSelect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown textBoxa;
        private System.Windows.Forms.Label labela;
        private System.Windows.Forms.NumericUpDown textBoxb;
        private System.Windows.Forms.Label labelb;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.RadioButton radioParabola;
        private System.Windows.Forms.GroupBox groupFuncSelect;
        private System.Windows.Forms.RadioButton radioEllipse;
        private System.Windows.Forms.RadioButton radioHyperbola;
        private System.Windows.Forms.GroupBox groupParamSelect;
        private System.Windows.Forms.Button buttonCalc;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.NumericUpDown textBoxx;
        private System.Windows.Forms.ColumnHeader x;
        private System.Windows.Forms.ColumnHeader y;
    }
}

