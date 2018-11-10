// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System;

namespace MagicKit
{
    /// <summary>
    /// SpatialObjects are used by the MLSpatialMapperController to encapsulate objects that require world mesh
    /// </summary>
    [Serializable]
    public class SpatialObject : MonoBehaviour
    {

        //----------- Public Members -----------

        [Tooltip("Volume around the object that impacts mesh availability")]
        public Vector3 boundsSize;

        public Bounds Bounds
        {
            get
            {
                _bounds.center = transform.position;
                _bounds.extents = boundsSize;
                return _bounds;
            }
        }

        //----------- Private Members -----------

        private Bounds _bounds;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _bounds = new Bounds(transform.position, boundsSize);
        }
    }
}