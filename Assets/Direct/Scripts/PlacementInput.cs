// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Utilities;

namespace MagicKit
{

    /// <summary>
    /// This class handles input and instantiating objects when placeable
    /// </summary>
    public class PlacementInput : MonoBehaviour
    {

        //----------- Private Variables -----------

        [SerializeField] private Placement _placement;
        [SerializeField] private GameObject _placementPrefab;
        [SerializeField] private PlaneConstructor _planeConstructor;

        private Camera _camera;
        private const float BoundsScale = 0.95f;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _camera = Camera.main;

            GestureManager.OnDwellBegin += OnDwellBegin;

            _placementPrefab.GetComponent<Renderer>().material.color = Color.green;

            _planeConstructor.ShowPlanes();
            
            _placement.volume = CalculateBounds(_placementPrefab);
            _placement.tilt = 0;
            _placement.allowHorizontal = true;
            _placement.allowVertical = true;
            _placement.Place(_camera.transform, HandleOnPlaced);
        }

        private void OnDestroy()
        {
            GestureManager.OnDwellBegin -= OnDwellBegin;
        }

        //----------- Private Mathods -----------

        private Vector3 CalculateBounds(GameObject go)
        {
            Bounds bounds = new Bounds();
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // Y is ommitted here because in this case we are placing objects on planes and thus the 3 dimension is not necessary.
            return new Vector3(bounds.size.x * BoundsScale, bounds.size.z * BoundsScale, bounds.size.y);
        }

        //------------ Event Handlers ------------

        private void HandleOnPlaced(Vector3 position, Quaternion rotation)
        {
            // Place game object
            GameObject obj = Instantiate(_placementPrefab);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            // Restart placement
            _placement.Place(_camera.transform, HandleOnPlaced);
        }

        private void OnDwellBegin(MLHand hand)
        {
            _placement.Confirm();
        }
    }
}
