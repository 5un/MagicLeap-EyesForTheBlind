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
    /// Example for toggling a looping sound and setting a parameter based on distance to listener.
    /// </summary>
    public class SphereAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private GameObject _sphere;

        private AudioListener _listener;
        private AudioEvent _sphereEvent;
        private SphereTween _sphereTween;

        //----------- MonoBehaviour Methods -----------

        void Start()
        {
            _listener = FindObjectOfType<AudioListener>();
            _sphereTween = _sphere.GetComponent<SphereTween>();
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Alpha5) || Input.GetKeyUp(KeyCode.Keypad5))
            {
                ToggleLoop();
            }

            if (_sphereEvent != null && _sphereEvent.audioSource.isPlaying)
            {
                SetDistanceParameter();
            }
        }

        //----------- Private Methods -----------

        /// <summary>
        /// Plays sound attached to _sphere and sets reference to _sphereEvent
        /// </summary>
        private void ToggleLoop()
        {
            if (_sphereEvent == null || !_sphereEvent.audioSource.isPlaying)
            {
                _sphereEvent = PlaySoundAttached("engine_loop", _sphere);
                _sphereTween.StartTween();
            }
            else if (_sphereEvent != null && _sphereEvent.audioSource.isPlaying)
            {
                StopSound(_sphereEvent);
                _sphereEvent = null;
                _sphereTween.StopTween();
            }
        }

        /// <summary>
        /// Example method for setting a parameter on an AudioEvent.
        /// </summary>
        private void SetDistanceParameter()
        {
            float distanceToListener = Vector3.Distance(_listener.transform.position, _sphere.transform.position);
            SetParameter(_sphereEvent, "Distance", distanceToListener);
        }
    }
}