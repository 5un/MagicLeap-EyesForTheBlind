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
using UnityEngine.XR.MagicLeap;

namespace MagicKit
{

    ///<summary>
    /// 
    ///</summary>
    public class GestureFeedbackExample : MonoBehaviour
    {

        //----------- Public Events -----------

        //----------- Public Members -----------

        //----------- Private Members -----------

        [SerializeField] private GameObject _recognizingIndicator;
        [SerializeField] private GameObject _keyPointPrefab;
        [SerializeField] private float _keyPointLaunchForce = 10.0f;
        [SerializeField,BitMask(typeof(KeyPoseTypes))]
        private KeyPoseTypes _keyPoses;

        private List<GameObject> _keyPointIndicators = new List<GameObject>();
        private Vector3 _indicatorStartSize;
        private Transform _headPose;

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            _recognizingIndicator.SetActive(false);
            _indicatorStartSize = _recognizingIndicator.transform.lossyScale;
            _headPose = Camera.main.transform;

            //maximum of 8 keypoints
            for (int i = 0; i < 8; i++)
            {
                GameObject keyPoint = Instantiate(_keyPointPrefab, transform.position, transform.rotation, transform) as GameObject;
                _keyPointIndicators.Add(keyPoint);
                keyPoint.SetActive(false);
            }
        }

        private void OnEnable()
        {
            GestureManager.OnKeyPoseRecognized += OnKeyPoseRecognized;
            GestureManager.OnKeyPoseUpdate += OnKeyPoseUpdate;
            GestureManager.OnKeyPoseLost += OnKeyPoseLost;
            GestureManager.OnDwellBegin += OnDwellBegin;
            GestureManager.OnDwellEnd += OnDwellEnd;
        }

        private void OnDisable()
        {
            GestureManager.OnKeyPoseRecognized -= OnKeyPoseRecognized;
            GestureManager.OnKeyPoseUpdate -= OnKeyPoseUpdate;
            GestureManager.OnKeyPoseLost -= OnKeyPoseLost;
            GestureManager.OnDwellBegin -= OnDwellBegin;
            GestureManager.OnDwellEnd -= OnDwellEnd;
        }

        private void OnKeyPoseRecognized(MLHand hand)
        {
            if(GestureManager.Matches(hand,_keyPoses))
            {
                _recognizingIndicator.transform.position = hand.Center;
                _recognizingIndicator.transform.localScale = _indicatorStartSize;
                _recognizingIndicator.SetActive(true);
            }
        }

        private void OnKeyPoseUpdate(MLHand hand, bool inDwell)
        {
            if (GestureManager.Matches(hand, _keyPoses))
            {
                if(!inDwell)
                {
                    //shrink the indicator
                    float percentRecognized = GestureManager.Instance.GetPercentRecognized(hand);
                    _recognizingIndicator.transform.localScale = _indicatorStartSize*(1.0f-percentRecognized);
                    _recognizingIndicator.transform.position = hand.Center;
                }
            }
        }

        private void OnKeyPoseLost(MLHandType handType, MLHandKeyPose keyPose)
        {
            if (GestureManager.Matches(keyPose, _keyPoses))
            {
                _recognizingIndicator.SetActive(false);
            }
        }

        private void OnDwellBegin(MLHand hand)
        {
            if (GestureManager.Matches(hand, _keyPoses))
            {
                _recognizingIndicator.SetActive(false);

                Vector3 force = (hand.Center - _headPose.position).normalized*_keyPointLaunchForce;
                List<MLKeyPoint> validPoints = GetValidKeypoints(hand);
                for(int i = 0; i < validPoints.Count; i++)
                {
                    GameObject keyPointIndicator = _keyPointIndicators[i];
                    keyPointIndicator.transform.position = validPoints[i].Position;
                    Rigidbody rigidbody = keyPointIndicator.GetComponent<Rigidbody>();
                    rigidbody.velocity = Vector3.zero;
                    keyPointIndicator.SetActive(true);
                    rigidbody.AddForce(force, ForceMode.Impulse);
                }
            }
        }

        private void OnDwellEnd(MLHandType handType, MLHandKeyPose keyPose)
        {
            if (GestureManager.Matches(keyPose, _keyPoses))
            {
                foreach(GameObject keyPointIndicator in _keyPointIndicators)
                {
                    keyPointIndicator.SetActive(false);
                }
            }
        }

        //----------- Event Handlers -----------

        //----------- Public Methods -----------

        //----------- Private Methods -----------

        private List<MLKeyPoint> GetValidKeypoints(MLHand hand)
        {
            List<MLKeyPoint> keypoints = new List<MLKeyPoint>();
            keypoints.AddRange(hand.Wrist.KeyPoints);
            keypoints.AddRange(hand.Thumb.KeyPoints);
            keypoints.AddRange(hand.Index.KeyPoints);
            keypoints.AddRange(hand.Middle.KeyPoints);
            keypoints.AddRange(hand.Ring.KeyPoints);
            keypoints.AddRange(hand.Pinky.KeyPoints);

            List<MLKeyPoint> validPoints = new List<MLKeyPoint>();
            foreach(MLKeyPoint k in keypoints)
            {
                if(k.IsValid)
                {
                    validPoints.Add(k);
                }
            }

            return validPoints;
        }

        //----------- Coroutines -----------

    }
}
