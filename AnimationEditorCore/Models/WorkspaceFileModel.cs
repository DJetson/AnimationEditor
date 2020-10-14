using AnimationEditorCore.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace AnimationEditorCore.Models
{
    public class WorkspaceFileModel
    {
        public string Filepath { get; set; }

        public CanvasPropertiesModel CanvasProperties { get; set; }

        public AnimationPropertiesModel AnimationProperties { get; set; }

        public List<LayerModel> Layers { get; set; } = new List<LayerModel>();

        private System.Text.Json.JsonSerializerOptions JsonSerializerOptions { get; set; }

        public static WorkspaceFileModel OpenWorkspaceFile(string filepath, System.Text.Json.JsonSerializerOptions options)
        {
            return System.Text.Json.JsonSerializer.Deserialize<WorkspaceFileModel>(File.ReadAllText(filepath), options);
        }

        public void SaveWorkspaceFile(string filepath)
        {
            if (filepath == null)
                return;

            JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new Models.StrokeCollectionConverter());

            Filepath = filepath;

            if(!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }

            File.WriteAllText(filepath, System.Text.Json.JsonSerializer.Serialize(this, JsonSerializerOptions));
        }

        public void SyncToViewModel(WorkspaceViewModel workspaceViewModel)
        {
            Filepath = workspaceViewModel.Filepath;

            Layers.Clear();

            foreach (var item in workspaceViewModel.TimelineViewModel.Layers)
            {
                Layers.Add(new LayerModel(item));
            }

            //TODO: These should eventually be coming from the workspace VM
            CanvasProperties = new CanvasPropertiesModel() { Height = workspaceViewModel.TimelineViewModel.CanvasHeight, Width = workspaceViewModel.TimelineViewModel.CanvasWidth };
            AnimationProperties = new AnimationPropertiesModel() { FramesPerSecond = workspaceViewModel.TimelineViewModel.FramesPerSecond };
        }
    }
}
