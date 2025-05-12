using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Feedback;

namespace UXVirtual.XRI.Haptics
{
    /// <summary>
    /// Provides a method to manually trigger haptics on an IXRInteractor
    /// using the settings defined in a SimpleHapticFeedback component on the same GameObject.
    /// Designed to work with XR Interaction Toolkit v3.
    /// Place this script on a central object, like the XR Rig/Origin.
    /// </summary>
    /// 
    [RequireComponent(typeof(XROrigin))] // Requires the XROrigin component
    public class ManualHapticsManager : MonoBehaviour
    {
        /// <summary>
        /// Enum to specify which haptic setting to use from SimpleHapticFeedback.
        /// </summary>
        public enum HapticType
        {
            SelectEnter,
            SelectExit,
            HoverEnter,
            HoverExit,
        }

        [Tooltip("Optional debug mode for logging. If enabled, additional debug information will be printed to the console.")]
        [SerializeField] private bool debugMode = false; // Optional debug mode for logging

        /// <summary>
        /// Manually triggers haptics on the specified interactor using pre-configured settings.
        /// </summary>
        /// <param name="interactor">The interactor (e.g., hand controller, poke interactor) that should vibrate.</param>
        /// <param name="hapticType">Which interaction event's haptic settings to use (SelectEnter or SelectExit).</param>
        public void TriggerHapticOnInteractor(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor interactor, HapticType hapticType)
        {
            if (interactor == null)
            {
                Debug.LogError("ManualHapticsTrigger: Provided interactor is null.", this);
                return;
            }

            // --- 1. Find the GameObject of the Interactor ---
            // IXRInteractor is an interface. We need the MonoBehaviour implementing it to get the GameObject.
            if (!(interactor is MonoBehaviour interactorMonoBehaviour))
            {
                Debug.LogWarning($"ManualHapticsTrigger: Interactor '{interactor}' is not a MonoBehaviour, cannot find associated GameObject.", this);
                return;
            }
            GameObject interactorGO = interactorMonoBehaviour.gameObject;

            // --- 2. Find the SimpleHapticFeedback component ---
            SimpleHapticFeedback hapticFeedback = interactorGO.GetComponent<SimpleHapticFeedback>();
            if (hapticFeedback == null)
            {
                return;
            }

            // --- 3. Determine which HapticImpulse settings to use ---
            HapticImpulseData impulse;
            bool playHaptics = false;

            switch (hapticType)
            {
                case HapticType.SelectEnter:
                    if (hapticFeedback.playSelectEntered)
                    {
                        impulse = hapticFeedback.selectEnteredData;
                        playHaptics = true;
                    }
                    else
                    {
                        impulse = default; // Assign default to satisfy compiler
                    }
                    break;

                case HapticType.SelectExit:
                    if (hapticFeedback.playSelectExited)
                    {
                        impulse = hapticFeedback.selectExitedData;
                        playHaptics = true;
                    }
                    else
                    {
                        impulse = default; // Assign default to satisfy compiler
                    }
                    break;

                case HapticType.HoverEnter:
                    if (hapticFeedback.playHoverEntered)
                    {
                        impulse = hapticFeedback.hoverEnteredData;
                        playHaptics = true;
                    }
                    else
                    {
                        impulse = default; // Assign default to satisfy compiler
                    }
                    break;

                case HapticType.HoverExit:
                    if (hapticFeedback.playHoverExited)
                    {
                        impulse = hapticFeedback.hoverExitedData;
                        playHaptics = true;
                    }
                    else
                    {
                        impulse = default; // Assign default to satisfy compiler
                    }
                    break;

                default:
                    Debug.LogWarning($"ManualHapticsTrigger: Unsupported HapticType '{hapticType}'.", this);
                    return; // Or handle default case if needed
            }

            // --- 4. Check if haptics should play for the selected type ---
            if (!playHaptics)
            {
                // Haptics are configured off for this event type in SimpleHapticFeedback
                if (debugMode)
                    Debug.Log($"ManualHapticsTrigger: Haptics for '{hapticType}' are disabled in SimpleHapticFeedback on '{interactorGO.name}'.", interactorGO);
                return;
            }

            // --- 5. Trigger the Haptics via the haptic impulse player ---
            if (impulse.amplitude > 0 && impulse.duration > 0)
            {
                if (debugMode)
                    Debug.Log($"ManualHapticsTrigger: Sending haptic impulse (Amp: {impulse.amplitude}, Dur: {impulse.duration}) to '{interactorGO.name}' for event '{hapticType}'.", this);
                hapticFeedback.hapticImpulsePlayer.SendHapticImpulse(impulse.amplitude, impulse.duration);
            }
            // else: Configured impulse has zero amplitude or duration, do nothing.
        }
    }
}