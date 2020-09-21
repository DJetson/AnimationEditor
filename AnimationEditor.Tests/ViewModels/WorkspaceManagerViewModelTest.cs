using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AnimationEditor.ViewModels
{
    [TestClass]
    public class WorkspaceManagerViewModelTest
    {
        [TestMethod]
        public void CreateNewWorkspace_CreatesWorkspaceWithUniqueName()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var displayNames = workspaceManager.Workspaces.Select(w => w.DisplayName).ToArray();
            var workspace = workspaceManager.CreateNewWorkspace();
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
            Assert.AreEqual("Untitled*", workspaceManager.SelectedWorkspace.DisplayName, "initial workspace display name");

            var openCommand = new Commands.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);
            Assert.AreEqual("dot", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");
        }

        [DeploymentItem("DeploymentItems/dot.anws")]
        [TestMethod]
        public void Open_SetsActiveLayer()
        {
            var workspaceManager = new WorkspaceManagerViewModel();
            var openCommand = new Commands.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("dot.anws", workspaceManager);

            Assert.IsNotNull(workspaceManager.SelectedWorkspace.AnimationTimelineViewModel.SelectedFrame.ActiveLayer);
        }
    }
}
