using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditorCore.ViewModels
{
    public class KeyFrameViewModel : FrameViewModel, IFrameViewModel
    {
        //private StrokeCollection _StrokeCollection = new StrokeCollection();
        //public StrokeCollection StrokeCollection
        //{
        //    get => _StrokeCollection;
        //    set { _StrokeCollection = value; NotifyPropertyChanged(); }
        //}

        private StrokeCollection _SelectedStrokes = new StrokeCollection();
        public StrokeCollection SelectedStrokes
        {
            get { return _SelectedStrokes; }
            set { _SelectedStrokes = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _UpdateSelectedStrokes;
        public DelegateCommand UpdateSelectedStrokes
        {
            get { return _UpdateSelectedStrokes; }
            set { _UpdateSelectedStrokes = value; NotifyPropertyChanged(); }
        }

        private StrokeCollection _DeferredSelectionStrokes;
        private int _StrokeMultiSelectOpCounter = 0;
        private bool _IsErasing = false;

        public KeyFrameViewModel(Models.FrameModel model, LayerViewModel layer) : base(model, layer)
        {
            StrokeCollection = model.StrokeCollection;

            foreach (var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }

            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            InitializeCommands();
        }

        public KeyFrameViewModel(LayerViewModel layer, int orderId) : base(layer, orderId)
        {
            InitializeCommands();

            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
        }

        public static KeyFrameViewModel FromBase(IFrameViewModel frame)
        {
            KeyFrameViewModel keyFrame = frame as KeyFrameViewModel 
                                         ?? new KeyFrameViewModel(frame as FrameViewModel);
            return keyFrame;
        }

        public KeyFrameViewModel(FrameViewModel originalFrame) : base(originalFrame)
        {
            InitializeCommands();

            StrokeCollection = new StrokeCollection();

            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            SelectedStrokes.StrokesChanged += SelectedStrokes_StrokesChanged;
        }

        public KeyFrameViewModel(KeyFrameViewModel originalFrame) : base((FrameViewModel)originalFrame)
        {
            InitializeCommands();

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

        public override void InitializeCommands()
        {
            UpdateSelectedStrokes = new DelegateCommand(UpdateSelectedStrokes_CanExecute, UpdateSelectedStrokes_Execute);
            LoadedInkCanvas = new DelegateCommand(LoadedInkCanvas_CanExecute, LoadedInkCanvas_Execute);
        }

        public override IFrameViewModel Clone()
        {
            var newFrame = new KeyFrameViewModel(this);

            return newFrame;
        }

        public static void CopyToFrame(KeyFrameViewModel original, KeyFrameViewModel destination)
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

        public override IFrameViewModel Duplicate(int newOrder = -1, string newDisplayName = "")
        {
            return DuplicateFrame(this, newOrder, newDisplayName);
        }

        public static KeyFrameViewModel DuplicateFrame(KeyFrameViewModel original, int newOrder = -1, string newDisplayName = "")
        {
            var newFrame = new KeyFrameViewModel(original);

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

        public void ClearStrokes()
        {
            foreach (var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            StrokeCollection.Clear();
        }

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

        public void RemoveStrokes(StrokeCollection strokes, bool createUndo = true)
        {
            var matchingStrokes = new StrokeCollection(strokes.Where(e => StrokeCollection.Contains(e)));

            System.Diagnostics.Debug.WriteLineIf((matchingStrokes != strokes), $"Attempting to remove strokes that do not exist in the target collection");

            if (createUndo == false)
            {
                StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            }

            StrokeCollection.Remove(matchingStrokes);

            foreach (var removedStroke in matchingStrokes)
            {
                removedStroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }

            if (createUndo == false)
            {
                StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            }
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Brush)
            {
                WorkspaceHistoryViewModel.PushUndoRecord($"Added Content to Layer {LayerViewModel.ZIndex} on Frame {Order}");
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Lasso && _IsErasing == false)
            {
                if (e.Removed.Count > 0)
                    WorkspaceHistoryViewModel.PushUndoRecord($"Deleted Content from Layer {LayerViewModel.ZIndex} on Frame {Order}");
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
                WorkspaceHistoryViewModel.PushUndoRecord($"Modified Content in Layer {LayerViewModel.ZIndex} on Frame {Order}");
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

                WorkspaceHistoryViewModel.PushUndoRecord($"Erased Content from Layer {LayerViewModel.ZIndex} on Frame {Order}");
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
            else if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Lasso)
            {
                _IsErasing = false;

                WorkspaceHistoryViewModel.PushUndoRecord($"Moved Content From Layer {LayerViewModel.ZIndex} on Frame {Order}");
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }

        protected override bool LoadedInkCanvas_CanExecute(object parameter)
        {
            if (!base.LoadedInkCanvas_CanExecute(parameter))
                return false;

            return true;
        }

        protected override void LoadedInkCanvas_Execute(object parameter)
        {
            base.LoadedInkCanvas_Execute(parameter);

            if (SelectedStrokes.Count > 0)
            {
                InkCanvas.Select(SelectedStrokes);
            }

            if (_DeferredSelectionStrokes != null)
            {
                InkCanvas.Select(_DeferredSelectionStrokes);
                _DeferredSelectionStrokes = null;
            }
        }
    }
}
