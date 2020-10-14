using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Properties;
using System.Windows;

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


        private void WriteToSettings()
        {
            Properties.Settings.Default.IsShowPreviousOnionSkinsEnabled = IsShowPreviousOnionSkinsEnabled;
            Properties.Settings.Default.IsShowNextOnionSkinsEnabled = IsShowNextOnionSkinsEnabled;
            Properties.Settings.Default.PreviousFrameSkinCount = PreviousFrameSkinCount;
            Properties.Settings.Default.NextFramesSkinCount = NextFramesSkinCount;
            Properties.Settings.Default.PreviousFrameSkinOpacityFalloff = PreviousFrameSkinOpacityFalloff;
            Properties.Settings.Default.NextFrameSkinOpacityFalloff = NextFrameSkinOpacityFalloff;

            Properties.Settings.Default.Save();
        }

        private void ReadFromSettings()
        {
            IsShowPreviousOnionSkinsEnabled = Properties.Settings.Default.IsShowPreviousOnionSkinsEnabled;
            IsShowNextOnionSkinsEnabled = Properties.Settings.Default.IsShowNextOnionSkinsEnabled;
            PreviousFrameSkinCount = Properties.Settings.Default.PreviousFrameSkinCount;
            NextFramesSkinCount = Properties.Settings.Default.NextFramesSkinCount;
            PreviousFrameSkinOpacityFalloff = Properties.Settings.Default.PreviousFrameSkinOpacityFalloff;
            NextFrameSkinOpacityFalloff = Properties.Settings.Default.NextFrameSkinOpacityFalloff;
        }
    }
}
