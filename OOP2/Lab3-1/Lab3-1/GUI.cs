using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace Lab3_1 {

    public partial class GUIForm : Form {

        public ChartArea chartArea;
        Depot depot;

        int dif = 100;

        Timer interval = new Timer();

        public GUIForm() {
            GUI.Instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            chartArea = chart.ChartAreas[0];
            chart.Legends.Add(new Legend("Trams") {
                Title = "Trams",
                MaximumAutoSize = 30
            });
            chart.Legends.Add(new Legend("Routes") {
                Title = "Routes",
                Docking = Docking.Left,
                MaximumAutoSize = 20
            });

            tramTechsView.View = View.Details;
            tramTechsView.Columns.Add("Id");
            tramTechsView.Columns.Add("Job");
            tramTechsView.Columns.Add("Time Left");

            routeTechsView.View = View.Details;
            routeTechsView.Columns.Add("Id");
            routeTechsView.Columns.Add("Job");
            routeTechsView.Columns.Add("Time Left");

            chartArea.AxisX.Interval = 1 * dif;
            chartArea.AxisY.Interval = 1 * dif;
            chartArea.AxisX.Maximum = 10 * dif;
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisY.Maximum = 10 * dif;
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisX.LabelStyle.Enabled = false;
            chartArea.AxisY.LabelStyle.Enabled = false;

            chartArea.AxisX.MajorGrid.LineWidth = 0;
            chartArea.AxisY.MajorGrid.LineWidth = 0;

            reserveView.Columns.Add("Tram");

            StartSimulation();
        }

        private void StartSimulation() {

            var tramTechs = 3;

            for(int i = 0; i < tramTechs; i++) {
                AddViewItem(tramTechsView);
            }

            var routeTechs = 1;
            for (int i = 0; i < routeTechs; i++) {
                AddViewItem(routeTechsView);
            }

            // Создаем депо
            depot = new Depot(tramTechs, routeTechs);

            List<Route> routes = new List<Route>() {
                depot.addRoute(6, new List<Point> { new Point(5, 4), new Point(3, 5), new Point(5, 8), new Point(9, 8) }, Color.Blue),
                depot.addRoute(4, new List<Point> { new Point(7, 4), new Point(7, 2), new Point(3, 2) }, Color.Green),
                depot.addRoute(8, new List<Point> { new Point(3, 6), new Point(3, 9), new Point(8, 9), new Point(8, 1), new Point(1, 1), new Point(1, 4) }, Color.Red),
                depot.addRoute(5, new List<Point> { new Point(5, 6), new Point(9, 3), new Point(3, 4), new Point(2, 7) }, Color.Purple)
            };
            routes.ForEach(route => {
                AddSeries(route.id, route.color, route.stops);
            });

            // Добавляем трамваи
            int numTrams = Prompt.ShowDialog("How many trams?", "Input", 24);
            for (int i = 0; i < numTrams; i++) {
                var tram = depot.addTram();
                AddPoint(tram.id, Color.Orange, new Point(-1, -1));
            }

            //depot.update();
            interval.Tick += new EventHandler((object sender, EventArgs e) => {
                reserveView.Items.Clear();
                depot.update();
            });
            interval.Interval = 1000;
            interval.Start();
        }

        void AddViewItem(ListView view) {
            view.Items.Add(new ListViewItem(new[] { "" + view.Items.Count, "Idling", "-" }));
        }

        void RemoveViewItem(ListView view) {
            view.Items.RemoveAt(view.Items.Count - 1);
        }

        private void AddSeries(int id, Color color, List<Point> points) {
            var series = new Series("route" + id) {
                ChartType = SeriesChartType.Line,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerSize = 8,
                BorderWidth = 2,
                Color = color,
                MarkerStyle = MarkerStyle.Circle
            };

            series.Points.AddXY(5 * dif, 5 * dif);
            for (int i = 0; i < points.Count; i++) {
                series.Points.AddXY(points[i].X * dif, points[i].Y * dif);
            }
            series.Points.AddXY(points[0].X * dif, points[0].Y * dif);
            series.IsVisibleInLegend = false;
            series.Legend = "Routes";
            var legendItem = new LegendItem() {
                ImageStyle = LegendImageStyle.Line,
                MarkerSize = 8,
                MarkerBorderWidth = 2,
                Name = "Route " + id,
                Color = color
            };
            chart.Legends["Routes"].CustomItems.Add(legendItem);
            chart.Series.Add(series);
        }

        private Point CalculateMidPoint(Point point1, Point point2) {
            return new Point((point1.X * 100 + point2.X * 100) / 2, (point1.Y * 100 + point2.Y * 100) / 2);
        }

        private void AddPoint(int id, Color color, Point point) {
            var series = new Series("tram" + id) {
                ChartType = SeriesChartType.Line,
                Color = color,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 10,
                MarkerBorderWidth = 2,
                MarkerBorderColor = Color.Black,
                Label = "Tram " + id
            };
            series.Points.AddXY(point.X, point.Y);
            series.IsVisibleInLegend = false;
            series.Legend = "Trams";
            var legendItem = new LegendItem() {
                ImageStyle = LegendImageStyle.Marker,
                MarkerSize = 10,
                MarkerBorderWidth = 1,
                Name = String.Format("{0, -30}", "Tram " + id),
                Color = color
            };
            chart.Legends["Trams"].CustomItems.Add(legendItem);
            chart.Series.Add(series);
        }

        private void trackBar_Scroll(object sender, EventArgs e) {
            if (trackBarSimSpeed.Value == 0) {
                interval.Stop();
                return;
            }
            interval.Interval = (trackBarSimSpeed.Maximum - trackBarSimSpeed.Value + 1) * 100;
            if (!interval.Enabled) {
                interval.Start();
            }
        }


        public void SetRepairQueueAmount(string actorName, int dif) {
            var label = Controls.Find(actorName + "sQueuedLabel", true)[0];

            label.Text = dif + " " + actorName + "s waiting";
        }

        public void SetTechnicianWorking(string actorName, int techId, int actorId, int timeToRepair) {
            ListView view = Controls.Find(actorName + "TechsView", true)[0] as ListView;

            view.Items[techId].SubItems[1].Text = actorName + " " + actorId;
            view.Items[techId].SubItems[2].Text = "" + timeToRepair;
        }

        public void SetTechnicianIdling(string actorName, int techId) {
            ListView view = Controls.Find(actorName + "TechsView", true)[0] as ListView;

            view.Items[techId].SubItems[1].Text = "Idling";
            view.Items[techId].SubItems[2].Text = "-";
        }

        public void UpdateTramStatus(int tramId, string status, bool stopped, int? routeId, Point prevStop, Point nextStop, float percentage) {
            var tramPoint = chart.Series["tram" + tramId];
            Point point;
            if(status == "no route") {
                point = new Point(-1, -1);
            }
            else if(stopped || prevStop == nextStop) {
                point = new Point(prevStop.X * dif, prevStop.Y * dif);                
            }
            else {
                var x = (nextStop.X * dif - prevStop.X * dif) * percentage;
                var y = (nextStop.Y * dif - prevStop.Y * dif) * percentage;
                point = new Point(prevStop.X * dif + (int)x, prevStop.Y * dif + (int)y);
            }
            tramPoint.Points[0].XValue = point.X;
            tramPoint.Points[0].SetValueY(point.Y);

            if(status == "waiting for repair") {
                tramPoint.Color = Color.Red;
            }
            else {
                tramPoint.Color = Color.Orange;
            }

            var tramLegend = chart.Legends["Trams"].CustomItems[tramId - 1];
            tramLegend.Name = "Tram " + tramId;
            tramLegend.Color = tramPoint.Color = status == "waiting for repair" ? Color.Red : status == "no route" ? Color.Gray : Color.Orange;

            if (status == "no route") {
                reserveView.Items.Add(new ListViewItem(new[] { "Tram " + tramId}));
            }
        }

        public void UpdateRouteStatus(int routeId, string status, int neededTrams) {
            var routeLine = chart.Series["route" + routeId];
            routeLine.BorderDashStyle = status == "online" ? ChartDashStyle.Solid : ChartDashStyle.Dash;
            var routeLegend = chart.Legends["Routes"].CustomItems[routeId - 1];
            routeLegend.Name = "Route " + routeId + " (-" + neededTrams + " trams)";
        }

        private void buttonAddTram_Click(object sender, EventArgs e) {
            var tram = depot.addTram();
            AddPoint(tram.id, Color.Orange, new Point(-1, -1));
        }

        private void buttonRemoveTram_Click(object sender, EventArgs e) {
            var tram = depot.removeTram();
            if (tram == null) return;
            chart.Series["tram" + tram.id].Enabled = false;
            chart.Legends["Trams"].CustomItems[tram.id - 1].Enabled = false;
            Console.WriteLine("Removed tram {0}", tram.id);
        }

        private void buttonAddMechRoute_Click(object sender, EventArgs e) {
            depot.addTech("route");
            AddViewItem(routeTechsView);
        }

        private void buttonRemoveMechRoute_Click(object sender, EventArgs e) {
            if (depot.removeTech("route")) {
                RemoveViewItem(routeTechsView);
            }
        }

        private void buttonAddMechTram_Click(object sender, EventArgs e) {
            depot.addTech("tram");
            AddViewItem(tramTechsView);
        }

        private void buttonRemoveMechTram_Click(object sender, EventArgs e) {
            if (depot.removeTech("tram")) {
                RemoveViewItem(tramTechsView);
            }
        }
    }

    public static class GUI {
        public static GUIForm Instance;
    }

    public static class Prompt {
        public static int ShowDialog(string text, string caption, int defaultValue) {
            Form prompt = new Form() {
                Width = 200,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            NumericUpDown input = new NumericUpDown() { Left = 50, Top = 50, Width = 100, Value = defaultValue };
            Button confirmation = new Button() { Text = "Ok", Left = 50, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(input);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? (int)input.Value : defaultValue;
        }
    }
}
