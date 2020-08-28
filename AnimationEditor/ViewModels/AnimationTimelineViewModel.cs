using AnimationEditor.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Media;

namespace AnimationEditor.ViewModels
{
    public enum FrameNavigation { Start, Previous, Current, Next, End };

    public class AnimationTimelineViewModel : ViewModelBase
    {
        private AnimationPlaybackViewModel _AnimationPlaybackViewModel = new AnimationPlaybackViewModel();
        public AnimationPlaybackViewModel AnimationPlaybackViewModel
        {
            get { return _AnimationPlaybackViewModel; }
            set { _AnimationPlaybackViewModel = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<FrameViewModel> _Frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get => _Frames;
            set { _Frames = value; NotifyPropertyChanged(); SelectedFrame = Frames.FirstOrDefault(); }
        }

        private FrameViewModel _SelectedFrame;
        public FrameViewModel SelectedFrame
        {
            get => _SelectedFrame;
            set
            {
                _SelectedFrame = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NextFrame));
                NotifyPropertyChanged(nameof(PreviousFrame));
                NotifyPropertyChanged(nameof(NextFrameStrokes));
                NotifyPropertyChanged(nameof(PreviousFrameStrokes));
            }
        }

        private double _FramesPerSecond = 24;
        public double FramesPerSecond
        {
            get { return _FramesPerSecond; }
            set { _FramesPerSecond = value; NotifyPropertyChanged(); }
        }

