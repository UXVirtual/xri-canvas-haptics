using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UXVirtual.XRI.Haptics
{
    /// <summary>
    /// Helper component to be placed on each XRKeyboardKey GameObject.
    /// It detects XR UI interactor pointer events (Enter, Exit, Down, Up, Click)
    /// and triggers corresponding haptics via the ManualHapticsManager.
    /// Haptics for SelectEnter are triggered on PointerDown.
    /// Haptics for SelectExit are triggered on PointerUp.
    /// Haptics for HoverEnter/HoverExit can be optionally triggered in OnPointerEnter/Exit.
    /// </summary>
    [RequireComponent(typeof(IPointerEnterHandler), typeof(IPointerExitHandler), typeof(IPointerDownHandler))] // Ensure it's on a button
    public class UIElementHapticHelper : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler, // Add interface
        IPointerUpHandler   // Add interface
    {
        // Keep references assignable via Initialize or Inspector (if preferred)
        [SerializeField] private ManualHapticsManager _hapticsManager;
        [SerializeField] private XRUIInputModule _inputModule;
        [SerializeField] private bool _debugMode = false; // Optional debug mode for logging

        // Store the last interactor known to be hovering over this key
        private IXRInteractor _lastHoveringInteractor;

        // --- Initialization ---

        /// <summary>
        /// Initializes the helper with necessary references.
        /// Called by the setup script (e.g., HapticButtonFeedback).
        /// </summary>
        public void Initialize(ManualHapticsManager hapticsManager, XRUIInputModule inputModule, bool debugMode)
        {
            _hapticsManager = hapticsManager;
            _inputModule = inputModule;
            _debugMode = debugMode;
            ValidateReferences();
        }

        /// <summary>
        /// Called by Unity when the script instance is being loaded.
        /// Also validates references in case they were assigned via Inspector.
        /// </summary>
        private void Awake()
        {
            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (_inputModule == null)
                // Attempt to find if not assigned (less ideal than Initialize)
                _inputModule = FindFirstObjectByType<XRUIInputModule>();
            if (_inputModule == null)
                Debug.LogError($"KeyHapticHelper on {gameObject.name}: XRUIInputModule reference is missing and couldn't be found!", this);
        }


        // --- Pointer Event Handlers ---

        /// <summary>
        /// Called by the EventSystem when a pointer enters the bounds of this UI element.
        /// Stores the hovering interactor and optionally triggers HoverEnter haptics.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanProcessEvent()) return;

            IXRInteractor interactor = (IXRInteractor)_inputModule.GetInteractor(eventData.pointerId);
            if (interactor != null)
            {
                _lastHoveringInteractor = interactor;
                if (_debugMode)
                    Debug.Log($"Interactor {interactor.transform.name} entered key {gameObject.name}", this);

                // Optional: Trigger HoverEnter haptics (requires HoverEnter type in ManualHapticsManager)
                _hapticsManager.TriggerHapticOnInteractor(interactor, ManualHapticsManager.HapticType.HoverEnter);
            }
            else
            {
                // Disable warning if OpenXR disabled to avoid spam in logs
                if(!OpenXRRuntimeChecker.IsXRLoaderActive())
                    return;
                Debug.LogWarning($"OnPointerEnter on {gameObject.name}: Could not get IXRInteractor for pointerId {eventData.pointerId}", this);
            }
        }

        /// <summary>
        /// Called by the EventSystem when a pointer exits the bounds of this UI element.
        /// Clears the stored interactor if it matches and optionally triggers HoverExit haptics.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!CanProcessEvent()) return;

            IXRInteractor interactor = (IXRInteractor)_inputModule.GetInteractor(eventData.pointerId);
            // Check if the exiting pointer is the one we were tracking
            if (interactor != null && interactor == _lastHoveringInteractor)
            {
                if (_debugMode)
                    Debug.Log($"Interactor {interactor.transform.name} exited key {gameObject.name}", this);
                _lastHoveringInteractor = null; // Clear the reference

                // Optional: Trigger HoverExit haptics (requires HoverExit type in ManualHapticsManager)
                _hapticsManager.TriggerHapticOnInteractor(interactor, ManualHapticsManager.HapticType.HoverExit);
            }
            // Else: Some other pointer exited, or we couldn't identify it, or it wasn't the one we stored.
        }

        /// <summary>
        /// Called by the EventSystem when a pointer down event occurs on this UI element.
        /// Triggers SelectEnter haptics for the interacting pointer.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanProcessEvent()) return;

            IXRInteractor interactor = (IXRInteractor)_inputModule.GetInteractor(eventData.pointerId);
            if (interactor != null)
            {
                if (_debugMode)
                    Debug.Log($"Pointer Down on key {gameObject.name} by {interactor.transform.name}. Triggering SelectEnter haptics.");
                _hapticsManager.TriggerHapticOnInteractor(interactor, ManualHapticsManager.HapticType.SelectEnter);
            }
            else
            {
                // Disable warning if OpenXR disabled to avoid spam in logs
                if(!OpenXRRuntimeChecker.IsXRLoaderActive())
                    return;
                Debug.LogWarning($"OnPointerDown on {gameObject.name}: Could not get IXRInteractor for pointerId {eventData.pointerId} to trigger haptics.", this);
            }
        }

        /// <summary>
        /// Called by the EventSystem when a pointer up event occurs on this UI element
        /// *if the pointer down also occurred on this element*.
        /// Triggers SelectExit haptics for the interacting pointer.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!CanProcessEvent()) return;

            IXRInteractor interactor = (IXRInteractor)_inputModule.GetInteractor(eventData.pointerId);
            if (interactor != null)
            {
                if (_debugMode)
                    Debug.Log($"Pointer Up on key {gameObject.name} by {interactor.transform.name}. Triggering SelectExit haptics.");
                _hapticsManager.TriggerHapticOnInteractor(interactor, ManualHapticsManager.HapticType.SelectExit);
            }
            else
            {
                // Disable warning if OpenXR disabled to avoid spam in logs
                if(!OpenXRRuntimeChecker.IsXRLoaderActive())
                    return;
                Debug.LogWarning($"OnPointerUp on {gameObject.name}: Could not get IXRInteractor for pointerId {eventData.pointerId} to trigger haptics.", this);
            }
        }

        // --- Helper Methods ---

        /// <summary>
        /// Checks if the necessary references are available to process pointer events.
        /// </summary>
        private bool CanProcessEvent()
        {
            if (_hapticsManager == null)
            {
                // Log error only once maybe? Or rely on Awake/Initialize logs.
                // Debug.LogError($"Cannot process pointer event on {gameObject.name}: ManualHapticsTrigger is missing.", this);
                return false;
            }
            if (_inputModule == null)
            {
                // Debug.LogError($"Cannot process pointer event on {gameObject.name}: XRUIInputModule is missing.", this);
                return false;
            }
            return true;
        }
    }
}