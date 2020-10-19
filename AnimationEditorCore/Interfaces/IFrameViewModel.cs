using AnimationEditorCore.Commands;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace AnimationEditorCore.Interfaces
{
    public interface IFrameViewModel
    {
        WorkspaceViewModel WorkspaceViewModel { get; set; }
        LayerViewModel LayerViewModel { get; set; }
        int Order { get; set; }
        bool IsCurrent { get; set; }
        DelegateCommand LoadedInkCanvas { get; set; }
        InkCanvas InkCanvas { get; set; }

        void InitializeCommands();
        IFrameViewModel Clone();
        IFrameViewModel Duplicate(int newOrder = -1, string newDisplayName = "");


    }
}
