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
    /// <summary>
    /// Audio Behavior for handling one-shot Story FX
    /// </summary>
    public class StoryAudioBehavior : AudioBehavior
    {
        //----------- Private Members -----------

        [SerializeField] private AudioSource _loopAudioSource;
        [SerializeField] private Story _story;
        private AudioEvent _storyAudioEvent;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            _story.OnAnimationStart += HandleStoryAnimationStart;
            _story.OnAnimationEnd += HandleStoryAnimationEnd;
        }

        private void OnDisable()
        {
            _story.OnAnimationStart -= HandleStoryAnimationStart;
            _story.OnAnimationEnd -= HandleStoryAnimationEnd;
        }

        //----------- Public Methods -----------

        public void PlayStoryLoop()
        {
            if (_storyAudioEvent == null)
            {
                // Specify a separate source to play the loop to keep it out of the pool.
                // This is necessary in order to keep in sync with the animation when paused/resumed
                _storyAudioEvent = PlaySoundAttached("story_loop", _story.gameObject, _loopAudioSource);
            }
            else
            {
                _storyAudioEvent.audioSource.Play();
            }
        }

        public void PauseStoryLoop()
        {
            if (_storyAudioEvent != null)
            {
                _storyAudioEvent.audioSource.Pause();
            }
        }

        public void PlayActivationFX()
        {
            PlaySoundAttached("story_expand", _story.gameObject);
        }

        public void PlayDeactivationFX()
        {
            PlaySoundAttached("story_collapse", _story.gameObject);
        }

        //----------- Event Handlers -----------

        private void HandleStoryAnimationStart()
        {
            PlaySoundAttached("story_travel", _story.gameObject);
        }

        private void HandleStoryAnimationEnd()
        {
            PlaySoundAttached("story_collide", _story.gameObject);
        }
    }
}