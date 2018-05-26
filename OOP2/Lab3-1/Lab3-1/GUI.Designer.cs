﻿namespace Lab3_1 {
    partial class GUIForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupDepot = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.reserveView = new System.Windows.Forms.ListView();
            this.groupRepairStation = new System.Windows.Forms.GroupBox();
            this.routesQueuedLabel = new System.Windows.Forms.Label();
            this.tramsQueuedLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tramTechsView = new System.Windows.Forms.ListView();
            this.routeTechsView = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBarSimSpeed = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.groupDepot.SuspendLayout();
            this.groupRepairStation.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSimSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // chart
            // 
            this.chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "Default";
            this.chart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart.Legends.Add(legend2);
            this.chart.Location = new System.Drawing.Point(0, 0);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(837, 559);
            this.chart.TabIndex = 0;
            this.chart.Text = "chart1";
            // 
            // groupDepot
            // 
            this.groupDepot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupDepot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupDepot.Controls.Add(this.label3);
            this.groupDepot.Controls.Add(this.reserveView);
            this.groupDepot.Location = new System.Drawing.Point(12, 339);
            this.groupDepot.MinimumSize = new System.Drawing.Size(0, 100);
            this.groupDepot.Name = "groupDepot";
            this.groupDepot.Size = new System.Drawing.Size(278, 212);
            this.groupDepot.TabIndex = 1;
            this.groupDepot.TabStop = false;
            this.groupDepot.Text = "Tram Depot";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Tram Reserve";
            // 
            // reserveView
            // 
            this.reserveView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.reserveView.Location = new System.Drawing.Point(9, 32);
            this.reserveView.Name = "reserveView";
            this.reserveView.Size = new System.Drawing.Size(260, 174);
            this.reserveView.TabIndex = 4;
            this.reserveView.UseCompatibleStateImageBehavior = false;
            // 
            // groupRepairStation
            // 
            this.groupRepairStation.Controls.Add(this.routesQueuedLabel);
            this.groupRepairStation.Controls.Add(this.tramsQueuedLabel);
            this.groupRepairStation.Controls.Add(this.label2);
            this.groupRepairStation.Controls.Add(this.label1);
            this.groupRepairStation.Controls.Add(this.tramTechsView);
            this.groupRepairStation.Controls.Add(this.routeTechsView);
            this.groupRepairStation.Location = new System.Drawing.Point(12, 12);
            this.groupRepairStation.Name = "groupRepairStation";
            this.groupRepairStation.Size = new System.Drawing.Size(278, 321);
            this.groupRepairStation.TabIndex = 2;
            this.groupRepairStation.TabStop = false;
            this.groupRepairStation.Text = "Repair Station";
            // 
            // routesQueuedLabel
            // 
            this.routesQueuedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.routesQueuedLabel.AutoSize = true;
            this.routesQueuedLabel.Location = new System.Drawing.Point(120, 20);
            this.routesQueuedLabel.Name = "routesQueuedLabel";
            this.routesQueuedLabel.Size = new System.Drawing.Size(149, 13);
            this.routesQueuedLabel.TabIndex = 5;
            this.routesQueuedLabel.Text = "1 routes waiting to be repaired";
            // 
            // tramsQueuedLabel
            // 
            this.tramsQueuedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tramsQueuedLabel.AutoSize = true;
            this.tramsQueuedLabel.Location = new System.Drawing.Point(124, 173);
            this.tramsQueuedLabel.Name = "tramsQueuedLabel";
            this.tramsQueuedLabel.Size = new System.Drawing.Size(145, 13);
            this.tramsQueuedLabel.TabIndex = 4;
            this.tramsQueuedLabel.Text = "3 trams waiting to be repaired";
            this.tramsQueuedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tram Mechanics";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Route Mechanics";
            // 
            // tramTechsView
            // 
            this.tramTechsView.Location = new System.Drawing.Point(9, 189);
            this.tramTechsView.Name = "tramTechsView";
            this.tramTechsView.Size = new System.Drawing.Size(260, 126);
            this.tramTechsView.TabIndex = 1;
            this.tramTechsView.UseCompatibleStateImageBehavior = false;
            // 
            // routeTechsView
            // 
            this.routeTechsView.Location = new System.Drawing.Point(9, 36);
            this.routeTechsView.Name = "routeTechsView";
            this.routeTechsView.Size = new System.Drawing.Size(260, 134);
            this.routeTechsView.TabIndex = 0;
            this.routeTechsView.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.groupRepairStation);
            this.panel1.Controls.Add(this.groupDepot);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(843, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 561);
            this.panel1.TabIndex = 5;
            // 
            // trackBarSimSpeed
            // 
            this.trackBarSimSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBarSimSpeed.BackColor = System.Drawing.Color.White;
            this.trackBarSimSpeed.Location = new System.Drawing.Point(12, 500);
            this.trackBarSimSpeed.Maximum = 20;
            this.trackBarSimSpeed.Name = "trackBarSimSpeed";
            this.trackBarSimSpeed.Size = new System.Drawing.Size(146, 45);
            this.trackBarSimSpeed.TabIndex = 6;
            this.trackBarSimSpeed.Value = 10;
            this.trackBarSimSpeed.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 484);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Simulation Speed";
            // 
            // GUIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1151, 561);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackBarSimSpeed);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(1100, 500);
            this.Name = "GUIForm";
            this.Text = "Tram Network Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.groupDepot.ResumeLayout(false);
            this.groupDepot.PerformLayout();
            this.groupRepairStation.ResumeLayout(false);
            this.groupRepairStation.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSimSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.GroupBox groupDepot;
        private System.Windows.Forms.GroupBox groupRepairStation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView tramTechsView;
        private System.Windows.Forms.ListView routeTechsView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView reserveView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBarSimSpeed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label routesQueuedLabel;
        private System.Windows.Forms.Label tramsQueuedLabel;
    }
}

