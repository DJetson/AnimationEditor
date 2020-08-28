using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.EditorTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace AnimationEditor.ViewModels
{
    public class EditorToolsViewModel : ViewModelBase
    {
        private static EditorToolsViewModel _Instance = null;
        public static EditorToolsViewModel Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new EditorToolsViewModel();
                return _Instance;
            }
        }

        private DrawingAttributes _DrawingAttributes = new DrawingAttributes();
        public DrawingAttributes DrawingAttributes
        {
            get => _DrawingAttributes;
            set { _DrawingAttributes = value; NotifyPropertyChanged();/* NotifyPropertyChanged(nameof(OnionSkingDrawingAttributes));*/ }
        }

        //public DrawingAttributes OnionSkingDrawingAttributes
        //{
        //    get
        //    {
        //        var onionSkinDrawingAttributes = new DrawingAttributes()
        //        {
        //            Color = Color.FromArgb(128, 255, 0, 0),
        //            FitToCurve = _DrawingAttributes.FitToCurve,
        //            Height = _DrawingAttributes.Height,
        //            Width = _DrawingAttributes.Width,
        //            IgnorePressure = _DrawingAttributes.IgnorePressure,
        //            IsHighlighter = true,
        //            StylusTip = _DrawingAttributes.StylusTip,
        //            StylusTipTransform = _DrawingAttributes.StylusTipTransform
        //        };

        //        //onionSkinDrawingAttributes.IsHighlighter = true;
        //        //onionSkinDrawingAttributes.Color = Color.FromArgb(128, 255, 0, 0);
        //        return onionSkinDrawingAttributes;
        //    }
        //}

        public string BrushColor
        {
            get => _DrawingAttributes.Color.ToString();
            set
            {
                _DrawingAttributes.Color = new Color()
                {
                    A = Byte.Parse(value.TrimStart('#').Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    R = Byte.Parse(value.TrimStart('#').Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    G = Byte.Parse(value.TrimStart('#').Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    B = Byte.Parse(value.TrimStart('#').Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                };
            }
        }

        public double BrushWidth
        {
            get => _DrawingAttributes.Width;
            set { _DrawingAttributes.Width = value; NotifyPropertyChanged(); }
        }

        public double BrushHeight
        {
            get => _DrawingAttributes.Height;
            set { _DrawingAttributes.Height = value; NotifyPropertyChanged(); }
        }


        private IEditorTool _CurrentTool;
        public IEditorTool CurrentTool
        {
            get => _CurrentTool;
            set { _CurrentTool = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _SelectTool;
        public DelegateCommand SelectTool
        {
            get => _SelectTool;
            set { _SelectTool = value; NotifyPropertyChanged(); }
        }

        private EditorToolsViewModel()
        {
            SelectTool = new DelegateCommand(SelectTool_CanExecute, SelectTool_Execute);
        }

        private InkCanvasEditingMode _EditingMode = InkCanvasEditingMode.Ink;
        public InkCanvasEditingMode EditingMode
        {
            get => _EditingMode;
            set { _EditingMode = value; NotifyPropertyChanged(); }
        }

        private EditorToolType _SelectedToolType;
        public EditorToolType SelectedToolType
        {
            get { return _SelectedToolType; }
            set { _SelectedToolType = value; NotifyPropertyChanged(); }
        }


        public void SelectTool_Execute(object obj)
        {
            var Parameter = (EditorToolType)Enum.Parse(typeof(EditorToolType), obj.ToString());

            SelectedToolType = Parameter;

            switch (Parameter)
            {
                case EditorToolType.Brush:
                    EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case EditorToolType.Eraser:
                    EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case EditorToolType.Lasso:
                    EditingMode = InkCanvasEditingMode.Select;
                    break;
                case EditorToolType.Zoom:
                    EditingMode = InkCanvasEditingMode.None;
                    //Enable Detecting Clicks on InkCanvas
                    break;
            }
        }

        public bool SelectTool_CanExecute(object obj)
        {
            var Parameter = Enum.Parse(typeof(EditorToolType), obj.ToString());

            return true;
        }
    }
}
