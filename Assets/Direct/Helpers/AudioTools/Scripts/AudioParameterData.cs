// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace MagicLeap.Utilities
{
    /// <summary>
    /// Audio parameter data / definition.
    /// </summary>
    [CreateAssetMenu(fileName = "NewParameter", menuName = "Audio/Audio Parameter")]
    public class AudioParameterData : ScriptableObject
    {
        // Name of this audio parameter.
        public string parameterName;

        // Which audio properties should be adjusted and how.
        public List<AudioPropertyCurve> audioPropertyCurves;
    }
}
