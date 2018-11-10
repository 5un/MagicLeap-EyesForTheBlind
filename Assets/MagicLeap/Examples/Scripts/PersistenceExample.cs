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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Demonstrates how to persist objects dynamically by interfacing with
    /// the MLPersistence API. This facilitates restoration of existing
    /// and creation of new persistent points.
    /// </summary>
    [RequireComponent(typeof(PrivilegeRequester))]
    public class PersistenceExample : MonoBehaviour
    {
        #region Private Variables
        [SerializeField]
        Text _progressText;

        [SerializeField]
        Text _persistentPointsStatusText;
        const string _persistentPointsStatusTextFormat = "Good Persistent Points: <color=green>{0}</color>\nBad Persistent Points: <color=red>{1}</color>";
        int _persistentPointsSuccess = 0;
        int _persistentPointsFail = 0;

        [SerializeField, Tooltip("Prefab, with MLPersistentPoint, instantiated on demand")]
        GameObject _prefab;

        [SerializeField, Space, Tooltip("ControllerConnectionHandler reference.")]
        ControllerConnectionHandler _controllerConnectionHandler;

        List<MLPersistentPoint> _persistentPoints = new List<MLPersistentPoint>();
        int _pointsToRestore = 0;
        bool _restoreComplete = false;

        PrivilegeRequester _privilegeRequester;

        [SerializeField, Tooltip("Visualizers to enable when the privilege is granted")]
        private GameObject[] _visualizers;
        #endregion // Private Variables

        #region Unity Methods
        /// <summary>
        /// Validate properties. Attach event listener to when privileges are granted
        /// on Awake because the privilege requester requests on Start.
        /// </summary>
        void Awake()
        {
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: PersistanceExample._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_prefab == null || _prefab.GetComponent<MLPersistentPoint>() == null)
            {
                Debug.LogError("Error: PersistanceExample._prefab is not set or is missing MLPersistentPoint behavior, disabling script.");
                enabled = false;
                return;
            }

            if (_progressText == null)
            {
                Debug.LogError("Error: PersistanceExample._progressText is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_persistentPointsStatusText == null)
            {
                Debug.LogError("Error: PersistanceExample._persistentPointsStatusText is not set, disabling script.");
                enabled = false;
                return;
            }

            // _privilegeRequester is expected to request for PwFoundObjRead privilege
            _privilegeRequester = GetComponent<PrivilegeRequester>();
            _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;

            MLInput.OnControllerButtonDown += HandleButtonDown;
        }

        /// <summary>
        /// Clean up
        /// </summary>
        void OnDestroy()
        {
            foreach (MLPersistentPoint point in _persistentPoints)
            {
                if (point != null)
                {
                    Destroy(point.gameObject);
                }
            }
            MLInput.OnControllerButtonDown -= HandleButtonDown;

            if (MLPersistentCoordinateFrames.IsStarted)
            {
                MLPersistentCoordinateFrames.Stop();
            }
            if (MLPersistentStore.IsStarted)
            {
                MLPersistentStore.Stop();
            }
            if (_privilegeRequester != null)
            {
                _privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
            }
        }
        #endregion // Unity Methods

        #region Event Handlers
        /// <summary>
        /// Responds to privilege requester result.
        /// </summary>
        /// <param name="result"/>
        void HandlePrivilegesDone(MLResult result)
        {
            _privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
            if (!result.IsOk)
            {
                string errorMsg = string.Format("Error: PersistenceExample failed to get requested privileges, disabling script. Reason: {0}", result);
                Debug.LogErrorFormat(errorMsg);
                SetProgress(errorMsg);
                enabled = false;
                return;
            }

            result = MLPersistentStore.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: PersistenceExample failed starting MLPersistentStore, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            result = MLPersistentCoordinateFrames.Start();
            if (!result.IsOk)
            {
                MLPersistentStore.Stop();
                Debug.LogErrorFormat("Error: PersistenceExample failed starting MLPersistentCoordinateFrames, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            if (MLPersistentCoordinateFrames.IsReady)
            {
                HandleReady();
            }
            else
            {
                MLPersistentCoordinateFrames.OnReady += HandleReady;
            }
        }

        /// <summary>
        /// Starts the restoration process after the basic systems are initialized.
        /// </summary>
        void HandleReady()
        {
            MLPersistentCoordinateFrames.OnReady -= HandleReady;

            Array.ForEach(_visualizers, v => v.gameObject.SetActive(true));
            ReadAllStoredObjects();
            CompleteRestore();
        }

        /// <summary>
        /// Handler when MLPersistentPoint encountered an error
        /// </summary>
        /// <param name="result">The specific error</param>
        void HandleContentRestoreError(MLResult result)
        {
            Debug.LogError(result);
        }

        /// <summary>
        /// Handler when MLPersistentPoint completes
        /// </summary>
        /// <param name="success">True when successfully restored or created, otherwise false</param>
        void HandleContentRestoreComplete(bool success)
        {
            --_pointsToRestore;
            if (_pointsToRestore == 0)
            {
                CompleteRestore();
            }

            if (success)
            {
                _persistentPointsSuccess++;
            }
            else
            {
                _persistentPointsFail++;
            }
            _persistentPointsStatusText.text = string.Format(_persistentPointsStatusTextFormat,
                _persistentPointsSuccess, _persistentPointsFail);
        }
        #endregion // Event Handlers

        #region Private Methods
        /// <summary>
        /// Reads all stored game object ids.
        /// </summary>
        void ReadAllStoredObjects()
        {
            List<MLContentBinding> allBindings = MLPersistentStore.AllBindings;
            foreach (MLContentBinding binding in allBindings)
            {
                GameObject gameObj = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
                gameObj.name = binding.ObjectId;
                MLPersistentPoint persistentPointBehavior = gameObj.GetComponent<MLPersistentPoint>();
                persistentPointBehavior.OnComplete += HandleContentRestoreComplete;
                persistentPointBehavior.OnError += HandleContentRestoreError;
                _persistentPoints.Add(persistentPointBehavior);
            }
        }

        /// <summary>
        /// Display progress text in UI
        /// </summary>
        /// <param name="progressText">Progress text.</param>
        void SetProgress(string progressText)
        {
            _progressText.text = progressText;
        }

        /// <summary>
        /// Handler for restore complete. After restore is complete we go into the Done state
        /// where you can start adding more objects.
        /// </summary>
        void CompleteRestore()
        {
            SetProgress("Ready to add objects (Press Bumper)");
            _restoreComplete = true;
        }

        /// <summary>
        /// Called only on device since it's registerd only on device
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="button">Button.</param>
        void HandleButtonDown(byte controllerIndex, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid(controllerIndex)
                && _restoreComplete
                && button == MLInputControllerButton.Bumper)
            {
                CreateObject();
            }
        }

        /// <summary>
        /// Instantiates a new object with MLPersistentPoint. The MLPersistentPoint is
        /// responsible for restoring and saving itself.
        /// </summary>
        void CreateObject()
        {
            Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * UnityEngine.Random.Range(1.5f, 6.0f);
            var gameObj = Instantiate(_prefab, position, UnityEngine.Random.rotation);
            gameObj.name = Guid.NewGuid().ToString();
            MLPersistentPoint persistentPointBehavior = gameObj.GetComponent<MLPersistentPoint>();
            persistentPointBehavior.OnComplete += HandleContentRestoreComplete;
            persistentPointBehavior.OnError += HandleContentRestoreError;
            _persistentPoints.Add(persistentPointBehavior);
        }
        #endregion // Private Methods
    }
}
