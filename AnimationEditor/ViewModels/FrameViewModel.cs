using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditor.ViewModels
{
    public class FrameViewModel : ViewModelBase, IMementoOriginator
    {
        private DelegateCommand _InsertLayerAbove;
        public DelegateCommand InsertLayerAbove
        {
            get { return _InsertLayerAbove; }
            set { _InsertLayerAbove = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _InsertLayerBelow;
        public DelegateCommand InsertLayerBelow
        {
            get { return _InsertLayerBelow; }
            set { _InsertLayerBelow = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _RemoveLayer;
        public DelegateCommand RemoveLayer
        {
            get { return _RemoveLayer; }
            set { _RemoveLayer = value; NotifyPropertyChanged(); }
        }

        private LayerViewModel _ActiveLayer;
        public LayerViewModel ActiveLayer
        {
            get { return _ActiveLayer; }
            set
            {
                if (_ActiveLayer != null)
                {
                    _ActiveLayer.IsActive = false;
                }
                _ActiveLayer = value;
                if (_ActiveLayer != null)
                {
                    _ActiveLayer.IsActive = true;
                }
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<LayerViewModel> _Layers;
        public ObservableCollection<LayerViewModel> Layers
        {
            get { return _Layers; }
            set { _Layers = value; NotifyPropertyChanged(); }
        }

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

        public void AddNewLayer(bool createUndoState = true)
        {
            if (Layers == null)
                Layers = new ObservableCollection<LayerViewModel>();

            var newLayer = new LayerViewModel(this);
            Layers.Add(newLayer);

            PopulateLayerIds();
            //ActiveLayerIndex = Layers.IndexOf(newLayer);
            ActiveLayer = newLayer;

            if (createUndoState)
                PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        }

        public void InitializeCommands()
        {
            InsertLayerAbove = new DelegateCommand(InsertLayerAbove_CanExecute, InsertLayerAbove_Execute);
            InsertLayerBelow = new DelegateCommand(InsertLayerBelow_CanExecute, InsertLayerBelow_Execute);
            RemoveLayer = new DelegateCommand(RemoveLayer_CanExecute, RemoveLayer_Execute);
        }

        private bool InsertLayerBelow_CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
            {
                return false;
            }

            return true;
        }

        private void InsertLayerBelow_Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;

            //var newLayer = new LayerViewModel(this);
            //newLayer.LayerId = Layers.Count;
            AddNewLayerAtIndex(Layers.IndexOf(Parameter));
            //Layers.Insert(Layers.IndexOf(Parameter), newLayer);

            //PopulateLayerIds(0);

            //var layerState = newLayer.SaveState() as LayerState;
            //var frameState = SaveState() as FrameState;

            //var multiState = new MultiState(null, $"Added Layer {newLayer.LayerId} to Frame {Order}", layerState, frameState);

            //PushUndoRecord(multiState);
        }

        private bool InsertLayerAbove_CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
            {
                return false;
            }

            return true;
        }

        private void InsertLayerAbove_Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;

            AddNewLayerAtIndex(Layers.IndexOf(Parameter) + 1);

            //var newLayer = new LayerViewModel(this);
            //newLayer.LayerId = Layers.Count;

            //if (Layers.IndexOf(Parameter) < Layers.Count - 1)
            //    Layers.Insert(Layers.IndexOf(Parameter) + 1, newLayer);
            //else
            //    Layers.Add(newLayer);

            //PopulateLayerIds();

            //var layerState = newLayer.SaveState() as LayerState;
            //var frameState = SaveState() as FrameState;

            //var multiState = new MultiState(null, $"Added Layer {newLayer.LayerId} to Frame {Order}", layerState, frameState);

            //PushUndoRecord(multiState);
        }

        private bool RemoveLayer_CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
            {
                return false;
            }

            return true;
        }

        public void PopulateLayerIds(int startIndex = 0)
        {
            foreach (var layer in Layers.Where(e => e.LayerId >= startIndex))
            {
                layer.LayerId = Layers.IndexOf(layer);
            }
        }

        private void RemoveLayer_Execute(object parameter)
        {
            var toRemove = parameter as LayerViewModel;

            var toRemoveIndex = Layers.IndexOf(toRemove);
            Layers.Remove(toRemove);

            LayerState newLayerState = null;

            if (Layers.Count == 0)
            {
                var newLayer = new LayerViewModel(this);
                AddNewLayerAtIndex(Layers.Count, $"Layer {Layers.Count}", false);
                newLayerState = newLayer.SaveState() as LayerState;
            }

            PopulateLayerIds();

            var frameState = SaveState() as FrameState;

            if (newLayerState != null)
            {
                var multiState = new MultiState(null, $"Removed Layer {toRemove.LayerId} from Frame {Order}", newLayerState, frameState);
                PushUndoRecord(multiState);
            }
            else
            {
                var multiState = new MultiState(null, $"Removed Layer {toRemove.LayerId} from Frame {Order}", frameState);
                PushUndoRecord(multiState);
            }
        }

        public void AddNewLayerAtIndex(int index, string layerName = "", bool createUndoState = true)
        {
            if (Layers == null)
                Layers = new ObservableCollection<LayerViewModel>();

            if (String.IsNullOrWhiteSpace(layerName))
            {
                layerName = $"Layer {Layers.Count}";
            }

            var newLayer = new LayerViewModel(this) { LayerId = Layers.Count, DisplayName = layerName, IsVisible = true };

            if (index < 0)
            {
                Layers.Insert(0, newLayer);
            }
            else if (index < Layers.Count)
            {
                Layers.Insert(index, newLayer);
            }
            else
            {
                Layers.Add(newLayer);
            }

            ActiveLayer = newLayer;
            PopulateLayerIds();

            if (createUndoState)
                PushUndoRecord(CreateUndoState($"Added New {newLayer.DisplayName} to Frame {Order}"));
        }

        public void AddLayer(LayerViewModel layer, bool createUndoState = true)
        {
            if (Layers == null)
                Layers = new ObservableCollection<LayerViewModel>();

            layer.LayerId = Layers.Count;
            Layers.Add(layer);
            ActiveLayer = layer;

            PopulateLayerIds();

            if (createUndoState)
                PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        }

        public FrameViewModel(WorkspaceViewModel workspace, bool createFirstLayer = true)
        {
            InitializeCommands();
            WorkspaceViewModel = workspace;
            Layers = new ObservableCollection<LayerViewModel>();

            if (createFirstLayer)
                Layers.Add(new LayerViewModel(this));

            PopulateLayerIds();

            StrokeCollection = new StrokeCollection();
            FlattenStrokesForPlayback();
        }

        public List<UndoStateViewModel> SaveLayerStates()
        {
            var layerStates = new List<UndoStateViewModel>();

            foreach (var layer in Layers)
            {
                layerStates.Add(layer.SaveState() as LayerState);
            }

            return layerStates;
        }

        public FrameViewModel(FrameViewModel frame)
        {
            InitializeCommands();
            WorkspaceViewModel = frame.WorkspaceViewModel;
            Layers = new ObservableCollection<LayerViewModel>(frame.Layers.ToList().Select(e => new LayerViewModel(this, e.StrokeCollection)));
            ActiveLayer = Layers[frame.Layers.IndexOf(frame.ActiveLayer)];
            FlattenStrokesForPlayback();
        }

        public void FlattenStrokesForPlayback()
        {
            StrokeCollection.Clear();

            foreach (var layer in Layers)
            {
                if (layer.IsVisible)
                    StrokeCollection.Add(layer.StrokeCollection);
            }
        }

        public FrameViewModel(Models.FrameModel model, WorkspaceViewModel workspace)
        {
            InitializeCommands();
            WorkspaceViewModel = workspace;
            //StrokeCollection = model.StrokeCollection;
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            Layers = new ObservableCollection<LayerViewModel>(model.Layers.Select(e => new LayerViewModel(e, this)));
            Order = model.Order;
            FlattenStrokesForPlayback();
        }

        public MultiState CreateUndoState(string title)
        {
            var state = SaveState() as FrameState;
            var multiState = new MultiState(null, title, state);
            return multiState;
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        private bool _IsErasing = false;

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            foreach (var stroke in e.Added)
            {
                stroke.StylusPointsChanged += Stroke_StylusPointsChanged;
            }
            foreach (var stroke in e.Removed)
            {
                stroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }

            if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Brush)
            {
                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, $"Added Content to Frame {Order}", state);
                PushUndoRecord(multiState);
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Lasso && _IsErasing == false)
            {
                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, $"Deleted Content from Frame {Order}", state);
                PushUndoRecord(multiState);
            }
            else if (EditorToolsViewModel.Instance.SelectedToolType == EditorToolType.Eraser && _IsErasing == false)
            {
                _IsErasing = true;
                Mouse.AddMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
        }

        private void Stroke_StylusPointsChanged(object sender, EventArgs e)
        {
            var state = SaveState() as FrameState;
            var multiState = new MultiState(null, $"Modified Frame {Order} Content", state);
            PushUndoRecord(multiState);
        }

        private void EraserOperation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Eraser)
            {
                _IsErasing = false;

                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, $"Erased Content from Frame {Order}", state);
                PushUndoRecord(multiState);
                Mouse.RemoveMouseUpHandler(Mouse.PrimaryDevice.ActiveSource.RootVisual as DependencyObject, EraserOperation_MouseUp);
            }
            else if (e.LeftButton == MouseButtonState.Released && EditorToolsViewModel.Instance.SelectedToolType == BaseClasses.EditorToolType.Lasso)
            {
                _IsErasing = false;

                var state = SaveState() as FrameState;
                var multiState = new MultiState(null, $"Moved Frame {Order} Content", state);
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

            Layers = new ObservableCollection<LayerViewModel>();

            foreach (var layer in Memento.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.FrameViewModel = this;
                Layers.Add(clonedLayer);
            }
            FlattenStrokesForPlayback();
            ActiveLayer = Layers[Memento.ActiveLayerIndex];
        }

        public FrameViewModel Clone()
        {
            var newFrame = new FrameViewModel(WorkspaceViewModel, false);
            newFrame.Layers = new ObservableCollection<LayerViewModel>();

            foreach (var layer in Layers)
            {
                var newLayer = layer.Clone();
                newLayer.FrameViewModel = newFrame;
                //newLayer.LayerId = newFrame.Layers.Count;
                newFrame.AddLayer(newLayer, false);
            }

            if (ActiveLayer == null)
            {
                newFrame.ActiveLayer = newFrame.Layers[0];
            }
            else
            {
                newFrame.ActiveLayer = newFrame.Layers[Layers.IndexOf(ActiveLayer)];
            }
            newFrame.Order = Order;
            newFrame.FlattenStrokesForPlayback();
            return newFrame;
        }
    }
}
