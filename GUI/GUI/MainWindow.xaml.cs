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
using DeveloperKit;

namespace GUI
{
    /// <summary>
    /// Logic MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> fileNames;
        private List<string> filePaths;
        private List<string> dllNames;
        private List<string> dllPaths;

        public MainWindow()
        {
            InitializeComponent();
            fileNames = new List<string>();
            filePaths = new List<string>();
            dllNames = new List<string>();
            dllPaths = new List<string>();
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
                    filePaths.Add(filename);
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
                    dllPaths.Add(filename);
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
                Report.Text += "Please choose executables and dll\n";
                return;
            }

            // Start analyzing:
            // ...
            int len = fileNames.Count + dllNames.Count;
            if (len < 2 || len % 2 != 0)
            {
                Report.Text += "Not enough arguments: couples required <filepath> <dllpath> ...";
                return;
            }
            string[] args = new string[len];
            int j = 0;
            for (int i = 0; i < len; i += 2)
            {
                args[i] = filePaths[j];
                args[i + 1] = dllPaths[j];
                j++;
            }
            //args[0] = @"C:\Users\jlemi\Downloads\FixMouseLMB.exe";
            //args[1] = @"C:\Users\jlemi\Documents\GitHub\Antivirus-Project\HashDll\HashDll\bin\Debug\HashDll.dll";
            AntivirusRunner antivirusRunner = new AntivirusRunner(Report);
            antivirusRunner.Run(args);
        }
    }
}
