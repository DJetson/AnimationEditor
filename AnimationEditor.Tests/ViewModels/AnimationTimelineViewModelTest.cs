using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnimationEditor.ViewModels
{
    [TestClass]
    public class AnimationTimelineViewModelTest
    {
        [TestMethod]
        public void AddBlankFrame_SelectsNewFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;
            Assert.AreEqual(0, timeline.Frames.IndexOf(timeline.SelectedFrame), "original: selected frame index");
            Assert.IsTrue(timeline.AddBlankFrame.CanExecute(FrameNavigation.Next));

            timeline.AddBlankFrame.Execute(FrameNavigation.Next);
            Assert.AreEqual(1, timeline.Frames.IndexOf(timeline.SelectedFrame), "after: selected frame index");
        }

        [TestMethod]
        public void AddBlankFrame_AddsExactlyOneLayer()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;

            timeline.AddBlankFrame.Execute(FrameNavigation.Next);

            Assert.AreEqual(1, timeline.SelectedFrame.Layers.Count);
        }

        [TestMethod]
        public void AddBlankFrame_ActivatesLayerInNewFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;

            timeline.AddBlankFrame.Execute(FrameNavigation.Next);

            Assert.IsNotNull(timeline.SelectedFrame.ActiveLayer);
        }
    }
}
