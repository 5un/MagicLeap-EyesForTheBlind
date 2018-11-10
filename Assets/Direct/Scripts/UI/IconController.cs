// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine;

namespace MagicKit
{

public class IconController : MonoBehaviour
    {

        public static event Action OnHovered;
        public static event Action OnUnhovered;

        //----------- Private Members -----------

        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _storyPrefab;
        private const float PlaySpeed = 1.0f;
        private const float PauseSpeed = 0.0f;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        private void OnEnable()
        {
            _animator.Play("Scene Active");
        }

        private void OnDisable()
        {
            OnDeactivate();
        }

        //----------- Public Methods -----------

        public GameObject GetStoryPrefab()
        {
            return _storyPrefab;
        }

        //----------- Event Handlers -----------

        public void OnTarget()
        {
            _animator.SetFloat("AnimSpeed", PlaySpeed);
            OnHovered?.Invoke();
        }

        public void OnDeactivate()
        {
            _animator.SetFloat("AnimSpeed", PauseSpeed);
            OnUnhovered?.Invoke();
        }
    }
}