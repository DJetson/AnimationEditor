using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class TimelineState : UndoStateViewModel
    {
        //public LayerViewModel ActiveLayer;
        //public ObservableCollection<LayerViewModel> Layers;
        //public int SelectedFrameIndex;
        public TimelineViewModel Timeline;

        public TimelineState(TimelineViewModel timeline, string stateName = "") : base(timeline, stateName)
        {
            Timeline = new TimelineViewModel(timeline);
            DisplayName = stateName;
            //Layers = new ObservableCollection<LayerViewModel>();

            //foreach (var layer in timeline.Layers)
            //{
            //    Layers.Add(layer.Clone());
            //}

            //ActiveLayer = Layers[timeline.Layers.IndexOf(timeline.ActiveLayer)];
            //SelectedFrameIndex = timeline.SelectedFrameIndex;
        }

        public override void LoadState()
        {
            Console.WriteLine("Loading TimelineState");
            TimelineViewModel originator = Originator as TimelineViewModel;
            originator.LoadState(this);
        }
    }
}
