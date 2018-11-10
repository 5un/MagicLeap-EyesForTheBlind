// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// This a utility class to help debug MLPersistentPoint. This class listens to
    /// events from MLPersistentPoint and displays them.
    /// </summary>
    public class ContentBindingVisualizer : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The MLPersistentPoint to listen to")]
        MLPersistentPoint _pointBehavior;

        [SerializeField, Tooltip("Text to display name")]
        TextMesh _nameText;

        Renderer[] _renderers;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Validate parameters, initialize renderers, and listen to events
        /// </summary>
        void Awake()
        {
            if (_pointBehavior == null)
            {
                Debug.LogError("Error: ContentBindingVisualizer._pointBehavior is not set, disabling script");
                enabled = false;
                return;
            }

            if (_nameText == null)
            {
                Debug.LogError("Error: ContentBindingStatusText._nameText is not set, disabling script");
                enabled = false;
                return;
            }

            _renderers = GetComponentsInChildren<Renderer>();
            EnableRenderers(false);

            _pointBehavior.OnComplete += HandleComplete;
        }

        /// <summary>
        /// Remove event listeners
        /// </summary>
        void OnDestroy()
        {
            if (_pointBehavior != null)
            {
                _pointBehavior.OnComplete -= HandleComplete;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Enable/Disable Renderers
        /// </summary>
        /// <param name="enable">Toggle value</param>
        void EnableRenderers(bool enable)
        {
            foreach(Renderer r in _renderers)
            {
                r.enabled = enable;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Listener for OnComplete.
        /// </summary>
        /// <param name="success">True if MLPersistentPoint was loaded or restored successfully, otherwise false.</param>
        void HandleComplete(bool success)
        {
            if (success)
            {
                _nameText.transform.position = transform.position + new Vector3(0, 0.25f, 0);
                _nameText.text = _pointBehavior.Binding.ObjectId;

                EnableRenderers(true);
            }
            else
            {
                Debug.LogErrorFormat("Error: ContentBindingStatusText failed to load/restore {0}.", _pointBehavior.Binding.ObjectId);
                Destroy(gameObject);
            }
        }
        #endregion // Event Handlers
    }
}
