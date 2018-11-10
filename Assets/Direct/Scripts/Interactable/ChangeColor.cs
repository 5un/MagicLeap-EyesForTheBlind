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
    /// This is an example script to use Interactables.  Will change this object's color based on focus
    /// </summary>
    public class ChangeColor : MonoBehaviour
    {
        //----------- Private Members -----------

        [SerializeField] private Color _focusColor;
        [SerializeField] private Color _unfocusColor;

        private Renderer _renderer;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            SetDeactivated();
        }

        //----------- Public Methods ------------

        public void SetActivated()
        {
            _renderer.material.color = _focusColor;
        }

        public void SetDeactivated()
        {
            _renderer.material.color = _unfocusColor;
        }
    }
}
