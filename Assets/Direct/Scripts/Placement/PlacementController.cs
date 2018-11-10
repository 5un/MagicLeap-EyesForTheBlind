// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using MagicLeap.Utilities;
using System;

namespace MagicKit
{
    /// <summary>
    /// PlacementController uses the placement tool to place objects in the environment
    /// </summary>
    public class PlacementController : MonoBehaviour
    {
        //----------- Public Events -----------

        public event Action<Vector3, Quaternion> OnPlaced;
        public event Action<GameObject> OnPlacedObject;
        public event Action OnInitializePlacement;

        //----------- Private Members -----------

        [SerializeField] private GameObject _fadePrefab;
        [SerializeField] private Transform _mainCamera;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private PlaneConstructor _planesVisual;
        [SerializeField] private Placement _placement;
        private GameObject _placementPrefab;
        private const float BoundsScale = 0.95f;
        
        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            OnPlaced += HandleOnPlaced;
        }

        private void OnDestroy()
        {
            OnPlaced -= HandleOnPlaced;
        }
        
        //----------- Event Handlers -----------

        private void HandleOnPlaced(Vector3 position, Quaternion rotation)
        {
            // Correct the instantiated window's rotation.
            Quaternion rotationCorrection = Quaternion.Euler(-90f, 0f, 0f);
            
            // Place the new window in the scene
            GameObject placedInstance = Instantiate(_placementPrefab.gameObject, position,
                rotation * rotationCorrection);
            
            // Create a fadeout out effect
            GameObject fade = Instantiate(_fadePrefab, position, rotation);
            Renderer fadeRenderer = fade.GetComponent<Renderer>();
            Color fadeColor = fadeRenderer.material.color;

            // Fade out is controlled by a tween
            Tween.Add(
                fadeRenderer.material.color, new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0.0f), _fadeDuration, 0, 
                Tween.EaseLinear, Tween.LoopType.None, 
                (value) => fadeRenderer.material.color = value,
                () => Destroy(fade),
                true
            );

            _planesVisual.HidePlanes();
            OnPlacedObject?.Invoke(placedInstance);
        }

        //----------- Public Methods -----------
        
        /// <summary>
        /// Returns true if Placement is currently running and the user is looking at a valid placement point
        /// </summary>
        public bool CanPlace()
        {
            return (_placement.Running && _placement.Fit == FitType.Fits);
        }
        /// <summary>
        /// Attempts to confirm placement
        /// </summary>
        public void Place()
        {
            _placement.Confirm();
        }
        /// <summary>
        /// Cancel placement
        /// </summary>
        public void Cancel()
        {
            _placement.Cancel();
            _planesVisual.HidePlanes();
        }
        /// <summary>
        /// InitializePlacement starts the placement system and shows a bounding box for the object you're planning on placing in the environment
        /// </summary>
        public void InitializePlacement(GameObject go)
        {
            _placementPrefab = go;
            Vector3 fullBounds = CalculateBounds(_placementPrefab);
            _placement.volume = fullBounds;
            _placement.tilt = 0;
            _placement.allowHorizontal = true;
            _placement.allowVertical = true;
            _placement.Place(_mainCamera.transform, OnPlaced);

            _fadePrefab.transform.localScale = fullBounds;
            _planesVisual.ShowPlanes();
            OnInitializePlacement?.Invoke();
        }

        //----------- Private Methods -----------

        private Vector3 CalculateBounds(GameObject go)
        {
            Bounds bounds = new Bounds();
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            
            return new Vector3(bounds.size.x * BoundsScale, bounds.size.z * BoundsScale, bounds.size.y); ;
        }
    }
}
