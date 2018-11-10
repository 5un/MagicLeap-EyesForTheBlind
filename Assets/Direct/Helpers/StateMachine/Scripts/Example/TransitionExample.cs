// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicLeap.Utilities;

///<summary>
/// Example to show how to use a state in the state machine.
///</summary>
[RequireComponent(typeof(State))]
public class TransitionExample : MonoBehaviour 
{

    //----------- Private Members -----------
    private State _state;

    //----------- MonoBehaviour Methods -----------
    private void Awake()
    {
        _state = GetComponent<State>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _state.parentStateMachine.GoToNext();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _state.parentStateMachine.GoToPrevious();
        }
    }

    private void OnEnable()
    {
        Debug.Log("Enabled : " + _state.name);
    }

    private void OnDisable()
    {
        Debug.Log("Disabled : " + _state.name);
    }
}
