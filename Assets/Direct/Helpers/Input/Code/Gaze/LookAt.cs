// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
namespace MagicKit
{
    /// <summary>
    /// The transform this script is attached to will always face towards the target.
    /// </summary>
    public class LookAt : MonoBehaviour
    {
        //----------- Private Members -----------

        [SerializeField] private Transform _target;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            if (_target == null)
            {
                Debug.LogError("No target, please attach a transform for this script to look at.");
                this.enabled = false;
            }
        }

        private void Update()
        {
            transform.LookAt(_target.transform, Vector3.up);
        }
    }
}
