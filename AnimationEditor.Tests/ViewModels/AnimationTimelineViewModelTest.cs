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
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            Assert.AreEqual(0, timeline.SelectedFrameIndex, "original: selected frame index");
            Assert.IsTrue(timeline.AnimationPlaybackViewModel.IsPlaybackActive);

            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "after: selected frame index");
        }

        [TestMethod]
        public void AddBlankFrame_AddsExactlyOneLayer()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);

            Assert.AreEqual(1, timeline.SelectedFrames.Count);
        }

        [TestMethod]
        public void AddBlankFrame_ActivatesLayerInNewFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);

            Assert.IsNotNull(timeline.ActiveLayer);
        }

        [TestMethod]
        public void AddBlankFrame_MakesLayerInNewFrameVisible()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);

            Assert.IsTrue(timeline.ActiveLayer.IsVisible);
        }

        [TestMethod]
        public void AddBlankFrame_SetsLayerName()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);

            Assert.AreEqual("Layer 0", timeline.ActiveLayer.DisplayName);
        }

        [TestMethod]
        public void AddBlankFrame_Previous_UpdatesAllFrameOrders()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;

            timeline.AddBlankFrameToTimeline(FrameNavigation.Previous);

            foreach (var layer in timeline.Layers)
            {
                CollectionAssert.AreEqual(new[] { 0, 1 }, layer.Frames.Select(f => f.Order).ToArray());
            }
        }

        [TestMethod]
        public void RemoveFrame_UpdatesFrameOrdinals()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.TimelineViewModel;
            timeline.AddBlankFrameToTimeline(FrameNavigation.Previous);
            timeline.AddBlankFrameToTimeline(FrameNavigation.Next);

            Assert.IsTrue(timeline.AnimationPlaybackViewModel.IsPlaybackActive);
            timeline.DeleteCurrentFrame();

            foreach (var layer in timeline.Layers)
            {
                CollectionAssert.AreEqual(new[] { 0, 1 }, layer.Frames.Select(f => f.Order).ToArray());
            }
        }
    }
}
