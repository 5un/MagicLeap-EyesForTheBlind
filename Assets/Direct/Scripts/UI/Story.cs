// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------
using System.Collections;
using MagicLeap.Utilities;
using UnityEngine;
using System;

namespace MagicKit
{
    /// <summary>
    /// Simple class to tween a story from a starting point to an ending point.
    /// </summary>
    public class Story : MonoBehaviour
    {
        //----------- Public Events -----------

        public event Action OnAnimationStart;
        public event Action OnAnimationEnd;
        public static event Action OnIntegrated;
        public event Action OnExpandEnd;

        //----------- Public Members -----------

        public float Duration = 1f;
        [HideInInspector] public Vector3 StartPosition;
        [HideInInspector] public Vector3 EndPosition;

        //----------- Private Members -----------

        [SerializeField] private Collider _collider;
        [SerializeField] private MeshAnchor _anchor;
        [SerializeField] private Interactable _interactable;
        [SerializeField] private Animator _animator;

        private const float PlaySpeed = 1.0f;
        private const float PauseSpeed = 0.0f;


        //----------- MonoBehaviour Methods -----------

        public void Start()
        {
            _animator.Play("Scene Active", 1);
            OnAnimationStart?.Invoke();

            Tween.Add(
                StartPosition, EndPosition, Duration, 0f,
                Tween.EaseInOut, Tween.LoopType.None,
                (value) => transform.position = value, completeCallback: HandlePlacementAnimationComplete
            );

            Vector3 scale = transform.localScale;

            Tween.Add(
                Vector3.zero, scale, Duration, 0f, Tween.EaseLinear, Tween.LoopType.None,
                (value) => transform.localScale = value
            );

            var startRot = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
            var endRot = transform.rotation;

            Tween.Add(
                startRot, endRot, Duration, 0f, Tween.EaseOut, Tween.LoopType.None,
                (value) => transform.rotation = value
            );
            WorldGeometryBounds.Instance.AddSpatialObject(GetComponent<SpatialObject>());
        }

        //----------- Event Handlers ------------

        public void ExpandEnd()
        {
            OnExpandEnd?.Invoke();
        }

        public void OnActivated()
        {
            _animator.SetTrigger("Activate");
        }

        public void OnDeactivated()
        {
            _animator.SetTrigger("Deactivate");
            if (_animator.layerCount > 2)
            {
                string layerName = _animator.GetLayerName(2);
                if (layerName == "Smoke")
                {
                    _animator.SetLayerWeight(2, PlaySpeed);
                }
            }
            _animator.SetFloat("AnimSpeed", PauseSpeed);
        }

        public void OnManipulate() 
        {
            if (_animator.layerCount > 2)
            {
                string layerName = _animator.GetLayerName(2);
                if (layerName == "Smoke")
                {
                    _animator.SetLayerWeight(2, PauseSpeed);
                }
            }
            _animator.SetFloat("AnimSpeed", PlaySpeed);
        }

        //----------- Private Methods -----------

        private void HandlePlacementAnimationComplete()
        {
            OnAnimationEnd?.Invoke();

            _anchor.anchorOnMeshUpdate = true;
            StartCoroutine(EnableColliderAfterSeconds(Duration));
        }

        //----------- Coroutines -----------

        private IEnumerator EnableColliderAfterSeconds(float time)
        {
            yield return new WaitForSeconds(time);
            OnIntegrated?.Invoke();
            _collider.enabled = true;
        }
    }
}
