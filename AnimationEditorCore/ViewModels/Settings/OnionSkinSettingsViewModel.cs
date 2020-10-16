using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Views;
using System.Windows;
using System.Windows.Media;

namespace AnimationEditorCore.ViewModels.Settings
{
    public class OnionSkinSettingsViewModel : ViewModelBase
    {
        public override string DisplayName => Resources.OnionSkinSettingsViewModelDisplayName;

        private DelegateCommand _AcceptChanges;
        public DelegateCommand AcceptChanges
        {
            get { return _AcceptChanges; }
            set { _AcceptChanges = value; NotifyPropertyChanged(); }
        }

        private bool AcceptChanges_CanExecute(object parameter)
        {
            if (!(parameter is Window Parameter))
            {
                return false;
            }

            return true;
        }

        private void AcceptChanges_Execute(object parameter)
        {
            var Parameter = parameter as Window;

            WriteToSettings();

            Parameter.Close();
        }

        private DelegateCommand _DiscardChanges;
        public DelegateCommand DiscardChanges
        {
            get { return _DiscardChanges; }
            set { _DiscardChanges = value; NotifyPropertyChanged(); }
        }

        private bool DiscardChanges_CanExecute(object parameter)
        {
            if (!(parameter is Window Parameter))
            {
                return false;
            }

            return true;
        }

        private void DiscardChanges_Execute(object parameter)
        {
            var Parameter = parameter as Window;

            Parameter.Close();
        }

        private DelegateCommand _ShowColorPickerWindow;
        public DelegateCommand ShowColorPickerWindow
        {
            get { return _ShowColorPickerWindow; }
            set { _ShowColorPickerWindow = value; NotifyPropertyChanged(); }
        }

        private bool ShowColorPickerWindow_CanExecute(object parameter)
        {
            if (!(parameter is Color Parameter))
            {
                return false;
            }

            return true;
        }

        private void ShowColorPickerWindow_Execute(object parameter)
        {
            ColorPickerView colorPicker = new ColorPickerView(GridLineColor, GridLineColor);
            var result = colorPicker.ShowDialog();

            if (result == true)
            {
                GridLineColor = colorPicker.SelectedColor;
            }
        }

        public OnionSkinSettingsViewModel()
        {
            ReadFromSettings();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AcceptChanges = new DelegateCommand(AcceptChanges_CanExecute, AcceptChanges_Execute);
            DiscardChanges = new DelegateCommand(DiscardChanges_CanExecute, DiscardChanges_Execute);
        }

        private bool _IsShowPreviousOnionSkinsEnabled;
        public bool IsShowPreviousOnionSkinsEnabled
        {
            get { return _IsShowPreviousOnionSkinsEnabled; }
            set { _IsShowPreviousOnionSkinsEnabled = value; NotifyPropertyChanged(); }
        }

        private bool _IsShowNextOnionSkinsEnabled;
        public bool IsShowNextOnionSkinsEnabled
        {
            get { return _IsShowNextOnionSkinsEnabled; }
            set { _IsShowNextOnionSkinsEnabled = value; NotifyPropertyChanged(); }
        }

        private int _PreviousFrameSkinCount;
        public int PreviousFrameSkinCount
        {
            get => _PreviousFrameSkinCount;
            set { _PreviousFrameSkinCount = value; NotifyPropertyChanged(); }
        }

        private int _NextFramesSkinCount;
        public int NextFramesSkinCount
        {
            get => _NextFramesSkinCount;
            set { _NextFramesSkinCount = value; NotifyPropertyChanged(); }
        }

        private double _PreviousFrameSkinOpacityFalloff;
        public double PreviousFrameSkinOpacityFalloff
        {
            get => _PreviousFrameSkinOpacityFalloff;
            set { _PreviousFrameSkinOpacityFalloff = value; NotifyPropertyChanged(); }
        }

        private double _NextFrameSkinOpacityFalloff;
        public double NextFrameSkinOpacityFalloff
        {
            get { return _NextFrameSkinOpacityFalloff; }
            set { _NextFrameSkinOpacityFalloff = value; NotifyPropertyChanged(); }
        }

