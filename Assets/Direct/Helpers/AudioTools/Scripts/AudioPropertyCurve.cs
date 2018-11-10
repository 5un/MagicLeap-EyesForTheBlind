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
    /// Audio property curve definition.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudioPropertyCurve", menuName = "Audio/Audio Property Curve")]
    public class AudioPropertyCurve : ScriptableObject
    {
        // Type of the audio property.
        public AudioProperty audioProperty;

        // Audio property adjustment curve.

        public AnimationCurve curve;
    }
}