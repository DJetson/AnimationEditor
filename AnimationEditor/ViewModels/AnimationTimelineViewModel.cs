using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;

namespace AnimationEditor.ViewModels
{
    public class AnimationTimelineViewModel : ViewModelBase
    {
        private ObservableCollection<FrameViewModel> _Frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get => _Frames;
            set
            {
                _Frames = value;
                NotifyPropertyChanged();

                SelectedFrame = Frames.FirstOrDefault();
            }
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

        private DelegateCommand _AppendFrame;
        public DelegateCommand AppendFrame
        {
            get => _AppendFrame;
            set { _AppendFrame = value; NotifyPropertyChanged(); }
        }

        //private ObservableCollection<FrameViewModel> _NextFrames = new ObservableCollection<FrameViewModel>();
        //public ObservableCollection<FrameViewModel> NextFrames
        //{
        //    get => _NextFrames;
        //    set { _NextFrames = value; NotifyPropertyChanged(); }
        //}

        //private FrameViewModel _NextFrame;
        public StrokeCollection NextFrameStrokes
        {
            get
            {
                if (NextFrame == null)
                    return null;
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
                    return null;
                var strokes = PreviousFrame.StrokeCollection.Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = Color.FromArgb(128, 255, 0, 0);
                }
                return strokes;
            }
        }

        public FrameViewModel NextFrame
        {
            get => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) + 1);
        }

        public FrameViewModel PreviousFrame
        {
            get => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) - 1);
        }

        public void InitializeCommands()
        {
            AppendFrame = new DelegateCommand(AppendFrame_CanExecute, AppendFrame_Execute);
        }

        public AnimationTimelineViewModel(List<IFrameModel> frames)
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

        public void AppendFrame_Execute(object parameter)
        {
            var newFrame = new FrameViewModel();
            AddFrameAtIndex(newFrame, Frames.Count);
            SelectedFrame = newFrame;
        }

        public bool AppendFrame_CanExecute(object parameter)
        {
            return true;
        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {
            if (index < Frames.Count)
            {
                Frames.Insert(index, frame);
            }
            else if (index == Frames.Count)
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
