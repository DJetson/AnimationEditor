using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels.Settings
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
