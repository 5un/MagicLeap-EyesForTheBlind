// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
using UnityEngine;

namespace MagicKit
{

    ///<summary>
    /// Places a copy of the prefab on all active keypoints during a gesture.
    /// These keypoints will change color when a dwell state is entered or exited.
    ///</summary>
    public class GestureDwellVisualizer : MonoBehaviour 
    {

        //----------- Private Members -----------

        [SerializeField, Tooltip("Object displayed at each point.")]
        private GameObject _keypointPrefab;
        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("These keyposes will be visualized on the left hand.")]
        private KeyPoseTypes _leftTypes;
        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("These keyposes will be visualized on the left hand.")]
        private KeyPoseTypes _rightTypes;
        [SerializeField] private Gradient _colorGradient;
        [SerializeField] private string _colorProperty = "_Color";
        
        private List<GameObject> _leftIndicators;
        private List<GameObject> _rightIndicators;
        private Vector3 _keyPointStartSize;
        private float _percentRecognized = 0.0f;

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            _keyPointStartSize = _keypointPrefab.transform.localScale;
        }

        private void OnEnable()
        {
            GestureManager.OnKeyPoseRecognized += OnKeyPoseRecognized;
            GestureManager.OnKeyPoseLost += OnKeyPoseLost;
            GestureManager.OnDwellBegin += OnDwellBegin;
            GestureManager.OnKeyPoseUpdate += OnKeyPoseUpdate;
            GestureManager.OnDwellEnd += OnDwellEnd;
        }

        private void OnDisable()
        {
            GestureManager.OnKeyPoseRecognized -= OnKeyPoseRecognized;
            GestureManager.OnKeyPoseLost -= OnKeyPoseLost;
            GestureManager.OnDwellBegin -= OnDwellBegin;
            GestureManager.OnKeyPoseUpdate -= OnKeyPoseUpdate;
            GestureManager.OnDwellEnd -= OnDwellEnd;
        }

        //----------- Event Handlers -----------

        private void OnKeyPoseRecognized(MLHand hand)
        {
            if (GestureManager.Matches(hand, _leftTypes, _rightTypes))
            {
                EnableContent(hand, true);
            }
        }

        private void OnKeyPoseLost(MLHandType handType, MLHandKeyPose keyPose)
        {
            if (GestureManager.Matches(handType, keyPose, _leftTypes, _rightTypes))
            {
                MLHand hand = handType == MLHandType.Left ? MLHands.Left : MLHands.Right;
                EnableContent(hand, false);
            }
        }

        private void OnDwellBegin(MLHand hand)
        {
            if (GestureManager.Matches(hand, _leftTypes, _rightTypes))
            {
                SetColors(hand, _colorGradient.Evaluate(1.0f));
            }
        }

        private void OnKeyPoseUpdate(MLHand hand,bool inDwell)
        {
            if(GestureManager.Matches(hand,_leftTypes,_rightTypes))
            {
                if (!inDwell)
                {
                    //shrink the indicator
                    _percentRecognized = Mathf.Max(_percentRecognized,GestureManager.Instance.GetPercentRecognized(hand));
                    SetColors(hand, _colorGradient.Evaluate(_percentRecognized));
                    AdjustKeypointSize(hand, _percentRecognized);
                }
                DrawKeypoints(hand);
            }
        }

        private void OnDwellEnd(MLHandType handType, MLHandKeyPose keyPose)
        {
            if (GestureManager.Matches(handType, keyPose, _leftTypes, _rightTypes))
            {
                _percentRecognized = 0.0f;
            }
        }

        //----------- Private Methods -----------

        private void EnableContent(MLHand hand,bool enabled)
        {
            List<GameObject> indicators = GetIndicators(hand);
            foreach (GameObject indicator in indicators)
            {
                indicator.SetActive(enabled);
            }
        }

        private void DrawKeypoints(MLHand hand)
        {
            //position keypoint indicators
            List<MLKeyPoint> keypoints = GetKeypoints(hand);
            List<GameObject> indicators = GetIndicators(hand);
            for (int i = 0; i < keypoints.Count; i++)
            {
                if (keypoints[i].IsValid)
                {
                    indicators[i].SetActive(true);
                    indicators[i].transform.position = keypoints[i].Position;
                }
                else
                {
                    indicators[i].SetActive(false);
                }
            }
            // position the hand.center indicator
            indicators[indicators.Count - 1].transform.position = hand.Center;
        }

        private List<GameObject> GetIndicators(MLHand hand)
        {
            if(hand.HandType == MLHandType.Left)
            {
                if(_leftIndicators == null)
                {
                    _leftIndicators = new List<GameObject>();
                    List<MLKeyPoint> keypoints = GetKeypoints(hand);
                    for(int i = 0; i < keypoints.Count; i++)
                    {
                        GameObject newIndicator = Instantiate(_keypointPrefab);
                        _leftIndicators.Add(newIndicator);
                        newIndicator.SetActive(false);
                    }
                    //add centerpoint as final indicator in the list.
                    GameObject leftCenter = Instantiate(_keypointPrefab);
                    leftCenter.SetActive(false);
                    _leftIndicators.Add(leftCenter);
                }

                return _leftIndicators;
            }
            else
            {
                if (_rightIndicators == null)
                {
                    _rightIndicators = new List<GameObject>();
                    List<MLKeyPoint> keypoints = GetKeypoints(hand);
                    for (int i = 0; i < keypoints.Count; i++)
                    {
                        GameObject newIndicator = Instantiate(_keypointPrefab);
                        _rightIndicators.Add(newIndicator);
                        newIndicator.SetActive(false);
                    }
                    //add centerpoint as final indicator in the list.
                    GameObject rightCenter = Instantiate(_keypointPrefab);
                    rightCenter.SetActive(false);
                    _rightIndicators.Add(rightCenter);
                }

                return _rightIndicators;
            }
        }

        private void SetColors(MLHand hand, Color newColor)
        {
            List<GameObject> indicators = GetIndicators(hand);
            foreach(GameObject indicator in indicators)
            {
                indicator.GetComponent<MeshRenderer>().material.SetColor(_colorProperty, newColor);
            }
        }

        private void AdjustKeypointSize(MLHand hand, float percentMaxSize)
        {
            List<GameObject> indicators = GetIndicators(hand);
            foreach (GameObject indicator in indicators)
            {
                indicator.transform.localScale = percentMaxSize * _keyPointStartSize;
            }
        }

        private List<MLKeyPoint> GetKeypoints(MLHand hand)
        {
            List<MLKeyPoint> keypoints = new List<MLKeyPoint>();
            keypoints.AddRange(hand.Wrist.KeyPoints);
            keypoints.AddRange(hand.Thumb.KeyPoints);
            keypoints.AddRange(hand.Index.KeyPoints);
            keypoints.AddRange(hand.Middle.KeyPoints);
            keypoints.AddRange(hand.Ring.KeyPoints);
            keypoints.AddRange(hand.Pinky.KeyPoints);
            return keypoints;
        }
    }
}
