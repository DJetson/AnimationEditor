using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimationEditorCore.Views
{
    /// <summary>
    /// Interaction logic for OnionSkinView.xaml
    /// </summary>
    public partial class OnionSkinView : UserControl
    {
        private static readonly DependencyProperty OnionSkinStrokesProperty = DependencyProperty.Register("OnionSkinStrokes", typeof(StrokeCollection), typeof(OnionSkinView), new FrameworkPropertyMetadata(new StrokeCollection(), OnionSkinStrokes_Changed));
        public StrokeCollection OnionSkinStrokes
        {
            get => (StrokeCollection)GetValue(OnionSkinStrokesProperty);
            set { SetValue(OnionSkinStrokesProperty, (StrokeCollection)value); }
        }

        private static void OnionSkinStrokes_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        private static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register("ZoomLevel", typeof(double), typeof(OnionSkinView), new FrameworkPropertyMetadata(1.0d, ZoomLevel_Changed));
        public double ZoomLevel
        {
            get => (double)GetValue(ZoomLevelProperty);
            set { SetValue(ZoomLevelProperty, (double)value); }
        }

        private static void ZoomLevel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

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
