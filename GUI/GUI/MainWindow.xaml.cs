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

namespace GUI
{
    /// <summary>
    /// Logic MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> fileNames;
        private List<string> dllNames;

        public MainWindow()
        {
            InitializeComponent();
            fileNames = new List<string>();
            dllNames = new List<string>();
        }

        /// <summary>
        /// Open files button logic
        /// </summary>
        private void buttonOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".exe",
                Filter = "EXE Files (*.exe)|*.exe|All files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    fileNames.Add(System.IO.Path.GetFileName(filename));
                    FileName.Items.Add(System.IO.Path.GetFileName(filename));
                }
            }

        }

        /// <summary>
        /// Choose analyzers button logic
        /// </summary>
        private void buttonChooseAnalyzers_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".dll",
                Filter = "DLL Files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };


            bool? result = dlg.ShowDialog();


            if (result == true)
            {
                foreach (string filename in dlg.FileNames)
                {
                    dllNames.Add(System.IO.Path.GetFileName(filename));
                    AnalyzerName.Items.Add(System.IO.Path.GetFileName(filename));
                }
            }

        }

        /// <summary>
        /// Analyze button logic
        /// </summary>
        private void buttonAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (fileNames.Count == 0 || dllNames.Count == 0)
            {
                Report.Text = "Please choose executables and dll";
                return;
            }

            // Start analyzing:
            // ...
        }
    }
}
