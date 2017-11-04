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

            // Кликаем по первой радио-кнопке
            radioParabola.Checked = true;
        }

        // Модифицирует интерфейс в зависимости от текущей выбранной функции и возвращает функцию
        public MathFunction getFunction()
        {
            if (radioParabola.Checked)
            {
                labela.Text = "p";
                labelb.Visible = false;
                textBoxb.Visible = false;
                return new Parabola(Convert.ToDouble(textBoxa.Text));
            }
            else if(radioHyperbola.Checked)
            {
                labela.Text = "a";
                labelb.Visible = true;
                textBoxb.Visible = true;
                return new Hyperbola(Convert.ToDouble(textBoxa.Text), Convert.ToDouble(textBoxb.Text));
            }
            else
            {
                labela.Text = "a";
                labelb.Visible = true;
                textBoxb.Visible = true;
                return new Ellipse(Convert.ToDouble(textBoxa.Text), Convert.ToDouble(textBoxb.Text));
            }
        }

        // Очищает результаты и устанавливает функцию в хранилище
        public void changeFunction()
        {
            listView.Items.Clear();
            series.setFunction(getFunction());
        }

        /* 
        * Хэндлеры нажатий и изменений элементов интерфейса 
        */
        private void textBoxa_TextChanged(object sender, EventArgs e)
        {
            changeFunction();
        }

        private void textBoxb_TextChanged(object sender, EventArgs e)
        {
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
            var item = series.store(Convert.ToDouble(textBoxx.Text));
            string[] textItem = { Convert.ToString(item.Item1), Convert.ToString(item.Item2) };
            var listItem = new ListViewItem(textItem);
            listView.Items.Add(listItem);
        }
    }
}
