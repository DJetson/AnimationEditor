using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IHasWorkspaceCollection
    {
        ObservableCollection<WorkspaceViewModel> Workspaces { get; set; }
        WorkspaceViewModel CreateNewWorkspace();
        void AddWorkspace(WorkspaceViewModel workspace);
        void RemoveWorkspace(WorkspaceViewModel workspace);
    }
}
