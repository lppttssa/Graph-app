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

namespace GraphApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		List<double> xList = new List<double>();
		List<double> yList = new List<double>();
		public MainWindow()
        {
            InitializeComponent();
        }

		private void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				string file = File.ReadAllText(openFileDialog.FileName);

				string[] lines = file.Split('\n');
				string[] xArrayString = lines[0].Split(' ');
				string[] yArrayString = lines[1].Split(' ');

				for (int i = 0; i < xArrayString.Length; i++)
                {
					xList.Add(Double.Parse(xArrayString[i]));
					yList.Add(Double.Parse(yArrayString[i]));
				}
			}

		}
	}

}
