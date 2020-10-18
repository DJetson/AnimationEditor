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
        public void NavigateToFirstLastFrame()
        {
            var workspaceManager = new WorkspaceManagerViewModel();

            var openCommand = new Commands.Workspace.OpenWorkspaceCommand();
            Assert.IsTrue(openCommand.CanExecute(workspaceManager));

            openCommand.OpenWorkspaceFile("TestFile.anws", workspaceManager);
            Assert.AreEqual("TestFile", workspaceManager.SelectedWorkspace.DisplayName, "selected workspace display name");


        }

    }
}
