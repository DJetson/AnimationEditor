using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditor.ViewModels
{
    public class LayerViewModel : ViewModelBase, IMementoOriginator
    {
        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { Console.WriteLine($"Setting StrokeCollection [OldCount={_StrokeCollection.Count} New={value.Count}]"); _StrokeCollection = value; NotifyPropertyChanged(); }
        }

        private int _LayerId;
        public int LayerId
        {
            get => _LayerId;
            set { Console.WriteLine($"Setting LayerId [Old={_LayerId} New={value}]"); _LayerId = value; NotifyPropertyChanged(); }
        }

        private FrameViewModel _FrameViewModel;
        public FrameViewModel FrameViewModel
        {
            get { return _FrameViewModel; }
            set { _FrameViewModel = value; NotifyPropertyChanged(); }
        }

        public LayerViewModel(LayerViewModel layer)
        {
            FrameViewModel = layer.FrameViewModel;

            StrokeCollection = new StrokeCollection();
            //StrokeCollection = layer.StrokeCollection.Clone();

            foreach (var stroke in layer.StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += Stroke_StylusPointsChanged;
                StrokeCollection.Add(newStroke);
            }
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public LayerViewModel(FrameViewModel frame)
        {
            FrameViewModel = frame;
            LayerId = FrameViewModel.Layers.Count();
            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public LayerViewModel(FrameViewModel frame, StrokeCollection strokeCollection)
        {
            FrameViewModel = frame;
            StrokeCollection = strokeCollection.Clone();
            foreach (var stroke in StrokeCollection)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }

            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public LayerViewModel(Models.LayerModel model, FrameViewModel frame)
        {
            FrameViewModel = frame;
            StrokeCollection = model.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public MultiState CreateUndoState(string title)
        {
            var parentState = FrameViewModel.SaveState() as FrameState;
            var state = SaveState() as LayerState;
            var multiState = new MultiState(null, title, state, parentState);

            return multiState;
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            FrameViewModel.WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        private bool _IsErasing = false;

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Brush)
            {
                PushUndoRecord(CreateUndoState($"Added Content to Layer {LayerId} on Frame {FrameViewModel.Order}"));
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Lasso && _IsErasing == false)
            {
                //if (e.Added.Count > 0)
                //    PushUndoRecord(CreateUndoState($"Pasted Content into Layer {LayerId} on Frame {FrameViewModel.Order}"));
                //else 
                if (e.Removed.Count > 0)
                    PushUndoRecord(CreateUndoState($"Deleted Content from Layer {LayerId} on Frame {FrameViewModel.Order}"));
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
            PushUndoRecord(CreateUndoState($"Modified Content in Layer {LayerId} on Frame {FrameViewModel.Order}"));
        }

        private void EraserOperation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Eraser)
            {
                _IsErasing = false;

                PushUndoRecord(CreateUndoState($"Erased Content from Layer {LayerId} on Frame {FrameViewModel.Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
            else if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Lasso)
            {
                _IsErasing = false;

                PushUndoRecord(CreateUndoState($"Moved Content From Layer {LayerId} on Frame {FrameViewModel.Order}"));
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }

        public IMemento SaveState()
        {
            var memento = new LayerState(this);

            memento.Originator = this;

            return memento;
        }

        public void LoadState(IMemento memento)
        {
            var Memento = (memento as LayerState);

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            StrokeCollection = new StrokeCollection(Memento.StrokeCollection);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public LayerViewModel Clone()
        {
            var newLayer = new LayerViewModel(FrameViewModel, StrokeCollection);
            newLayer.LayerId = LayerId;

            return newLayer;
        }
    }
}
