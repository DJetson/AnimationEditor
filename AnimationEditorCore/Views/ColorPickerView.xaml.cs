using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AnimationEditorCore.Views
{
    public partial class ColorPickerView : Window
    {
        private static readonly DependencyProperty TriColorProperty = DependencyProperty.Register("TriColor", typeof(Color), typeof(ColorPickerView));
        public Color TriColor
        {
            get => (Color)GetValue(TriColorProperty);
            set { SetValue(TriColorProperty, (Color)value); }
        }

        private static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerView), new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 0, 0)));
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set { SetValue(SelectedColorProperty, (Color)value); }
        }

        private static readonly DependencyProperty LastSelectedColorProperty = DependencyProperty.Register("LastSelectedColor", typeof(Color), typeof(ColorPickerView), new FrameworkPropertyMetadata(Color.FromArgb(255, 255, 0, 0)));
        public Color LastSelectedColor
        {
            get => (Color)GetValue(LastSelectedColorProperty);
            set { SetValue(LastSelectedColorProperty, (Color)value); }
        }

        public ColorPickerView()
        {
            InitializeComponent();
        }

        public ColorPickerView(Color currentColor, Color lastColor)
        {
            LastSelectedColor = lastColor;
            SelectedColor = currentColor;
            
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var ColorValue = (int)e.NewValue;

            int newColorR;
            int newColorG;
            int newColorB;

            if (ColorValue <= 768)
            {
                var offSetColorValue = ColorValue - 256;

                if (offSetColorValue < 256)
                {
                    newColorR = 255;
                    newColorB = offSetColorValue;
                }
                else
                {
                    offSetColorValue -= 256;
                    newColorB = 255;
                    newColorR = 255 - offSetColorValue;
                }
                newColorG = 0;
            }
            else if (ColorValue < 1280)
            {
                var offSetColorValue = ColorValue - 768;

                if (offSetColorValue < 256)
                {
                    newColorB = 255;
                    newColorG = offSetColorValue;
                }
                else
                {
                    offSetColorValue -= 256;
                    newColorG = 255;
                    newColorB = 255 - offSetColorValue;
                }
                newColorR = 0;
            }
            else
            {
                var offSetColorValue = ColorValue - 1280;

                if (offSetColorValue < 256)
                {
                    newColorG = 255;
                    newColorR = offSetColorValue;
                }
                else
                {
                    offSetColorValue -= 256;
                    newColorR = 255;
                    newColorG = 255 - offSetColorValue;
                }
                newColorB = 0;
            }
            TriColor = Color.FromArgb(255, (byte)newColorR, (byte)newColorG, (byte)newColorB);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid Sender)
            {
                var gridActualWidth = Sender.ActualWidth;
                var gridActualHeight = Sender.ActualHeight;

                var position = e.GetPosition(Sender);

                var normalizedPosition = new Point((gridActualWidth - position.X) * (1 / gridActualWidth), (gridActualHeight - position.Y) * (1 / gridActualHeight));

                Console.WriteLine($"Normalized MousePosition:{normalizedPosition}");
                var selectedColor = Color.FromArgb((byte)255,
                    (byte)(normalizedPosition.Y * (TriColor.R + ((255 - TriColor.R) * normalizedPosition.X))),
                    (byte)(normalizedPosition.Y * (TriColor.G + ((255 - TriColor.G) * normalizedPosition.X))),
                    (byte)(normalizedPosition.Y * (TriColor.B + ((255 - TriColor.B) * normalizedPosition.X))));

                LastSelectedColor = SelectedColor;
                SelectedColor = selectedColor;

                Console.WriteLine($"SelectedColor:{SelectedColor}");
            }
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
