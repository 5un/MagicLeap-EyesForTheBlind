// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicLeap.Utilities
{
    /// <summary>
    /// Simple script for a pingpong tween for movement
    /// </summary>
    public class SphereTween : MonoBehaviour
    {
        //----------- Private Members -----------

        [SerializeField] private Vector3 _moveToLocalPosition;

        private Vector3 _startPosition;
        private Vector3 _endPositon;
        private string _tweenID;

        //----------- MonoBehaviour Methods -----------

        void Start()
        {
            _startPosition = transform.position;
            _endPositon = _startPosition + _moveToLocalPosition;
        }

        //----------- Public Methods -----------

        public void StartTween()
        {
            if (string.IsNullOrEmpty(_tweenID))
            {
                _tweenID = Tween.Add(_startPosition, _endPositon, 2.5f, 0, Tween.EaseInOut, Tween.LoopType.PingPong, HandleMoveTween);
            }
            else
            {
                TweenObject movementTween = Tween.GetTween(_tweenID);
                movementTween.isComplete = false;
            }
        }

        public void StopTween()
        {
            TweenObject movementTween = Tween.GetTween(_tweenID);
            movementTween.isComplete = true;
            Tween.Instance.tweens.Remove(movementTween);
            _tweenID = null;

            transform.position = _startPosition;
        }

        //----------- Private Methods -----------

        private void HandleMoveTween(Vector3 value)
        {
            transform.position = value;
        }
    }
}