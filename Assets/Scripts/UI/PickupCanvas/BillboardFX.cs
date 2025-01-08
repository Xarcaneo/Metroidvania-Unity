/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.UI.WorldSpace
{
    using Opsive.UltimateInventorySystem.Utility;
    using UnityEngine;

    /// <summary>
    /// Makes an object's transform always face the camera, creating a billboard effect.
    /// Useful for UI elements that should always be visible to the player.
    /// </summary>
    [AddComponentMenu("Ultimate Inventory System/UI/World Space/Billboard FX")]
    public class BillboardFX : MonoBehaviour
    {
        #region Serialized Fields
        /// <summary>
        /// Reference to the camera transform. If not set, will use Camera.main.
        /// </summary>
        [Tooltip("The camera transform. Will use Camera.main if not set.")]
        [SerializeField] protected Transform m_CamTransform;
        #endregion

        #region Private Fields
        /// <summary>
        /// The original local rotation of the transform.
        /// </summary>
        protected Quaternion m_OriginalRotation;

        /// <summary>
        /// Flag to track if component is properly initialized.
        /// </summary>
        private bool m_IsInitialized;
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Validates component settings in the editor.
        /// </summary>
        private void OnValidate()
        {
            if (OnValidateUtility.IsPrefab(this)) { return; }
            SetupCamera();
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void Start()
        {
            SetupCamera();
            if (m_CamTransform == null)
            {
                Debug.LogWarning($"[BillboardFX] No camera found for {gameObject.name}. Billboard effect will not work.");
                enabled = false;
                return;
            }

            m_OriginalRotation = transform.localRotation;
            m_IsInitialized = true;
        }

        /// <summary>
        /// Updates the rotation to face the camera.
        /// Called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (!m_IsInitialized) return;

            if (m_CamTransform == null)
            {
                SetupCamera();
                if (m_CamTransform == null)
                {
                    Debug.LogWarning($"[BillboardFX] Camera reference lost on {gameObject.name}");
                    enabled = false;
                    return;
                }
            }

            transform.rotation = m_CamTransform.rotation * m_OriginalRotation;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Attempts to find and set up the camera reference.
        /// </summary>
        private void SetupCamera()
        {
            if (m_CamTransform != null) return;

            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                m_CamTransform = mainCamera.transform;
            }
        }
        #endregion
    }
}
