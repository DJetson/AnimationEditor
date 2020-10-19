using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AnimationEditorCore.Views
{
    public partial class LayeredFrameView : UserControl
    {
        private static readonly DependencyProperty FrameLayersProperty = DependencyProperty.Register("FrameLayers", typeof(ObservableCollection<IFrameViewModel>), typeof(LayeredFrameView), new FrameworkPropertyMetadata(new ObservableCollection<IFrameViewModel>()));
        public ObservableCollection<IFrameViewModel> FrameLayers
        {
            get => (ObservableCollection<IFrameViewModel>)GetValue(FrameLayersProperty);
            set { SetValue(FrameLayersProperty, (ObservableCollection<IFrameViewModel>)value); }
        }

        public LayeredFrameView()
        {
            InitializeComponent();
        }
    }
}
