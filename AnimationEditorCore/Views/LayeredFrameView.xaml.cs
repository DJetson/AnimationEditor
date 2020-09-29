using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimationEditorCore.Views
{
    /// <summary>
    /// Interaction logic for LayeredFrameView.xaml
    /// </summary>
    public partial class LayeredFrameView : UserControl
    {
        private static readonly DependencyProperty FrameLayersProperty = DependencyProperty.Register("FrameLayers", typeof(ObservableCollection<FrameViewModel>), typeof(LayeredFrameView), new FrameworkPropertyMetadata(new ObservableCollection<FrameViewModel>()));
        public ObservableCollection<FrameViewModel> FrameLayers
        {
            get => (ObservableCollection<FrameViewModel>)GetValue(FrameLayersProperty);
            set { SetValue(FrameLayersProperty, (ObservableCollection<FrameViewModel>)value); }
        }

        public LayeredFrameView()
        {
            InitializeComponent();
        }
    }
}
