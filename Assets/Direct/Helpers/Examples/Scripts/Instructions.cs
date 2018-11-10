// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using MagicLeap.Utilities;
using UnityEngine;

namespace MagicKit
{

    ///<summary>
    /// Manages the state of instructions in example scenes.
    ///</summary>
    public class Instructions : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] private ControllerInput _controllerInput;
        [SerializeField] private GameObject _instructionObj;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _controllerInput.OnBumperDown += OnBumperDown;
        }

        private void OnDestroy()
        {
            _controllerInput.OnBumperDown -= OnBumperDown;
        }

        //----------- Event Handlers -----------

        private void OnBumperDown()
        {
            _instructionObj.SetActive(!_instructionObj.activeSelf);
        }
    }
}