// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

﻿using UnityEngine;

namespace MagicLeap.Utilities
{
    /// <summary>
    /// Attaches this object to another object.
    /// </summary>
    public class Tether : MonoBehaviour
    {

        //----------- Public Properties -----------

        public Vector3 Offset;
        public Transform target;
        public float threshold = 0.1f;
        public float lerpSpeed = 50.0f;
        public bool rotateWith = true;
        
        //----------- Private Members -----------

        private float _currentDistance;
        private Vector3 _targetPosition;
        private float lockedDistance;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError("Error Tether.connectedObject is not set, disabling script.");
                enabled = false;
            }
            Vector3 meToConnectedObject = transform.position - target.position;
            _currentDistance = meToConnectedObject.magnitude;
            lockedDistance = Offset.magnitude;
            _targetPosition = CalculateNewTargetPosition();
        }

        private void LateUpdate()
        {
            _currentDistance = (transform.position - CalculateNewTargetPosition()).magnitude;
            lockedDistance = Offset.magnitude;
            if(DistanceCheck())
            {
                if (rotateWith)
                {
                    _targetPosition = CalculateNewTargetPosition();
                    lockedDistance = Offset.magnitude;
                }
                else
                {
                    _targetPosition = target.transform.position + Offset;
                }
            }
            Move();
        }

        //----------- Private Methods ----------

        private Vector3 CalculateNewTargetPosition()
        {
            if(rotateWith)
            {
                Vector3 posTo = target.TransformDirection(Offset);
                return target.transform.position + (posTo);
            }
            else
            {
                return target.transform.position + (Offset.normalized * lockedDistance);
            }
        }

        private bool DistanceCheck()
        {
            // Returns true if tethered content needs to move
            return (_currentDistance > threshold);
        }

        private void Move()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * lerpSpeed);
        }
    }
}
