// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicKit.Gestures
{

    ///<summary>
    /// An example class that shows how to subscribe to gesture events.
    ///</summary>
    public class GestureEventsExample : MonoBehaviour 
    {
        //----------- Private Members -----------

        [SerializeField,BitMask(typeof(KeyPoseTypes)),Tooltip("Recognize these keyposes")]
        private KeyPoseTypes _keyPoseTypes;
        [SerializeField] private Text _leftStatus;
        [SerializeField] private Text _rightStatus;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            GestureManager.OnKeyPoseRecognized += OnKeyPoseRecognized;
            GestureManager.OnKeyPoseLost += OnKeyPoseLost;
            GestureManager.OnDwellBegin += OnDwellBegin;
            GestureManager.OnDwellEnd += OnDwellEnd;
        }

        private void OnDisable()
        {
            GestureManager.OnKeyPoseRecognized -= OnKeyPoseRecognized;
            GestureManager.OnKeyPoseLost -= OnKeyPoseLost;
            GestureManager.OnDwellBegin -= OnDwellBegin;
            GestureManager.OnDwellEnd -= OnDwellEnd;
        }

        //----------- Event Handlers -----------

        private void OnKeyPoseRecognized(MLHand hand)
        {
            if (GestureManager.Matches(hand, _keyPoseTypes))
            {
                GetText(hand.HandType).text = "KeyPoseRecognized: \n" + hand.HandType + " " + hand.KeyPose;
            }
        }

        private void OnKeyPoseLost(MLHandType handType, MLHandKeyPose keyPose)
        {
            if (GestureManager.Matches(keyPose, _keyPoseTypes))
            {
                GetText(handType).text = "KeyPoseLost: \n" + handType + " " + keyPose;
            }
        }

        private void OnDwellEnd(MLHandType handType, MLHandKeyPose keyPose)
        {
            if(GestureManager.Matches(keyPose,_keyPoseTypes))
            {
                GetText(handType).text = "DwellEnd: \n" + handType + " " + keyPose;
            }
        }

        private void OnDwellBegin(MLHand hand)
        {
            if (GestureManager.Matches(hand, _keyPoseTypes))
            {
                GetText(hand.HandType).text = "DwellBegin: \n" + hand.HandType + " " + hand.KeyPose;
            }
        }

        private Text GetText(MLHandType handType)
        {
            return handType == MLHandType.Left ? _leftStatus : _rightStatus;
        }

    }
}
