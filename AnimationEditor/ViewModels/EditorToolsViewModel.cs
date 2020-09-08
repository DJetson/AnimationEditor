using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.EditorTools;
using AnimationEditor.Views;
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

        private DelegateCommand _ShowColorPickerWindow;
        public DelegateCommand ShowColorPickerWindow
        {
            get { return _ShowColorPickerWindow; }
            set { _ShowColorPickerWindow = value; NotifyPropertyChanged(); }
        }


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

        private Color _LastSelectedBrushColor;
        public Color LastSelectedBrushColor
        {
            get { return _LastSelectedBrushColor; }
            set { _LastSelectedBrushColor = value; NotifyPropertyChanged(); }
        }

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

        private Color _SelectedBrushColor = Color.FromArgb(255,0,0,0);
        public Color SelectedBrushColor
        {
            get { return _SelectedBrushColor; }
            set { _SelectedBrushColor = value; NotifyPropertyChanged(); UpdateDrawingAttributes(); }
        }

        private void UpdateDrawingAttributes()
        {
            var colorWithOpacity = Color.FromArgb((byte)(255 * (BrushOpacity / 100)), SelectedBrushColor.R, SelectedBrushColor.G, SelectedBrushColor.B);
            _DrawingAttributes.Color = colorWithOpacity;
            _DrawingAttributes.Width = BrushSize;
            _DrawingAttributes.Height = BrushSize;
            //_DrawingAttributes.Height = BrushHeight;
        }

        public double BrushSize
        {
            get => _DrawingAttributes.Width;
            set { _DrawingAttributes.Width = value; NotifyPropertyChanged(); UpdateDrawingAttributes(); }
        }

        private double _BrushOpacity = 100;
        public double BrushOpacity
        {
            get { return _BrushOpacity; }
            set { _BrushOpacity = value; NotifyPropertyChanged(); UpdateDrawingAttributes(); }
        }


        //public double BrushHeight
        //{
        //    get => _DrawingAttributes.Height;
        //    set { _DrawingAttributes.Height = value; NotifyPropertyChanged(); UpdateDrawingAttributes(); }
        //}


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

        public void InitializeCommands()
        {
            SelectTool = new DelegateCommand(SelectTool_CanExecute, SelectTool_Execute);
            ShowColorPickerWindow = new DelegateCommand(ShowColorPickerWindow_CanExecute, ShowColorPickerWindow_Execute);
        }

        private EditorToolsViewModel()
        {
            InitializeCommands();
        }

        public bool ShowColorPickerWindow_CanExecute(object parameter)
        {
            return true;
        }

        public void ShowColorPickerWindow_Execute(object parameter)
        {
            var ColorPickerWindow = new ColorPickerView(SelectedBrushColor, LastSelectedBrushColor);
            var result = ColorPickerWindow.ShowDialog();

            if(result == true)
            {
                LastSelectedBrushColor = ColorPickerWindow.LastSelectedColor;
                SelectedBrushColor = ColorPickerWindow.SelectedColor;
            }
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
