using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
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
    public class FrameViewModel : ViewModelBase/*, IMementoOriginator*/
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

            SelectedStrokes.StrokesChanged -= SelectedStrokes_StrokesChanged;
            SelectedStrokes.Clear();
            SelectedStrokes.Add(Parameter.GetSelectedStrokes());
            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;

            Console.WriteLine($"Selected {SelectedStrokes.Count} Strokes on Frame {Order}");
        }

        private DelegateCommand _LoadedInkCanvas;
        public DelegateCommand LoadedInkCanvas
        {
            get { return _LoadedInkCanvas; }
            set { _LoadedInkCanvas = value; NotifyPropertyChanged(); }
        }

        private bool LoadedInkCanvas_CanExecute(object parameter)
        {
            if (!(parameter is InkCanvas Parameter))
                return false;

            return true;
        }

        private void LoadedInkCanvas_Execute(object parameter)
        {
            var Parameter = parameter as InkCanvas;

            InkCanvas = Parameter;

            if(SelectedStrokes.Count > 0)
            {
                InkCanvas.Select(SelectedStrokes);
            }

            if(_DeferredSelectionStrokes != null)
            {
                InkCanvas.Select(_DeferredSelectionStrokes);
                _DeferredSelectionStrokes = null;
            }
        }

        private InkCanvas _InkCanvas;
        public InkCanvas InkCanvas
        {
            get { return _InkCanvas; }
            set { _InkCanvas = value; NotifyPropertyChanged(); }
        }

        public FrameViewModel(Models.FrameModel model, LayerViewModel layer)
        {
            LayerViewModel = layer;

            StrokeCollection = model.StrokeCollection;

            foreach (var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }

            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            Order = model.Order;
            InitializeCommands();

        }

        private StrokeCollection _DeferredSelectionStrokes;

        private void SelectedStrokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (e.Added.Count > 0)
            {
                if (InkCanvas != null)
                    InkCanvas.Select(e.Added);
                else
                {
                    _DeferredSelectionStrokes = _DeferredSelectionStrokes ?? new StrokeCollection();
                    _DeferredSelectionStrokes.Add(e.Added);
                }
            }
            if(e.Removed.Count > 0)
            {

            }

        }

        public void RemoveStrokes(StrokeCollection strokes, bool createUndo = true)
        {
            if (createUndo == false)
            {
                StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            }

            StrokeCollection.Remove(strokes);

            if (createUndo == false)
            {
                StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            }
        }

        public void InitializeCommands()
        {
            UpdateSelectedStrokes = new DelegateCommand(UpdateSelectedStrokes_CanExecute, UpdateSelectedStrokes_Execute);
            LoadedInkCanvas = new DelegateCommand(LoadedInkCanvas_CanExecute, LoadedInkCanvas_Execute);
        }

        //public IMemento SaveState()
        //{
        //    var memento = new FrameState(this);

        //    memento.Originator = this;

        //    return memento;
        //}

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
            destination.SelectedStrokes.StrokesChanged += destination.SelectedStrokes_StrokesChanged;
        }

        //public void LoadState(IMemento memento)
        //{
        //    var Memento = (memento as FrameState);

        //    CopyToFrame(Memento.Frame, this);
        //}

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
            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
        }

        public FrameViewModel(FrameViewModel originalFrame)
        {
            InitializeCommands();
            LayerViewModel = originalFrame.LayerViewModel;
            Order = originalFrame.Order;
            DisplayName = originalFrame.DisplayName;
            StrokeCollection = new StrokeCollection();
            foreach (var stroke in originalFrame.StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += Stroke_StylusPointsChanged;
                StrokeCollection.Add(newStroke);
            }
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
        }

        public static FrameViewModel DuplicateFrame(FrameViewModel original, int newOrder = -1, string newDisplayName = "")
        {
            var newFrame = new FrameViewModel(original);

            if (newOrder != -1)
            {
                newFrame.Order = newOrder;
            }

            if (!(String.IsNullOrWhiteSpace(newDisplayName)))
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
                LayerViewModel.TimelineViewModel.PushUndoRecord(LayerViewModel.TimelineViewModel.CreateUndoState($"Added Content to Layer {LayerViewModel.ZIndex} on Frame {Order}"));
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Lasso && _IsErasing == false)
            {
                if (e.Removed.Count > 0)
                    LayerViewModel.TimelineViewModel.PushUndoRecord(LayerViewModel.TimelineViewModel.CreateUndoState($"Deleted Content from Layer {LayerViewModel.ZIndex} on Frame {Order}"));
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
        }

        private void Stroke_StylusPointsChanged(object sender, EventArgs e)
        {
            if (_StrokeMultiSelectOpCounter == 0)
                _StrokeMultiSelectOpCounter = SelectedStrokes.Count;

            if (_StrokeMultiSelectOpCounter == 1)
            {
                LayerViewModel.TimelineViewModel.PushUndoRecord(LayerViewModel.TimelineViewModel.CreateUndoState($"Modified Content in Layer {LayerViewModel.ZIndex} on Frame {Order}"));
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

                LayerViewModel.TimelineViewModel.PushUndoRecord(LayerViewModel.TimelineViewModel.CreateUndoState($"Erased Content from Layer {LayerViewModel.ZIndex} on Frame {Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
            else if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Lasso)
            {
                _IsErasing = false;

                LayerViewModel.TimelineViewModel.PushUndoRecord(LayerViewModel.TimelineViewModel.CreateUndoState($"Moved Content From Layer {LayerViewModel.ZIndex} on Frame {Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }
    }
}
