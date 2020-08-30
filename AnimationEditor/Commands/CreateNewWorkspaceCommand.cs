using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class CreateNewWorkspaceCommand : RequeryBase
    {
        public override string DisplayName => "Create New Workspace";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is IHasWorkspaceCollection Parameter))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as IHasWorkspaceCollection;

            Parameter.CreateNewWorkspace();
        }
    }
}
