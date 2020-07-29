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
        double aLin, bLin, aExp, bExp;
        bool IsDraw, isLine;



        public MainWindow()
        {
            InitializeComponent();
        }

		private void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{
            xList.Clear();
            yList.Clear();
            xArrayString = new string[0];
            yArrayString = new string[0];
            TextBoxForPrediction.Clear();
            isLine = false;

            //чтение из файла
            ReadTxtFile(sender, e);

            FindLinTrendLine();

            FindExpTrendLine();

            CheckTrendLine();

            //отрисовка графика 
            DrawGraph();

            ShowStatistics();

            TextBoxForPrediction.IsEnabled = true;
        }


        public void ReadTxtFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            char[] delimiters = new char[] { ' ', '\t', ';' }; 
            if (openFileDialog.ShowDialog() == true)
            {

                string file = File.ReadAllText(openFileDialog.FileName);

                string[] lines = file.Split('\n');
                xArrayString = lines[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                yArrayString = lines[1].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                xName = xArrayString[0];
                yName = yArrayString[0];
                xArrayString = xArrayString.Skip(1).ToArray();
                yArrayString = yArrayString.Skip(1).ToArray();


                try
                {
                    for (int i = 0; i < xArrayString.Length; i++)
                    {
                        xList.Add(Double.Parse(xArrayString[i]));
                        yList.Add(Double.Parse(yArrayString[i]));
                    }
                }
                catch
                {
                    for (int i = 0; i < xArrayString.Length; i++)
                    {
                        xList.Add(i + 1);
                        yList.Add(Double.Parse(yArrayString[i]));
                    }
                }
                

            }
        }

        public void DrawGraph()
        {
            if (IsDraw)
            {
                SeriesCollection.RemoveAt(0);
                SeriesCollection.RemoveAt(0);
                SeriesCollection.Add(new LineSeries
                {
                    Title = yName,
                    Values = yList.AsChartValues()
                }); ; ;
            }
            else {
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = yName,
                        Values = yList.AsChartValues()
                    }
                };
            }

            AxisX.Title = xName;
            AxisY.Title = yName;
            

            Labels = xArrayString;

            List<double> yListForTrend = new List<double>();
            if (isLine)
            {
                for (int i = 0; i < xList.Count; i++)
                {
                    yListForTrend.Add(xList[i] * aLin + bLin);
                }
            }
            else
            {
                for (int i = 0; i < xList.Count; i++)
                {
                    yListForTrend.Add(aExp * Math.Pow(Math.E, bExp * xList[i]));
                }
            }
            

            this.SeriesCollection.Add(new LineSeries
            {
                Title = "Trend",
                Values = yListForTrend.AsChartValues()
            });

            IsDraw = true;
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
                aLin = detA / det;

                double detB = SumXSqr * SumY - SumX * SumXY;
                bLin = detB / det;
            }
        }

        public void FindExpTrendLine()
        {
            double SumX = 0, SumY = 0, SumXSqr = 0, SumXY = 0;
            for (int i = 0; i < xList.Count; i++)
            {
                SumX += xList[i];
                SumY += Math.Log(yList[i]);
                SumXSqr += xList[i] * xList[i];
                SumXY += Math.Log(yList[i]) * xList[i];
            }

            aExp = Math.Pow(Math.E, Convert.ToDouble((SumY * SumXSqr - SumXY * SumX))
                / Convert.ToDouble((xList.Count * SumXSqr - SumX * SumX)));
            bExp = Convert.ToDouble((xList.Count * SumXY - SumY * SumX))
                / Convert.ToDouble((xList.Count * SumXSqr - SumX * SumX));
        }

        public void CheckTrendLine()
        {
            double LineMistake = 0, ExpMistake = 0;
            for (int i = 0; i < yList.Count; i++)
            {
                LineMistake += Math.Abs(yList[i] - (aLin * xList[i] + bLin));
                ExpMistake += Math.Abs(yList[i] - aExp * Math.Pow(Math.E, bExp * xList[i]));
            }
            if (LineMistake < ExpMistake)
            {
                isLine = true;
            }

        }

        public void ShowStatistics()
        {
            try
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

                TextBlockForAvg.Text = "Average value:" + "\n" + avg.ToString();

                TextBlockForMax.Text = "Max element:" + "\n" + max.ToString();

                TextBlockForMin.Text = "Min element:" + "\n" + min.ToString();
            }
            catch
            {

            }

        }
   
        public SeriesCollection SeriesCollection { get; set; }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TextBoxForPrediction_KeyDown(object sender, KeyEventArgs e)
        {
            int predNum;
            if (e.Key == Key.Enter)
            {
                try
                {
                    predNum = Convert.ToInt32(TextBoxForPrediction.Text);
                    string[] memArray = new string[xArrayString.Length + predNum];
                    double dif = xList[1] - xList[0];
                    for (int i = 0; i < xArrayString.Length; i++)
                    {
                        memArray[i] = xArrayString[i];
                    }
                    for (int i = 1; i <= predNum; i++)
                    {
                        xList.Add(xList[xList.Count - 1] + dif);
                        memArray[xList.Count - 1] = xList[xList.Count - 1].ToString();
                    }
                    xArrayString = memArray;
                    TextBoxForPrediction.Clear();
                    DrawGraph();
                }
                catch
                {

                }
            }
        }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }

}
