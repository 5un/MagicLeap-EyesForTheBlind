// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
#if MSA
using MSA;
#endif

namespace MagicLeap.Utilities
{
    //----------- Data Definitions -----------

    public enum AudioProperty
    {
        VOLUME,
        PITCH,
        LP_CUTOFF,
        DIRECT_GAIN
    }

    ///<summary>
    /// Audio parameter.
    ///</summary>
    public class AudioParameterManager : MonoBehaviour
    {
        //----------- Constants ---------------

        public const string FADE_OUT = "__FADE_OUT";
        public const string FADE_IN = "__FADE_IN";

        //----------- Data Structures ---------

        private struct ParameterOperation
        {
            public AudioEvent audioEvent;
            public AudioParameterData audioParameterData;
            public AudioSourceData audioSourceData;
            public float parameterValue;
        }

        //----------- Properties ----------
        public static AudioParameterManager Instance
        {
            get
            {
                return _instance;
            }
        }
        protected static AudioParameterManager _instance;

        //----------- Private Fields ----------

        private List<ParameterOperation> _parameterOperations = new List<ParameterOperation>();

        //----------- Public Events -----------

        public void SetParameter(AudioEvent audioEvent, AudioParameterData audioParameterData, AudioSourceData audioSourceData, float parameterValue)
        {
            ParameterOperation item = new ParameterOperation
            {
                audioEvent = audioEvent,
                audioParameterData = audioParameterData,
                audioSourceData = audioSourceData,
                parameterValue = parameterValue
            };

            _parameterOperations.Add(item);
        }

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            // Make this a singleton.
            if (_instance == null)
            {
                _instance = this;
            }
            else if (this != _instance)
            {
                Debug.LogWarning("Multiple AudioParameterManagers in scene. There can only be one.", gameObject);
            }
        }

        private void LateUpdate()
        {
            AudioEvent audioEvent;
            AudioParameterData audioParameterData;
            AudioSourceData audioSourceData;
            float parameterValue;

            for (int j = 0; j < _parameterOperations.Count; ++j)
            {
                audioEvent = _parameterOperations[j].audioEvent;
                audioParameterData = _parameterOperations[j].audioParameterData;
                audioSourceData = _parameterOperations[j].audioSourceData;
                parameterValue = _parameterOperations[j].parameterValue;

                for (int i = 0; i < audioParameterData.audioPropertyCurves.Count; ++i)
                {
                    AudioPropertyCurve audioPropertyCurve = audioParameterData.audioPropertyCurves[i];
                    bool validAudioProperty = true;
                    float value = audioPropertyCurve.curve.Evaluate(parameterValue);
                    float newValue;

                    // TODO: Clamp values;
                    switch (audioPropertyCurve.audioProperty)
                    {
                        case AudioProperty.VOLUME:
                            newValue = audioEvent.volume;
                            if (audioEvent.volumeLastUpdatedFrame == Time.frameCount)
                            {
                                newValue = audioEvent.audioSource.volume;
                            }
                            audioEvent.volumeLastUpdatedFrame = Time.frameCount;
                            audioEvent.audioSource.volume = newValue * value;
                            break;

                        case AudioProperty.PITCH:
                            newValue = audioEvent.pitch;
                            if (audioEvent.pitchLastUpdatedFrame == Time.frameCount)
                            {
                                newValue = audioEvent.audioSource.pitch;
                            }
                            audioEvent.pitchLastUpdatedFrame = Time.frameCount;

                            audioEvent.audioSource.pitch = newValue * value;
                            break;

                        case AudioProperty.LP_CUTOFF:
                            // TODO: Make work with multiple parameters.
                            if (audioSourceData.lowPassFilter == null)
                            {
                                Debug.Log("Unable to set parameter. Low-pass filter not found.", audioEvent.audioSource);
                                validAudioProperty = false;
                            }
                            audioSourceData.lowPassFilter.cutoffFrequency = value;
                            break;

#if MSA
                    case AudioProperty.DIRECT_GAIN:
                        // TODO: Make work with multiple parameters.
                        if (audioSourceData.msaSource == null)
                        {
                            Debug.Log("Unable to set parameter. MSA source not found.", audioEvent.audioSource);
                            validAudioProperty = false;
                        }
                        audioSourceData.msaSource.DirectGain = Mathf.Clamp01(value);
                        break;
#endif

                        default:
                            validAudioProperty = false;
                            break;
                    }

                    if (validAudioProperty)
                    {
                        audioEvent.StoreAudioPropertyValue(audioPropertyCurve.audioProperty, value);
                    }
                }
            }

            _parameterOperations.Clear();
        }
    }
}