        private bool _IsShowGridEnabled;
        public bool IsShowGridEnabled
        {
            get { return _IsShowGridEnabled; }
            set { _IsShowGridEnabled = value; NotifyPropertyChanged(); }
        }

        private double _GridLineOpacity;
        public double GridLineOpacity
        {
            get { return _GridLineOpacity; }
            set { _GridLineOpacity = value; NotifyPropertyChanged(); }
        }

        private double _GridLineThickness;
        public double GridLineThickness
        {
            get { return _GridLineThickness; }
            set { _GridLineThickness = value; NotifyPropertyChanged(); }
        }

        private double _GridUnitSize;
        public double GridUnitSize
        {
            get { return _GridUnitSize; }
            set { _GridUnitSize = value; NotifyPropertyChanged(); }
        }

        //private double _GridUnitWidth;
        //public double GridUnitWidth
        //{
        //    get { return _GridUnitWidth; }
        //    set { _GridUnitWidth = value; NotifyPropertyChanged(); }
        //}

        //private double _GridUnitHeight;
        //public double GridUnitHeight
        //{
        //    get { return _GridUnitHeight; }
        //    set { _GridUnitHeight = value; NotifyPropertyChanged(); }
        //}

        private Color _GridLineColor;
        public Color GridLineColor
        {
            get { return _GridLineColor; }
            set { _GridLineColor = value; NotifyPropertyChanged(); }
        }


        private void WriteOnionSkinSettings()
        {
            Properties.Settings.Default.IsShowPreviousOnionSkinsEnabled = IsShowPreviousOnionSkinsEnabled;
            Properties.Settings.Default.IsShowNextOnionSkinsEnabled = IsShowNextOnionSkinsEnabled;
            Properties.Settings.Default.PreviousFrameSkinCount = PreviousFrameSkinCount;
            Properties.Settings.Default.NextFramesSkinCount = NextFramesSkinCount;
            Properties.Settings.Default.PreviousFrameSkinOpacityFalloff = PreviousFrameSkinOpacityFalloff;
            Properties.Settings.Default.NextFrameSkinOpacityFalloff = NextFrameSkinOpacityFalloff;
        }

        private void WriteGridSettings()
        {
            Properties.Settings.Default.IsShowGridEnabled = IsShowGridEnabled;
            Properties.Settings.Default.GridLineOpacity = GridLineOpacity;
            Properties.Settings.Default.GridLineThickness = GridLineThickness;
            Properties.Settings.Default.GridUnitSize = new Rect(0, 0, GridUnitSize, GridUnitSize);
            Properties.Settings.Default.GridLineColor = GridLineColor;
        }

        private void WriteToSettings()
        {
            WriteOnionSkinSettings();
            WriteGridSettings();

            Properties.Settings.Default.Save();
        }

        private void ReadOnionSkinSettings()
        {
            IsShowPreviousOnionSkinsEnabled = Properties.Settings.Default.IsShowPreviousOnionSkinsEnabled;
            IsShowNextOnionSkinsEnabled = Properties.Settings.Default.IsShowNextOnionSkinsEnabled;
            PreviousFrameSkinCount = Properties.Settings.Default.PreviousFrameSkinCount;
            NextFramesSkinCount = Properties.Settings.Default.NextFramesSkinCount;
            PreviousFrameSkinOpacityFalloff = Properties.Settings.Default.PreviousFrameSkinOpacityFalloff;
            NextFrameSkinOpacityFalloff = Properties.Settings.Default.NextFrameSkinOpacityFalloff;
        }

        private void ReadGridSettings()
        {
            IsShowGridEnabled = Properties.Settings.Default.IsShowGridEnabled;
            GridLineOpacity = Properties.Settings.Default.GridLineOpacity;
            GridLineThickness = Properties.Settings.Default.GridLineThickness;
            GridUnitSize = Properties.Settings.Default.GridUnitSize.Width;
            //GridUnitHeight = Properties.Settings.Default.GridUnitSize.Height;
            GridLineColor = Properties.Settings.Default.GridLineColor;
        }

        private void ReadFromSettings()
        {
            ReadOnionSkinSettings();
            ReadGridSettings();
        }
    }
}
