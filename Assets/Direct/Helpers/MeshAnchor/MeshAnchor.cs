// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.Experimental.XR;

namespace MagicKit
{
    ///<summary>
    /// Uses Raycasts to anchor a rendered object or a rect transform to another object and maintains an offset distance.
    ///</summary>
    public class MeshAnchor : MonoBehaviour
    {
        //----------- Private Members -----------

        private enum OffsetDir
        {
            Forward,
            Normal
        };

        [SerializeField] private OffsetDir _offsetDir;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _minOffsetDistance = 0.05f;
        [SerializeField] private float _raycastDistanceOffset = 0.5f;

        private Vector3[] _anchors = new Vector3[5];
        private Vector3 _trueScale;
        private Vector3 _surfaceNormal;

        //----------- Public Members -----------

        public bool anchorOnMeshUpdate;

        private MLSpatialMapper _mapper;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            _mapper = GameObject.FindObjectOfType<MLSpatialMapper>();
            if (_mapper == null)
            {
                Debug.Log("Game object contains no renderers of rect transforms and cannot be anchored");
                enabled = false;
            }
            
            _mapper.meshUpdated += OnMeshUpdated;

            // Immediately call Adjust because it could be in between mesh updates and thus there could be some mesh
            // bleeding through.
            Adjust();
        }

        private void OnDestroy()
        {
            if (_mapper != null)
            {
                _mapper.meshUpdated -= OnMeshUpdated;
            }
        }

        private void OnMeshUpdated(TrackableId id)
        {
            if (anchorOnMeshUpdate)
            {
                Adjust();
            }
        }

        //----------- Public Methods -----------

        ///<summary>
        ///   Calculate the anchor position and move the object to that position.
        ///</summary>
        public void Adjust()
        {
            _trueScale = CalculateBounds();

            if (_trueScale == Vector3.zero) 
            {
                Debug.Log("Game object contains no renderers of rect transforms and cannot be anchored");
                enabled = false;
            }

            CalculateRaycastPositions();
            
            float minDist = Mathf.Infinity;
            Vector3 minPoint = transform.position;
            // Find the minimal distance and point to the colliding object 
            for (int i = 0; i < _anchors.Length; ++i)
            {
                RaycastHit hit;

                bool res = Physics.Raycast(_anchors[i], -transform.forward, out hit, Mathf.Infinity, _layerMask);
                if (res)
                {
                    float dist = Vector3.Distance(hit.point, _anchors[i]);
                    if (dist < minDist)
                    {
                        minPoint = hit.point;
                        minDist = dist;
                    }
                }
            }
            
            if (minDist != Mathf.Infinity)
            {
                Vector3 pos = minPoint - transform.position;
                Vector3 move = Vector3.Project(pos, _surfaceNormal);
                transform.position = transform.position + move + (_surfaceNormal * _minOffsetDistance);
            }
        }
        
        private void CalculateRaycastPositions()
        {
            // Creates 4 points at bounds + 1 point at center
            RaycastHit surfaceHit;
            Vector3 surfacePosition;

            // Initialize the surface normal to the transform attributes or 
            // if the raycast sucessfully hits the object use the surface normal
            _surfaceNormal = transform.forward;
            surfacePosition = transform.position;
            if (_offsetDir == OffsetDir.Normal)
            {
                if (Physics.Raycast(transform.position + (_raycastDistanceOffset * transform.forward), -transform.forward, out surfaceHit, Mathf.Infinity, _layerMask))
                {
                    _surfaceNormal = surfaceHit.normal;
                    surfacePosition = surfaceHit.point;
                }
            }
            _anchors[0] = transform.TransformPoint(new Vector3(0.0f, 0.0f, _raycastDistanceOffset + (_surfaceNormal * _minOffsetDistance).magnitude));

            // Remaining 4 points at the edges of the object 
            Vector3 hitCornerA = Vector3.zero;
            Vector3 hitCornerB = Vector3.zero;
            Vector3 hitCornerC = Vector3.zero;
            Vector3 hitCornerD = Vector3.zero;

            // Calculate the local x/y plane based off of the initial raycast
            Vector3 xAxis = Vector3.Cross(_surfaceNormal, transform.up).normalized;
            Vector3 yAxis = Vector3.Cross(_surfaceNormal, -transform.right).normalized;

            Vector3 halfVolume = _trueScale * 0.5f;
            float halfXVolume = halfVolume.x;
            float halfYVolume = halfVolume.y;

            // Space the points out 
            hitCornerA = surfacePosition + xAxis * (halfXVolume) + yAxis * (halfYVolume) + _surfaceNormal * _minOffsetDistance;
            hitCornerB = surfacePosition + xAxis * (halfXVolume) + yAxis * (-halfYVolume) + _surfaceNormal * _minOffsetDistance;
            hitCornerC = surfacePosition + xAxis * (-halfXVolume) + yAxis * (-halfYVolume) + _surfaceNormal * _minOffsetDistance;
            hitCornerD = surfacePosition + xAxis * (-halfXVolume) + yAxis * (halfYVolume) + _surfaceNormal * _minOffsetDistance;

            // Move the points RaycastDistanceOffset out in front.  This ensures you will always be hitting a piece of the occlusion space
            hitCornerA = transform.InverseTransformPoint(hitCornerA) + new Vector3(0, 0, _raycastDistanceOffset);
            hitCornerB = transform.InverseTransformPoint(hitCornerB) + new Vector3(0, 0, _raycastDistanceOffset);
            hitCornerC = transform.InverseTransformPoint(hitCornerC) + new Vector3(0, 0, _raycastDistanceOffset);
            hitCornerD = transform.InverseTransformPoint(hitCornerD) + new Vector3(0, 0, _raycastDistanceOffset);

            _anchors[1] = transform.TransformPoint(hitCornerA);
            _anchors[2] = transform.TransformPoint(hitCornerB);
            _anchors[3] = transform.TransformPoint(hitCornerC);
            _anchors[4] = transform.TransformPoint(hitCornerD);
        }
        
        private Vector3 CalculateBounds() 
        {
            Quaternion initRotation = transform.rotation;
            transform.rotation = Quaternion.identity;
            Vector3 position = transform.position;
            transform.position = Vector3.zero;

            // Go through all the renders (if any) and encapsulate the bounds
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds();
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // Go through all the rect transforms (if any) and encapsulate the bounds
            foreach (Transform child in transform)
            {
                // Try to cast the transform to a rect transform
                RectTransform trans = child as RectTransform;
                if (trans != null)
                {
                    Bounds childBounds = new Bounds(child.position, new Vector3((float)trans.rect.width * child.lossyScale.x, (float)trans.rect.height * child.lossyScale.y, 0.0f));
                    bounds.Encapsulate(childBounds);
                }
            }

            Vector3 fullBounds = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);

            // Move the object back.
            transform.rotation = initRotation;
            transform.position = position;

            return fullBounds;
        }
    }
}