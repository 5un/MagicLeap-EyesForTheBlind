// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicKit
{

    ///<summary>
    /// Translates object in a ping pong fashion.
    ///</summary>
    public class TranslateObject : MonoBehaviour
    {

        //----------- Private Members -----------

        [Tooltip("Relative to the object's current position"), SerializeField] private Vector3 _startPosition;
        [Tooltip("Relative to the object's current position"), SerializeField] private Vector3 _endPosition;
        [SerializeField] private float _pingPongTime = 4f;

        private float _timer = 0;
        private Vector3 _targetPosition = Vector3.zero;
        private bool _flipDirection = false;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _endPosition += transform.position;
            _startPosition += transform.position;

            transform.position = _startPosition;
            _targetPosition = _endPosition;
        }

        private void LateUpdate()
        {
            _timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime);
            
            if (_timer > _pingPongTime)
            {
                _timer = 0;
                SwitchDirection();
            }
        }

        //----------- Private Methods -----------

        private void SwitchDirection()
        {
            _flipDirection = !_flipDirection;

            if (_flipDirection)
            {
                _targetPosition = _endPosition;
            }
            else
            {
                _targetPosition = _startPosition;
            }
        }
    }
}