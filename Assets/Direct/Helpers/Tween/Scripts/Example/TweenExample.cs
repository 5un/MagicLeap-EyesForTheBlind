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
using MagicLeap.Utilities;

///<summary>
/// 
///</summary>
public class TweenExample : MonoBehaviour 
{
    //----------- Public Events -----------

    //----------- Public Members -----------
    public Vector3 endPosition = Vector3.up;
    public Transform target;

    //----------- Private Members -----------
    TweenObject _scaleTween;
    string _pulseTweenID;
    bool _rotating;
    Renderer _renderer;

    //----------- MonoBehaviour Methods -----------
    private void Awake()
    {
        //reusable tween that allows rapid calling:
        _scaleTween = new TweenObject(target.localScale, target.localScale, .35f, 0, Tween.EaseOutStrong, Tween.LoopType.None, (value) => target.localScale = value, () => Debug.Log("Scale Complete"), false);
        Tween.Add(_scaleTween);

        //cache:
        _renderer = target.GetComponent<Renderer>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scale Down"))
        {
            _scaleTween.startVector3 = target.localScale;
            _scaleTween.endVector3 = target.localScale * .75f;
            _scaleTween.Reset();
        }

        if (GUILayout.Button("Scale Up"))
        {
            _scaleTween.startVector3 = target.localScale;
            _scaleTween.endVector3 = target.localScale * 1.25f;
            _scaleTween.Reset();
        }

        if (GUILayout.Button("Move to 'endPosition'"))
        {
            Tween.Add(target.position, endPosition, 1, 0, Tween.EaseSpring, Tween.LoopType.None, HandleMoveTween);
        }

        if (GUILayout.Button("Move to 'origin'"))
        {
            Tween.Add(target.position, Vector3.zero, 1, 0, Tween.EaseBounce, Tween.LoopType.None, HandleMoveTween);
        }

        //example of a non-interruptable tween:
        if (GUILayout.Button("Rotate") && !_rotating)
        {
            _rotating = true;
            Quaternion targetRotation = target.rotation * Quaternion.AngleAxis(90, target.up);
            Tween.Add(target.localEulerAngles, targetRotation.eulerAngles, 1, 0, Tween.EaseInOutBack, Tween.LoopType.None, (value) => target.localEulerAngles = value, () => _rotating = false);
        }

        //loop:
        if (GUILayout.Button("Pulse"))
        {
            if (string.IsNullOrEmpty(_pulseTweenID))
            {
                _pulseTweenID = Tween.Add(Color.white, Color.red, .5f, 0, Tween.EaseLinear, Tween.LoopType.PingPong, (value) => _renderer.material.color = value);
            }else{
                TweenObject pulseTween = Tween.GetTween(_pulseTweenID);
                pulseTween.isComplete = false;
            }
        }

        if (GUILayout.Button("Stop Pulse"))
        {
            TweenObject pulseTween = Tween.GetTween(_pulseTweenID);
            pulseTween.isComplete = true;
        }
    }

    //----------- Event Handlers -----------
    private void HandleMoveTween(Vector3 value)
    {
        target.position = value;
    }
}