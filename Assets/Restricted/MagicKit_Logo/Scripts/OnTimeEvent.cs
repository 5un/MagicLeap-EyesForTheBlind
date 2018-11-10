// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

namespace MagicKit
{

    ///<summary>
    /// Fire an inspector event after x amount of time has passed.
    ///</summary>
    public class OnTimeEvent : MonoBehaviour 
    {
        //----------- Public Events -----------

        public UnityEvent OnTime;

        //----------- Private Members -----------

        [SerializeField] private float _lifeTime = 3.0f;

        private float _timer;
        private bool _done = false;

        //----------- MonoBehaviour Methods -----------

        private void OnEnable()
        {
            _timer = 0;
            _done = false;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if(!_done && _timer > _lifeTime)
            {
                OnTime.Invoke();
                _done = true;
            }
        }
    }
}
