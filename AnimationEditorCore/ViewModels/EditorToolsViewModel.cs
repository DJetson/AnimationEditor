using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.EditorTools;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;

namespace AnimationEditorCore.ViewModels
{
    public class EditorToolsViewModel : ViewModelBase
    {

        private DelegateCommand _ShowColorPickerWindow;
        public DelegateCommand ShowColorPickerWindow
        {
            get { return _ShowColorPickerWindow; }
            set { _ShowColorPickerWindow = value; NotifyPropertyChanged(); }
        }

        public static void SelectToolType(EditorToolType parameter)
        {
            Instance.SelectedToolType = parameter;

            switch (parameter)
            {
                case EditorToolType.Brush:
                    Instance.EditingMode = InkCanvasEditingMode.Ink;
                    break;
                case EditorToolType.Eraser:
                    Instance.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    break;
                case EditorToolType.Lasso:
                    Instance.EditingMode = InkCanvasEditingMode.Select;
                    break;
                case EditorToolType.Zoom:
                    Instance.EditingMode = InkCanvasEditingMode.None;
                    break;
            }
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

        private IEditorTool _CurrentTool;
        public IEditorTool CurrentTool
        {
            get => _CurrentTool;
            set { _CurrentTool = value; NotifyPropertyChanged(); }
        }

        public void InitializeCommands()
        {
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
    }
}
