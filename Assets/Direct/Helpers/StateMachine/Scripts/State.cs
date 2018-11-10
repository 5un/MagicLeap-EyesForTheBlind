// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicLeap.Utilities
{
    ///<summary>
    /// Contains information about a state and it's parent state machine.
    ///</summary>
    public class State : MonoBehaviour
    {

        //----------- Public Members -----------
        [HideInInspector] public string stateName;
        [HideInInspector] public StateMachine parentStateMachine;

        //----------- Private Members -----------

        //----------- MonoBehaviour Methods -----------
        private void Start()
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Reset();
            }
            parentStateMachine = GetComponentInParent<StateMachine>();
            if (parentStateMachine == null)
            {
                enabled = false;
            }
        }

        private void Reset()
        {
            stateName = name;
        }

        //----------- Event Handlers -----------

        //----------- Public Methods -----------

        //----------- Private Methods -----------

        //----------- Coroutines -----------

    }
}