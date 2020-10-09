﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using AnimationEditorCore.ViewModels.Settings;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands
{
    public class OpenLayerPropertiesCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            return true;
        }


        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;

            var layerProperties = new LayerPropertiesView() { DataContext = new LayerPropertiesViewModel(Parameter) };
            layerProperties.ShowDialog();
        }
    }
}