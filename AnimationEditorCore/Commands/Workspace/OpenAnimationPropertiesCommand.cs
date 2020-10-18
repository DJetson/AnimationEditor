using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using AnimationEditorCore.ViewModels.Settings;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.Commands.Workspace
{
    public class OpenAnimationPropertiesCommand : RequeryBase
    {
        public override string Description => Resources.OpenAnimationPropertiesDescription;
        public override string ToolTip => Resources.OpenAnimationPropertiesToolTip;
        public override string UndoStateTitle => Resources.OpenAnimationPropertiesUndoStateTitle;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var layerProperties = new AnimationPropertiesWindow() { Owner = System.Windows.Application.Current.MainWindow, DataContext = new AnimationPropertiesViewModel(Parameter) };
            var result = layerProperties.ShowDialog();

            if(result == true)
                WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
