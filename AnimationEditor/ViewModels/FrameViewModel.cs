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

        //private int _Order;
        //public int Order
        //{
        //    get => _Order;
        //    set { _Order = value; NotifyPropertyChanged(); }
        //}

        public FrameViewModel()
        {
            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(StrokeCollection strokeCollection)
        {
            StrokeCollection = new StrokeCollection(strokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(Models.FrameModel model)
        {
            StrokeCollection = model.StrokeCollection;
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            //NOTE: This is something of a special case of the patterns used for Undo
            //      Because strokes are applied to the InkCanvas BEFORE we're able to call SaveState
            //      So any changes to the StrokeCollection must be reversed on the StrokeCollection contained
            //      in the state being pushed on the undo stack
            var state = SaveState();
            var stateStrokeCollection = (state as UndoStateViewModel<FrameState>).State.StrokeCollection;

            foreach (var stroke in e.Added)
            {
                //stroke.StylusPointsChanged += Stroke_Invalidated;

                if (stateStrokeCollection.Contains(stroke))
                    stateStrokeCollection.Remove(e.Added);

            }
            foreach (var stroke in e.Removed)
            {
                if (!(stateStrokeCollection.Contains(stroke)))
                    stateStrokeCollection.Add(e.Removed);
            }

            //_PreviousStrokes = new StrokeCollection(StrokeCollection);

            MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);
        }

        //private void Stroke_Invalidated(object sender, EventArgs e)
        //{
        //    var state = SaveState() as UndoStateViewModel<FrameState>;

        //    (state as UndoStateViewModel<FrameState>).State.StrokeCollection = _PreviousStrokes;

        //    MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);
        //}

        public IMemento SaveState()
        {
            var memento = new UndoStateViewModel<FrameState>();

            memento.Originator = this;
            memento.State = new FrameState(this);

            return memento;
        }

        public void LoadState(IMemento memento)
        {
            var Memento = (memento as UndoStateViewModel<FrameState>);

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            StrokeCollection = Memento.State.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }
    }
}
