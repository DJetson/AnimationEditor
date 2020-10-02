using AnimationEditorCore.BaseClasses;

namespace AnimationEditorCore.ViewModels
{
    public class ViewModelBase : NotifyBase
    {
        protected string _DisplayName;
        public virtual string DisplayName
        {
            get => _DisplayName;
            set { _DisplayName = value; NotifyPropertyChanged(); }
        }
    }
}
