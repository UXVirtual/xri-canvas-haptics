using UnityEngine.XR.Management;

namespace UXVirtual.XRI.Haptics
{
    public class OpenXRRuntimeChecker
    {
        /// <summary>
        /// Checks if any XR loader has been initialized and is active.
        /// </summary>
        /// <returns>True if an XR loader is active, false otherwise.</returns>
        public static bool IsXRLoaderActive()
        {
            // Checks the singleton instance for the manager and if a loader is active.
            return XRGeneralSettings.Instance != null &&
                   XRGeneralSettings.Instance.Manager != null &&
                   XRGeneralSettings.Instance.Manager.activeLoader != null;
        }

        /// <summary>
        /// Specifically checks if the active loader is the OpenXR loader.
        /// Assumes an XR loader is already active.
        /// </summary>
        /// <returns>True if the active loader is OpenXR, false otherwise.</returns>
        public static bool IsOpenXRLoader()
        {
            if (!IsXRLoaderActive())
            {
                return false;
            }

            // Get the class name of the active loader and check if it's the OpenXR one.
            return XRGeneralSettings.Instance.Manager.activeLoader.GetType().Name.Contains("OpenXR");
        }
    }
}