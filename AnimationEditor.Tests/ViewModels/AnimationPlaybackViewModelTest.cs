using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.ViewModels
{
    [TestClass]
    public class AnimationPlaybackViewModelTest
    {
        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void StartPlayback()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            var playback = timeline.AnimationPlaybackViewModel;

            //Assert.IsFalse(playback.IsPlaybackActive, "Incorrect playback state");

            var startPlaybackCommand = new Commands.Timeline.Playback.TogglePlaybackCommand();

            Assert.IsTrue(startPlaybackCommand.CanExecute(timeline));

            startPlaybackCommand.Execute(timeline);

            Assert.IsTrue(playback.IsPlaybackActive);
        }

        [TestMethod]
        [DeploymentItem("DeploymentItems/TestFile.anws")]
        public void StopPlayback()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");

            var workspace = workspaceManager.SelectedWorkspace;
            var timeline = workspace.TimelineViewModel;
            var playback = timeline.AnimationPlaybackViewModel;

            //Assert.IsFalse(playback.IsPlaybackActive, "Incorrect playback state");

            var StopPlaybackCommand = new Commands.Timeline.Playback.StopPlaybackCommand();
            var startPlaybackCommand = new Commands.Timeline.Playback.TogglePlaybackCommand();

            Assert.IsTrue(startPlaybackCommand.CanExecute(timeline));

            startPlaybackCommand.Execute(timeline);

            Assert.IsTrue(playback.IsPlaybackActive);

            Assert.IsTrue(StopPlaybackCommand.CanExecute(timeline));

            StopPlaybackCommand.Execute(timeline);

            Assert.AreEqual(1, timeline.SelectedFrameIndex);
            //Assert.IsTrue(playback.IsPlaybackActive);
        }
    }
}
