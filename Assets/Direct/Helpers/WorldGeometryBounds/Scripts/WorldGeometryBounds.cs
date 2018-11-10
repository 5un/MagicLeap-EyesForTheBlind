// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;
using System.Linq;
using MagicLeap.Utilities;

namespace MagicKit
{
    ///<summary>
    /// This class organically grows the spatial mapper bounds (localScale) depending on the states of the spatialObjects.
    ///</summary>
    [RequireComponent(typeof(MLSpatialMapper))]
    public class WorldGeometryBounds : MonoBehaviour
    {

        //----------- Public Members -----------

        [SerializeField] private List<SpatialObject> _spatialObjects; 
        public bool removeOutsideExtents;

        public static WorldGeometryBounds Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (WorldGeometryBounds)FindObjectOfType(typeof(WorldGeometryBounds));

                    if (_instance == null)
                    {
                        GameObject singletonCreation = new GameObject(typeof(WorldGeometryBounds).Name);
                        _instance = singletonCreation.AddComponent<WorldGeometryBounds>();
                    }
                }

                return _instance;
            }
        }

        //----------- Private Members -----------

        private Bounds _bounds;
        private Vector3 _initialSize;
        private static WorldGeometryBounds _instance;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _initialSize = transform.localScale;
            ResetBounds();
        }

        private void Update()
        {
            if (removeOutsideExtents)
            {
                ResetBounds();
            }

            // Encapsulate the volumes of all objects
            foreach(SpatialObject sp in _spatialObjects)
            {
                _bounds.Encapsulate(sp.Bounds);
            }

            // Adjust the values
            transform.localScale = _bounds.size;
            transform.position = _bounds.center;
        }

        //----------- Public Methods -----------

        /// <summary>
        /// Add a spatial Object
        /// </summary>
        public void AddSpatialObject(SpatialObject obj)
        {
            if(ContainsSpatialObject(obj))
            {
                Debug.LogError("Object already in list");
                return;
            }
            _spatialObjects.Add(obj);
        }

        /// <summary>
        /// Remove a spatial Object
        /// </summary>
        public void RemoveSpatialObject(SpatialObject obj)
        {
            foreach(SpatialObject sp in _spatialObjects)
            {
                if (obj == sp)
                {
                    _spatialObjects.Remove(sp);
                    return;
                }
            }
        }

        /// <summary>
        /// Check to see if a transform is already being tracked.
        /// </summary>
        public bool ContainsSpatialObject(SpatialObject obj)
        {
            return _spatialObjects.Contains(obj);
        }

        //----------- Private Methods -----------

        private void ResetBounds()
        {
            _bounds = new Bounds(transform.position, _initialSize);
        }
    }
}