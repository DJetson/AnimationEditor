using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.ObjectModel;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class FrameViewModel : ViewModelBase, IMementoOriginator
    {
        //private StrokeCollection _PreviousStrokes = new StrokeCollection();
        //public StrokeCollection PreviousStrokes => _PreviousStrokes;

        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { _StrokeCollection = value; NotifyPropertyChanged(); }
        }

        //private FrameState _CurrentState;
        //public IMemento CurrentState
        //{
        //    get => _CurrentState;
        //    set { _CurrentState = value as FrameState; NotifyPropertyChanged(); }
        //}

        //private int _Order;
        //public int Order
        //{
        //    get => _Order;
        //    set { _Order = value; NotifyPropertyChanged(); }
        //}

        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }


        public FrameViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            var state = SaveState() as FrameState;
            state.DisplayName = "Create Blank Frame";
            PushUndoRecord(state);
            //CurrentState = new FrameState(this)
            //{
            //    DisplayName = "New Frame",
            //    Originator = this
            //};
        }

        public FrameViewModel(WorkspaceViewModel workspace, StrokeCollection strokeCollection)
        {
            WorkspaceViewModel = workspace;
            StrokeCollection = new StrokeCollection(strokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            var state = SaveState() as FrameState;
            state.DisplayName = "Duplicate Frame";
            PushUndoRecord(state);

            //CurrentState = new FrameState(this)
            //{
            //    DisplayName = "New Frame from Stroke Collection",
            //    Originator = this
            //};
        }

        public FrameViewModel(Models.FrameModel model)
        {
            StrokeCollection = model.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            //CurrentState = new FrameState(this)
            //{
            //    DisplayName = "Loaded Frame from FrameModel",
            //    Originator = this
            //};
        }

        public void PushUndoRecord(FrameState nextState)
        {
            //var mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
            //if (mainWindowViewModel != null)
            //{
            //if (CurrentState != null)
            //{
            //    WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(CurrentState);
            //}
            ////}
            //CurrentState = nextState;
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState);
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            //NOTE: This is something of a special case of the patterns used for Undo
            //      Because strokes are applied to the InkCanvas BEFORE we're able to call SaveState
            //      So any changes to the StrokeCollection must be reversed on the StrokeCollection contained
            //      in the state being pushed on the undo stack
            //var state = SaveState() as UndoStateViewModel<FrameState>;
            //state.DisplayName = "Frame Content Change";
            //var stateStrokeCollection = state.State.StrokeCollection;

            //foreach (var stroke in e.Added)
            //{
            //    //stroke.StylusPointsChanged += Stroke_Invalidated;

            //    if (stateStrokeCollection.Contains(stroke))
            //        stateStrokeCollection.Remove(e.Added);

            //}
            //foreach (var stroke in e.Removed)
            //{
            //    if (!(stateStrokeCollection.Contains(stroke)))
            //        stateStrokeCollection.Add(e.Removed);
            //}

            //_PreviousStrokes = new StrokeCollection(StrokeCollection);
            var state = SaveState() as FrameState;
            state.DisplayName = "Frame Content Change";
            PushUndoRecord(state);

            //MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);
        }

        //private void Stroke_Invalidated(object sender, EventArgs e)
        //{
        //    var state = SaveState() as UndoStateViewModel<FrameState>;

        //    (state as UndoStateViewModel<FrameState>).State.StrokeCollection = _PreviousStrokes;

        //    MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);
        //}

        //public void ValidateUndoSelectionContext()
        //{
        //    //This 
        //    if (MainWindowViewModel.WorkspaceManager.ActiveUndoStack.Peek().Originator is AnimationTimelineViewModel)

        //}

        public IMemento SaveState()
        {

            var memento = new FrameState(this);

            memento.Originator = this;

            return memento;
        }

        public void LoadState(IMemento memento)
        {
            var Memento = (memento as FrameState);

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            StrokeCollection = Memento.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            //CurrentState = Memento;
        }
    }
}
