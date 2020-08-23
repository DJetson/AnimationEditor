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
        string Filepath { get; set; }

        ICanvasPropertiesModel CanvasProperties { get; set; }
        IAnimationPropertiesModel AnimationProperties { get; set; }
        List<IFrameModel> Frames { get; set; }

        void SyncToViewModel(WorkspaceViewModel workspaceViewModel);
        void LoadWorkspaceFile(string filepath);
        void SaveWorkspaceFile(string filepath);
    }
}
