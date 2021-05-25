using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AppGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> dllNames;
        private readonly List<string> dllPaths;

        public MainWindow()
        {
            InitializeComponent();
            dllNames = new List<string>();
            dllPaths = new List<string>();
        }


        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            ApplicationGUI.Run(dllPaths.ToArray(), Report);
            ApplicationGUI.Dashboard.WriteLn("Starting application...");
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationGUI.Dashboard != null)
            {
                ApplicationGUI.Dashboard.WriteLn("Stopping application...");
                ApplicationGUI.EventTracer.GetKernelSession()?.Dispose();
            }
            Application.Current.Shutdown();
        }

        private void buttonAddAnalyzers_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".dll",
                Filter = "DLL Files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var result = dlg.ShowDialog();

            if (result != true) return;
            foreach (var filename in dlg.FileNames)
            {
                dllNames.Add(System.IO.Path.GetFileName(filename));
                dllPaths.Add(filename);
                AnalyzerName.Items.Add(System.IO.Path.GetFileName(filename));
                ApplicationGUI.Dashboard.WriteLn("Loading Analyzer: " + filename);
                ApplicationGUI.LoadAnalyzer(filename);
            }
        }

        private void buttonKill_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationGUI.Dashboard != null)
            {
                ApplicationGUI.Dashboard.WriteLn("Trying to kill " + ProcessId.Text + " process...");
                if (int.TryParse(ProcessId.Text, out var id))
                {
                    ApplicationGUI.Dashboard.Kill(id);
                }
                else
                {
                    ApplicationGUI.Dashboard.WriteLn("Please enter valid process id.");
                }
            }
            else
            {
                Report.Text += "Please start application.\n";
            }
        }
    }
}
