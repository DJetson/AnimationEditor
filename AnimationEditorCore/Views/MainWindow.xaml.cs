using AnimationEditorCore.ViewModels;
using System.Windows;
using System.Linq;

namespace AnimationEditorCore.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(sender is Window Sender))
                return;

            if (!(Sender.DataContext is MainWindowViewModel dc))
                return;


            foreach (var workspace in dc.WorkspaceManager.Workspaces.ToList())
            {
                dc.WorkspaceManager.SelectedWorkspace = workspace;
                
                if (workspace.Close() == false)
                    e.Cancel = true;
            }
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            if (!(sender is Window Sender))
                return;

            if (!(Sender.DataContext is MainWindowViewModel dc))
                return;

            dc.WorkspaceManager.CreateNewWorkspace();
        }
    }
}
