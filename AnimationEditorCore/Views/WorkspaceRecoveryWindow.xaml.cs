using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnimationEditorCore.Views
{
    /// <summary>
    /// Interaction logic for WorkspaceRecoveryWindow.xaml
    /// </summary>
    public partial class WorkspaceRecoveryWindow : Window
    {
        public WorkspaceRecoveryWindow()
        {
            InitializeComponent();
        }

        private void HeaderCheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox Sender))
                return;

            if (!(Sender.DataContext is WorkspaceRecoveryViewModel dc))
                return;

            int totalItems = dc.WorkspaceFileItems.Count;
            int selectedItems = dc.WorkspaceFileItems.Where(e => e.IsSelected).Count();

            if (selectedItems < totalItems)
            {
                foreach (var item in dc.WorkspaceFileItems)
                {
                    item.IsSelected = true;
                }
                Sender.IsChecked = true;
            }
            else if(selectedItems == totalItems)
            {
                foreach (var item in dc.WorkspaceFileItems)
                {
                    item.IsSelected = false;
                }
                Sender.IsChecked = false;
            }

            e.Handled = true;
        }

        private void UpdateHeaderCheckBoxState()
        {
            if (!(HeaderCheckBox.DataContext is WorkspaceRecoveryViewModel dc))
                return;

            int totalItems = dc.WorkspaceFileItems.Count;
            int selectedItems = dc.WorkspaceFileItems.Where(e => e.IsSelected).Count();

            if (selectedItems < 0)
            {
                HeaderCheckBox.IsChecked = false;
            }
            else if (selectedItems == totalItems)
            {
                HeaderCheckBox.IsChecked = true;
            }
            else
            {
                HeaderCheckBox.IsChecked = null;
            }
        }

        private void FileItemCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateHeaderCheckBoxState();
        }
    }
}
