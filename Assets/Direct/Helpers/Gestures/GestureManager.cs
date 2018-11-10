// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicLeap.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicKit
{

    ///<summary>
    /// One stop shop for gestures.
    ///</summary>
    [RequireComponent(typeof(HandTracking))]
    public class GestureManager : Singleton<GestureManager>
    {

        //----------- Public Events -----------

        public static event Action<MLHand> OnKeyPoseRecognized;
        public static event Action<MLHand> OnDwellBegin;
        public static event Action<MLHand,bool> OnKeyPoseUpdate;
        public static event Action<MLHandType, MLHandKeyPose> OnDwellEnd;
        public static event Action<MLHandType, MLHandKeyPose> OnKeyPoseLost;

        //----------- Public Members -----------

        public static MLHand LeftHand
        {
            get
            {
                return MLHands.Left;
            }
        }

        public static MLHand RightHand
        {
            get
            {
                return MLHands.Right;
            }
        }

        public MLHandKeyPose LeftDwellPose
        {
            get
            {
                return _leftTracker.dwellPose;
            }
        }

        public MLHandKeyPose RightDwellPose
        {
            get
            {
                return _rightTracker.dwellPose;
            }
        }

        //----------- Private Members -----------

        [SerializeField,Tooltip("Gestures will take this long to enter dwell state. 0.18 to 0.3 seconds reccomended based on user data.")]
        private float _activationTime = 0.18f;
        [SerializeField, Tooltip("Gestures will take this long to exit dwell state 0.18 to 0.3 seconds reccomended based on user data.")]
        private float _deactivationTime = 0.18f;

        private KeyPoseTracker _leftTracker;
        private KeyPoseTracker _rightTracker;
        // Thresholds used to filter out low-confidence gestures.
        private const float StartThreshold = 0.9f;
        private const float StopThreshold = 0.6f;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            MLResult result = MLHands.Start();
            if (!result.IsOk)
            {
                Debug.LogError("MLHands.Start() error");
                enabled = false;
                return;
            }

            _leftTracker = new KeyPoseTracker(MLHands.Left);
            _rightTracker = new KeyPoseTracker(MLHands.Right);

            MLHands.KeyPoseManager.OnHandKeyPoseBegin += OnKeyPoseBegin;
            MLHands.KeyPoseManager.OnHandKeyPoseEnd += OnKeyPoseEnd;
        }

        private void OnDisable()
        {
            MLHands.KeyPoseManager.OnHandKeyPoseBegin -= OnKeyPoseBegin;
            MLHands.KeyPoseManager.OnHandKeyPoseEnd -= OnKeyPoseEnd;
            MLHands.Stop();
        }

        private void Update()
        {
            UpdateState(_leftTracker);
            UpdateState(_rightTracker);
        }

        //----------- Public Methods -----------

        /// <summary>
        /// Returns a % value 0-1, representing how close this hand is to entering a dwell state.
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public float GetPercentRecognized(MLHand hand)
        {
            if(InDwellState(hand))
            {
                return 1.0f;
            }
            else
            {
                KeyPoseTracker tracker = GetTrackerState(hand.HandType);
                return tracker.timer / _activationTime;
            }
        }

        public bool InDwellState(MLHand hand)
        {
            MLHandKeyPose dwellPose = hand.HandType == MLHandType.Left ? LeftDwellPose : RightDwellPose; ;
            return hand.KeyPose == dwellPose;
        }

        public MLHandKeyPose GetDwellPose(MLHandType handType)
        {
            return handType == MLHandType.Left ? LeftDwellPose : RightDwellPose;
        }

        public static bool Matches(MLHand hand, MLHandKeyPose keyPose, MLHandType handType)
        {
            return hand.KeyPose == keyPose && hand.HandType == handType;
        }

        public static bool Matches(MLHand hand, KeyPoseTypes _leftTypes, KeyPoseTypes _rightTypes)
        {
            return Matches(hand.HandType, hand.KeyPose, _leftTypes, _rightTypes);
        }

        public static bool Matches(MLHandType handType, MLHandKeyPose keyPose, KeyPoseTypes _leftTypes, KeyPoseTypes _rightTypes)
        {
            if (handType == MLHandType.Left)
            {
                return Matches(keyPose, _leftTypes);
            }
            else
            {
                return Matches(keyPose, _rightTypes);
            }
        }

        /// <summary>
        /// Compare MLHandKeyPose, to our GestureTypes bitmask.
        /// </summary>
        public static bool Matches(MLHand hand, KeyPoseTypes keyPose)
        {
            return Matches(hand.KeyPose, keyPose);
        }

        /// <summary>
        /// Compares MLHandKeyPose to KeyPoseTypes and returns true if they match.
        /// </summary>
        public static bool Matches(MLHandKeyPose handKeyPose, KeyPoseTypes keyPoseTypes)
        {
            int g = (int)handKeyPose;
            KeyPoseTypes convertedKeyPose = (KeyPoseTypes)(1 << g);
            return (keyPoseTypes & convertedKeyPose) == convertedKeyPose;
        }

        /// <summary>
        /// Convenience function, returns true of both hands are contained in the given gestureTypes bitmask.
        /// </summary>
        public static bool Matches(MLHandKeyPose leftPose, MLHandKeyPose rightPose, KeyPoseTypes keyPose)
        {
            return Matches(leftPose, keyPose) && Matches(rightPose, keyPose);
        }

        //----------- Private Methods -----------

        private void ChangeState(KeyPoseTracker tracker, KeyPoseState newState)
        {
            tracker.timer = 0;
            tracker.state = newState;

            switch (newState)
            {
                case KeyPoseState.Dwell:
                    tracker.dwellPose = tracker.hand.KeyPose;
                    if(OnDwellBegin != null)
                    {
                        OnDwellBegin(tracker.hand);
                    }
                    break;
                case KeyPoseState.Lost:
                    if(OnDwellEnd != null)
                    {
                        OnDwellEnd(tracker.hand.HandType, tracker.dwellPose);
                    }
                    tracker.dwellPose = MLHandKeyPose.NoPose;
                    break;
                default:
                    break;
            }
        }

        private void UpdateState(KeyPoseTracker tracker)
        {
            if (tracker.hand.KeyPose != tracker.lastKeyPose)
            {
                tracker.timer = 0.0f;
            }

            switch (tracker.state)
            {
                case KeyPoseState.Lost:
                    if (tracker.Recognized)
                    {
                        ChangeState(tracker, KeyPoseState.Recognized);
                    }
                    break;
                case KeyPoseState.Recognized:
                    if (tracker.Recognized && OnKeyPoseUpdate != null)
                    {
                        OnKeyPoseUpdate(tracker.hand,false);
                    }
                    if(tracker.hand.KeyPoseConfidenceFiltered > StartThreshold)
                    {
                        tracker.timer += Time.deltaTime;
                        if (tracker.timer > _activationTime)
                        {
                            ChangeState(tracker, KeyPoseState.Dwell);
                        }
                    }
                    break;
                case KeyPoseState.Dwell:
                    if (tracker.Recognized && OnKeyPoseUpdate != null)
                    {
                        OnKeyPoseUpdate(tracker.hand,true);
                    }
                    if (!tracker.Recognized || tracker.hand.KeyPose != tracker.dwellPose || tracker.hand.KeyPoseConfidenceFiltered < StopThreshold)
                    {
                        tracker.timer += Time.deltaTime;
                        if (tracker.timer > _deactivationTime)
                        {
                            ChangeState(tracker, KeyPoseState.Lost);
                        }
                    }
                    break;
            }

            tracker.lastKeyPose = tracker.hand.KeyPose;
        }

        private MLHand GetHand(MLHandType handType)
        {
            return handType == MLHandType.Left ? LeftHand : RightHand;
        }

        private KeyPoseTracker GetTrackerState(MLHandType handType)
        {
            return handType == MLHandType.Left ? _leftTracker : _rightTracker;
        }

        private void OnKeyPoseBegin(MLHandKeyPose gestureType, MLHandType handType)
        {
            if (gestureType == MLHandKeyPose.NoHand)
            {
                return;
            }

            if(OnKeyPoseRecognized != null)
            {
                OnKeyPoseRecognized(GetHand(handType));
            }
            GetTrackerState(handType).Recognized = true;
        }

        private void OnKeyPoseEnd(MLHandKeyPose gestureType, MLHandType handType)
        {
            if (gestureType == MLHandKeyPose.NoHand)
            {
                return;
            }
            if (OnKeyPoseLost != null)
            {
                OnKeyPoseLost(handType, gestureType);
            }
            GetTrackerState(handType).Recognized = false;
        }
    }
}
