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
        //private StrokeCollection _PreviousStrokes = new StrokeCollection();
        //public StrokeCollection PreviousStrokes => _PreviousStrokes;

        //private LayerViewModel _ActiveLayer;
        //public LayerViewModel ActiveLayer
        //{
        //    get { return _ActiveLayer; }
        //    set { _ActiveLayer = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(ActiveLayerIndex)); }
        //}

        private DelegateCommand _InsertLayer;
        public DelegateCommand InsertLayer
        {
            get { return _InsertLayer; }
            set { _InsertLayer = value; NotifyPropertyChanged(); }
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


        //public LayerViewModel ActiveLayer
        //{
        //    get => Layers[ActiveLayerIndex];
        //    //set { _ActiveLayerIndex = Layers.IndexOf(ActiveLayer); }
        //}
        //public int ActiveLayerIndex => Layers?.IndexOf(ActiveLayer) ?? -1;


        //private int _ActiveLayerIndex = 0;
        //public int ActiveLayerIndex
        //{
        //    get { return _ActiveLayerIndex; }
        //    set { _ActiveLayerIndex = value; NotifyPropertyChanged(); ActiveLayer.IsActive = false; NotifyPropertyChanged(nameof(ActiveLayer)); ActiveLayer.IsActive = true; }
        //}


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

        //private FrameState _CurrentState;
        //public IMemento CurrentState
        //{
        //    get => _CurrentState;
        //    set { _CurrentState = value as FrameState; NotifyPropertyChanged(); }
        //}

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
            //ActiveLayerIndex = Layers.IndexOf(newLayer);
            ActiveLayer = newLayer;

            if (createUndoState)
                PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        }

        public void InitializeCommands()
        {
            InsertLayer = new DelegateCommand(InsertLayer_CanExecute, InsertLayer_Execute);
            RemoveLayer = new DelegateCommand(RemoveLayer_CanExecute, RemoveLayer_Execute);
        }

        private bool InsertLayer_CanExecute(object parameter)
        {
            return true;
        }

        private void InsertLayer_Execute(object parameter)
        {
            var newLayer = new LayerViewModel(this);
            newLayer.LayerId = Layers.Count;
            Layers.Insert(/*ActiveLayerIndex*/Layers.IndexOf(ActiveLayer), newLayer);
            var layerState = newLayer.SaveState() as LayerState;
            var frameState = SaveState() as FrameState;

            var multiState = new MultiState(null, $"Added Layer {newLayer.LayerId} to Frame {Order}", layerState, frameState);

            PushUndoRecord(multiState);
        }

        private bool RemoveLayer_CanExecute(object parameter)
        {
            //if (ActiveLayerIndex < 0 || ActiveLayerIndex > Layers.Count)
            if (!Layers.Contains(ActiveLayer))
                return false;

            return true;
        }

        private void RemoveLayer_Execute(object parameter)
        {
            var toRemove = ActiveLayer;//Layers[ActiveLayerIndex];
            var toRemoveIndex = Layers.IndexOf(toRemove);
            Layers.Remove(toRemove);

            foreach (var layer in Layers.Where(e => e.LayerId > toRemoveIndex))
            {
                layer.LayerId = Layers.IndexOf(layer);
            }

            if (Layers.Count == 0)
            {
                AddLayer(new LayerViewModel(this), false);
            }

            //var layerState = toRemove.SaveState() as LayerState;
            var frameState = SaveState() as FrameState;

            var multiState = new MultiState(null, $"Removed Layer {toRemove.LayerId} from Frame {Order}", frameState);

            PushUndoRecord(multiState);
        }

        public void AddLayer(LayerViewModel layer, bool createUndoState = true)
        {
            if (Layers == null)
                Layers = new ObservableCollection<LayerViewModel>();

            //var newLayer = new LayerViewModel(this);
            layer.LayerId = Layers.Count;
            Layers.Add(layer);
            ActiveLayer = layer;

            if (createUndoState)
                PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        }

        public FrameViewModel(WorkspaceViewModel workspace, bool createFirstLayer = true)
        {
            InitializeCommands();
            WorkspaceViewModel = workspace;
            //AddLayer();
            Layers = new ObservableCollection<LayerViewModel>();

            if (createFirstLayer)
                Layers.Add(new LayerViewModel(this));

            StrokeCollection = new StrokeCollection();
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
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
            //StrokeCollection = frame.StrokeCollection.Clone();
            Layers = new ObservableCollection<LayerViewModel>(frame.Layers.ToList().Select(e => new LayerViewModel(this, e.StrokeCollection)));
            //Layers.ToList().ForEach(e => e.LayerId = Layers.IndexOf(e));
            ActiveLayer = Layers[frame.Layers.IndexOf(frame.ActiveLayer)];
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        //public FrameViewModel(WorkspaceViewModel workspace, StrokeCollection strokeCollection)
        //{
        //    WorkspaceViewModel = workspace;
        //    StrokeCollection = strokeCollection.Clone();
        //    StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        //}

        public void FlattenStrokesForPlayback()
        {
            StrokeCollection.Clear();

            foreach (var layer in Layers)
            {
                StrokeCollection.Add(layer.StrokeCollection);
            }
        }

        public FrameViewModel(Models.FrameModel model, WorkspaceViewModel workspace)
        {
            InitializeCommands();
            WorkspaceViewModel = workspace;
            StrokeCollection = model.StrokeCollection;
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
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
                Layers.Add(new LayerViewModel(layer));
            }

            //StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            //StrokeCollection = new StrokeCollection(Memento.StrokeCollection);
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            //CurrentState = Memento;
        }

        public FrameViewModel Clone()
        {
            var newFrame = new FrameViewModel(WorkspaceViewModel, false);
            newFrame.Layers = new ObservableCollection<LayerViewModel>();

            foreach (var layer in Layers)
            {
                var newLayer = layer.Clone();
                newLayer.LayerId = newFrame.Layers.Count;
                newFrame.AddLayer(newLayer, false);
            }

            //if (ActiveLayerIndex >= 0 && ActiveLayerIndex < newFrame.Layers.Count)
            //newFrame.ActiveLayerIndex = ActiveLayerIndex;

            newFrame.ActiveLayer = newFrame.Layers[Layers.IndexOf(ActiveLayer)];

            newFrame.Order = Order;

            return newFrame;
        }
    }
}
