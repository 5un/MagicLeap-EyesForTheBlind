// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicKit.Gestures;
using MagicLeap;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicKit
{

    ///<summary>
    /// An example class that shows how to pull for gesture events.
    ///</summary>
    public class GesturePollingExample : MonoBehaviour 
    {
        //----------- Private Members -----------

        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("Recognize these keyposes")]
        private KeyPoseTypes _keyPoseTypes;

        private MLHandKeyPose _lastLeftKeyPose = MLHandKeyPose.NoHand;
        private MLHandKeyPose _lastRightKeyPose = MLHandKeyPose.NoHand;

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            //left hand polling
            if(GestureManager.Matches(GestureManager.LeftHand.KeyPose,_keyPoseTypes))
            {
                if(GestureManager.LeftHand.KeyPose != _lastLeftKeyPose)
                {
                    Debug.Log("End: Left " + _lastLeftKeyPose);
                    Debug.Log("Begin: Left" + GestureManager.LeftHand.KeyPose);
                }

                Debug.Log(GestureManager.LeftHand.KeyPoseConfidence);

                _lastLeftKeyPose = GestureManager.LeftHand.KeyPose;
            }

            //right hand polling
            if (GestureManager.Matches(GestureManager.RightHand.KeyPose, _keyPoseTypes))
            {
                if (GestureManager.RightHand.KeyPose != _lastRightKeyPose)
                {
                    Debug.Log("End: Right " + _lastRightKeyPose);
                    Debug.Log("Begin: Right " + GestureManager.RightHand.KeyPose);
                }

                Debug.Log(GestureManager.RightHand.KeyPoseConfidence);

                _lastRightKeyPose = GestureManager.RightHand.KeyPose;
            }
        }


    }
}
