using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels
{
    public class LayerViewModel : ViewModelBase
    {
        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { Console.WriteLine($"Setting StrokeCollection [OldCount={_StrokeCollection.Count} New={value.Count}]"); _StrokeCollection = value; NotifyPropertyChanged(); }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set 
            { 
                _IsVisible = value; 
                NotifyPropertyChanged(nameof(IsVisible), nameof(IsAcceptingInput));
                TimelineViewModel.NotifyPropertyChanged(nameof(TimelineViewModel.PreviousFrameStrokes),
                                                        nameof(TimelineViewModel.PreviousOnionSkins),
                                                        nameof(TimelineViewModel.NextFrameStrokes),
                                                        nameof(TimelineViewModel.NextOnionSkins));
            }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; NotifyPropertyChanged(nameof(IsActive), nameof(IsAcceptingInput)); }
        }

        public bool IsAcceptingInput
        {
            get => (IsActive && IsVisible);
        }

        private int _ArrangedZIndex;
        public int ArrangedZIndex
        {
            get { return _ArrangedZIndex; }
            set { _ArrangedZIndex = value; NotifyPropertyChanged(nameof(ArrangedZIndex), nameof(EffectiveZIndex)); }
        }

        private int _ZIndex;
        public int ZIndex
        {
            get => _ZIndex;
            set { _ZIndex = value; NotifyPropertyChanged(nameof(ZIndex), nameof(EffectiveZIndex)); }
        }

        public int EffectiveZIndex
        {
            get => ZIndex + (IsActive ? 0 : 10000);
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

        private TimelineViewModel _TimelineViewModel;
        public TimelineViewModel TimelineViewModel
        {
            get { return _TimelineViewModel; }
            set { _TimelineViewModel = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<FrameViewModel> _Frames = new ObservableCollection<FrameViewModel>();
        public ObservableCollection<FrameViewModel> Frames
        {
            get { return _Frames; }
            set { _Frames = value; NotifyPropertyChanged(); }
        }

        public LayerViewModel(TimelineViewModel timeline, int zIndex, string displayName ="")
        {
            TimelineViewModel = timeline;
            ZIndex = zIndex;
            DisplayName = displayName;
        }

        public LayerViewModel(LayerViewModel originalLayer)
        {
            TimelineViewModel = originalLayer.TimelineViewModel;
            ZIndex = originalLayer.ZIndex;
            ArrangedZIndex = originalLayer.ArrangedZIndex;
            IsVisible = originalLayer.IsVisible;
            DisplayName = originalLayer.DisplayName;

            Frames = new ObservableCollection<FrameViewModel>();

            foreach(var frame in originalLayer.Frames)
            {
                var clonedFrame = frame.Clone();
                clonedFrame.LayerViewModel = this;
                Frames.Add(clonedFrame);
            }

            SelectedFrameIndex = originalLayer.SelectedFrameIndex;
            SelectedFrame = Frames[SelectedFrameIndex];

            IsActive = originalLayer.IsActive;
        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {
            frame.Order = index;
            if (index < Frames.Count)
            {
                Frames.Insert(index, frame);
            }
            else if (index >= Frames.Count)
            {
                Frames.Add(frame);
            }
            else
            {
                Console.WriteLine($"WorkspaceViewModel.InsertFrame ERROR: Attempted to insert a frame at an invalid index = {index}");
            }

            UpdateFrameOrderIds();
        }

        public void UpdateFrameOrderIds()
        {
            foreach (var frame in Frames)
            {
                frame.Order = Frames.IndexOf(frame);
            }
        }

        private FrameViewModel _SelectedFrame;
        public FrameViewModel SelectedFrame
        {
            get { return _SelectedFrame; }
            set { _SelectedFrame = value; NotifyPropertyChanged(); }
        }

        private int _SelectedFrameIndex;
        public int SelectedFrameIndex
        {
            get { return _SelectedFrameIndex; }
            set { _SelectedFrameIndex = value; NotifyPropertyChanged(); }
        }

        public LayerViewModel(Models.LayerModel model, TimelineViewModel timeline)
        {
            TimelineViewModel = timeline;
            DisplayName = model.DisplayName;
            IsVisible = model.IsVisible;
            ZIndex = model.LayerId;
            ArrangedZIndex = model.ArrangedZIndex;

            Frames = new ObservableCollection<FrameViewModel>(model.Frames.Select(e => new FrameViewModel(e,this)));

            SelectedFrameIndex = model.SelectedFrameIndex;
            IsActive = model.IsActive;
        }

        public void ClearFrames()
        {
            foreach (var frame in Frames)
            {
                frame.ClearStrokes();
            }

            Frames.Clear();
        }

        public static void CopyToLayer(LayerViewModel original, LayerViewModel destination)
        {
            destination.ClearFrames();

            destination.TimelineViewModel = original.TimelineViewModel;
            destination.ZIndex = original.ZIndex;
            destination.ArrangedZIndex = original.ArrangedZIndex;
            destination.IsVisible = original.IsVisible;
            destination.DisplayName = original.DisplayName;

            destination.Frames = new ObservableCollection<FrameViewModel>();

            foreach (var frame in original.Frames)
            {
                var clonedFrame = frame.Clone();
                clonedFrame.LayerViewModel = destination;
                destination.Frames.Add(clonedFrame);
            }

            destination.SelectedFrameIndex = original.SelectedFrameIndex;
            destination.SelectedFrame = destination.Frames[destination.SelectedFrameIndex];
            destination.IsActive = original.IsActive;
        }

        public LayerViewModel Clone()
        {
            var newLayer = new LayerViewModel(this);

            return newLayer;
        }
    }
}
