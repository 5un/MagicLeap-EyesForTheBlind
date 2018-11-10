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
    /// Example audio behavior.
    ///</summary>
    public class AudioBehaviorExample : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private AudioSource _loopingAudioSource;

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
            {
                ToggleLoop();
            }

            if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
            {
                TriggerOneshot();
            }
        }

        //----------- Private Methods -----------

        /// <summary>
        /// Toggles the looping sound on and off.
        /// </summary>
        private void ToggleLoop()
        {
            if (!_loopingAudioSource.isPlaying)
            {
                PlayAndFadeInSound("sound_01", 0.25f, _loopingAudioSource);
            }
            else
            {
                FadeOutAudio(_loopingAudioSource, 0.5f);
            }
        }

        private void TriggerOneshot()
        {
            PlaySound("sound_02");
        }
    }
}