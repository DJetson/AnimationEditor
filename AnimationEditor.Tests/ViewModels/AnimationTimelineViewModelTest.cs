using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

        [TestMethod]
        public void AddBlankFrame_MakesLayerInNewFrameVisible()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;

            timeline.AddBlankFrame.Execute(FrameNavigation.Next);

            Assert.IsTrue(timeline.SelectedFrame.ActiveLayer.IsVisible);
        }

        [TestMethod]
        public void AddBlankFrame_SetsLayerName()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;

            timeline.AddBlankFrame.Execute(FrameNavigation.Next);

            Assert.AreEqual("Layer 0", timeline.SelectedFrame.ActiveLayer.DisplayName);
        }

        [TestMethod]
        public void AddBlankFrame_Previous_UpdatesAllFrameOrders()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;

            timeline.AddBlankFrame.Execute(FrameNavigation.Previous);

            CollectionAssert.AreEqual(new[] { 0, 1 }, timeline.Frames.Select(f => f.Order).ToArray());
        }

        [TestMethod]
        public void RemoveFrame_UpdatesFrameOrdinals()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var timeline = workspaceManager.SelectedWorkspace.AnimationTimelineViewModel;
            timeline.AddBlankFrame.Execute(FrameNavigation.Previous);
            timeline.AddBlankFrame.Execute(FrameNavigation.Next);

            Assert.IsTrue(timeline.DeleteCurrentFrame.CanExecute(null));
            timeline.DeleteCurrentFrame.Execute(null);

            CollectionAssert.AreEqual(new[] { 0, 1 }, timeline.Frames.Select(f => f.Order).ToArray());
        }
    }
}
