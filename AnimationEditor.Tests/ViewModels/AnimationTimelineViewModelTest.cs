using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AnimationEditorCore.ViewModels
{
    [TestClass]
    public class AnimationTimelineViewModelTest
    {
        [TestMethod]
        public void AddBlankFrame_SelectsNewFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            Assert.AreEqual(0, timeline.SelectedFrameIndex, "original: selected frame index");
            //Assert.IsTrue(timeline.AnimationPlaybackViewModel.IsPlaybackActive);

            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "after: selected frame index");
        }

        [TestMethod]
        public void AddBlankFrame_AddsExactlyOneLayer()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);

            Assert.AreEqual(1, timeline.SelectedFrames.Count);
        }

        [TestMethod]
        public void AddBlankFrame_ActivatesLayerInNewFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);

            Assert.IsNotNull(timeline.Layers.ActiveLayer);
        }

        [TestMethod]
        public void AddBlankFrame_MakesLayerInNewFrameVisible()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);

            Assert.IsTrue(timeline.Layers.ActiveLayer.IsVisible);
        }

        [TestMethod]
        public void AddBlankFrame_SetsLayerName()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);

            Assert.AreEqual("Layer 0", timeline.Layers.ActiveLayer.DisplayName);
        }

        [TestMethod]
        public void AddBlankFrame_Previous_UpdatesAllFrameOrders()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex);

            foreach (var layer in timeline.Layers)
            {
                CollectionAssert.AreEqual(new[] { 0, 1 }, layer.Frames.Select(f => f.Order).ToArray());
            }
        }

        [TestMethod]
        public void RemoveFrame_UpdatesFrameOrdinals()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex);
            timeline.AddBlankKeyFrameToTimeline(timeline.SelectedFrameIndex + 1);

            //Assert.IsTrue(timeline.AnimationPlaybackViewModel.IsPlaybackActive);
            timeline.DeleteCurrentFrame();

            foreach (var layer in timeline.Layers)
            {
                CollectionAssert.AreEqual(new[] { 0, 1 }, layer.Frames.Select(f => f.Order).ToArray());
            }
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/dot.anws")]
        public void MoveSelectedStrokesToNextFrame_UpdatesSelectedFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));
           
            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);
            Assert.AreEqual("dot", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var moveSelectedStrokes = new Commands.Timeline.FrameContent.MoveSelectedContentsToNextFrameCommand();

            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            var selectedFrame = timeline.Layers.ActiveLayer.Frames[timeline.Layers.ActiveLayer.SelectedFrameIndex];
            selectedFrame.SelectedStrokes = selectedFrame.StrokeCollection;
            Assert.IsTrue(moveSelectedStrokes.CanExecute(workspaceManager.SelectedWorkspace.TimelineViewModel));

            moveSelectedStrokes.Execute(timeline);
            Assert.AreEqual(1, timeline.SelectedFrameIndex);
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/dot.anws")]
        public void MoveSelectedStrokesToPreviousFrame_CreatesNewFrameAndUpdatesSelectedFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);
            Assert.AreEqual("dot", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var moveSelectedStrokes = new Commands.Timeline.FrameContent.MoveSelectedContentsToPreviousFrameCommand();

            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            var selectedFrame = timeline.Layers.ActiveLayer.Frames[timeline.Layers.ActiveLayer.SelectedFrameIndex];
            selectedFrame.SelectedStrokes = selectedFrame.StrokeCollection;
            Assert.IsTrue(moveSelectedStrokes.CanExecute(workspaceManager.SelectedWorkspace.TimelineViewModel));

            moveSelectedStrokes.Execute(timeline);
            Assert.AreEqual(0, timeline.SelectedFrameIndex);
        }
    }
}
