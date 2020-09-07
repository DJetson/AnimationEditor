using AnimationEditor.ViewModels;
using AnimationEditor.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel() };
            MainWindow.Show();

            //new Window() { Content = new ColorPickerView() { DataContext = new ColorPickerViewModel() } }.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //This is the best way I can think of right now to save settings that are
            //stored in the Settings table and also have their state bound directly to 
            //File menu checkboxes as there is no other point in the existing control flow
            //in which to reasonably insert a call to the Save method.
            AnimationEditor.Properties.Settings.Default.Save();
        }
    }
}
