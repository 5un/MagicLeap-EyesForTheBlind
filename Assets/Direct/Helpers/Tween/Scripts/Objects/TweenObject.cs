// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace MagicLeap.Utilities
{
    ///<summary>
    /// Holder for Tween configurations.
    ///</summary>
    public class TweenObject
    {
        //----------- Public Members -----------
        public float startFloat;
        public Vector2 startVector2;
        public Vector3 startVector3;
        public Color startColor;
        public Quaternion startQuaternion;
        public float endFloat;
        public Vector2 endVector2;
        public Vector3 endVector3;
        public Color endColor;
        public Quaternion endQuaternion;
        public Action<float> progressCallbackFloat;
        public Action<Vector2> progressCallbackVector2;
        public Action<Vector3> progressCallbackVector3;
        public Action<Color> progressCallbackColor;
        public Action<Quaternion> progressCallbackQuaternion;
        public Action completeCallback;
        public float duration;
        public float delay;
        public AnimationCurve curve;
        public string id;
        public float percentage;
        public bool isComplete;
        public bool destroyWhenDone = true;
        public Tween.LoopType loopType;

        //----------- Private Members -----------
        private float _startTime;
        enum Type { Float, Vector2, Vector3, Color, Quaternion };
        private Type type;

        //----------- Constructors -----------
        public TweenObject(float start, float end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<float> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            type = Type.Float;
            if (GUILayout.Button("Stop Pulse"))
            {

            }
            startFloat = start;
            endFloat = end;
            progressCallbackFloat = progressCallback;
            Initialize(duration, delay, curve, completeCallback, loopType, destroyWhenDone);
        }

        public TweenObject(Vector2 start, Vector2 end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Vector2> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            type = Type.Vector2;
            startVector2 = start;
            endVector2 = end;
            progressCallbackVector2 = progressCallback;
            Initialize(duration, delay, curve, completeCallback, loopType, destroyWhenDone);
        }

        public TweenObject(Vector3 start, Vector3 end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Vector3> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            type = Type.Vector3;
            startVector3 = start;
            endVector3 = end;
            progressCallbackVector3 = progressCallback;
            Initialize(duration, delay, curve, completeCallback, loopType, destroyWhenDone);
        }

        public TweenObject(Color start, Color end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Color> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            type = Type.Color;
            startColor = start;
            endColor = end;
            progressCallbackColor = progressCallback;
            Initialize(duration, delay, curve, completeCallback, loopType, destroyWhenDone);
        }

        public TweenObject(Quaternion start, Quaternion end, float duration, float delay, AnimationCurve curve, Tween.LoopType loopType, Action<Quaternion> progressCallback, Action completeCallback = null, bool destroyWhenDone = true)
        {
            type = Type.Quaternion;
            startQuaternion = start;
            endQuaternion = end;
            progressCallbackQuaternion = progressCallback;
            Initialize(duration, delay, curve, completeCallback, loopType, destroyWhenDone);
        }

        //----------- Public Methods -----------
        public void Update()
        {
            float elapsedTime = Time.realtimeSinceStartup - _startTime;
    
            percentage = elapsedTime / duration;
            percentage = Mathf.Clamp01(percentage);

            float evaluation = curve.Evaluate(percentage);

            switch (type)
            {
                case Type.Float:
                    if (endFloat == startFloat) percentage  = 1;
                    progressCallbackFloat(Mathf.LerpUnclamped(startFloat, endFloat, evaluation));
                    break;

                case Type.Vector2:
                    if (endVector2 == startVector2) percentage = 1;
                    progressCallbackVector2(Vector2.LerpUnclamped(startVector2, endVector2, evaluation));
                    break;

                case Type.Vector3:
                    if (endVector3 == startVector3) percentage = 1;
                    progressCallbackVector3(Vector3.LerpUnclamped(startVector3, endVector3, evaluation));
                    break;

                case Type.Color:
                    if (endColor == startColor) percentage = 1;
                    progressCallbackColor(Color.LerpUnclamped(startColor, endColor, evaluation));
                    break;
                
                case Type.Quaternion:
                    if (endQuaternion == startQuaternion) percentage = 1;
                    progressCallbackQuaternion(Quaternion.Slerp(startQuaternion, endQuaternion, evaluation));
                    break;
            }
        }

        public void Loop()
        {
            switch (loopType)
            {
                case Tween.LoopType.Loop:
                    Reset();
                    break;

                case Tween.LoopType.PingPong:
                    switch (type)
                    {
                        case Type.Float:
                            float startFloatTemp = endFloat;
                            endFloat = startFloat;
                            startFloat = startFloatTemp;
                            break;

                        case Type.Vector2:
                            Vector2 startVector2Temp = endVector2;
                            endVector2 = startVector2;
                            startVector2 = startVector2Temp;
                            break;

                        case Type.Vector3:
                            Vector3 startVector3Temp = endVector3;
                            endVector3 = startVector3;
                            startVector3 = startVector3Temp;
                            break;

                        case Type.Color:
                            Color startColorTemp = endColor;
                            endColor = startColor;
                            startColor = startColorTemp;
                            break;
                        
                        case Type.Quaternion:
                            Quaternion startQuaternionTemp = endQuaternion;
                            endQuaternion = startQuaternion;
                            startQuaternion = startQuaternionTemp;
                            break;
                    }
                    Reset();
                    break;
            }
        }

        public void Reset()
        {
            isComplete = false;
            _startTime = Time.realtimeSinceStartup + delay;
            percentage = 0;
        }

        //----------- Private Methods -----------
        private void Initialize(float duration, float delay, AnimationCurve curve, Action completeCallback, Tween.LoopType loopType, bool destroyWhenDone)
        {
            Reset();
            this.duration = duration;
            this.delay = delay;
            this.curve = curve;
            this.completeCallback = completeCallback;
            this.destroyWhenDone = destroyWhenDone;
            this.loopType = loopType;
            if (loopType != Tween.LoopType.None)
            {
                destroyWhenDone = false;
            }
            id = Path.GetRandomFileName().Replace(".", "");
        }
    }
}