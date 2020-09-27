using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class LayerState : UndoStateViewModel
    {
        public int LayerId;
        public bool IsVisible;
        public bool IsActive;
        public ObservableCollection<FrameViewModel> Frames;
        public LayerState(LayerViewModel layer, string stateName = "") : base(layer, stateName)
        {
            DisplayName = layer.DisplayName;
            LayerId = layer.LayerId;
            IsVisible = layer.IsVisible;
            IsActive = layer.IsActive;
            Frames = new ObservableCollection<FrameViewModel>();

            foreach (var frame in layer.Frames)
            {
                Frames.Add(frame.Clone());
            }
        }

        public override void LoadState()
        {
            LayerViewModel originator = Originator as LayerViewModel;
            originator.LoadState(this);
        }
    }
}
