using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimationEditor.ViewModels;

namespace AnimationEditor.Interfaces
{
    public interface IWorkspaceFileModel
    {
        string Filepath { get; }

        ICanvasPropertiesModel CanvasProperties { get; }
        IAnimationPropertiesModel AnimationProperties { get; }
        List<IFrameModel> Frames { get; }

        void SyncToViewModel(WorkspaceViewModel workspaceViewModel);
        //IWorkspaceFileModel LoadWorkspaceFile(string filepath);
        void SaveWorkspaceFile(string filepath);
    }
}
