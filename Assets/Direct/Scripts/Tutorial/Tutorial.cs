// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System;

namespace MagicKit
{
    /// <summary>
    /// Tracks actions by the user and updates the tutorial state.
    /// </summary>
    public class Tutorial : MonoBehaviour
    {
        //----------- Public Events -----------

        public static event Action<TutorialStep> OnChangeTutorialStep;

        //----------- Public Members -----------

        public enum TutorialStep
        {
            SummonUi = 0,
            SelectObject,
            PlaceStory,
            Complete
        };

        public TutorialStep CurrentTutorialStep
        {
            get
            {
                return _currentTutorialStep;
            }
        }

        //----------- Private Members -----------

        [SerializeField] private PlacementController _placementController;
        [SerializeField] private AppController _appController;
        private TutorialStep _currentTutorialStep = TutorialStep.SummonUi;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _placementController.OnPlacedObject += HandleOnPlaceObject;
            _placementController.OnInitializePlacement += HandleOnInitializePlacement;
            _appController.OnSummonUi += HandleOnSummonUi;
            RaiseTutorialStepChanged();
        }
        
        private void OnDisable()
        {
            _placementController.OnPlacedObject -= HandleOnPlaceObject;
            _placementController.OnInitializePlacement -= HandleOnInitializePlacement;
            _appController.OnSummonUi -= HandleOnSummonUi;
        }

        //----------- Event Handlers -----------

        private void HandleOnSummonUi()
        {
            if (TutorialCompleted())
            {
                return;
            }

            if (_currentTutorialStep == TutorialStep.SummonUi)
            {
                _currentTutorialStep = TutorialStep.SelectObject;
                RaiseTutorialStepChanged();
            }
        }

        private void HandleOnInitializePlacement()
        {
            if (TutorialCompleted())
            {
                return;
            }

            if (_currentTutorialStep == TutorialStep.SelectObject)
            {
                _currentTutorialStep = TutorialStep.PlaceStory;
            }
            RaiseTutorialStepChanged();
        }

        private void HandleOnPlaceObject(GameObject go)
        {
            if (TutorialCompleted())
            {
                return;
            }

            if (_currentTutorialStep == TutorialStep.PlaceStory)
            {
                _currentTutorialStep = TutorialStep.Complete;
            }

            RaiseTutorialStepChanged();
        }

        //----------- Private Methods -----------

        private bool TutorialCompleted()
        {
            return _currentTutorialStep == TutorialStep.Complete;
        }

        private void RaiseTutorialStepChanged()
        {
            OnChangeTutorialStep?.Invoke(_currentTutorialStep);
        }
    }
}
