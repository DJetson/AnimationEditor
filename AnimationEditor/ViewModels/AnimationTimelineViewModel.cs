using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            set { _SelectedFrame = value; NotifyPropertyChanged(); }
        }

        public AnimationTimelineViewModel()
        {
        }
    }
}
