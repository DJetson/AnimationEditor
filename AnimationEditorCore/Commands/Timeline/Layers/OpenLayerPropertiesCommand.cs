﻿using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using AnimationEditorCore.ViewModels.Settings;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class OpenLayerPropertiesCommand : RequeryBase
    {
        public override string Description => Resources.OpenLayerPropertiesDescription;
        public override string ToolTip => Resources.OpenLayerPropertiesToolTip;
        public override string UndoStateTitle => Resources.OpenLayerPropertiesUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            return true;
        }


        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;

            var layerProperties = new LayerPropertiesView() { Owner = System.Windows.Application.Current.MainWindow, DataContext = new LayerPropertiesViewModel(Parameter) };
            var result = layerProperties.ShowDialog();

            if(result == true)
                WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
