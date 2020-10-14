using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;

namespace AnimationEditorCore.Commands
{
    public class SelectToolCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;

            var Parameter = Enum.Parse(typeof(EditorToolType), parameter.ToString());

            if (Parameter == null)
                return false;

            if (EditorToolsViewModel.Instance == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = (EditorToolType)Enum.Parse(typeof(EditorToolType), parameter.ToString());

            EditorToolsViewModel.SelectToolType(Parameter);
        }
    }
}
