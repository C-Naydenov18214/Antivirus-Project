using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AppGUI
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
        }
    }
}
