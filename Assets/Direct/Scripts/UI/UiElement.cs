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
    /// A convenience class for showing and hiding our UI icons
    /// </summary>
    public class UiElement : MonoBehaviour
    {

        //----------- Private Variables -----------

        [SerializeField] private AppController _appController;

        private Component[] _meshRenderers;
        private Component[] _skinnedMeshRenderers;
        private Component[] _colliders;

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            _appController.OnShowUi += HandleOnShow;
            _appController.OnHideUi += HandleOnHide;

            _meshRenderers = GetComponentsInChildren(typeof(MeshRenderer), true);
            _skinnedMeshRenderers = GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);
            _colliders = GetComponentsInChildren(typeof(Collider), true);
        }

        private void OnDestroy()
        {
            _appController.OnShowUi -= HandleOnShow;
            _appController.OnHideUi -= HandleOnHide;
        }

        //----------- Private Methods -----------

        private void HandleOnShow()
        {
            foreach (MeshRenderer renderer in _meshRenderers)
            {
                renderer.enabled = true;
            }
            foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderers)
            {
                renderer.enabled = true;
            }
            foreach (Collider collider in _colliders)
            {
                collider.enabled = true;
            }
        }

        private void HandleOnHide()
        {
            foreach (MeshRenderer renderer in _meshRenderers)
            {
                renderer.enabled = false;
            }
            foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderers)
            {
                renderer.enabled = false;
            }
            foreach (Collider collider in _colliders)
            {
                collider.enabled = false;
            }
        }
    }
}
