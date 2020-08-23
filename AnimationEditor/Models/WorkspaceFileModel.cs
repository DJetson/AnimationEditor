using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Models
{
    public class WorkspaceFileModel : IWorkspaceFileModel
    {
        public string Filepath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ICanvasPropertiesModel CanvasProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAnimationPropertiesModel AnimationProperties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<IFrameModel> Frames { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void LoadWorkspaceFile(string filepath)
        {
            throw new NotImplementedException();
        }

        public void SaveWorkspaceFile(string filepath)
        {
            throw new NotImplementedException();
        }

        public void SyncToViewModel(WorkspaceViewModel workspaceViewModel)
        {
            throw new NotImplementedException();
        }
    }
}
