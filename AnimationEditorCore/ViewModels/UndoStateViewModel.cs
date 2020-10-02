using AnimationEditorCore.Interfaces;

namespace AnimationEditorCore.ViewModels
{
    public class UndoStateViewModel : ViewModelBase, IMemento
    {
        public override string DisplayName
        {
            get => _DisplayName;
            set { _DisplayName = value; NotifyPropertyChanged(); }
        }

        private IMementoOriginator _Originator;
        public IMementoOriginator Originator
        {
            get => _Originator;
            set { _Originator = value; NotifyPropertyChanged(); }
        }

        public UndoStateViewModel(IMementoOriginator viewModel, string stateName = "")
        {
            Originator = viewModel;
            DisplayName = stateName;
        }

        public virtual void LoadState()
        {

        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
