namespace Toolbox.Editor
{
    /// <summary>
    /// Utility class to handle Hierarchy Window related data.
    /// </summary>
    internal static class ToolboxHierarchyUtility
    {       
        /// <summary>
        /// Settings provided to handle Hierarchy Window overlay.
        /// </summary>
        private static IToolboxHierarchySettings settings;


        internal static void PerformData()
        {
            if (settings == null)
            {
                return;
            }

            PerformData(settings);
        }

        internal static void PerformData(IToolboxHierarchySettings settings)
        {
            ToolboxHierarchyUtility.settings = settings;

            ToolboxHierarchyAllowed = settings.UseToolboxHierarchy;
        }


        internal static bool ToolboxHierarchyAllowed { get; private set; }   
    }
}