        public StrokeCollection NextFrameStrokes
        {
            get
            {
                if (NextFrame == null)
                    return new StrokeCollection();

                var strokes = NextFrame.StrokeCollection.Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = Color.FromArgb(128, 0, 255, 0);
                }
                return strokes;
            }
        }

        public StrokeCollection PreviousFrameStrokes
        {
            get
            {
                if (PreviousFrame == null)
                    return new StrokeCollection();

                var strokes = PreviousFrame.StrokeCollection.Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = Color.FromArgb(128, 255, 0, 0);
                }
                return strokes;
            }
        }

        public FrameViewModel NextFrame => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) + 1);
        public FrameViewModel PreviousFrame => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) - 1);

        #region PlayAnimation Command
        private DelegateCommand _PlayAnimation;
        public DelegateCommand PlayAnimation
        {
            get { return _PlayAnimation; }
            set { _PlayAnimation = value; NotifyPropertyChanged(); }
        }

        public void PlayAnimation_Execute(object parameter)
        {
            //NOTE: Eventually this will be able to be set to the set of currently selected frames as well
            var playbackFrames = Frames.ToList();

            //NOTE: I may have to eventually do some caching up top here. Rasterize all of the InkCanvases in the series
            //      and just flip through those instead of directly showing the InkCanvases. It will depend entirely on
            //      how well it performs in it's initial, most basic form.

            switch (AnimationPlaybackViewModel.CurrentState)
            {
                case PlaybackStates.Pause:
                    AnimationPlaybackViewModel.ResumePlayback();
                    break;
                case PlaybackStates.Play:
                    AnimationPlaybackViewModel.PausePlayback();
                    break;
                case PlaybackStates.Stop:
                    AnimationPlaybackViewModel.StartPlayback(playbackFrames, FramesPerSecond);
                    break;
            }
        }

        public bool PlayAnimation_CanExecute(object parameter)
        {
            //TODO: Eventually I need to add the conditions to return false If we're currently 
            //in a "modal" state with the ink canvas for any reason, return false. We don't 
            //need an active Scale transform going all screwy because the user accidentally hits 
            //the play button before they resolve the resize, causing the playback object to yank 
            //the target frame right out from under them

            return true;
        }
        #endregion PlayAnimation Command

        #region StopAnimation Command
        private DelegateCommand _StopAnimation;
        public DelegateCommand StopAnimation
        {
            get { return _StopAnimation; }
            set { _StopAnimation = value; NotifyPropertyChanged(); }
        }

        public void StopAnimation_Execute(object parameter)
        {
            switch (AnimationPlaybackViewModel.CurrentState)
            {
                case PlaybackStates.Pause:
                    AnimationPlaybackViewModel.StopPlayback();
                    break;
                case PlaybackStates.Play:
                    AnimationPlaybackViewModel.StopPlayback();
                    break;
            }
        }

        public bool StopAnimation_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Stop)
                return false;

            return true;
        }
        #endregion StopAnimation Command

        #region NavigateToFrame Command
        private DelegateCommand _NavigateToFrame;
        public DelegateCommand NavigateToFrame
        {
            get { return _NavigateToFrame; }
            set { _NavigateToFrame = value; NotifyPropertyChanged(); }
        }

        public void NavigateToFrame_Execute(object parameter)
        {
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int navigateToFrameIndex = selectedFrameIndex;

            switch (Parameter)
            {
                case FrameNavigation.Start:
                    navigateToFrameIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    navigateToFrameIndex = Math.Max(selectedFrameIndex - 1, 0);
                    break;
                case FrameNavigation.Next:
                    navigateToFrameIndex = Math.Min(selectedFrameIndex + 1, Frames.Count - 1);
                    break;
                case FrameNavigation.End:
                    navigateToFrameIndex = Frames.Count - 1;
                    break;
            }

            SelectedFrame = Frames[navigateToFrameIndex];
        }

        public bool NavigateToFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }
        #endregion NavigateToFrame Command

        #region AddBlankFrame Command
        private DelegateCommand _AddBlankFrame;
        public DelegateCommand AddBlankFrame
        {
            get => _AddBlankFrame;
            set { _AddBlankFrame = value; NotifyPropertyChanged(); }
        }

        public void AddBlankFrame_Execute(object parameter)
        {
            var newFrame = new FrameViewModel();

            int insertAtIndex = Frames.Count;
            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());
            switch (Parameter)
            {
                case FrameNavigation.Start:
                    insertAtIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    insertAtIndex = selectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    insertAtIndex = selectedFrameIndex + 1;
                    break;
                case FrameNavigation.End:
                    insertAtIndex = Frames.Count;
                    break;
            }

            AddFrameAtIndex(newFrame, insertAtIndex);
            SelectedFrame = newFrame;
        }

        public bool AddBlankFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Pause)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }
        #endregion AddBlankFrame Command

        #region DuplicateCurrentFrame Command
        private DelegateCommand _DuplicateCurrentFrame;
        public DelegateCommand DuplicateCurrentFrame
        {
            get { return _DuplicateCurrentFrame; }
            set { _DuplicateCurrentFrame = value; NotifyPropertyChanged(); }
        }

        public void DuplicateCurrentFrame_Execute(object parameter)
        {
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int DuplicateCurrentFrameToIndex = selectedFrameIndex;

            var newFrame = new FrameViewModel()
            {
                StrokeCollection = SelectedFrame.StrokeCollection.Clone(),
                Strokes = new ObservableCollection<Stroke>(SelectedFrame.Strokes),
            };

            switch (Parameter)
            {
                //case FrameNavigation.Start:
                //    DuplicateCurrentFrameToIndex = 0;
                //    break;
                case FrameNavigation.Previous:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex + 1;
                    break;
                //case FrameNavigation.End:
                //    DuplicateCurrentFrameToIndex = Frames.Count - 1;
                //    break;
            }

            AddFrameAtIndex(newFrame, DuplicateCurrentFrameToIndex);
            SelectedFrame = Frames[DuplicateCurrentFrameToIndex];
        }

        public bool DuplicateCurrentFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Pause)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }
        #endregion DuplicateCurrentFrame Command

        public void InitializeCommands()
        {
            AddBlankFrame = new DelegateCommand(AddBlankFrame_CanExecute, AddBlankFrame_Execute);
            PlayAnimation = new DelegateCommand(PlayAnimation_CanExecute, PlayAnimation_Execute);
            StopAnimation = new DelegateCommand(StopAnimation_CanExecute, StopAnimation_Execute);
            NavigateToFrame = new DelegateCommand(NavigateToFrame_CanExecute, NavigateToFrame_Execute);
            DuplicateCurrentFrame = new DelegateCommand(DuplicateCurrentFrame_CanExecute, DuplicateCurrentFrame_Execute);
        }

        public AnimationTimelineViewModel(List<Models.FrameModel> frames)
        {
            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in frames)
            {
                Frames.Add(new FrameViewModel(item));
            }

            SelectedFrame = Frames.FirstOrDefault();
        }

        public AnimationTimelineViewModel()
        {
            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            Frames.Add(new FrameViewModel());
            SelectedFrame = Frames.FirstOrDefault();
        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {
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
        }
    }
}
