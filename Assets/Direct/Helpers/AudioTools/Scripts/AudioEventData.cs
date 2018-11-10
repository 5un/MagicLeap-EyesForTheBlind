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
    [CreateAssetMenu(fileName = "NewAudioEvent", menuName = "Audio/Audio Event")]

    public class AudioEventData : ScriptableObject
    {
        // Name of this audio event.
        public string eventName;

        // List of audio clips to choose from at random for this event.
        public AudioClip[] audioClips;

        [Range(-3.0f, 3.0f)] public float pitch = 1.0f;

        [Range(0.0f, 1.0f)] public float pitchVariance;

        [Range(0.0f, 1.0f)] public float volume = 1.0f;

        [Range(0.0f, 1.0f)] public float volumeVariance;

        public bool loop = false;
    }
}
