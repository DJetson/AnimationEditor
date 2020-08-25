using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace AnimationEditor.Models
{
    [Serializable]
    public class WorkspaceFileModel : IWorkspaceFileModel
    {
        private string _Filepath;
        public string Filepath => _Filepath;

        private ICanvasPropertiesModel _CanvasPropertiesModel;
        public ICanvasPropertiesModel CanvasProperties => _CanvasPropertiesModel;

        private IAnimationPropertiesModel _AnimationPropertiesModel;
        public IAnimationPropertiesModel AnimationProperties => _AnimationPropertiesModel;

        private List<IFrameModel> _Frames = new List<IFrameModel>();
        public List<IFrameModel> Frames => _Frames;

        public static IWorkspaceFileModel LoadWorkspaceFile(string filepath)
        {
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                var formatter = new BinaryFormatter();
                var newModel = (WorkspaceFileModel)formatter.Deserialize(stream);

                return newModel;
            }
        }

        public void SaveWorkspaceFile(string filepath)
        {
            _Filepath = filepath;

            using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }


        public static void SaveWorkspaceFile(WorkspaceFileModel model, string filepath)
        {
            model._Filepath = filepath;

            using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, model);
            }
        }

        public void SyncToViewModel(WorkspaceViewModel workspaceViewModel)
        {
            _Filepath = workspaceViewModel.Filepath;

            _Frames.Clear();

            //Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in workspaceViewModel.AnimationTimelineViewModel.Frames)
            {
                _Frames.Add(new FrameModel(item.StrokeCollection));
            }

            //TODO: These should eventually be coming from the workspace VM
            _CanvasPropertiesModel = new CanvasPropertiesModel();
            _AnimationPropertiesModel = new AnimationPropertiesModel();
        }
    }
}
