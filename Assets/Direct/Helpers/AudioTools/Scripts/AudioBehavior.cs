// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MagicLeap.Utilities
{
    /// <summary>
    /// Base audio behavior class.
    /// </summary>
    public class AudioBehavior : MonoBehaviour
    {
        //----------- Public Events -----------

        public static event System.Action<AudioEvent> OnPlaySound;

        //----------- Public Members -----------

        public static Dictionary<AudioSource, AudioEvent> allAudioEvents
        {
            get
            {
                return _allAudioEvents;
            }
        }

        //----------- Private Members -----------

        [SerializeField]
        // Main audio source.
        private AudioSource _audioSource;

        [SerializeField]
        [Range(0, 10)]
        private int _audioSourcePoolSize = 1;

        [SerializeField]
        private bool _preloadAudioSources = false;

        [SerializeField]
        // Audio event definitions in the inspector.
        private List<AudioEventData> _audioEventData;

        [SerializeField]
        // Audio parameter definitions in the inspector.
        private List<AudioParameterData> _audioParameterData;

        // Local registry of all audio events for this Audio Behavior.
        private Dictionary<AudioSource, AudioEvent> _audioEvents = new Dictionary<AudioSource, AudioEvent>();

        // Global registry of all audio events across all Audio Behaviors.
        private static Dictionary<AudioSource, AudioEvent> _allAudioEvents = new Dictionary<AudioSource, AudioEvent>();

        // List of audio sources in the pool.
        private List<AudioSource> _audioSources = new List<AudioSource>();
        private int _currentAudioSourceIndex = 0;

        // Cache of events, indexed by event name.
        private Dictionary<string, AudioEventData> _audioEventsCache = new Dictionary<string, AudioEventData>();
        private bool _audioEventsInitialized = false;

        // Cache of audio sources and their starting settings, indexed by unique object ID of the audio source.
        private Dictionary<AudioSource, AudioSourceData> _audioSourcesCache = new Dictionary<AudioSource, AudioSourceData>();
        private bool _audioSourcesInitialized = false;

        // Cache of parameters, indexed by parameter name.
        private Dictionary<string, AudioParameterData> _audioParametersCache = new Dictionary<string, AudioParameterData>();
        private bool _audioParametersInitialized = false;

        // Getter of audio events cache, includes initialization.
        public Dictionary<string, AudioEventData> AudioEventsInfo
        {
            get
            {
                if (!_audioEventsInitialized)
                {
                    InitAudioEvents();
                }
                return _audioEventsCache;
            }
        }

        // Getter of audio sources cache, includes initialization.
        protected Dictionary<AudioSource, AudioSourceData> _audioSourcesInfo
        {
            get
            {
                if (!_audioSourcesInitialized)
                {
                    InitAudioSources();
                }
                return _audioSourcesCache;
            }
        }

        // Getter of audio parameters cache, includes initialization.
        private Dictionary<string, AudioParameterData> _audioParametersInfo
        {
            get
            {
                if (!_audioParametersInitialized)
                {
                    InitAudioParameters();
                }
                return _audioParametersCache;
            }
        }

        //----------- Public Methods -----------

        /// <summary>
        /// Plays the first sound available.
        /// </summary>
        /// <returns></returns>
        public AudioEvent PlaySound()
        {
            // Use the first event available.
            string eventName = _audioEventData[0].eventName;
            return PlaySound(eventName);
        }

        /// <summary>
        /// Play the first sound available on the specified audio source.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <returns></returns>
        public AudioEvent PlaySound(AudioSource audioSource)
        {
            // Use the first event available.
            string eventName = _audioEventData[0].eventName;
            return PlaySound(eventName, audioSource);
        }

        /// <summary>
		/// Plays the specified event.
		/// </summary>
        public AudioEvent PlaySound(string eventName, AudioSource audioSource = null)
        {
            AudioEvent audioEvent = PrepareAudioEvent(eventName, audioSource);
            audioEvent.audioSource.Play();
            if (OnPlaySound != null)
            {
                OnPlaySound(audioEvent);
            }
            return audioEvent;
        }

        /// <summary>
        /// Stops the sound on a specified Audio Event.
        /// </summary>
        /// <param name="audioEvent">Audio event.</param>
        public void StopSound(AudioEvent audioEvent)
        {
            audioEvent.audioSource.Stop();
        }

        /// <summary>
        /// Stops the sound by Audio Source.
        /// </summary>
        /// <param name="audioSource">Audio source.</param>
        public void StopSound(AudioSource audioSource)
        {
            if (_audioEvents.ContainsKey(audioSource))
            {
                StopSound(_audioEvents[audioSource]);
            }
            else
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Stops and removed sound on a specified Audio Event. Removes sound source from any parents. 
        /// Removes Audio Event from registries.
        /// </summary>
        /// <param name="audioEvent">Audio event.</param>
        public void StopAndRemoveSound(AudioEvent audioEvent)
        {
            StopSound(audioEvent);

            audioEvent.audioSource.transform.parent = null;
            audioEvent.audioSource.transform.rotation = Quaternion.identity;

            // Remove event from registries.
            if (_audioEvents.ContainsKey(audioEvent.audioSource))
            {
                _audioEvents.Remove(audioEvent.audioSource);
            }
            if (_allAudioEvents.ContainsKey(audioEvent.audioSource))
            {
                _allAudioEvents.Remove(audioEvent.audioSource);
            }
        }

        /// <summary>
        /// Stops and removed sound on a specified Audio Source. Removes sound source from any parents. 
        /// Removes Audio Event from registries.
        /// </summary>
        /// <param name="audioSource">Audio source.</param>
        public void StopAndRemoveSound(AudioSource audioSource)
        {
            if (_audioEvents.ContainsKey(audioSource))
            {
                StopAndRemoveSound(_audioEvents[audioSource]);
            }
            else
            {
                audioSource.Stop();
                audioSource.transform.parent = null;
            }
        }

        /// <summary>
        /// Plays the specified event, and fades in sound over a period of time.
        /// </summary>
        /// <returns>Audio event.</returns>
        /// <param name="eventName">Event name.</param>
        /// <param name="fadeInDuration">Fade in duration.</param>
        /// <param name="audioSource">Audio source.</param>
        public AudioEvent PlayAndFadeInSound(string eventName, float fadeInDuration, AudioSource audioSource = null)
        {
            AudioEvent audioEvent = PrepareAudioEvent(eventName, audioSource);
            audioEvent.audioSource.volume = 0.0f;
            audioEvent.audioSource.Play();
            if (OnPlaySound != null)
            {
                OnPlaySound(audioEvent);
            }
            FadeInAudio(audioEvent.audioSource, fadeInDuration);
            return audioEvent;
        }

        /// <summary>
        /// Attach sound source to a game object as a child and play sound.
        /// </summary>
        /// <returns>The sound attached.</returns>
        /// <param name="eventName">Event name.</param>
        /// <param name="attachTo">Attach to.</param>
        /// <param name="audioSource">Audio source.</param>
        public AudioEvent PlaySoundAttached(string eventName, GameObject attachTo, AudioSource audioSource = null)
        {
            AudioEvent audioEvent = PrepareAudioEvent(eventName, audioSource);
            audioEvent.audioSource.transform.SetParent(null);
            audioEvent.audioSource.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            audioEvent.audioSource.transform.SetParent(attachTo.transform, false);
            audioEvent.audioSource.transform.localPosition = Vector3.zero;
            audioEvent.audioSource.Play();
            if (OnPlaySound != null)
            {
                OnPlaySound(audioEvent);
            }
            return audioEvent;
        }

        public AudioEvent PlaySoundAt(string eventName, GameObject playAt, AudioSource audioSource = null)
        {
            AudioEvent audioEvent = PrepareAudioEvent(eventName, audioSource);
            audioEvent.audioSource.transform.SetParent(null);
            audioEvent.audioSource.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            audioEvent.audioSource.transform.position = playAt.transform.position;
            audioEvent.audioSource.transform.rotation = playAt.transform.rotation;
            audioEvent.audioSource.Play();
            if (OnPlaySound != null)
            {
                OnPlaySound(audioEvent);
            }
            return audioEvent;
        }

        public AudioEvent PlaySoundAt(string eventName, Vector3 playAt, AudioSource audioSource = null)
        {
            AudioEvent audioEvent = PrepareAudioEvent(eventName, audioSource);
            audioEvent.audioSource.transform.SetParent(null);
            audioEvent.audioSource.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            audioEvent.audioSource.transform.position = playAt;
            audioEvent.audioSource.Play();
            if (OnPlaySound != null)
            {
                OnPlaySound(audioEvent);
            }
            return audioEvent;
        }

        /// <summary>
        /// Prepare and return the next available audio source.
        /// </summary>
        /// <returns></returns>
        public AudioSource AllocateAudioSource()
        {
            AudioSource audioSource = GetNextAudioSource();
            InitAudioSource(audioSource);
            return audioSource;
        }

        /// <summary>
		/// Set new volume. This value will be scaled according to the sound-designer-provided volume setting.
		/// </summary>
		/// <param name="newVolume">New volume.</param>
		/// <param name="audioSource">Optional audio source. If not provided, will use the default audio source.</param>
        public void SetSourceVolume(float newVolume, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                if (_audioSource == null)
                {
                    Debug.LogError("You must assign an Audio Source.", gameObject);
                    return;
                }
                audioSource = _audioSource;
            }
            audioSource.volume = _audioEvents[audioSource].volume * newVolume;
        }

        /// <summary>
        /// Fades the out an audio source with specified fade duration.
        /// </summary>
        /// <param name="audioSource">Which audio source to fade out.</param>
        /// <param name="fadeDuration">How long to fade it out over.</param>
        /// <param name="stopAfterFade">Whether to stop the playback after fadeout.</param>
        public void FadeOutAudio(AudioSource audioSource = null, float fadeDuration = 1.0f, bool stopAfterFade = true)
        {
            // If Audio Source isn't specified, use the default one.
            if (audioSource == null)
            {
                if (_audioSource == null)
                {
                    Debug.LogError("You must assign an Audio Source.", gameObject);
                    return;
                }
                audioSource = _audioSource;
            }

            // Are we tracking this audio source?
            bool managedSource = false;
            if (_audioEvents.ContainsKey(audioSource))
            {
                managedSource = true;

                // If this sound source is already fading out, do nothing.
                if (_audioEvents[audioSource].fadeout != null)
                {
                    return;
                }

                // If this source is already fading in, stop it.
                if (_audioEvents[audioSource].fadein != null)
                {
                    StopCoroutine(_audioEvents[audioSource].fadein);
                    _audioEvents[audioSource].fadein = null;
                }
            }

            // Save the reference to the coroutine, so we can stop it later, if needed.
            if (managedSource)
            {
                Coroutine fadeout = StartCoroutine(PerformEventFadeOut(fadeDuration, _audioEvents[audioSource], stopAfterFade));
                _audioEvents[audioSource].fadeout = fadeout;
            }
            else
            {
                StartCoroutine(PerformAudioFadeOut(fadeDuration, audioSource, stopAfterFade));
            }
        }

        /// <summary>
        /// Fades the in an audio source with specified fade duration.
        /// </summary>
        /// <param name="audioSource">Audio source to fade in.</param>
        /// <param name="fadeDuration">Fade duration.</param>
        public void FadeInAudio(AudioSource audioSource = null, float fadeDuration = 1.0f)
        {
            // If Audio Source isn't specified, use the default one.
            if (audioSource == null)
            {
                if (_audioSource == null)
                {
                    Debug.LogError("You must assign an Audio Source.", gameObject);
                    return;
                }
                audioSource = _audioSource;
            }

            // Are we tracking this audio source?
            bool managedSource = false;
            if (_audioEvents.ContainsKey(audioSource))
            {
                managedSource = true;

                // If this sound source is already fading in, do nothing.
                if (_audioEvents[audioSource].fadein != null)
                {
                    return;
                }

                // If this source is already fading out, stop it.
                if (_audioEvents[audioSource].fadeout != null)
                {
                    StopCoroutine(_audioEvents[audioSource].fadeout);
                    _audioEvents[audioSource].fadeout = null;
                }
            }

            // Save the reference to the coroutine, so we can stop it later, if needed.
            Coroutine fadein = StartCoroutine(PerformAudioFadeIn(fadeDuration, audioSource));
            if (managedSource)
            {
                _audioEvents[audioSource].fadein = fadein;
            }
        }

        /// <summary>
        /// Set parameter on an audio event.
        /// </summary>
        /// <param name="audioEvent">Audio event reference.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        public void SetParameter(AudioEvent audioEvent, string parameterName, float parameterValue)
        {
            // Do nothing if the audio source isn't playing.
            if (!audioEvent.audioSource.isPlaying)
            {
                return;
            }

            // See if we have a valid parameter.
            if (!_audioParametersInfo.ContainsKey(parameterName))
            {
                Debug.LogError("Parameter " + parameterName.ToString() + " is not defined for this audio behavior.");
                return;
            }

            if (!AudioParameterManager.Instance)
            {
                Debug.LogError("You must have an AudioParameterManager in the scene.");
                return;
            }

            AudioParameterManager.Instance.SetParameter(audioEvent, _audioParametersInfo[parameterName], _audioSourcesInfo[audioEvent.audioSource], parameterValue);
        }

        //----------- MonoBehavior Methods -----------

        private void Reset()
        {
            _audioEventData = new List<AudioEventData>()
            {
                new AudioEventData()
            };
        }

        //----------- Private Methods -----------

        private AudioEvent PrepareAudioEvent(string eventName, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                audioSource = GetNextAudioSource();
            }

            InitAudioSource(audioSource);

            audioSource.clip = GetAudioClip(eventName);

            if (audioSource.clip == null)
            {
                return new AudioEvent();
            }

            audioSource.pitch = _audioSourcesInfo[audioSource].defaultPitch * GetPitch(eventName);
            audioSource.volume = _audioSourcesInfo[audioSource].defaultVolume * GetVolume(eventName);
            audioSource.loop = GetLooping(eventName);

            AudioEvent audioEvent = new AudioEvent()
            {
                eventName = eventName,
                audioSource = audioSource,
                clip = audioSource.clip,
                audioBehavior = this,
                pitch = audioSource.pitch,
                volume = audioSource.volume,
                loop = audioSource.loop
            };

            // Save the playing event info to the local registry...
            if (_audioEvents.ContainsKey(audioSource))
            {
                _audioEvents[audioSource] = audioEvent;
            }
            else
            {
                _audioEvents.Add(audioSource, audioEvent);
            }

            // ...and the global registry
            if (_allAudioEvents.ContainsKey(audioSource))
            {
                _allAudioEvents[audioSource] = audioEvent;
            }
            else
            {
                _allAudioEvents.Add(audioSource, audioEvent);
            }

            PostPrepareAudioEvent(ref audioEvent);

            return audioEvent;
        }

        /// <summary>
        /// Post-processing on a new audio event. Meant to be overridden by child classes.
        /// </summary>
        /// <param name="audioEvent">Reference to the audio event to be post processed.</param>
        protected virtual void PostPrepareAudioEvent(ref AudioEvent audioEvent) {}

        private AudioSource GetNextAudioSource()
        {
            InitAudioSources();

            // If the pool isn't unlimited, and we have reached the end of the pool, start reusing from the beginning.
            if (_audioSourcePoolSize > 0 && _currentAudioSourceIndex >= _audioSourcePoolSize)
            {
                _currentAudioSourceIndex = 0;
            }

            // If next audio source isn't available, create it.
            if (_audioSources.Count <= _currentAudioSourceIndex)
            {
                CreateNewAudioSource();
            }

            AudioSource audioSource = _audioSources[_currentAudioSourceIndex];
            _currentAudioSourceIndex++;

            return audioSource;
        }

        private void CreateNewAudioSource()
        {
            AudioSource newAudiosource;

            // Make sure that the main audio source is on a separate game object, so that we don't end up with cloning madness.
            if (_audioSources[0].transform == gameObject.transform)
            {
                Debug.LogError("In order to use the audio pool functionality, please place the Audio Source on a separate child object", gameObject);
                return;
            }

            newAudiosource = (AudioSource)Instantiate(_audioSources[0]);
            newAudiosource.transform.SetParent(gameObject.transform, false);
            newAudiosource.transform.localPosition = _audioSources[0].transform.localPosition;
            newAudiosource.transform.localRotation = _audioSources[0].transform.localRotation;

            // Copy default volume and pitch values from the original audio source.
            InitAudioSource(_audioSources[0]);

            newAudiosource.pitch = _audioSourcesInfo[_audioSources[0]].defaultPitch;
            newAudiosource.volume = _audioSourcesInfo[_audioSources[0]].defaultVolume;

            InitAudioSource(newAudiosource);
            _audioSources.Add(newAudiosource);
        }

        private AudioClip GetAudioClip(string eventName)
        {
            if (!AudioEventsInfo.ContainsKey(eventName))
            {
                Debug.LogError("Please assign audio clips for " + eventName, gameObject);
                return null;
            }

            return AudioEventsInfo[eventName].audioClips[Random.Range(0, AudioEventsInfo[eventName].audioClips.Length)];
        }

        private bool GetLooping(string eventName)
        {
            if (!AudioEventsInfo.ContainsKey(eventName))
            {
                Debug.LogError("Please assign audio clips for " + eventName, gameObject);
                return false;
            }

            return AudioEventsInfo[eventName].loop;
        }

        private AudioEventData GetAudioClipData(string eventName)
        {
            if (!AudioEventsInfo.ContainsKey(eventName))
            {
                Debug.LogError("Please assign audio clips for " + eventName, gameObject);
                return null;
            }

            return AudioEventsInfo[eventName];
        }

        private float GetPitch(string eventName)
        {
            AudioEventData clipData = GetAudioClipData(eventName);

            float pitch = clipData.pitch;

            if (clipData.pitchVariance > 0.0f)
            {
                pitch = Mathf.Clamp(Random.Range(clipData.pitch - clipData.pitchVariance, clipData.pitch + clipData.pitchVariance), 0.01f, 3.0f);
            }

            return pitch;
        }

        private float GetVolume(string eventName)
        {
            AudioEventData clipData = GetAudioClipData(eventName);

            float volume = clipData.volume;

            if (clipData.volumeVariance > 0.0f)
            {
                volume = Mathf.Clamp(Random.Range(clipData.volume - clipData.volumeVariance, clipData.volume + clipData.volumeVariance), 0.01f, 1.0f);
            }

            return volume;
        }

        private void InitAudioSources()
        {
            if (_audioSourcesInitialized)
            {
                return;
            }
            _audioSourcesInitialized = true;

            // Make sure we have at least one audio source attached.
            if (_audioSource != null)
            {
                _audioSources.Add(_audioSource);
                InitAudioSource(_audioSources[0]);
            }
            else
            {
                Debug.LogError("You must assign an Audio Source.", gameObject);
            }

            // Preload audio sources, if needed.
            if (_preloadAudioSources)
            {
                for (int i = 0; i < _audioSourcePoolSize; ++ i)
                {
                    GetNextAudioSource();
                }
            }
        }

	    /// <summary>
        /// Validates and organizes audio event definitions for easy access.
        /// </summary>
        private void InitAudioEvents()
        {
            if (_audioEventsInitialized)
            {
                return;
            }
            _audioEventsInitialized = true;

            for (int i = 0; i < _audioEventData.Count; ++i)
            {
                if (string.IsNullOrEmpty(_audioEventData[i].eventName))
                {
                    Debug.LogError("Please assign a key to the audio clip list.", gameObject);
                }
                else if (_audioEventsCache.ContainsKey(_audioEventData[i].eventName))
                {
                    Debug.LogError("Please assign unique keys to the audio clips.", gameObject);
                }
                else
                {
                    if (_audioEventData[i].audioClips.Length == 0)
                    {
                        Debug.LogError("Please assign at least one audio clip to the AudioHandler", gameObject);
                    }
                    else
                    {
                        for (int j = 0; j < _audioEventData[i].audioClips.Length; ++j)
                        {
                            if (_audioEventData[i].audioClips[j] == null)
                            {
                                Debug.LogError("Please assign audio clips to all available slots.", gameObject);
                            }
                        }
                    }
                    _audioEventsCache.Add(_audioEventData[i].eventName, _audioEventData[i]);
                }
            }
        }

        /// <summary>
        /// Validates and organizes audio parameter definitions for easy access.
        /// </summary>
        private void InitAudioParameters()
        {
            if (_audioParametersInitialized)
            {
                return;
            }
            _audioParametersInitialized = true;

            // Add built-in fade-in and fade-out parameters.
            AudioParameterData fadeInParameter = ScriptableObject.CreateInstance("AudioParameterData") as AudioParameterData;
            fadeInParameter.parameterName = AudioParameterManager.FADE_IN;
            fadeInParameter.audioPropertyCurves = new List<AudioPropertyCurve>();
            fadeInParameter.audioPropertyCurves.Add(Resources.Load<AudioPropertyCurve>("FadeInCurve_Default"));
            _audioParameterData.Add(fadeInParameter);

            AudioParameterData fadeOutParameter = ScriptableObject.CreateInstance("AudioParameterData") as AudioParameterData;
            fadeOutParameter.parameterName = AudioParameterManager.FADE_OUT;
            fadeOutParameter.audioPropertyCurves = new List<AudioPropertyCurve>();
            fadeOutParameter.audioPropertyCurves.Add(Resources.Load<AudioPropertyCurve>("FadeOutCurve_Default"));
            _audioParameterData.Add(fadeOutParameter);

            for (int i = 0; i < _audioParameterData.Count; ++i)
            {
                if (string.IsNullOrEmpty(_audioParameterData[i].parameterName))
                {
                    Debug.LogError("Please name your parameters.", gameObject);
                }
                else if (_audioParametersCache.ContainsKey(_audioParameterData[i].parameterName))
                {
                    Debug.LogError("Please assign unique names to parameters.", gameObject);
                }
                else
                {
                    _audioParametersCache.Add(_audioParameterData[i].parameterName, _audioParameterData[i]);
                }
            }
        }

        private void InitAudioSource(AudioSource audioSource)
        {
            if (_audioSourcesCache.ContainsKey(audioSource))
            {
                return;
            }

            AudioSourceData data = new AudioSourceData
            {
                audioSource = audioSource,
                defaultPitch = audioSource.pitch,
                defaultVolume = audioSource.volume
            };

            PostInitAudioSource(ref audioSource);

            _audioSourcesCache.Add(audioSource, data);
        }

        /// <summary>
        /// Post-processing on a fresh audio source.
        /// Meant to be extended by child classes.
        /// </summary>
        /// <param name="audioSource">Audio source.</param>
        protected virtual void PostInitAudioSource(ref AudioSource audioSource) {}

        //----------- Coroutines -----------

        /// <summary>
        /// Coroutine that performs the audio fade out.
        /// </summary>
        /// <returns>The audio fade.</returns>
        /// <param name="fadeDuration">Fade duration.</param>
        /// <param name="audioSource">Audio source to fade out.</param>
        /// <param name="stopAfterFade">Whether to stop the sound after fadeout.</param>
        private IEnumerator PerformAudioFadeOut(float fadeDuration, AudioSource audioSource, bool stopAfterFade = true)
        {
            InitAudioSource(audioSource);
            float defaultVolume = _audioSourcesInfo[audioSource].defaultVolume;

            while (audioSource.volume > 0.0f)
            {
                audioSource.volume -= defaultVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            if (stopAfterFade)
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Performs the event fade out.
        /// </summary>
        /// <returns>The event fade out.</returns>
        /// <param name="fadeDuration">Fade duration.</param>
        /// <param name="audioEvent">Audio event.</param>
        /// <param name="stopAfterFade">If set to <c>true</c> stop after fade.</param>
        private IEnumerator PerformEventFadeOut(float fadeDuration, AudioEvent audioEvent, bool stopAfterFade = true)
        {
            InitAudioSource(audioEvent.audioSource);

            float elapsedTime = 0.0f;
            while (elapsedTime < fadeDuration)
            {
                SetParameter(audioEvent, AudioParameterManager.FADE_OUT, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (stopAfterFade)
            {
                audioEvent.audioSource.Stop();
            }

            _audioEvents[audioEvent.audioSource].fadeout = null;
        }

        /// <summary>
        /// Coroutine that performs the audio fade in.
        /// </summary>
        /// <returns>The audio fade.</returns>
        /// <param name="fadeDuration">Fade duration.</param>
        /// <param name="audioSource">Audio source to fade in.</param>
        private IEnumerator PerformAudioFadeIn(float fadeDuration, AudioSource audioSource)
        {
            InitAudioSource(audioSource);
            float defaultVolume = _audioSourcesInfo[audioSource].defaultVolume;

            while (audioSource.volume < defaultVolume)
            {
                audioSource.volume += defaultVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }

            _audioEvents[audioSource].fadein = null;
        }
    }
}
