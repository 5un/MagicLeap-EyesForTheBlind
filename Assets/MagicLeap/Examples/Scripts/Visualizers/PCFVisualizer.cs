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
    ///  Shows all Persistent Points in the world around you.
    /// </summary>
    public class PCFVisualizer : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("Prefab to represent a PCF visually")]
        private GameObject _prefab;
        private List<GameObject> _pcfObjs = new List<GameObject>();

        [SerializeField, Tooltip("UI Text to show PCF Count")]
        private Text _countText;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Validates variables, initializes systems, and prepares to show PCFs
        /// </summary>
        void Start()
        {
            if (_prefab == null)
            {
                Debug.LogError("Error: PCFVisualizer._representativePrefab not set, disabling script.");
                enabled = false;
                return;
            }

            if (_countText == null)
            {
                Debug.LogError("Error: PCFVisualizer._countText not set, disabling script.");
                enabled = false;
                return;
            }

            MLResult result = MLPersistentStore.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: PCFVisualizer failed starting MLPersistentStore, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            result = MLPersistentCoordinateFrames.Start();
            if (!result.IsOk)
            {
                MLPersistentStore.Stop();
                Debug.LogErrorFormat("Error: PCFVisualizer failed starting MLPersistentCoordinateFrames, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            if (MLPersistentCoordinateFrames.IsReady)
            {
                TryShowingAllPCFs(GetPCFList());
            }
            else
            {
                MLPersistentCoordinateFrames.OnReady += HandleReady;
            }
        }

        /// <summary>
        /// Clean up
        /// </summary>
        void OnDestroy()
        {
            foreach (GameObject go in _pcfObjs)
            {
                Destroy(go);
            }

            if (MLPersistentStore.IsStarted)
            {
                MLPersistentStore.Stop();
            }
            if (MLPersistentCoordinateFrames.IsStarted)
            {
                MLPersistentCoordinateFrames.Stop();
                MLPersistentCoordinateFrames.OnReady -= HandleReady;
            }
        }
        #endregion // Unity Methods

        #region Event Handlers
        /// <summary>
        /// Handler for OnReady. Should only be executed once.
        /// </summary>
        private void HandleReady()
        {
            MLPersistentCoordinateFrames.OnReady -= HandleReady;
            TryShowingAllPCFs(GetPCFList());
        }
        #endregion // Event Handlers

        #region Private Methods
        /// <summary>
        /// Acquires list of PCFs
        /// </summary>
        /// <returns>List of PCFs</returns>
        List<MLPCF> GetPCFList()
        {
            List<MLPCF> pcfList;
            MLResult result = MLPersistentCoordinateFrames.GetAllPCFs(out pcfList);
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("PCFVisualizer failed getting all PCFs, disabling script. Reason: {0}", result);
                enabled = false;
            }

            _countText.text = String.Format("PCF Count: {0}", pcfList.Count);
            return pcfList;
        }

        /// <summary>
        /// Tries showing all PCF.
        /// </summary>
        /// <param name="pcfList">PCF list.</param>
        void TryShowingAllPCFs(List<MLPCF> pcfList)
        {
            foreach (MLPCF pcf in pcfList)
            {
                if (pcf.CurrentResult == MLResultCode.Pending)
                {
                    MLResult result = MLPersistentCoordinateFrames.GetPCFPosition(pcf, (queryResult, pcfObj) =>
                    {
                        if (queryResult.IsOk)
                        {
                            AddPCFObject(pcfObj);
                            MLPersistentCoordinateFrames.QueueForUpdates(pcfObj);
                        }
                        else
                        {
                            Debug.LogErrorFormat("PCFVisualizer failed to get position for PCF: {0}. Reason: {1}", pcfObj, queryResult);
                        }
                    });

                    if (!result.IsOk && result.Code != MLResultCode.Pending)
                    {
                        Debug.LogErrorFormat("PCFVisualizer failed to attempt to get position for PCF: {0}. Reason: {1}", pcf, result);
                    }
                }
                else
                {
                    AddPCFObject(pcf);
                }
            }
        }

        /// <summary>
        /// Creates the PCF game object.
        /// </summary>
        /// <param name="pcf">Pcf.</param>
        void AddPCFObject(MLPCF pcf)
        {
            if(!_pcfObjs.Contains(pcf.GameObj))
            {
                GameObject repObj = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
                repObj.name = pcf.GameObj.name;
                repObj.transform.SetParent(pcf.GameObj.transform, false);

                PCFStatusText statusTextBehavior = repObj.GetComponent<PCFStatusText>();
                if (statusTextBehavior != null)
                {
                    statusTextBehavior.PCF = pcf;
                }

                _pcfObjs.Add(pcf.GameObj);
            }
        }
        #endregion // Private Methods
    }
}
