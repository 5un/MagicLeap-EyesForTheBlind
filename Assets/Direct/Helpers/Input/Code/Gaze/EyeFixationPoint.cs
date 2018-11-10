// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;

/// <summary>
/// This class set the position of the object to the eyes fixation point returned from MLEyes API.
/// </summary>
public class EyeFixationPoint : MonoBehaviour
{

    //----------- MonoBehaviour Methods -----------

    private void Start () 
    {
        MLResult result = MLEyes.Start();
        if(!result.IsOk)
        {
            Debug.LogError("MLEyes not start");
        }
    }
    
    private void Update () 
    {
        if(!MLEyes.IsStarted)
        {
            return;
        }
        transform.position = MLEyes.FixationPoint;
    }
}
