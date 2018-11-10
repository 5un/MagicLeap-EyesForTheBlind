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
    /// InteractableFrameData holds a list of all InteractableData on a per frame basis.  
    /// This tool can be used by the application controller to change states of interactables based on data
    /// </summary>
    public class InteractableFrameData : MonoBehaviour
    {

        //----------- Public Members -----------

        public enum DwellInput
        {
            Angle,
            Raycast
        };

        public Dictionary<Interactable, InteractableData> InteractableData
        {
            get
            {
                return _interactableData;
            }
        }

        public float dwellAngleThreshold = 10f;
        public float spherecastRadius = 0.01f;

        //----------- Private Members -----------

        [SerializeField] private Transform _targetingTransform;
        [SerializeField, BitMask(typeof(Interactable.InteractableType))] public Interactable.InteractableType interactableType;
        [SerializeField] private LayerMask _raycastLayerMask;
        [SerializeField] private DwellInput _dwellInput;

        private List<Interactable> _interactables;
        private Dictionary<Interactable, InteractableData> _interactableData;

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            Interactable.OnCreated.AddListener(Subscribe);
            Interactable.OnDestroyed.AddListener(UnSubscribe);
            _interactables = new List<Interactable>();
            _interactableData = new Dictionary<Interactable, InteractableData>();
        }

        private void Update()
        {
            UpdateData();
        }

        //----------- Event Handlers ------------

        private void Subscribe(Interactable.InteractableType type, Interactable interactable)
        {
            if (interactableType.HasFlag(type))
            {

                AddInteractableObject(interactable);
            }
        }

        private void UnSubscribe(Interactable.InteractableType type, Interactable interactable)
        {
            if (interactableType.HasFlag(type))
            {
                RemoveInteractableObject(interactable);
            }
        }

        //----------- Private Methods -----------

        private void UpdateData()
        {
            UpdateRaycastInfo();
            foreach (Interactable obj in _interactables)
            {
                UpdateInteractableData(obj);
            }
        }

        private void UpdateInteractableData(Interactable obj)
        {
            InteractableData objData = _interactableData[obj];
            objData.angle = CalculateAngle(obj);
            objData.dwellDuration = UpdateDwell(obj);

            _interactableData[obj] = objData;
        }

        private float UpdateDwell(Interactable obj)
        {

            switch (_dwellInput)
            {
                case DwellInput.Raycast:
                    if (_interactableData[obj].spherecastHit)
                    {
                        return _interactableData[obj].dwellDuration += Time.deltaTime;
                    }
                    else
                    {
                        return 0.0f;
                    }
                case DwellInput.Angle:
                    if (_interactableData[obj].angle < dwellAngleThreshold)
                    {
                        return _interactableData[obj].dwellDuration += Time.deltaTime;
                    }
                    else
                    {
                        return 0.0f;
                    }
                default:
                    return 0.0f;
            }
        }

        private void UpdateRaycastInfo()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(_targetingTransform.position, spherecastRadius, _targetingTransform.forward, out hitInfo, Mathf.Infinity, _raycastLayerMask))
            {
                Interactable hitTarget;
                if(hitInfo.collider.GetComponent<Interactable>() != null)
                {
                    hitTarget = hitInfo.collider.GetComponent<Interactable>();
                }
                else
                {
                    CleanRaycastData();
                    return;
                }
                if (_interactableData.ContainsKey(hitTarget))
                {
                    _interactableData[hitInfo.collider.GetComponent<Interactable>()].spherecastHit = true;
                    CleanRaycastData(hitTarget);
                }
            }
            else
            {
                CleanRaycastData();
            }
        }

        private void CleanRaycastData()
        {
            foreach (KeyValuePair<Interactable, InteractableData> data in _interactableData)
            {
                data.Value.spherecastHit = false;
            }
        }

        private void CleanRaycastData(Interactable obj)
        {
            foreach(KeyValuePair<Interactable, InteractableData> data in _interactableData)
            {
                if(data.Key != obj)
                {
                    data.Value.spherecastHit = false;
                }
            }
        }

        private InteractableData CreateNewData(Interactable obj)
        {
            InteractableData data = new InteractableData();
            data.angle = CalculateAngle(obj);
            data.dwellDuration = 0.0f;
            return data;
        }

        private float CalculateAngle(Interactable obj)
        {
            Vector3 originForward = _targetingTransform.transform.forward;
            return Vector3.Angle(originForward, (obj.transform.position - _targetingTransform.transform.position).normalized);
        }

        private void AddInteractableObject(Interactable obj)
        {
            if (_interactables.Contains(obj))
            {
                return;
            }
            _interactables.Add(obj);
            InteractableData data = CreateNewData(obj);
            _interactableData.Add(obj, data);
        }

        private void RemoveInteractableObject(Interactable obj)
        {
            if (_interactables.Contains(obj))
            {
                _interactables.Remove(obj);
                _interactableData.Remove(obj);
            }
        }

        //----------- Public Methods -----------

        public Interactable LongestDwelledInteractable(float minDwell)
        {
            Interactable longestDwell = null;
            float longestDwellDuration = minDwell;
            foreach (KeyValuePair<Interactable, InteractableData> data in _interactableData)
            {
                if(longestDwellDuration < data.Value.dwellDuration)
                {
                    longestDwell = data.Key;
                    longestDwellDuration = data.Value.dwellDuration;
                }
            }
            return longestDwell;
        }
    }
}