using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

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
        }

        public FrameViewModel(WorkspaceViewModel workspace, StrokeCollection strokeCollection)
        {
            WorkspaceViewModel = workspace;
            StrokeCollection = new StrokeCollection(strokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(Models.FrameModel model, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            StrokeCollection = model.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        private bool _IsErasing = false;

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (EditorToolsViewModel.Instance.SelectedToolType != BaseClasses.EditorToolType.Eraser)
            {
                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, "Added Frame Content", state);
                PushUndoRecord(multiState);
            }
            else if (_IsErasing == false)
            {
                _IsErasing = true;
                Mouse.AddMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }

        private void EraserOperation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _IsErasing = false;

                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, "Erased Frame Content", state);
                PushUndoRecord(multiState);
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
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
            StrokeCollection = new StrokeCollection(Memento.StrokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            //CurrentState = Memento;
        }
    }
}
