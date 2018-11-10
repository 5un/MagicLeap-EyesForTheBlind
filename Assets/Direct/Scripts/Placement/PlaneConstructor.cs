// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicKit
{
    /// <summary>
    /// This class maintains the list of planes returned from the Planes API
    /// </summary>
    public class PlaneConstructor : MonoBehaviour
    {
        //----------- Private Members -----------

        [SerializeField] private GameObject _planePrefab;
        private List<GameObject> _planeCache = new List<GameObject>();
        private Renderer _planesRenderer;

        //----------- MonoBehaviour Methods -----------
        private void Awake()
        {
            _planesRenderer = _planePrefab.GetComponent<Renderer>();
        }

        private void OnDestroy()
        {
            foreach (GameObject plane in _planeCache)
            {
                Destroy(plane);
            }
            _planeCache.Clear();
        }

        //----------- Event Handlers -----------

        public void WorldPlanesUpdated(MLWorldPlane[] planes)
        {
            // Disable any planes that were created but no longer needed.
            for (int i = planes.Length; i < _planeCache.Count; i++)
            {
                _planeCache[i].SetActive(false);
            }

            // Create new planes if we need them
            for (int i = _planeCache.Count; i < planes.Length; i++)
            {
                GameObject planeGO = Instantiate(_planePrefab);
                _planeCache.Add(planeGO);
            }

            // Position and size each active plane
            for (int i = 0; i < planes.Length; i++)
            {
                GameObject planeGO = _planeCache[i];
                MLWorldPlane plane = planes[i];

                planeGO.SetActive(true);

                planeGO.transform.position = plane.Center;
                planeGO.transform.rotation = plane.Rotation;
                planeGO.transform.localScale = new Vector3(plane.Width, plane.Height, 1f);

                SetPlaneTextureProperties(planeGO, plane);
            }
        }

        //----------- Public Methods -----------

        public void ShowPlanes()
        {
            _planesRenderer.enabled = true;
            foreach (var plane in _planeCache)
            {
                Renderer renderer = plane.GetComponent<Renderer>();
                renderer.enabled = true;
            }
        }

        public void HidePlanes()
        {
            _planesRenderer.enabled = false;
            foreach (var plane in _planeCache)
            {
                Renderer renderer = plane.GetComponent<Renderer>();
                renderer.enabled = false;
            }
        }

        //----------- Private Methods -----------

        private void SetPlaneTextureProperties(GameObject planeGO, MLWorldPlane plane)
        {
            Renderer planeRenderer = planeGO.GetComponent<Renderer>();
            float xScale = plane.Width;
            float yScale = plane.Height;
            float xOffset = xScale - Mathf.Floor(xScale);
            float yOffset = yScale - Mathf.Floor(yScale);
            planeRenderer.material.SetTextureScale("_MainTex", new Vector2(xScale, yScale));
            planeRenderer.material.SetTextureOffset("_MainTex", new Vector2(-xOffset * 0.5f, -yOffset * 0.5f));
        }
    }
}