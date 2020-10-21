using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels
{
    public class FrameViewModel : ViewModelBase, IFrameViewModel
    {
        protected StrokeCollection _StrokeCollection = new StrokeCollection();
        public virtual StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { _StrokeCollection = value; NotifyPropertyChanged(); }
        }

        protected WorkspaceViewModel _WorkspaceViewModel;
        public virtual WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        protected LayerViewModel _LayerViewModel;
        public virtual LayerViewModel LayerViewModel
        {
            get { return _LayerViewModel; }
            set { _LayerViewModel = value; NotifyPropertyChanged(); }
        }

        protected int _Order;
        public virtual int Order
        {
            get => _Order;
            set { _Order = value; NotifyPropertyChanged(); }
        }

        protected bool _IsCurrent;
        public virtual bool IsCurrent
        {
            get { return _IsCurrent; }
            set { _IsCurrent = value; NotifyPropertyChanged(); }
        }

        protected DelegateCommand _LoadedInkCanvas;
        public virtual DelegateCommand LoadedInkCanvas
        {
            get { return _LoadedInkCanvas; }
            set { _LoadedInkCanvas = value; NotifyPropertyChanged(); }
        }

        protected InkCanvas _InkCanvas;
        public virtual InkCanvas InkCanvas
        {
            get { return _InkCanvas; }
            set { _InkCanvas = value; NotifyPropertyChanged(); }
        }

        protected virtual bool LoadedInkCanvas_CanExecute(object parameter)
        {
            if (!(parameter is InkCanvas Parameter))
                return false;

            return true;
        }

        protected virtual void LoadedInkCanvas_Execute(object parameter)
        {
            var Parameter = parameter as InkCanvas;

            InkCanvas = Parameter;
        }

        public virtual void InitializeCommands()
        {
            LoadedInkCanvas = new DelegateCommand(LoadedInkCanvas_CanExecute, LoadedInkCanvas_Execute);
        }

        public FrameViewModel(Models.FrameModel model, LayerViewModel layer)
        {
            LayerViewModel = layer;

            Order = model.Order;
            InitializeCommands();

            var precedingKeyFrame = layer.GetPrecedingKeyFrame(this);
            if (precedingKeyFrame != null)
                StrokeCollection = precedingKeyFrame.StrokeCollection;

            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(LayerViewModel layer, int orderId)
        {
            LayerViewModel = layer;
            Order = orderId;

            InitializeCommands();
        }

        public FrameViewModel(FrameViewModel originalFrame)
        {
            LayerViewModel = originalFrame.LayerViewModel;
            Order = originalFrame.Order;
            InitializeCommands();

            DisplayName = originalFrame.DisplayName;
        }

        public virtual IFrameViewModel Clone()
        {
            var newFrame = new FrameViewModel(this);

            return newFrame;
        }

        public virtual IFrameViewModel Duplicate(int newOrder = -1, string newDisplayName = "")
        {
            return DuplicateFrame(this, newOrder, newDisplayName);
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

    }
}
