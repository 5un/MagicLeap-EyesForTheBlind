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
    /// <summary>
    /// Handles the example scene for Interactable.  
    /// </summary>
    public class InteractableFrameDataExample : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] private InteractableFrameData _frameData;
        [SerializeField] private float _dwellAngle = 5.0f;
        [SerializeField] private float _dwellTime;

        private Interactable _currentInteractable = null;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _frameData.dwellAngleThreshold = _dwellAngle;
        }

        private void Update()
        {
            Interactable newInteractable = _frameData.LongestDwelledInteractable(_dwellTime);
            if (_currentInteractable != newInteractable)
            {
                if (_currentInteractable != null)
                {
                    _currentInteractable.Deactivate();
                }

                _currentInteractable = newInteractable;

                if (newInteractable != null)
                {
                    _currentInteractable.Activate();
                }
            }
        }
    }
}