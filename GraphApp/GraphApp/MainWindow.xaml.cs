using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;

namespace GraphApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		List<double> xList = new List<double>();
		List<double> yList = new List<double>();
        string[] xArrayString = new string[0];
        string[] yArrayString = new string[0];
        string xName, yName;
        double a, b;



        public MainWindow()
        {
            InitializeComponent();
        }

		private void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{

            //чтение из файла
            ReadTxtFile(sender, e);

            //отрисовка графика 
            DrawGraph();

            ShowStatistics();
        }


        public void ReadTxtFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string file = File.ReadAllText(openFileDialog.FileName);

                string[] lines = file.Split('\n');
                xArrayString = lines[2].Split(' ');
                yArrayString = lines[3].Split(' ');
                xName = lines[0];
                yName = lines[1];


                for (int i = 0; i < xArrayString.Length; i++)
                {
                    xList.Add(Double.Parse(xArrayString[i]));
                    yList.Add(Double.Parse(yArrayString[i]));
                }
            }
        }

        public void DrawGraph()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Values = yList.AsChartValues()
                }
            };

            Labels = xArrayString;



            FindLinTrendLine();
            List<double> yListForTrend = new List<double>();
            for (int i = 0; i < yList.Count; i++)
            {
                yListForTrend.Add(xList[i] * a + b);
            }

            SeriesCollection.Add(new LineSeries
            {
                Values = yListForTrend.AsChartValues()
            });

            DataContext = this;
        }

        public void FindLinTrendLine()
        {
            double SumX = 0, SumXSqr = 0, SumXY = 0, SumY = 0;

            for (int i = 0; i < xList.Count; i++)
            {
                SumX += xList[i];
                SumXSqr += xList[i] * xList[i];
                SumXY += xList[i] * yList[i];
                SumY += yList[i];
            }

            double det = SumXSqr * xList.Count - SumX * SumX;
            if (det != 0)
            {
                double detA = SumXY * xList.Count - SumY * SumX;
                a = detA / det;

                double detB = SumXSqr * SumY - SumX * SumXY;
                b = detB / det;
            }
        }

        public void ShowStatistics()
        {
            double avg = 0, max, min;
            max = yList[0];
            min = yList[0];

            for (int i = 0; i < yList.Count; i++)
            {
                avg += yList[i];
                if (max < yList[i])
                    max = yList[i];
                if (min > yList[i])
                    min = yList[i];
            }
            avg /= yList.Count;

            /*TextBlock textStat = new TextBlock();
            textStat.Text = "Statistics";
            textStat.FontSize = 22;
            textStat.FontFamily = new FontFamily("Arial");
            textStat.TextAlignment = TextAlignment.Center;
            textStat.Width = StatisticListBox.Width - 20;
            textStat.Text = "Statistics";
            StatisticListBox.Items.Add(textStat);*/

            /*TextBlock text = new TextBlock();
            text.Text = "Average value:" + "\n" + avg.ToString();
            text.FontSize = 18;
            text.FontFamily = new FontFamily("Arial");
            text.TextAlignment = TextAlignment.Center;
            text.Width = StatisticListBox.Width - 20;*/
            TextBlockForAvg.Text = "Average value:" + "\n" + avg.ToString();
            //StatisticListBox.Items.Add(text);

            /*TextBlock text1 = new TextBlock();
            text1.FontSize = 18;
            text1.FontFamily = new FontFamily("Arial");
            text1.TextAlignment = TextAlignment.Center;
            text1.Text = "Max element:" + "\n" + max.ToString();
            text1.Width = StatisticListBox.Width - 20;
            StatisticListBox.Items.Add(text1);*/
            TextBlockForMax.Text = "Max element:" + "\n" + max.ToString();

            /*TextBlock text2 = new TextBlock();
            text2.FontSize = 18;
            text2.FontFamily = new FontFamily("Arial");
            text2.TextAlignment = TextAlignment.Center;
            text2.Text = "Min element:" + "\n" + min.ToString();
            text2.Width = StatisticListBox.Width - 20;
            StatisticListBox.Items.Add(text2);*/
            TextBlockForMin.Text = "Min element:" + "\n" + min.ToString();

        }

        public SeriesCollection SeriesCollection { get; set; }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }

}
