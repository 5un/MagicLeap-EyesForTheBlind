// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine;
using MagicLeap.Utilities;

namespace MagicKit
{
    /// <summary>
    /// CursorController is used to continuously raycast into the world and return the object that is hit
    /// The Cursor will also return a normal of the geometry that is being hit.  
    /// </summary>
    public class CursorController : Singleton<CursorController>
    {

        //----------- Private Members -----------

        private enum CursorColorState
        {
            UnHighlighted,
            Highlighted,
        };
    
        // Transform from which to raycast
        [SerializeField] private Transform _originTransform;
        // Max distance to cast
        [SerializeField] private float _cursorMaxDistance;
        // Unhighlight color
        [SerializeField] private Color _unHighlighted;
        // Highlight color
        [SerializeField] private Color _highlighted;
        // Defines the layers to interact with
        [SerializeField] private LayerMask _layermask;

        private Renderer _cursorRenderer;
        private static CursorController _instance;


        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _cursorRenderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            RaycastHit hit;

            // Cast ray into the world.
            if (Physics.Raycast(_originTransform.transform.position, (_originTransform.transform.forward).normalized, out hit, 10f, _layermask))
            {
                HandleRaycastHit(hit);
            }
            else
            {
                HandleRaycastMiss();
            }
        }

        //----------- Public Methods -----------

        /// <summary>
        /// These property function allow for getting things like the normal, position, and selected game object.
        /// There is also a property for whether or not to draw the cursor.
        /// </summary>
        public Vector3 HitNormal { get; private set; }

        public Vector3 HitPosition { get; private set;}

        public GameObject SelectedGameObject { get; private set; }

        //----------- Private Methods -----------

        /// <summary>
        ///   This function is called by Update() to change the color of the cursor when it is colliding with objects
        ///   And when it stops colliding.
        /// </summary>
        private void SetCursorColor(CursorColorState colorState)
        {
            switch (colorState)
            {
                case CursorColorState.Highlighted:
                    _cursorRenderer.material.color = _highlighted;
                    break;
                case CursorColorState.UnHighlighted:
                    _cursorRenderer.material.color = _unHighlighted;
                    break;
            }
        }

        private void HandleRaycastHit(RaycastHit hit)
        {
            // Highlight the game object that was hit if it is not already highlighted
            // And set the cursor color to highlighted
            if (SelectedGameObject != hit.collider.gameObject && SelectedGameObject == null)
            {
                SelectedGameObject = hit.collider.gameObject;
                SetCursorColor(CursorColorState.Highlighted);
            }
            else if (SelectedGameObject != null)
            {
                //Check to make sure we aren't colliding with a new object without firing an Unhighlight event

                SelectedGameObject = hit.collider.gameObject;

            }
            // Set the position of the cursor to the hit point and the lookAt to the view normal
            transform.position = hit.point;
            transform.LookAt(hit.point + hit.normal);
            // Keep track of the current position and normal
            HitNormal = hit.normal;
            HitPosition = hit.point;
        }
        
        private void HandleRaycastMiss()
        {
            // If we didn't hit anything on the raycast make sure to unlightlight the cursor and object
            SelectedGameObject = null;
            // Set the cursor position to max distance and original lookAt position
            transform.position = _originTransform.transform.position + (_cursorMaxDistance * _originTransform.transform.forward);
            transform.LookAt(_originTransform);
            HitNormal = Vector3.zero;
            HitPosition = Vector3.zero;
            // Set the cursor position to Unhighlight
            SetCursorColor(CursorColorState.UnHighlighted);
        }
    }
}