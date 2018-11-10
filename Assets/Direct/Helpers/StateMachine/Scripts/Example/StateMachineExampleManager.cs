// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using MagicLeap.Utilities;

///<summary>
/// Example to show how to use the state machine.
///</summary>
[RequireComponent(typeof(StateMachine))]
public class StateMachineExampleManager : MonoBehaviour 
{

    //----------- Private Members -----------
    private StateMachine _stateMachine;

    //----------- MonoBehaviour Methods -----------
    private void Start()
    {
        _stateMachine = GetComponent<StateMachine>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _stateMachine.GoToState(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _stateMachine.GoToState("State (0)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _stateMachine.GoToState("State (1)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _stateMachine.GoToState("State (2)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _stateMachine.GoToState("State (3)");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _stateMachine.GoToState("State (4)");
        }
    }
}
