﻿using AnimationEditor.Utilities;
using AnimationEditor.ViewModels;
using BumpKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AnimationEditor.Models
{
    public class WorkspaceFileModel
    {
        public string Filepath { get; set; }

        public CanvasPropertiesModel CanvasProperties { get; set; }

        public AnimationPropertiesModel AnimationProperties { get; set; }

        public List<FrameModel> Frames { get; set; } = new List<FrameModel>();

        private System.Text.Json.JsonSerializerOptions JsonSerializerOptions { get; set; }

        public static WorkspaceFileModel OpenWorkspaceFile(string filepath, System.Text.Json.JsonSerializerOptions options)
        {
            return System.Text.Json.JsonSerializer.Deserialize<WorkspaceFileModel>(File.ReadAllText(filepath), options);
        }

        public void SaveWorkspaceFile(string filepath)
        {
            JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new Models.StrokeCollectionConverter());

            Filepath = filepath;

            File.WriteAllText(filepath, System.Text.Json.JsonSerializer.Serialize(this, JsonSerializerOptions));
        }

        public void SyncToViewModel(WorkspaceViewModel workspaceViewModel)
        {
            Filepath = workspaceViewModel.Filepath;

            Frames.Clear();

            //Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in workspaceViewModel.AnimationTimelineViewModel.Frames)
            {
                Frames.Add(new FrameModel(item));
            }

            //TODO: These should eventually be coming from the workspace VM
            CanvasProperties = new CanvasPropertiesModel();
            AnimationProperties = new AnimationPropertiesModel();
        }
    }
}
