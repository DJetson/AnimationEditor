using AnimationEditor.BaseClasses;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class UndoCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (MainWindowViewModel.WorkspaceManager?.PeekUndo() == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            MainWindowViewModel.WorkspaceManager.Undo();
        }
    }
}
