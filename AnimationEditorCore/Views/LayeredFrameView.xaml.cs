using AnimationEditorCore.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AnimationEditorCore.Views
{
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
