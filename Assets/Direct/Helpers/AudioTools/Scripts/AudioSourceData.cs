// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
#if MSA
using MSA;
#endif

namespace MagicLeap.Utilities
{
    /// <summary>
	/// Audio source data.
	/// </summary>
    [System.Serializable]
    public class AudioSourceData
    {
        //----------- Public Members -----------

        public AudioSource audioSource;

        public float defaultVolume;

        public float defaultPitch;

        public AudioLowPassFilter lowPassFilter
        {
            get
            {
                if (_hasLowPassFilter && _lowPassFilter == null)
                {
                    _lowPassFilter = audioSource.GetComponent<AudioLowPassFilter>();
                    if (_lowPassFilter == null)
                    {
                        _hasLowPassFilter = false;
                    }
                }

                return _lowPassFilter;
            }
        }

        #if MSA
        public MSASource msaSource
        {
            get
            {
                if (_hasMsaSource && _msaSource == null)
                {
                    _msaSource = audioSource.GetComponent<MSASource>();
                    if (_msaSource == null)
                    {
                        _hasMsaSource = false;
                    }
                }

                return _msaSource;
            }
        }
        #endif

        //----------- Private Members -----------
        private AudioLowPassFilter _lowPassFilter;
        private bool _hasLowPassFilter = true;

        #if MSA
        private MSASource _msaSource;
        private bool _hasMsaSource = true;
        #endif
    }
}