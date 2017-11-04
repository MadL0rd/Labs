using System;
using System.Windows.Forms;
using MathFunctions;

namespace Lab6_GUI
{
    public partial class Form1 : Form
    {
        public Series series;

        public Form1()
        {
            // Хранилище результатов выполнения функций
            series = new Series();

            // Инициализируем интерфейс
            InitializeComponent();

            // Фикс горизонтального скроллбара
            listView.Columns[0].Width = listView.Columns[1].Width = (listView.Width - 4 - SystemInformation.VerticalScrollBarWidth) / 2;

            // Включаем отрицательные числа
            textBoxa.Minimum = textBoxb.Minimum = textBoxx.Minimum = Int32.MinValue;
            textBoxa.Maximum = textBoxb.Maximum = textBoxx.Maximum = Int32.MaxValue;

            // Кликаем по первой радио-кнопке
            radioParabola.Checked = true;
        }

        // Модифицирует интерфейс в зависимости от текущей выбранной функции и возвращает функцию
        public MathFunction getFunction()
        {
            string a = textBoxa.Text;
            string b = textBoxb.Text;
            if (radioParabola.Checked)
            {
                labela.Text = "p";
                labelb.Visible = false;
                textBoxb.Visible = false;
                return new Parabola(a.Length > 0 ? Convert.ToDouble(a) : 0);
            }
            else if(radioHyperbola.Checked)
            {
                labela.Text = "a";
                labelb.Visible = true;
                textBoxb.Visible = true;
                return new Hyperbola(a.Length > 0 ? Convert.ToDouble(a) : 0, b.Length > 0 ? Convert.ToDouble(b) : 0);
            }
            else
            {
                labela.Text = "a";
                labelb.Visible = true;
                textBoxb.Visible = true;
                return new Ellipse(a.Length > 0 ? Convert.ToDouble(a) : 0, b.Length > 0 ? Convert.ToDouble(b) : 0);
            }
        }

        // Очищает результаты и устанавливает функцию в хранилище
        public void changeFunction()
        {
            listView.Items.Clear();
            series.setFunction(getFunction());
            labelFunc.Text = series.getInfo();
        }

        /* 
        * Хэндлеры нажатий и изменений элементов интерфейса 
        */
        private void textBoxa_TextChanged(object sender, EventArgs e)
        {
            if (textBoxa.Text.Length == 0)
            {
                textBoxa.Text = "0";
            }
            changeFunction();
        }

        private void textBoxb_TextChanged(object sender, EventArgs e)
        {
            if (textBoxb.Text.Length == 0)
            {
                textBoxb.Text = "0";
            }
            changeFunction();
        }

        private void radioParabola_CheckedChanged(object sender, EventArgs e)
        {
            changeFunction();
        }

        private void radioHyperbola_CheckedChanged(object sender, EventArgs e)
        {
            changeFunction();
        }

        private void radioEllipse_CheckedChanged(object sender, EventArgs e)
        {
            changeFunction();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
        }

        // Вызывает y(x), конвертирует результат в строки и добавляет в список
        private void buttonCalc_Click(object sender, EventArgs e)
        {
            if (textBoxx.Text.Length == 0)
            {
                textBoxx.Text = "0";
            }
            var item = series.store(textBoxx.Text.Length > 0 ? Convert.ToDouble(textBoxx.Text) : 0);
            string[] textItem = { Convert.ToString(item.Item1), Convert.ToString(item.Item2) };
            var listItem = new ListViewItem(textItem);
            listView.Items.Insert(0, listItem);
        }

        private void linkGraph_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.desmos.com/calculator/4gsilibcxn");
        }
    }
}
