// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicKit
{

    ///<summary>
    /// Example showing how the world geometry bounds example works
    ///</summary>
    [RequireComponent(typeof(SpatialObject))]
    public class WorldGeometryBoundsExample : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] private ControllerInput _controllerInput;

        private Camera _mainCamera;
        private bool _inFOV = false;
        private bool _activated = false;
        private Material _material;
        private SpatialObject _spatialObj;

        private const float ObjectInFovMinAngle = 20;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _spatialObj = GetComponent<SpatialObject>();
            _controllerInput.OnTriggerDown += HandOnTriggerDown;
            _mainCamera = Camera.main;
            _material = GetComponent<Renderer>().material;
        }

        private void Update()
        {
            EditorTest();

            Vector3 direction = (transform.position - _mainCamera.transform.position).normalized;

            // find an object in FOV and activate mesh around that object
            if (Vector3.Angle(direction, _mainCamera.transform.forward) < ObjectInFovMinAngle)
            {
                _inFOV = true;
                return;
            }
            _inFOV = false;
        }

        private void OnDestroy()
        {
            _controllerInput.OnTriggerDown -= HandOnTriggerDown;
        }

        //----------- Event Handlers -----------

        private void HandOnTriggerDown()
        {
            if (_inFOV)
            {
                ActivateWorldMeshNearObject();
            }
        }

        //----------- Private Methods -----------

        private void ActivateWorldMeshNearObject()
        {
            if (_activated)
            {
                WorldGeometryBounds.Instance.RemoveSpatialObject(_spatialObj);
                _material.DisableKeyword("_EMISSION");
            }
            else
            {
                WorldGeometryBounds.Instance.AddSpatialObject(_spatialObj);
                _material.EnableKeyword("_EMISSION");
            }
            _activated = !_activated;
        }

        private void EditorTest()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandOnTriggerDown();
            }
        }
    }
}