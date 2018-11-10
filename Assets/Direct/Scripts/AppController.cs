// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------
using UnityEngine;
using MagicKit.Gestures;
using MagicLeap.Utilities;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;
using System;

namespace MagicKit
{
    /// <summary>
    /// Listens for input and directs the app to perform relevant application layer behavior.
    /// </summary>
    public class AppController : MonoBehaviour
    {
        //----------- Public Events -----------

        public event Action OnSummonUi;
        public event Action OnSelectStory;
        public event Action OnGoodPlacement;
        public event Action OnBadPlacement;
        public event Action OnHideUi;
        public event Action OnShowUi;

        //----------- Private Members -----------

        [SerializeField] private PlacementController _placer;
        [SerializeField] private Transform _selectionUi;
        [SerializeField] private float _selectUiOffset;
        [SerializeField] private GestureKeypointFollower _gestureSuccessFXPrefab;
        [SerializeField] private Placement _placement;
        [SerializeField] private HandTracking _handTracking;
        [SerializeField] private InteractableFrameData _gazeFrameData;
        [SerializeField] private InteractableFrameData _headposeFrameData;
        [SerializeField] private float _interactableDwellDurationThreshold;
        [SerializeField] private float _interactableDwellLostThreshold;
        
        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("Keyposes for selection")]
        private KeyPoseTypes _selectGesture = KeyPoseTypes.Ok | KeyPoseTypes.Pinch;
        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("Keyposes for story placement")]
        private KeyPoseTypes _storyPlacement = KeyPoseTypes.OpenHandBack;
        [SerializeField, BitMask(typeof(KeyPoseTypes)), Tooltip("Keyposes for summon ui")]
        private KeyPoseTypes _summonUi = KeyPoseTypes.Finger;

        private const float SummonUiDuration = 0.4f;
        private float _dwellEndTimer = 0.0f;
        private Vector3 _placeCenter;
        private bool _hasSummonedUi = false;
        private bool _placing = false;
        private Interactable _currentGazeInteractable = null;
        private Interactable _currentHeadposeInteractable = null;
        private Transform _headTransform;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _headTransform = Camera.main.transform;
            GestureManager.OnDwellBegin += OnDwellBegin;
            GestureManager.OnKeyPoseRecognized += HandleKeyposeBegin;
            _placer.OnPlacedObject += HandleContentPlacedInWorld;

            _handTracking.AddKeyPose(_selectGesture);
            _handTracking.AddKeyPose(_summonUi);
            _handTracking.AddKeyPose(_storyPlacement);
            CursorController.Instance.gameObject.SetActive(false);
            
            HideUi();
        }

        private void Update()
        {
            if (!_placing)
            {
                foreach (KeyValuePair<Interactable, InteractableData> interactionTuple in _gazeFrameData.InteractableData)
                {
                    ProcessGazeInteraction(interactionTuple.Key, interactionTuple.Value);
                }
            }

            if(_hasSummonedUi)
            {
                foreach(KeyValuePair<Interactable, InteractableData> interactionTuple in _headposeFrameData.InteractableData)
                {
                    ProcessHeadposeInteraction(interactionTuple.Key, interactionTuple.Value);
                }
            }
        }

        private void OnDestroy()
        {
            GestureManager.OnDwellBegin -= OnDwellBegin;
            GestureManager.OnKeyPoseRecognized -= HandleKeyposeBegin;
            _placer.OnPlacedObject -= HandleContentPlacedInWorld;

            _handTracking.RemoveKeyPose(_selectGesture);
            _handTracking.RemoveKeyPose(_summonUi);
            _handTracking.RemoveKeyPose(_storyPlacement);
        }

        //----------- Event Handlers -----------

        private void HandleKeyposeBegin(MLHand hand)
        {
            if(GestureManager.Matches(hand, _summonUi) || GestureManager.Matches(hand, _selectGesture) || GestureManager.Matches(hand, _storyPlacement))
            {
                SpawnGesturePfx(hand);
            }
        }

        private void HandleOnExpandEnd()
        {
            if (_currentGazeInteractable.CurrentState == Interactable.InteractableState.Activating)
            {
                _currentGazeInteractable.ChangeState(Interactable.InteractableState.Manipulating);
            }
        }

        private void OnDwellBegin(MLHand hand)
        {
            if (GestureManager.Matches(hand, _summonUi))
            {
                SummonUi(hand.Index.Tip.Position);
            }
            if (GestureManager.Matches(hand, _selectGesture))
            {
                SelectObjectBegin(hand);
            }
            if (GestureManager.Matches(hand.KeyPose, _storyPlacement))
            {
                _placeCenter = hand.Center;
                PlaceStoryInWorldStay();
            }
        }

        /// <summary>
        /// Setup the story animation when one is successfully created in the world.
        /// </summary>
        private void HandleContentPlacedInWorld(GameObject placedContent)
        {
            OnGoodPlacement?.Invoke();
            Vector3 destinationPosition = placedContent.transform.position;

            // Tell the story to animate from the hand to its spot on the environment mesh
            Story story = placedContent.GetComponent<Story>();
            story.StartPosition = _placeCenter;
            story.EndPosition = destinationPosition;
        }

        //----------- Private Methods -----------

        private void ProcessHeadposeInteraction(Interactable interactable, InteractableData data)
        {
            if (data.spherecastHit)
            {
                //Deactivate current icon first if there is one.  
                if (_currentHeadposeInteractable != null && _currentHeadposeInteractable != interactable)
                {
                    _currentHeadposeInteractable.ChangeState(Interactable.InteractableState.Deactivating);
                }
                _currentHeadposeInteractable = interactable;
                interactable.ChangeState(Interactable.InteractableState.Targeting);
            }
            //If no longer being hit with spherecast, deactivate the interactable
            else if (interactable == _currentHeadposeInteractable)
            {
                _currentHeadposeInteractable = null;
                interactable.ChangeState(Interactable.InteractableState.Deactivating);
            }
        }

        private void ProcessGazeInteraction(Interactable interactable, InteractableData data)
        {
            if (HasDwellStarted(data) && _currentGazeInteractable == null)
            {
                interactable.ChangeState(Interactable.InteractableState.Activating);
                _currentGazeInteractable = interactable;
                _currentGazeInteractable.GetComponent<Story>().OnExpandEnd += HandleOnExpandEnd;
            }
            else
            {
                if (_currentGazeInteractable == interactable)
                {
                    if(!IsStillDwelling(data))
                    { 
                        _dwellEndTimer += Time.deltaTime;
                    }
                    else
                    {
                        _dwellEndTimer = 0.0f;
                    }
                    
                    if (_dwellEndTimer >= _interactableDwellLostThreshold)
                    {
                        DeactivateCurrentGazeInteractable();
                    }
                }
            }
        }

        private void DeactivateCurrentGazeInteractable()
        {
            _dwellEndTimer = 0.0f;
            _currentGazeInteractable.ChangeState(Interactable.InteractableState.Deactivating);
            _currentGazeInteractable.GetComponent<Story>().OnExpandEnd -= HandleOnExpandEnd;
            _currentGazeInteractable = null;
        }

        private bool IsStillDwelling(InteractableData data)
        {
            return (data.dwellDuration > 0.0f);
        }

        private bool HasDwellStarted(InteractableData data)
        {
            return (data.dwellDuration > _interactableDwellDurationThreshold);
        }

        private void DisableAllInteractables()
        {
            foreach (KeyValuePair<Interactable, InteractableData> data in _gazeFrameData.InteractableData)
            {
                data.Key.ChangeState(Interactable.InteractableState.Deactivating);
            }
        }

        private void SpawnGesturePfx(MLHand hand)
        {
            List<MLKeyPoint> indexKeyPoints = hand.Index.KeyPoints;

            Vector3[] gesturePoints = new Vector3[indexKeyPoints.Count];
            for (int i = 0; i < indexKeyPoints.Count; ++i)
            {
                gesturePoints[i] = indexKeyPoints[i].Position;
            }
            for (int i = 0; i < gesturePoints.Length; i++)
            {
                GestureKeypointFollower fx = Instantiate(_gestureSuccessFXPrefab, gesturePoints[i], Quaternion.identity);
                fx.SetPointToFollow(hand, i);
            }
        }

        private void SummonUi(Vector3 fingerTip)
        {
            // Cancel the Placement state, if it is active
            _placer.Cancel();
            _placing = false;
            ShowUi();
            CursorController.Instance.gameObject.SetActive(true);

            // Move UI to the user's fingers
            Vector3 headPosition = _headTransform.position;

            // We want to position the UI a little bit above the user's finger
            Vector3 headToFingerDirection = (fingerTip - headPosition).normalized;
            Vector3 handUpDirection = Vector3.Cross(headToFingerDirection, _headTransform.right);
            fingerTip += handUpDirection * _selectUiOffset;

            // Make sure our UI is positioned 1m away from the user
            fingerTip = GetClampedPosition(fingerTip, headPosition, 1f, 1f);

            // Slide the UI into position
            Tween.Add(
                _selectionUi.transform.position, fingerTip, SummonUiDuration, 0f,
                Tween.EaseInOut, Tween.LoopType.None,
                (value) => _selectionUi.transform.position = value
            );
            OnSummonUi?.Invoke();
        }

        private void HideUi()
        {
            OnHideUi?.Invoke();
            _hasSummonedUi = false;
        }

        private void ShowUi()
        {
            OnShowUi?.Invoke();
            _hasSummonedUi = true;
        }

        private Vector3 GetClampedPosition(Vector3 objectPosition, Vector3 anchorPosition, float minDistance, float maxDistance)
        {
            Vector3 anchorToObject = objectPosition - anchorPosition;
            float distance = anchorToObject.magnitude;
            float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
            return anchorPosition + anchorToObject.normalized * clampedDistance;
        }

        private void SelectObjectBegin(MLHand hand)
        {
            // Try to initialize the placement system
            if (_currentHeadposeInteractable != null)
            {
                IconController iconController = _currentHeadposeInteractable.GetComponent<IconController>();
                GameObject placeablePrefab = iconController.GetStoryPrefab();
                InitializePlacement(placeablePrefab);
            }
        }

        private void InitializePlacement(GameObject placeablePrefab)
        {
            // Start the placement system
            if (placeablePrefab != null)
            {
                _placer.InitializePlacement(placeablePrefab);
                _placing = true;
                DisableAllInteractables();
                OnSelectStory?.Invoke();
            }

            // Turn off the UI
            HideUi();
            CursorController.Instance.gameObject.SetActive(false);
        }

        private void PlaceStoryInWorldStay()
        {
            // Track the place object in world gesture
            if (_placement.Running)
            {
                // Place story in world
                if (_placer.CanPlace())
                {
                    _placer.Place();
                    Story.OnIntegrated += HandleOnStoryIntegrated;
                }
                else
                {
                    OnBadPlacement?.Invoke();
                }
            }
        }
        private void HandleOnStoryIntegrated()
        {
            _placing = false;
            Story.OnIntegrated -= HandleOnStoryIntegrated;
        }
    }
}