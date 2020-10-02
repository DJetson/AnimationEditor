using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;

namespace AnimationEditorCore.Views
{
    public partial class OnionSkinView : UserControl
    {
        private static readonly DependencyProperty OnionSkinStrokesProperty = DependencyProperty.Register("OnionSkinStrokes", typeof(StrokeCollection), typeof(OnionSkinView), new FrameworkPropertyMetadata(new StrokeCollection()));
        public StrokeCollection OnionSkinStrokes
        {
            get => (StrokeCollection)GetValue(OnionSkinStrokesProperty);
            set { SetValue(OnionSkinStrokesProperty, (StrokeCollection)value); }
        }

        private static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel", typeof(double), typeof(OnionSkinView), new FrameworkPropertyMetadata(1.0d));
        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set { SetValue(ZoomLevelProperty, (double)value); }
        }

        private static readonly DependencyProperty IsOnionSkinEnabledProperty = DependencyProperty.Register("IsOnionSkinEnabled", typeof(bool), typeof(OnionSkinView), new FrameworkPropertyMetadata(true,IsOnionSkinEnabled_Changed));
        public bool IsOnionSkinEnabled
        {
            get => (bool)GetValue(IsOnionSkinEnabledProperty);
            set { SetValue(IsOnionSkinEnabledProperty, (bool)value); }
        }

        private static void IsOnionSkinEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as OnionSkinView;

            if (instance != null)
                instance.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
        }

        public OnionSkinView()
        {
            InitializeComponent();
        }
    }
}
