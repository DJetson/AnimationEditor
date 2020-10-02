using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditorCore.ViewModels
{
    public class FrameViewModel : ViewModelBase, IMementoOriginator
    {
        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { _StrokeCollection = value; NotifyPropertyChanged(); }
        }

        private int _Order;
        public int Order
        {
            get => _Order;
            set { _Order = value; NotifyPropertyChanged(); }
        }

        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        private bool _IsCurrent;
        public bool IsCurrent
        {
            get { return _IsCurrent; }
            set { _IsCurrent = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _UpdateSelectedStrokes;
        public DelegateCommand UpdateSelectedStrokes
        {
            get { return _UpdateSelectedStrokes; }
            set { _UpdateSelectedStrokes = value; NotifyPropertyChanged(); }
        }

        private bool UpdateSelectedStrokes_CanExecute(object parameter)
        {
            if (!(parameter is InkCanvas Parameter))
                return false;

            return true;
        }

        private void UpdateSelectedStrokes_Execute(object parameter)
        {
            var Parameter = parameter as InkCanvas;

            SelectedStrokes = Parameter.GetSelectedStrokes();
            Console.WriteLine($"Selected {SelectedStrokes.Count} Strokes on Frame {Order}");
        }

        public FrameViewModel(Models.FrameModel model, LayerViewModel layer)
        {
            LayerViewModel = layer;
            
            StrokeCollection = model.StrokeCollection;

            foreach(var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }

            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            Order = model.Order;
            InitializeCommands();

        }

        public void InitializeCommands()
        {
            UpdateSelectedStrokes = new DelegateCommand(UpdateSelectedStrokes_CanExecute, UpdateSelectedStrokes_Execute);
        }

        public TimelineState CreateUndoState(string title, List<UndoStateViewModel> additionalStates = null)
        {
            var state = LayerViewModel.TimelineViewModel.CreateUndoState(title);
     
            return state;
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            LayerViewModel.TimelineViewModel.WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        public IMemento SaveState()
        {
            var memento = new FrameState(this);

            memento.Originator = this;

            return memento;
        }

        public void ClearStrokes()
        {
            foreach (var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            StrokeCollection.Clear();
        }

        public static void CopyToFrame(FrameViewModel original, FrameViewModel destination)
        {
            destination.ClearStrokes();

            destination.LayerViewModel = original.LayerViewModel;
            destination.Order = original.Order;
            destination.DisplayName = original.DisplayName;

            foreach (var stroke in original.StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += destination.Stroke_StylusPointsChanged;
                destination.StrokeCollection.Add(newStroke);
            }

            destination.StrokeCollection.StrokesChanged += destination.StrokeCollection_StrokesChanged;
        }

        public void LoadState(IMemento memento)
        {
            var Memento = (memento as FrameState);

            CopyToFrame(Memento.Frame, this);
        }

        public FrameViewModel Clone()
        {
            var newFrame = new FrameViewModel(this);

            return newFrame;
        }

        private LayerViewModel _LayerViewModel;
        public LayerViewModel LayerViewModel
        {
            get { return _LayerViewModel; }
            set { _LayerViewModel = value; NotifyPropertyChanged(); }
        }
        
        int _StrokeMultiSelectOpCounter = 0;
        
        private StrokeCollection _SelectedStrokes = new StrokeCollection();
        public StrokeCollection SelectedStrokes
        {
            get { return _SelectedStrokes; }
            set { _SelectedStrokes = value; NotifyPropertyChanged(); }
        }

        public FrameViewModel(LayerViewModel layer, int orderId)
        {
            InitializeCommands();
            LayerViewModel = layer;
            Order = orderId;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(FrameViewModel originalFrame)
        {
            InitializeCommands();
            LayerViewModel = originalFrame.LayerViewModel;
            Order = originalFrame.Order;
            DisplayName = originalFrame.DisplayName;
            StrokeCollection = new StrokeCollection();
            foreach(var stroke in originalFrame.StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += Stroke_StylusPointsChanged;
                StrokeCollection.Add(newStroke);
            }
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public static FrameViewModel DuplicateFrame(FrameViewModel original, int newOrder = -1, string newDisplayName = "")
        {
            var newFrame = new FrameViewModel(original);

            if(newOrder != -1)
            {
                newFrame.Order = newOrder;
            }

            if(!(String.IsNullOrWhiteSpace(newDisplayName)))
            {
                newFrame.DisplayName = newDisplayName;
            }

            return newFrame;
        }
       
        private bool _IsErasing = false;

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Brush)
            {
                PushUndoRecord(CreateUndoState($"Added Content to Layer {LayerViewModel.LayerId} on Frame {Order}"));
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Lasso && _IsErasing == false)
            {
                if (e.Removed.Count > 0)
                    PushUndoRecord(CreateUndoState($"Deleted Content from Layer {LayerViewModel.LayerId} on Frame {Order}"));
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Eraser && _IsErasing == false)
            {
                _IsErasing = true;
                Mouse.AddMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }

            foreach (var stroke in e.Added)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }
            foreach (var stroke in e.Removed)
            {
                stroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }
            LayerViewModel.TimelineViewModel.FlattenFrames(); 
        }

        private void Stroke_StylusPointsChanged(object sender, EventArgs e)
        {
            if (_StrokeMultiSelectOpCounter == 0)
                _StrokeMultiSelectOpCounter = SelectedStrokes.Count;

            if (_StrokeMultiSelectOpCounter == 1)
            {
                PushUndoRecord(CreateUndoState($"Modified Content in Layer {LayerViewModel.LayerId} on Frame {Order}"));
                _StrokeMultiSelectOpCounter = 0;
            }
            else
            {
                _StrokeMultiSelectOpCounter--;
            }
        }

        private void EraserOperation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Eraser)
            {
                _IsErasing = false;

                PushUndoRecord(CreateUndoState($"Erased Content from Layer {LayerViewModel.LayerId} on Frame {Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
            else if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Lasso)
            {
                _IsErasing = false;

                PushUndoRecord(CreateUndoState($"Moved Content From Layer {LayerViewModel.LayerId} on Frame {Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }
    }
}
