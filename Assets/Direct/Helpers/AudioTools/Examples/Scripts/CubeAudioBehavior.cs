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
    /// <summary>
    /// AudioBehavior that plays sound at specific locations.
    /// </summary>
    public class CubeAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private GameObject _cube01;

        [SerializeField] private GameObject _cube02;

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
            {
                TriggerPlayAttached();
            }

            if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4))
            {
                TriggerPlayAt();
            }
        }

        //----------- Private Methods -----------

        /// <summary>
        /// Plays sound attached to _cube01
        /// AudioSource object will become a child object of _cube01
        /// </summary>
        private void TriggerPlayAttached()
        {
            PlaySoundAttached("sound_01_randPitch", _cube01);
        }

        /// <summary>
        /// Plays sound at the location of _cube02
        /// Sound will remain in the location in which it started playing
        /// AudioSource object will have no parent object
        /// </summary>
        private void TriggerPlayAt()
        {
            PlaySoundAt("sound_02_randVol", _cube02);
        }
    }
}
