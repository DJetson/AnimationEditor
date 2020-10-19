using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.ViewModels
{
    [TestClass]
    public class TimelineNavigationTest
    {
        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void NavigateToFirstFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "The TestFile did not have the correct SelectedFrameIndex");

            var navigateToFirstFrameCommand = new Commands.Timeline.Navigation.NavigateToFirstFrameCommand();
            Assert.IsTrue(navigateToFirstFrameCommand.CanExecute(timeline));

            navigateToFirstFrameCommand.Execute(timeline);
            Assert.AreEqual(0, timeline.SelectedFrameIndex, "NavigateToFirstFrameCommand failed to navigate to frame 0");
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void NavigateToLastFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "The TestFile did not have the correct SelectedFrameIndex");

            var navigateToLastFrameCommand = new Commands.Timeline.Navigation.NavigateToLastFrameCommand();
            Assert.IsTrue(navigateToLastFrameCommand.CanExecute(timeline));

            navigateToLastFrameCommand.Execute(timeline);
            Assert.AreEqual(4, timeline.SelectedFrameIndex, "NavigateToLastFrameCommand failed to navigate to the last frame index");
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void NavigateToPreviousFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "The TestFile did not have the correct SelectedFrameIndex");

            var navigateToPreviousFrameCommand = new Commands.Timeline.Navigation.NavigateToPreviousFrameCommand();
            Assert.IsTrue(navigateToPreviousFrameCommand.CanExecute(timeline));

            navigateToPreviousFrameCommand.Execute(timeline);
            Assert.AreEqual(0, timeline.SelectedFrameIndex, "NavigateToPreviousFrameCommand failed to navigate to the Previous frame index");
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void NavigateToNextFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            Assert.AreEqual(1, timeline.SelectedFrameIndex, "The TestFile did not have the correct SelectedFrameIndex");

            var navigateToNextFrameCommand = new Commands.Timeline.Navigation.NavigateToNextFrameCommand();
            Assert.IsTrue(navigateToNextFrameCommand.CanExecute(timeline));

            navigateToNextFrameCommand.Execute(timeline);
            Assert.AreEqual(2, timeline.SelectedFrameIndex, "NavigateToNextFrameCommand failed to navigate to the Next frame index");
        }
    }
}
