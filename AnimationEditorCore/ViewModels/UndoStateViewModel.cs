using AnimationEditorCore.Interfaces;

namespace AnimationEditorCore.ViewModels
{
    public enum StateType { Undo, Current, Redo };
    public class UndoStateViewModel : ViewModelBase
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

        private TimelineViewModel _Originator;
        public TimelineViewModel Originator
        {
            get => _Originator;
            set { _Originator = value; NotifyPropertyChanged(); }
        }

        public UndoStateViewModel(TimelineViewModel viewModel, string stateName = "")
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
