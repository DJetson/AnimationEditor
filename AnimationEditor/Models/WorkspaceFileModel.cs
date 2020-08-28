using AnimationEditor.ViewModels;
using System.Collections.Generic;
using System.IO;

namespace AnimationEditor.Models
{
    public class WorkspaceFileModel
    {
        public string Filepath { get; set; }

        public CanvasPropertiesModel CanvasProperties { get; set; }

        public AnimationPropertiesModel AnimationProperties { get; set; }

        public List<FrameModel> Frames { get; set; } = new List<FrameModel>();

        public static WorkspaceFileModel LoadWorkspaceFile(string filepath, System.Text.Json.JsonSerializerOptions options)
        {
            return System.Text.Json.JsonSerializer.Deserialize<WorkspaceFileModel>(File.ReadAllText(filepath), options);
        }

        public void SaveWorkspaceFile(string filepath, System.Text.Json.JsonSerializerOptions options)
        {
            Filepath = filepath;

            File.WriteAllText(filepath, System.Text.Json.JsonSerializer.Serialize(this, options));
        }

        public void SyncToViewModel(WorkspaceViewModel workspaceViewModel)
        {
            Filepath = workspaceViewModel.Filepath;

            Frames.Clear();

            //Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in workspaceViewModel.AnimationTimelineViewModel.Frames)
            {
                Frames.Add(new FrameModel(item.StrokeCollection));
            }

            //TODO: These should eventually be coming from the workspace VM
            CanvasProperties = new CanvasPropertiesModel();
            AnimationProperties = new AnimationPropertiesModel();
        }
    }
}
