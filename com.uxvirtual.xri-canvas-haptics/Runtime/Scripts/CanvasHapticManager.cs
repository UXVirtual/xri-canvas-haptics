using UnityEngine;
using UnityEngine.EventSystems; // Required for EventSystem
using UnityEngine.UI;           // Required for UnityEngine.UI.Selectable, ScrollRect
using System.Collections.Generic; // Required for List
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UXVirtual.XRI.Haptics
{

    /// <summary>
    /// Sets up haptic feedback for Unity UI elements (Selectables like Buttons, Sliders, Toggles,
    /// and ScrollRects) found recursively under a specified buttonContainer GameObject.
    /// This script is intended for use with the new Unity Input System and XR Interaction Toolkit.
    /// It adds and initializes a UIElementHapticHelper component on each element's GameObject, which
    /// then handles triggering haptics via the ManualHapticsManager on pointer events.
    /// </summary>
    public class CanvasHapticManager : MonoBehaviour
    {
        [Tooltip("Automatically initialize haptic helpers on Start. If disabled, you can call InitializeHelpers() manually to set up the haptic helpers.")]
        [SerializeField] private bool autoInitializeHelpers = true;

        [Tooltip("Reference to the GameObject that acts as a container for the UI elements. All child UI.Selectable and UI.ScrollRect components will be targeted recursively.")]
        [SerializeField] private GameObject buttonContainer;

        [Tooltip("Reference to the ManualHapticsManager script on your XR Rig or Manager object. UIElementHapticHelper will use this to trigger haptics.")]
        [SerializeField] private ManualHapticsManager manualHapticsManager;

        [Tooltip("Optional debug mode for logging. If enabled, additional debug information will be printed to the console.")]
        [SerializeField] private bool debugMode = false; // Optional debug mode for logging

        private XRUIInputModule _inputModule;
        // List to store all found Selectable components (Buttons, Sliders, Toggles, etc.)
        private List<Selectable> _uiSelectables = new List<Selectable>();
        // List to store all found ScrollRect components
        private List<ScrollRect> _uiScrollRects = new List<ScrollRect>();

        void Start()
        {
            // --- Find Dependencies ---
            if (manualHapticsManager == null)
            {
                // Try to find the ManualHapticsManager in the scene if not assigned
                manualHapticsManager = FindFirstObjectByType<ManualHapticsManager>();
                if (manualHapticsManager == null)
                {
                    Debug.LogError("CanvasHapticButtonManager: ManualHapticsManager not assigned and not found in scene! Haptics will not work.", this);
                    enabled = false; // Disable this script if the manager is missing
                    return;
                }
            }

            if (buttonContainer == null)
            {
                Debug.LogError("CanvasHapticButtonManager: Button Container GameObject reference not set in the Inspector! Cannot find UI elements.", this);
                enabled = false; // Disable if the container isn't set
                return;
            }

            // Find the active EventSystem
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem != null)
            {
                // Get the current XRUIInputModule from the EventSystem
                _inputModule = eventSystem.GetComponent<XRUIInputModule>();
                if (_inputModule == null)
                {
                    Debug.LogError("CanvasHapticButtonManager: XRUIInputModule not found as the current input module on the active EventSystem. UI Interactor detection for haptics will likely fail. Ensure an XRUIInputModule is active and configured for the new Input System.", this);
                    enabled = false; // Disable if the required input module is missing
                    return;
                }
            }
            else
            {
                Debug.LogError("CanvasHapticButtonManager: No EventSystem found in scene! UI interactions and haptics will not work.", this);
                enabled = false;
                return;
            }

            if (!autoInitializeHelpers)
            {
                return;
            }

            InitializeHelpers();
        }

        /// <summary>
        /// Initializes haptic helpers for all UI Selectable and ScrollRect components
        /// found recursively under the specified buttonContainer GameObject. 
        /// You can also manually add UIElementHapticHelper to each UI element if you
        /// want specific control over which elements have haptic feedback.
        /// </summary>
        public void InitializeHelpers()
        {
            // --- Find UI Selectables and Set Up Helpers ---
            buttonContainer.GetComponentsInChildren(true, _uiSelectables);

            if (_uiSelectables.Count > 0)
            {
                if (debugMode)
                    Debug.Log($"CanvasHapticButtonManager: Found {_uiSelectables.Count} UI Selectable elements. Setting up haptic helpers...", this);
                foreach (Selectable selectableElement in _uiSelectables)
                {
                    if (selectableElement == null) continue;
                    AddAndInitializeHelper(selectableElement.gameObject, selectableElement.GetType().Name);
                }
            }
            else
            {
                Debug.LogWarning($"CanvasHapticButtonManager: No Unity UI Selectable components (Buttons, Sliders, etc.) found under the '{buttonContainer.name}' GameObject.", this);
            }

            // --- Find UI ScrollRects and Set Up Helpers ---
            buttonContainer.GetComponentsInChildren(true, _uiScrollRects);

            if (_uiScrollRects.Count > 0)
            {
                if (debugMode)
                    Debug.Log($"CanvasHapticButtonManager: Found {_uiScrollRects.Count} UI ScrollRect elements. Setting up haptic helpers...", this);
                foreach (ScrollRect scrollRectElement in _uiScrollRects)
                {
                    if (scrollRectElement == null) continue;
                    // The AddAndInitializeKeyHapticHelper method will check if KeyHapticHelper already exists
                    // (e.g. if a child of ScrollRect was a Selectable and already processed).
                    // It's generally safe to call for the ScrollRect's GameObject itself.
                    AddAndInitializeHelper(scrollRectElement.gameObject, scrollRectElement.GetType().Name);
                }
            }
            else
            {
                Debug.LogWarning($"CanvasHapticButtonManager: No Unity UI ScrollRect components found under the '{buttonContainer.name}' GameObject.", this);
            }

            if (_uiSelectables.Count == 0 && _uiScrollRects.Count == 0)
            {
                Debug.LogWarning($"CanvasHapticButtonManager: No Unity UI Selectable or ScrollRect components found under the '{buttonContainer.name}' GameObject. No haptic helpers will be set up.", this);
                return; // No elements of either type to process
            }
        }

        /// <summary>
        /// Adds a UIElementHapticHelper component to the target GameObject if one doesn't already exist,
        /// and then initializes it.
        /// </summary>
        /// <param name="targetObject">The GameObject to add the helper to.</param>
        /// <param name="elementType">The string name of the UI element type for logging purposes.</param>
        private void AddAndInitializeHelper(GameObject targetObject, string elementType)
        {
            if (targetObject == null)
            {
                Debug.LogWarning("CanvasHapticButtonManager: Attempted to add KeyHapticHelper to a null GameObject.", this);
                return;
            }

            UIElementHapticHelper helper = targetObject.GetComponent<UIElementHapticHelper>();
            if (helper == null)
            {
                helper = targetObject.AddComponent<UIElementHapticHelper>();
                if (debugMode)
                    Debug.Log($"CanvasHapticButtonManager: Added UIElementHapticHelper to {targetObject.name} (Type: {elementType})", targetObject);
            }
            // If helper is not null here, it either existed or was just added.

            if (helper != null)
            {
                helper.Initialize(manualHapticsManager, _inputModule, debugMode);
            }
            else
            {
                // This case should ideally not be reached if AddComponent was successful.
                // It might indicate an issue with adding the component, though AddComponent usually throws an error itself if it fails.
                Debug.LogError($"CanvasHapticButtonManager: Failed to add or find UIElementHapticHelper on {targetObject.name} (Type: {elementType}).", targetObject);
            }
        }

        // Optional: Cleanup if needed, though UIElementHapticHelper instances should handle their own event unsubscriptions if any.
        // private void OnDestroy() { }
    }
}