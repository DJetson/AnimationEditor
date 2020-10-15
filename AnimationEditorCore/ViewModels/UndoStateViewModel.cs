using AnimationEditorCore.Interfaces;

namespace AnimationEditorCore.ViewModels
{
    public enum StateType { Undo, Current, Redo };
    public class UndoStateViewModel : ViewModelBase, IMemento
    {

        private StateType _CurrentStateType;
        public StateType CurrentStateType
        {
            get { return _CurrentStateType; }
            set { _CurrentStateType = value; NotifyPropertyChanged(); }
        }

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
            CurrentStateType = StateType.Current;
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
