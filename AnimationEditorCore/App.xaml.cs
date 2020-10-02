using AnimationEditorCore.ViewModels;
using AnimationEditorCore.Views;
using System.Windows;

namespace AnimationEditorCore
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel() };
            MainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //TODO: Add an unsaved changes prompt here

            AnimationEditorCore.Properties.Settings.Default.Save();
        }
    }
}
