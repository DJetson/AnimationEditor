namespace AnimationEditorCore.ViewModels.Settings
{
    public class ViewSettingsViewModel : ViewModelBase
    {
        private OnionSkinSettingsViewModel _OnionSkinSettings = new OnionSkinSettingsViewModel();
        public OnionSkinSettingsViewModel OnionSkinSettings
        {
            get => _OnionSkinSettings;
            set { _OnionSkinSettings = value; NotifyPropertyChanged(); }
        }
    }
}
