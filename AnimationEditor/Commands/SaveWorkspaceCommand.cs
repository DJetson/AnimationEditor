using AnimationEditor.BaseClasses;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class SaveWorkspaceCommand : RequeryBase
    {
        public override string DisplayName => "Save Workspace";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as WorkspaceViewModel;

            Parameter.UpdateModelAndSaveWorkspace();
        }
    }
}
