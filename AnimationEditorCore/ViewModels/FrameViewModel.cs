using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditorCore.ViewModels
{
    public class FrameViewModel : ViewModelBase, IMementoOriginator
    {
        //private DelegateCommand _InsertLayerAbove;
        //public DelegateCommand InsertLayerAbove
        //{
        //    get { return _InsertLayerAbove; }
        //    set { _InsertLayerAbove = value; NotifyPropertyChanged(); }
        //}

        //private DelegateCommand _InsertLayerBelow;
        //public DelegateCommand InsertLayerBelow
        //{
        //    get { return _InsertLayerBelow; }
        //    set { _InsertLayerBelow = value; NotifyPropertyChanged(); }
        //}

        //private DelegateCommand _RemoveLayer;
        //public DelegateCommand RemoveLayer
        //{
        //    get { return _RemoveLayer; }
        //    set { _RemoveLayer = value; NotifyPropertyChanged(); }
        //}

        //private LayerViewModel _ActiveLayer;
        //public LayerViewModel ActiveLayer
        //{
        //    get { return _ActiveLayer; }
        //    set
        //    {
        //        if (_ActiveLayer != null)
        //        {
        //            _ActiveLayer.IsActive = false;
        //        }
        //        _ActiveLayer = value;
        //        if (_ActiveLayer != null)
        //        {
        //            _ActiveLayer.IsActive = true;
        //        }
        //        NotifyPropertyChanged();
        //    }
        //}

        //private ObservableCollection<LayerViewModel> _Layers;
        //public ObservableCollection<LayerViewModel> Layers
        //{
        //    get { return _Layers; }
        //    set { _Layers = value; NotifyPropertyChanged(); }
        //}

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


        //public bool IsFrameZIndexVisible
        //{
        //    get => WorkspaceViewModel.AnimationTimelineViewModel.AnimationPlaybackViewModel.CurrentFrame.Order == Order;
        //}
        //public void AddNewLayer(bool createUndoState = true)
        //{
        //    if (Layers == null)
        //        Layers = new ObservableCollection<LayerViewModel>();

        //    var newLayer = new LayerViewModel(this);
        //    Layers.Add(newLayer);

        //    PopulateLayerIds();
        //    ActiveLayer = newLayer;

        //    if (createUndoState)
        //        PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        //}

        //private bool InsertLayerBelow_CanExecute(object parameter)
        //{
        //    if (!(parameter is LayerViewModel Parameter))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private void InsertLayerBelow_Execute(object parameter)
        //{
        //    var Parameter = parameter as LayerViewModel;
        //    AddNewLayerAtIndex(Layers.IndexOf(Parameter));
        //}

        //private bool InsertLayerAbove_CanExecute(object parameter)
        //{
        //    if (!(parameter is LayerViewModel Parameter))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private void InsertLayerAbove_Execute(object parameter)
        //{
        //    var Parameter = parameter as LayerViewModel;

        //    AddNewLayerAtIndex(Layers.IndexOf(Parameter) + 1);
        //}

        //private bool RemoveLayer_CanExecute(object parameter)
        //{
        //    if (!(parameter is LayerViewModel Parameter))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public void PopulateLayerIds(int startIndex = 0)
        //{
        //    foreach (var layer in Layers.Where(e => e.LayerId >= startIndex))
        //    {
        //        layer.LayerId = Layers.IndexOf(layer);
        //    }
        //}

        //private void RemoveLayer_Execute(object parameter)
        //{
        //    var toRemove = parameter as LayerViewModel;

        //    var toRemoveIndex = Layers.IndexOf(toRemove);
        //    Layers.Remove(toRemove);

        //    LayerState newLayerState = null;

        //    if (Layers.Count == 0)
        //    {
        //        var newLayer = new LayerViewModel(this);
        //        AddNewLayerAtIndex(Layers.Count, $"Layer {Layers.Count}", false);
        //        newLayerState = newLayer.SaveState() as LayerState;
        //    }

        //    PopulateLayerIds();

        //    var frameState = SaveState() as FrameState;

        //    if (newLayerState != null)
        //    {
        //        var multiState = new MultiState(null, $"Removed Layer {toRemove.LayerId} from Frame {Order}", newLayerState, frameState);
        //        PushUndoRecord(multiState);
        //    }
        //    else
        //    {
        //        var multiState = new MultiState(null, $"Removed Layer {toRemove.LayerId} from Frame {Order}", frameState);
        //        PushUndoRecord(multiState);
        //    }
        //}

        //public void AddNewLayerAtIndex(int index, string layerName = "", bool createUndoState = true)
        //{
        //    if (Layers == null)
        //        Layers = new ObservableCollection<LayerViewModel>();

        //    if (String.IsNullOrWhiteSpace(layerName))
        //    {
        //        layerName = $"Layer {Layers.Count}";
        //    }

        //    var newLayer = new LayerViewModel(this) { LayerId = Layers.Count, DisplayName = layerName, IsVisible = true };

        //    if (index < 0)
        //    {
        //        Layers.Insert(0, newLayer);
        //    }
        //    else if (index < Layers.Count)
        //    {
        //        Layers.Insert(index, newLayer);
        //    }
        //    else
        //    {
        //        Layers.Add(newLayer);
        //    }

        //    ActiveLayer = newLayer;
        //    PopulateLayerIds();

        //    if (createUndoState)
        //        PushUndoRecord(CreateUndoState($"Added New {newLayer.DisplayName} to Frame {Order}"));
        //}

        //public void AddLayer(LayerViewModel layer, bool createUndoState = true)
        //{
        //    if (Layers == null)
        //        Layers = new ObservableCollection<LayerViewModel>();

        //    layer.LayerId = Layers.Count;
        //    Layers.Add(layer);
        //    ActiveLayer = layer;

        //    PopulateLayerIds();

        //    if (createUndoState)
        //        PushUndoRecord(CreateUndoState($"Added New Layer to Frame {Order}"));
        //}

        //public FrameViewModel(WorkspaceViewModel workspace, bool createFirstLayer = true)
        //{
        //    //InitializeCommands();
        //    WorkspaceViewModel = workspace;
        //    Layers = new ObservableCollection<LayerViewModel>();

        //    if (createFirstLayer)
        //        Layers.Add(new LayerViewModel(this));

        //    ActiveLayer = Layers.FirstOrDefault();

        //    PopulateLayerIds();

        //    StrokeCollection = new StrokeCollection();
        //    FlattenStrokesForPlayback();
        //}

        //public List<UndoStateViewModel> SaveLayerStates()
        //{
        //    var layerStates = new List<UndoStateViewModel>();

        //    foreach (var layer in Layers)
        //    {
        //        layerStates.Add(layer.SaveState() as LayerState);
        //    }

        //    return layerStates;
        //}

        //public FrameViewModel(FrameViewModel frame)
        //{
        //    InitializeCommands();
        //    WorkspaceViewModel = frame.WorkspaceViewModel;
        //    Layers = new ObservableCollection<LayerViewModel>(frame.Layers.ToList().Select(e => new LayerViewModel(this, e.StrokeCollection)));
        //    ActiveLayer = Layers[frame.Layers.IndexOf(frame.ActiveLayer)];
        //    FlattenStrokesForPlayback();
        //}

        //public void FlattenStrokesForPlayback()
        //{
        //    StrokeCollection.Clear();

        //    foreach (var layer in Layers)
        //    {
        //        if (layer.IsVisible)
        //            StrokeCollection.Add(layer.StrokeCollection);
        //    }
        //}

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

        public FrameViewModel(Models.FrameModel model, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            //StrokeCollection = model.StrokeCollection;
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            //Layers = new ObservableCollection<LayerViewModel>(model.Layers.Select(e => new LayerViewModel(e, this)));
            Order = model.Order;
            InitializeCommands();
            //ActiveLayer = Layers[model.ActiveLayerIndex];
            //FlattenStrokesForPlayback();
        }

        public void InitializeCommands()
        {
            UpdateSelectedStrokes = new DelegateCommand(UpdateSelectedStrokes_CanExecute, UpdateSelectedStrokes_Execute);
        }

        public MultiState CreateUndoState(string title)
        {
            var state = SaveState() as FrameState;
            var multiState = new MultiState(null, title, state);
            return multiState;
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

        public void LoadState(IMemento memento)
        {
            var Memento = (memento as FrameState);

            StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            
            //This should really be happening when a state is pushed rather than when its loaded
            foreach(var stroke in Memento.StrokeCollection)
            {
                stroke.StylusPointsChanged -= Stroke_StylusPointsChanged;
            }

            StrokeCollection.Clear();
            var loadedStrokes = new StrokeCollection();
            foreach(var stroke in Memento.StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += Stroke_StylusPointsChanged;
                loadedStrokes.Add(newStroke);
            }
            
            StrokeCollection = new StrokeCollection(loadedStrokes);
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            Order = Memento.Order;
        }

        public FrameViewModel Clone()
        {
            var newFrame = new FrameViewModel(LayerViewModel, Order);

            newFrame.StrokeCollection = StrokeCollection.Clone();

            foreach (var stroke in StrokeCollection)
            {
                var newStroke = stroke.Clone();
                newStroke.StylusPointsChanged += newFrame.Stroke_StylusPointsChanged;
                newFrame.StrokeCollection.Add(newStroke);
            }


            newFrame.StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;

            return newFrame;
        }
            #region Refactor

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
                //if (e.Added.Count > 0)
                //    PushUndoRecord(CreateUndoState($"Pasted Content into Layer {LayerId} on Frame {FrameViewModel.Order}"));
                //else 
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
            LayerViewModel.TimelineViewModel.FlattenStrokesForPlayback(); 
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
        #endregion Refactor
    }
}
