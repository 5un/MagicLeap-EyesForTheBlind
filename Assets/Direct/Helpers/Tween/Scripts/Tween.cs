// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using System;

namespace MagicLeap.Utilities
{
    ///<summary>
    /// Tween utility for developer assistance.
    ///</summary>
    public class Tween : MonoBehaviour
    {
        //----------- Private Members -----------
        private static Tween _instance;

        //----------- Public Members -----------
        public static Tween Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (Tween)FindObjectOfType(typeof(Tween));

                    if (_instance == null)
                    {
                        GameObject singletonCreation = new GameObject(typeof(Tween).Name);
                        _instance = singletonCreation.AddComponent<Tween>();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// What style of loop, if any, should be applied.
        /// </summary>
        public enum LoopType { None, Loop, PingPong };

        /// <summary>
        /// A linear curve.
        /// </summary>
        public static AnimationCurve EaseLinear
        {
            get { return AnimationCurve.Linear(0, 0, 1, 1); }
        }

        /// <summary>
        /// A curve that runs slow in the beginning.
        /// </summary>
        public static AnimationCurve EaseIn
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 1, 0)); }
        }

        /// <summary>
        /// A curve that runs slow in the beginning but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseInStrong
        {
            get { return new AnimationCurve(new Keyframe(0, 0, .03f, .03f), new Keyframe(0.45f, 0.03f, 0.2333333f, 0.2333333f), new Keyframe(0.7f, 0.13f, 0.7666667f, 0.7666667f), new Keyframe(0.85f, 0.3f, 2.233334f, 2.233334f), new Keyframe(0.925f, 0.55f, 4.666665f, 4.666665f), new Keyframe(1, 1, 5.999996f, 5.999996f)); }
        }

        /// <summary>
        /// A curve that runs slow at the end.
        /// </summary>
        public static AnimationCurve EaseOut
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 0, 0)); }
        }

        /// <summary>
        /// A curve that runs slow at the end but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseOutStrong
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 13.80198f, 13.80198f), new Keyframe(0.04670785f, 0.3973127f, 5.873408f, 5.873408f), new Keyframe(0.1421811f, 0.7066917f, 2.313627f, 2.313627f), new Keyframe(0.2483539f, 0.8539293f, 0.9141542f, 0.9141542f), new Keyframe(0.4751028f, 0.954047f, 0.264541f, 0.264541f), new Keyframe(1, 1, .03f, .03f)); }
        }

        /// <summary>
        /// A curve that runs slow in the beginning and at the end.
        /// </summary>
        public static AnimationCurve EaseInOut
        {
            get { return AnimationCurve.EaseInOut(0, 0, 1, 1); }
        }

        /// <summary>
        /// A curve that runs slow in the beginning and the end but with more overall energy.
        /// </summary>
        public static AnimationCurve EaseInOutStrong
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 0.03f, 0.03f), new Keyframe(0.5f, 0.5f, 3.257158f, 3.257158f), new Keyframe(1, 1, .03f, .03f)); }
        }

        /// <summary>
        /// A curve that appears to "charge up" before it moves.
        /// </summary>
        public static AnimationCurve EaseInBack
        {
            get { return new AnimationCurve(new Keyframe(0, 0, -1.1095f, -1.1095f), new Keyframe(1, 1, 2, 2)); }
        }

        /// <summary>
        /// A curve that shoots past its target and springs back into place.
        /// </summary>
        public static AnimationCurve EaseOutBack
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 2, 2), new Keyframe(1, 1, -1.1095f, -1.1095f)); }
        }

        /// <summary>
        /// A curve that appears to "charge up" before it moves, shoots past its target and springs back into place.
        /// </summary>
        public static AnimationCurve EaseInOutBack
        {
            get { return new AnimationCurve(new Keyframe(1, 1, -1.754543f, -1.754543f), new Keyframe(0, 0, -1.754543f, -1.754543f)); }
        }

        /// <summary>
        /// A curve that snaps to its value with a fun spring motion.
        /// </summary>
        public static AnimationCurve EaseSpring
        {
            get { return new AnimationCurve(new Keyframe(0f, -0.0003805831f, 8.990726f, 8.990726f), new Keyframe(0.35f, 1f, -4.303913f, -4.303913f), new Keyframe(0.55f, 1f, 1.554695f, 1.554695f), new Keyframe(0.7730452f, 1f, -2.007816f, -2.007816f), new Keyframe(1f, 1f, -1.23451f, -1.23451f)); }
        }

        /// <summary>
        /// A curve that settles to its value with a fun bounce motion.
        /// </summary>
        public static AnimationCurve EaseBounce
        {
            get { return new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(0.25f, 1, 11.73749f, -5.336508f), new Keyframe(0.4f, 0.6f, -0.1904764f, -0.1904764f), new Keyframe(0.575f, 1, 5.074602f, -3.89f), new Keyframe(0.7f, 0.75f, 0.001192093f, 0.001192093f), new Keyframe(0.825f, 1, 4.18469f, -2.657566f), new Keyframe(0.895f, 0.9f, 0, 0), new Keyframe(0.95f, 1, 3.196362f, -2.028364f), new Keyframe(1, 1, 2.258884f, 0.5f)); }
        }

        /// <summary>
        /// A curve that appears to apply a jolt that wobbles back down to where it was.
        /// </summary>
        public static AnimationCurve EaseWobble
        {
            get { return new AnimationCurve(new Keyframe(0f, 0f, 11.01978f, 30.76278f), new Keyframe(0.08054394f, 1f, 0f, 0f), new Keyframe(0.3153235f, -0.75f, 0f, 0f), new Keyframe(0.5614113f, 0.5f, 0f, 0f), new Keyframe(0.75f, -0.25f, 0f, 0f), new Keyframe(0.9086903f, 0.1361611f, 0f, 0f), new Keyframe(1f, 0f, -4.159244f, -1.351373f)); }
        }

        //----------- Private Members -----------
        public List<TweenObject> tweens = new List<TweenObject>();

        //----------- MonoBehaviour Methods -----------
        private void Update()
        {
            if (tweens.Count == 0) return;
        
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                if (tweens[i].isComplete) continue;

                tweens[i].Update();
                if (tweens[i].percentage == 1)
                {
                    if (!tweens[i].isComplete)
                    {
                        tweens[i].isComplete = true;

                        if (tweens[i].completeCallback != null)
                        {
                            tweens[i].completeCallback();
                        }

                        if (tweens[i].loopType != LoopType.None)
                        {
                            tweens[i].Loop();
                            return;
                        }

                        if (tweens[i].destroyWhenDone)
                        {
                            tweens.RemoveAt(i);
                        }
                    }
                }
            }
        }

        //----------- Public Methods -----------
        public static string Add(float start, float end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<float> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            TweenObject newTweenObject = new TweenObject(start, end, duration, delay, curve, loopType, progressCallback, completeCallback, destroyWhenDone);
            Instance.tweens.Add(newTweenObject);
            return newTweenObject.id;
        }

        public static string Add(Vector2 start, Vector2 end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Vector2> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            TweenObject newTweenObject = new TweenObject(start, end, duration, delay, curve, loopType, progressCallback, completeCallback, destroyWhenDone);
            Instance.tweens.Add(newTweenObject);
            return newTweenObject.id;
        }

        public static string Add(Vector3 start, Vector3 end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Vector3> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            TweenObject newTweenObject = new TweenObject(start, end, duration, delay, curve, loopType, progressCallback, completeCallback, destroyWhenDone);
            Instance.tweens.Add(newTweenObject);
            return newTweenObject.id;
        }

        public static string Add(Color start, Color end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Color> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            TweenObject newTweenObject = new TweenObject(start, end, duration, delay, curve, loopType, progressCallback, completeCallback, destroyWhenDone);
            Instance.tweens.Add(newTweenObject);
            return newTweenObject.id;
        }

        public static string Add(Quaternion start, Quaternion end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Quaternion> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            TweenObject newTweenObject = new TweenObject(start, end, duration, delay, curve, loopType, progressCallback, completeCallback, destroyWhenDone);
            Instance.tweens.Add(newTweenObject);
            return newTweenObject.id;
        }

        public static string Add(TweenObject tweenObject)
        {
            Instance.tweens.Add(tweenObject);
            return tweenObject.id;
        }

        public static TweenObject GetTween(string id)
        {
            for (int i = 0; i < Instance.tweens.Count; i++)
            {
                if (Instance.tweens[i].id == id)
                {
                    return (Instance.tweens[i]);
                }
            }

            return null;
        }
    }
}