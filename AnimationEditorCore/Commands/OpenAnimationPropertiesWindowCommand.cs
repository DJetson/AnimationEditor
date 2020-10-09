using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using AnimationEditorCore.ViewModels.Settings;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.Commands
{
    public class OpenAnimationPropertiesWindowCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var layerProperties = new AnimationPropertiesWindow() { Owner = Application.Current.MainWindow, DataContext = new AnimationPropertiesViewModel(Parameter) };
            layerProperties.ShowDialog();
        }
    }
}
