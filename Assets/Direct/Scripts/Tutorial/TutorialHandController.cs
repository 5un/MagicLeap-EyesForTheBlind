// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicKit;
using UnityEngine;

/// <summary>
/// Sets animator parameters and hand geometry visibility to match the current tutorial state.
/// </summary>
public class TutorialHandController : MonoBehaviour
{

    //----------- Private Members -----------
	
    [SerializeField] private Animator _handAnimator;
    [SerializeField] private GameObject _handMesh;
    [SerializeField] private Tutorial _tutorial;
    private bool _hovered;

    //----------- MonoBehaviour Methods -----------

    private void Awake()
    {
        Tutorial.OnChangeTutorialStep += HandleTutorialStepChanged;
        IconController.OnHovered += HandleOnHovered;
        IconController.OnUnhovered += HandleOnUnhovered;
    }

    private void OnDestroy()
    {
        Tutorial.OnChangeTutorialStep -= HandleTutorialStepChanged;
        IconController.OnHovered -= HandleOnHovered;
        IconController.OnUnhovered -= HandleOnUnhovered;
    }

    //----------- Event Handlers -----------

    private void HandleTutorialStepChanged(Tutorial.TutorialStep tutorialStep)
    {
        if (tutorialStep == Tutorial.TutorialStep.SummonUi)
        {
            _handMesh.SetActive(true);
            _handAnimator.Play("Point");
        }

        if (tutorialStep == Tutorial.TutorialStep.SelectObject)
        {
            if (!_hovered)
            {
                _handMesh.SetActive(false);
            }
            _handAnimator.Play("OK");
        }

        if (tutorialStep == Tutorial.TutorialStep.PlaceStory)
        {
            _handMesh.SetActive(true);
            _handAnimator.Play("Open Hand Back");
        }

        if (tutorialStep == Tutorial.TutorialStep.Complete)
        {
            _handMesh.SetActive(false);
        }
    }

    private void HandleOnHovered()
    {
        _hovered = true;
        if (_tutorial.CurrentTutorialStep == Tutorial.TutorialStep.SelectObject)
        {
            _handMesh.SetActive(true);
        }
    }

    private void HandleOnUnhovered()
    {
        _hovered = false;
        if (_tutorial.CurrentTutorialStep == Tutorial.TutorialStep.SelectObject)
        {
            _handMesh.SetActive(false);
        }
    }
}
