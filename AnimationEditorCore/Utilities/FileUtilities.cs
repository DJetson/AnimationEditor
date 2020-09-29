using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.Utilities
{
    public class FileUtilities
    {
        public static string GetUniquePlaceholderName(IHasWorkspaceCollection workspaces)
        {
            string tempName = "Untitled";
            int duplicateCount = 0;
            string CopyString = "-Copy";
            string result = tempName;
            while (workspaces.Workspaces.Select(e => e.Filename).Contains(result))
            {
                duplicateCount++;
                result = $"{tempName}{CopyString} {duplicateCount}";
            }

            return result;
        }

        public static string GetUniqueNameForCollection(List<string> items, string desiredName)
        {
            string tempName = desiredName;
            int duplicateCount = 0;
            string CopyString = "-Copy";
            string result = tempName;
            while (items.Contains(result))
            {
                duplicateCount++;
                result = $"{tempName}{CopyString} {duplicateCount}";
            }

            return result;
        }
    }
}
