using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AnimationEditor.ViewModels
{
    public class ColorPickerViewModel : ViewModelBase
    {
        private Color _SelectedColor;
        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; NotifyPropertyChanged(); }
        }

        //private int _ColorValue;
        //public int ColorValue
        //{
        //    get { return _ColorValue; }
        //    set { _ColorValue = value; NotifyPropertyChanged(); CalculateSelectedColor(); }
        //}

        //private void CalculateSelectedColor()
        //{
        //    int newColorR = 0;
        //    int newColorG = 0;
        //    int newColorB = 0;

        //    if (ColorValue <= 768)
        //    {
        //        var offSetColorValue = ColorValue - 256;

        //        if (offSetColorValue < 256)
        //        {
        //            newColorR = 255;
        //            newColorB = offSetColorValue;
        //        }
        //        else
        //        {
        //            offSetColorValue -= 256;
        //            newColorB = 255;
        //            newColorR = 255 - offSetColorValue;
        //        }
        //        newColorG = 0;
        //    }
        //    else if (ColorValue < 1280)
        //    {
        //        var offSetColorValue = ColorValue - 768;

        //        if (offSetColorValue < 256)
        //        {
        //            newColorB = 255;
        //            newColorG = offSetColorValue;
        //        }
        //        else
        //        {
        //            offSetColorValue -= 256;
        //            newColorG = 255;
        //            newColorB = 255 - offSetColorValue;
        //        }
        //        newColorR = 0;
        //    }
        //    else
        //    {
        //        var offSetColorValue = ColorValue - 1280;

        //        if (offSetColorValue < 256)
        //        {
        //            newColorG = 255;
        //            newColorR = offSetColorValue;
        //        }
        //        else
        //        {
        //            offSetColorValue -= 256;
        //            newColorR = 255;
        //            newColorG = 255 - offSetColorValue;
        //        }
        //        newColorB = 0;
        //    }
        //    SelectedColor = Color.FromArgb(255, (byte)newColorR, (byte)newColorG, (byte)newColorB);
        //}

        public ColorPickerViewModel()
        {
            SelectedColor = Color.FromRgb(255, 0, 0);
        }
    }
}
