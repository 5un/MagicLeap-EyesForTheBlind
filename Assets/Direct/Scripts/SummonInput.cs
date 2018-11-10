// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Utilities;

namespace MagicKit
{
    /// <summary>
    /// This class handles summoning the ui and update the buttons
    /// </summary>
    public class SummonInput : MonoBehaviour
    {

        //----------- Private Variables -----------

        [SerializeField] private GameObject _ui;
        [SerializeField] private MLHandKeyPose _summonPose;
        private Camera _camera;

        private const float SelectUiOffset = 0.05f;
        private const float MoveGestureAllowedDuration = 0.2f;
        private const float MinDistance = 1.0f;
        private const float MaxDistance = 1.0f;
       
        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            GestureManager.OnDwellBegin += OnDwellBegin;

            _camera = Camera.main;
        }

        private void OnDestroy()
        {
            GestureManager.OnDwellBegin -= OnDwellBegin;
        }

        //----------- Event Handlers -----------

        private void OnDwellBegin(MLHand hand)
        {
            if (hand.KeyPose == _summonPose)
            {
                _ui.SetActive(true);
                SummonUi(hand);
            }
        }

        private void SummonUi(MLHand hand)
        {
            // Move UI to the user's fingers
            Vector3 headPosition = _camera.transform.position;
            Vector3 fingerTip = hand.Index.Tip.Position;

            // We want to position the UI a little bit above the user's finger
            Vector3 headToFingerDirection = (fingerTip - headPosition).normalized;
            Vector3 handUpDirection = Vector3.Cross(headToFingerDirection, _camera.transform.right);
            fingerTip += handUpDirection * SelectUiOffset;

            // Make sure our UI is positioned 1m away from the user
            fingerTip = GetClampedPosition(fingerTip, headPosition, MinDistance, MaxDistance);

            // Slide the UI into position
            Tween.Add(
                _ui.transform.position, fingerTip, MoveGestureAllowedDuration, 0f,
                Tween.EaseInOut, Tween.LoopType.None,
                (value) => _ui.transform.position = value
            );
        }
        

        //----------- Private Methods -----------

        private Vector3 GetClampedPosition(Vector3 objectPosition, Vector3 anchorPosition, float minDistance, float maxDistance)
        {
            Vector3 anchorToObject = objectPosition - anchorPosition;
            float distance = anchorToObject.magnitude;
            float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
            return anchorPosition + anchorToObject.normalized * clampedDistance;
        }
    }

}
