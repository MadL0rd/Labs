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

            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 1;
            chartArea.AxisX.Maximum = 10;
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisY.Maximum = 10;
            chartArea.AxisY.Minimum = 0;

            chartArea.AxisX.MajorGrid.LineWidth = 0;
            chartArea.AxisY.MajorGrid.LineWidth = 0;

            reserveView.Columns.Add("Tram");

            StartSimulation();
        }

        private void StartSimulation() {

            var tramTechs = 3;

            for(int i = 0; i < tramTechs; i++) {
                tramTechsView.Items.Add(new ListViewItem(new[] { "" + i, "Idling", "0" }));
            }

            var routeTechs = 1;
            for (int i = 0; i < routeTechs; i++) {
                routeTechsView.Items.Add(new ListViewItem(new[] { "" + i, "Idling", "0" }));
            }

            // Создаем депо
            var depot = new Depot(tramTechs, routeTechs);

            List<Route> routes = new List<Route>() {
                depot.addRoute(6, new List<Point> { new Point(9, 7), new Point(5, 3), new Point(3, 5), new Point(5, 7) }, Color.Blue),
                depot.addRoute(4, new List<Point> { new Point(7, 6), new Point(7, 2), new Point(3, 2) }, Color.Green),
                depot.addRoute(8, new List<Point> { new Point(1, 1), new Point(1, 4), new Point(3, 6), new Point(3, 9), new Point(8, 9), new Point(8, 1) }, Color.Red)
            };
            routes.ForEach(route => {
                AddSeries(route.id, route.color, route.stops);
            });

            // Добавляем трамваи
            int numTrams = Prompt.ShowDialog("How many trams?", "Input", 20);
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

        private void AddSeries(int id, Color color, List<Point> points) {
            var series = new Series("route" + id) {
                ChartType = SeriesChartType.Line,
                BorderDashStyle = ChartDashStyle.Dash,
                MarkerSize = 8,
                BorderWidth = 2,
                Color = color,
                MarkerStyle = MarkerStyle.Circle
            };
            for(int i = 0; i < points.Count; i++) {
                series.Points.AddXY(points[i].X, points[i].Y);
            }
            series.Points.AddXY(points[0].X, points[0].Y);
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
            return new Point((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2);
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

            label.Text = dif + " " + actorName + "s waiting to be repaired";
        }

        public void SetTechnicianWorking(string actorName, int techId, int actorId, int timeToRepair) {
            ListView view = Controls.Find(actorName + "TechsView", true)[0] as ListView;

            view.Items[techId].SubItems[1].Text = "Tram " + actorId;
            view.Items[techId].SubItems[2].Text = "" + timeToRepair;
        }

        public void SetTechnicianIdling(string actorName, int techId) {
            ListView view = Controls.Find(actorName + "TechsView", true)[0] as ListView;

            view.Items[techId].SubItems[1].Text = "Idling";
            view.Items[techId].SubItems[2].Text = "-";
        }

        public void UpdateTramStatus(int tramId, string status, bool stopped, int? routeId, Point prevStop, Point nextStop) {
            var tramPoint = chart.Series["tram" + tramId];
            Point point;
            if(status == "waiting for repair" || status == "no route") {
                point = new Point(-1, -1);
            }
            else if(stopped) {
                point = prevStop;                
            }
            else {
                point = CalculateMidPoint(prevStop, nextStop);
            }
            tramPoint.Points[0].XValue = point.X;
            tramPoint.Points[0].SetValueY(point.Y);

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
            routeLegend.Name = "Route " + routeId + " (needs " + neededTrams + " trams)";
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
