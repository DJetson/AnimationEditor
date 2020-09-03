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

            //var state = SaveState() as FrameState;
            //state.DisplayName = "Create Blank Frame";
            //PushUndoRecord(state);
        }

        public FrameViewModel(WorkspaceViewModel workspace, StrokeCollection strokeCollection)
        {
            WorkspaceViewModel = workspace;
            StrokeCollection = new StrokeCollection(strokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            //var state = SaveState() as FrameState;
            //state.DisplayName = "Duplicate Frame";
            //PushUndoRecord(state);
        }

        public FrameViewModel(Models.FrameModel model)
        {
            StrokeCollection = model.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public void PushUndoRecord(FrameState nextState)
        {
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState);
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            var state = SaveState() as FrameState;
            state.DisplayName = "Frame Content Change";
            PushUndoRecord(state);
        }

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
