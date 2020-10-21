using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Windows.Controls;

namespace AnimationEditorCore.ViewModels
{
    [TestClass]
    public class WorkspaceManagerViewModelTest
    {
        [TestMethod]
        public void CreateNewWorkspace_CreatesWorkspaceWithUniqueName()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var displayNames = workspaceManager.Workspaces.Select(w => w.DisplayName).ToArray();
            var workspace = workspaceManager.CreateNewWorkspace(false);
            CollectionAssert.DoesNotContain(displayNames, workspace.DisplayName);
        }

        [TestMethod]
        public void AddNewWorkspace()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var workspaces = workspaceManager.Workspaces.ToArray();
            var workspace = new WorkspaceViewModel();

            workspaceManager.AddWorkspace(workspace);
            Assert.AreEqual(workspaces.Length + 1, workspaceManager.Workspaces.Count());

            CollectionAssert.IsSubsetOf(workspaces, workspaceManager.Workspaces);
        }

        [TestMethod]
        public void AddExistingWorkspace_DoesNotChangeWorkspaces()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            var workspaces = workspaceManager.Workspaces.ToArray();
            var workspace = workspaces.First();

            workspaceManager.AddWorkspace(workspace);
            Assert.AreEqual(workspaces.Length, workspaceManager.Workspaces.Count());

            CollectionAssert.AreEqual(workspaces, workspaceManager.Workspaces);
        }

        [TestMethod]
        public void AddWorkspace_FollowedByRemoveWorkspace_ReturnsWorkspacesInManagerToPreviousState()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var workspaces = workspaceManager.Workspaces.ToArray();

            var workspace = new WorkspaceViewModel();
            workspaceManager.AddWorkspace(workspace);

            workspaceManager.RemoveWorkspace(workspace);
            Assert.AreEqual(workspaces.Length, workspaceManager.Workspaces.Count());

            CollectionAssert.AreEqual(workspaces, workspaceManager.Workspaces);
        }

        [DeploymentItem("DeploymentItems/dot.anws")]
        [TestMethod]
        public void Open_SelectsNewlyOpenedWorkspace()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            workspaceManager.CreateNewWorkspace(false);
            Assert.AreEqual("Untitled", workspaceManager.SelectedWorkspace.DisplayName, "initial workspace display name");

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);
            Assert.AreEqual("dot", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");
        }

        [DeploymentItem("DeploymentItems/dot.anws")]
        [TestMethod]
        public void Open_SetsActiveLayer()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);

            Assert.IsNotNull(workspaceManager.SelectedWorkspace.TimelineViewModel.Layers.ActiveLayer);
        }

        [DeploymentItem("DeploymentItems/dot.anws")]
        [TestMethod]
        public void Open_MoveStroke_AddsUndoRecord()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            new Commands.Workspace.OpenWorkspaceCommand().OpenWorkspaceFile("dot.anws", workspaceManager);
            var workspace = workspaceManager.SelectedWorkspace;

            LayerViewModel activeLayer = workspace.TimelineViewModel.Layers.ActiveLayer;

            var stroke = activeLayer.ConvertToKeyFrame(activeLayer.SelectedFrameIndex).StrokeCollection[0];
            activeLayer.ConvertToKeyFrame(activeLayer.SelectedFrameIndex).SelectedStrokes = activeLayer.ConvertToKeyFrame(activeLayer.SelectedFrameIndex).StrokeCollection;

            stroke.Transform(new System.Windows.Media.TranslateTransform(offsetX: 1d, offsetY: 2d).Value, applyToStylusTip: false);

            Assert.AreEqual(2, workspace.WorkspaceHistoryViewModel.HistoricalStates.Count);
        }

        [DeploymentItem("DeploymentItems/dot.anws")]
        [TestMethod]
        public void Open_MoveStroke_Undo_DoesNotCrash()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            new Commands.Workspace.OpenWorkspaceCommand().OpenWorkspaceFile("dot.anws", workspaceManager);
            var workspace = workspaceManager.SelectedWorkspace;
            var activeLayer = workspace.TimelineViewModel.Layers.ActiveLayer;
            var stroke = activeLayer.ConvertToKeyFrame(activeLayer.SelectedFrameIndex).StrokeCollection[0];
            stroke.Transform(new System.Windows.Media.TranslateTransform(offsetX: 1d, offsetY: 2d).Value, applyToStylusTip: false);
            var historyItem = workspace.WorkspaceHistoryViewModel.HistoricalStates.First();
            //Assert.IsTrue(workspace.WorkspaceHistoryViewModel.UndoToState(historyItem));

            WorkspaceHistoryViewModel.UndoToState(historyItem);

            // 😞 To reproduce this issue, I will have to replay some changes out of order.
            // The order I (re)fixed the issues resulted in *this* issue being fixed implicitly.
            // In other words, I'm not sure what Assert method to use here!
            //
            // Right now this test is written in such a way that *any* exception thrown will cause it to fail.
            // The fact that no exceptions are thrown means it passes.
            // I usually handle this by wrapping the call which results in an exception by itself in a try
            // with the associated catch simply containing Assert.Fail.
            // That is technically unnecessary as, again, the exception itself will cause the test to fail.
            // But it does make explicit which exception arose originally,
            // which gives room for other exceptions to arise, due to presumably different causes,
            // and then we can update the test to catch those exceptions explicitly.
        }
    }
}
