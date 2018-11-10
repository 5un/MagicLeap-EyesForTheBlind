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

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Utility class to help visual the status of the PCF
    /// </summary>
    public class PCFStatusText : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("Text to display status")]
        private TextMesh _statusText;

        [SerializeField, Tooltip("Text to display name")]
        private TextMesh _nameText;

        private MLPCF _pcf;
        #endregion

        #region Public Properties
        public MLPCF PCF
        {
            set
            {
                _pcf = value;
                _pcf.OnLost += HandleLost;
                _pcf.OnRegain += HandleRegain;
                _pcf.OnUpdate += HandleUpdate;
                _statusText.text = "Good";
            }
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            if (_statusText == null)
            {
                Debug.LogError("Error: PCFStatusText._statusText is not set, disabling script");
                enabled = false;
                return;
            }

            if (_nameText == null)
            {
                Debug.LogError("Error: PCFStatusText._nameText is not set, disabling script");
                enabled = false;
                return;
            }

            _nameText.text = gameObject.name;
        }

        void OnDestroy()
        {
            if (_pcf != null)
            {
                _pcf.OnLost -= HandleLost;
                _pcf.OnRegain -= HandleRegain;
                _pcf.OnUpdate -= HandleUpdate;
            }
        }
        #endregion

        #region Event Handlers
        void HandleLost()
        {
            _statusText.text = "<color=red>Lost</color>";
        }

        void HandleRegain()
        {
            _statusText.text = "<color=cyan>Regained</color>";
        }

        void HandleUpdate()
        {
            _statusText.text = "<color=yellow>Updated</color>";
        }
        #endregion
    }
}
