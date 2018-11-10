// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine.XR.MagicLeap;

namespace MagicKit
{

    ///<summary>
    /// Used by GestureManager to track the state keyposes over time.
    ///</summary>
    public class KeyPoseTracker
    {
        //----------- Public Members -----------

        public bool Recognized
        {
            get
            {
                return recognized;
            }
            set
            {
                recognized = value;
                timer = 0.0f;
            }
        }

        public KeyPoseState state;
        public float timer;
        public MLHand hand;
        public MLHandKeyPose dwellPose; 
        public MLHandKeyPose lastKeyPose;

        //----------- Private Members -----------

        private bool recognized;

        //----------- Public Methods -----------

        public KeyPoseTracker(MLHand hand)
        {
            state = KeyPoseState.Lost;
            dwellPose = MLHandKeyPose.NoPose;
            lastKeyPose = MLHandKeyPose.NoPose;
            timer = 0.0f;
            this.hand = hand;
        }

    }

    public enum KeyPoseState
    {
        Lost, // hand is out of scene, or not in a recognizable pose.
        Recognized, // a key pose has been detected
        Dwell // a key pose has been held for x seconds.
    }
}
