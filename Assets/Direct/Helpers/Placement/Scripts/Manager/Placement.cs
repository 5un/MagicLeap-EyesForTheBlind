// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 472

namespace MagicLeap.Utilities
{
    //----------- Public Enums -----------
    public enum SurfaceType { Unknown, Horizontal, Vertical, Slanted }
    public enum FitType { Unknown, Fits, NoSurface, Uneven, Overhang, VolumeIntersection, WrongOrientation }

    //----------- Unity Events -----------
    [System.Serializable]
    public class PlacementEvent : UnityEvent<FitType> { }

    /// <summary>
    /// Handles content placement.
    /// </summary>
    public class Placement : MonoBehaviour
    {
        //----------- Public Events -----------
        public PlacementEvent OnPlacementEvent;

        //----------- Public variables -----------
        [Tooltip("A visual to be displayed when the volume will fit.  This transform's scale should be 1, 1, 1.")]
        public GameObject willFitVolume;
        [Tooltip("A visual to be displayed when the volume will not fit.  This transform's scale should be 1, 1, 1.")]
        public GameObject willNotFitVolume;
        [Tooltip("Tilts the placement's cast lower or higher than the source's forward.")]
        public float tilt;
        [Tooltip("The size we are trying to fit.")]
        public Vector3 volume = Vector3.one;
        [Tooltip("When on a wall should the volume's y axis match gravity?")]
        public bool matchGravityOnVerticals = true;
        [Tooltip("How far to detect a surface.")]
        public float maxDistance = 3.048f;
        [Tooltip("Beyond this value a surface will be determined to be uneven.")]
        public float maxUneven = 0.0508f;
        [Tooltip("Enables automatic scaling of the playable space.")]
        public bool scaleVolumeWithDistance;
        [Tooltip("Minimal size of the playable space.")]
        public float minAutoScaleSize = 0.2f;
        [Tooltip("Maximal size of the playable space.")]
        public float maxAutoScaleSize = 1.2f;
        [Tooltip("Below this distance, the playable space will be set to its minimum volume.")]
        public float minAutoScaleDistance = 0.5f;
        [Tooltip("Above this distance, the playable space will be set to its maximum volume.")]
        public float maxAutoScaleDistance = 3.5f;
        [Tooltip("Is placement allowed on horizontal surfaces?")]
        public bool allowHorizontal = true;
        [Tooltip("Is placement allowed on vertical surfaces?")]
        public bool allowVertical = true;
        [Tooltip("The layers to raycast against.")]
        public LayerMask layerMask = -1;

        //----------- Public Properties -----------

        public bool Running
        {
            get;
            private set;
        }

        public FitType Fit
        {
            get;
            private set;
        }

        public Vector3 Position
        {
            get;
            private set;
        }

        public Quaternion Rotation
        {
            get;
            private set;
        }

        public SurfaceType Surface
        {
            get;
            private set;
        }

        //----------- Private Variables -----------
        private Transform _source;
        private System.Action<Vector3, Quaternion> _callback;
        private FitType _fitStatus;
        private Vector3 _yAxis;
        private Vector3 _volume;
        private Predicate<Vector3> _customRule;

        //----------- MonoBehaviour Methods -----------
        private void Awake()
        {
            //resource check:
            if (willFitVolume == null || willFitVolume.scene == null)
            {
                Debug.LogError("Placement: 'willFitVolume' not set to a scene object.");
                gameObject.SetActive(false);
                return;
            }

            if (willNotFitVolume == null || willNotFitVolume.scene == null)
            {
                Debug.LogError("Placement: 'willNotFitVolume' not set to a scene object.");
                gameObject.SetActive(false);
                return;
            }

            //hide volumes:
            willFitVolume.SetActive(false);
            willNotFitVolume.SetActive(false);
        }

        private void Update()
        {
            if (!Running)
            {
                return;
            }

            //apply volume scale:
            willFitVolume.transform.localScale = volume;
            willNotFitVolume.transform.localScale = volume;

            //evaluate surface:
            RaycastHit surfaceHit;

            //tilt cast vector:
            Quaternion adjustmentQuaternion = Quaternion.AngleAxis(tilt, _source.right);
            Vector3 castVector = adjustmentQuaternion * _source.forward;

            if (Physics.Raycast(_source.position, castVector, out surfaceHit, maxDistance, layerMask))
            {
                //feedback:
                Debug.DrawLine(_source.position, surfaceHit.point);

                //set initial position:
                Position = surfaceHit.point;

                //autosize:
                if (scaleVolumeWithDistance)
                {
                    ScaleVolumeByDistance();
                }

                //phase 1: surface validity
                Surface = GetSurfaceType(surfaceHit.normal);
                _fitStatus = SurfaceTypeCheck(Surface);

                //phase 2: corner evenness
                if (_fitStatus == FitType.Fits || _fitStatus == FitType.WrongOrientation)
                {
                    //get a "perfect" normal since meshing is not perfect:
                    _yAxis = GetPerfectNormal(Surface, surfaceHit.normal);

                    //find axis:
                    Vector3 xAxis = GetCrossAxis(Surface, _yAxis);
                    Vector3 zAxis = Vector3.Cross(_yAxis, xAxis);

                    //set rotation:
                    if (Surface == SurfaceType.Vertical && matchGravityOnVerticals)
                    {
                        Rotation = Quaternion.LookRotation(_yAxis, Vector3.up);
                    }
                    else
                    {
                        Rotation = Quaternion.LookRotation(zAxis, _yAxis);
                    }

                    //locate each surface-proximity corner:
                    Vector3 halfVolume = volume * .5f;
                    Vector3 cornerA = Vector3.zero;
                    Vector3 cornerB = Vector3.zero;
                    Vector3 cornerC = Vector3.zero;
                    Vector3 cornerD = Vector3.zero;
                    RaycastHit cornerAHit = new RaycastHit();
                    RaycastHit cornerBHit = new RaycastHit();
                    RaycastHit cornerCHit = new RaycastHit();
                    RaycastHit cornerDHit = new RaycastHit();

                    if (matchGravityOnVerticals && Surface == SurfaceType.Vertical)
                    {
                        cornerA = surfaceHit.point + (zAxis * -halfVolume.y) + (xAxis * -halfVolume.x);
                        cornerB = surfaceHit.point + (zAxis * -halfVolume.y) + (xAxis * halfVolume.x);
                        cornerC = surfaceHit.point + (zAxis * halfVolume.y) + (xAxis * halfVolume.x);
                        cornerD = surfaceHit.point + (zAxis * halfVolume.y) + (xAxis * -halfVolume.x);

                        //find corner-to-surface points:
                        cornerAHit = CornerCast(_yAxis, cornerA, volume.z);
                        cornerBHit = CornerCast(_yAxis, cornerB, volume.z);
                        cornerCHit = CornerCast(_yAxis, cornerC, volume.z);
                        cornerDHit = CornerCast(_yAxis, cornerD, volume.z);
                    }
                    else
                    {
                        cornerA = surfaceHit.point + (zAxis * halfVolume.z) + (xAxis * halfVolume.x);
                        cornerB = surfaceHit.point + (zAxis * halfVolume.z) + (xAxis * -halfVolume.x);
                        cornerC = surfaceHit.point + (zAxis * -halfVolume.z) + (xAxis * -halfVolume.x);
                        cornerD = surfaceHit.point + (zAxis * -halfVolume.z) + (xAxis * halfVolume.x);

                        //find corner-to-surface points:
                        cornerAHit = CornerCast(_yAxis, cornerA, volume.y);
                        cornerBHit = CornerCast(_yAxis, cornerB, volume.y);
                        cornerCHit = CornerCast(_yAxis, cornerC, volume.y);
                        cornerDHit = CornerCast(_yAxis, cornerD, volume.y);
                    }

                    //all corners have hit something:
                    if (cornerAHit.collider != null && cornerBHit.collider != null && cornerCHit.collider != null && cornerDHit.collider != null)
                    {
                        //get evenness values:
                        float cornerAEvenness = EvenSurfaceCheck(surfaceHit.point, cornerAHit.point, xAxis, zAxis);
                        float cornerBEvenness = EvenSurfaceCheck(surfaceHit.point, cornerBHit.point, xAxis, zAxis);
                        float cornerCEvenness = EvenSurfaceCheck(surfaceHit.point, cornerCHit.point, xAxis, zAxis);
                        float cornerDEvenness = EvenSurfaceCheck(surfaceHit.point, cornerDHit.point, xAxis, zAxis);

                        //are we within the maxUneven threshold?
                        float largestBump = Mathf.Max(cornerAEvenness, cornerBEvenness, cornerCEvenness, cornerDEvenness);

                        //determine if we passed evenness testing:
                        if (largestBump > maxUneven)
                        {
                            _fitStatus = FitType.Uneven;
                        }
                        else
                        {
                            //only set as fits if we are in the correct orientation:
                            if (_fitStatus != FitType.WrongOrientation)
                            {
                                _fitStatus = FitType.Fits;
                                Position = surfaceHit.point + (_yAxis * largestBump);
                            }
                        }
                    }
                    else
                    {
                        //we are likely hanging over a physical edge:
                        _fitStatus = FitType.Overhang;
                    }

                    //push out if we want to align with gravity while on a wall:
                    if (matchGravityOnVerticals && Surface == SurfaceType.Vertical)
                    {
                        //out:
                        Position += _yAxis * (volume.z * .5f);
                    }
                }

                //phase 3: volume clearance
                if (_fitStatus == FitType.Fits)
                {
                    //locate the center of our volume:
                    Vector3 centerPoint = Position + (_yAxis * (volume.y * .5f));

                    //test a volume that is smaller than our actual volume by uneven allowance as a buffer:
                    Vector3 collisionVolumeSize = volume * (.5f - maxUneven);

                    //check for volume collisions:
                    Collider[] volumeCollisions = Physics.OverlapBox(centerPoint, collisionVolumeSize, Rotation);

                    //passed?
                    if (volumeCollisions.Length > 0)
                    {
                        _fitStatus = FitType.VolumeIntersection;
                    }
                    else
                    {
                        _fitStatus = FitType.Fits;
                    }
                }
            }
            else
            {
                //feedback:
                Debug.DrawRay(_source.position, castVector * maxDistance);

                //status:
                _fitStatus = FitType.NoSurface;

                //resets:
                Surface = SurfaceType.Unknown;

                Position = _source.position + (castVector * maxDistance);
                Rotation = Quaternion.LookRotation(-_source.forward);
            }

            bool customRuleResult = true;
            if (_customRule != null)
            {
                customRuleResult = _customRule(Position);
            }

            if (_fitStatus == FitType.Fits && customRuleResult)
            {
                //volume handling:
                willFitVolume.SetActive(true);
                willNotFitVolume.SetActive(false);
            }
            else
            {
                //volume handling:
                willFitVolume.SetActive(false);
                willNotFitVolume.SetActive(true);
            }

            //pose our volumes:
            if (_fitStatus == FitType.NoSurface)
            {
                willFitVolume.transform.position = Position;
                willNotFitVolume.transform.position = Position;
                willFitVolume.transform.rotation = Rotation;
                willNotFitVolume.transform.rotation = Rotation;
            }
            else
            {
                Vector3 finalLocation = Vector3.zero;
                if (matchGravityOnVerticals && Surface == SurfaceType.Vertical)
                {
                    //finalLocation = Position + (_yAxis * Volume.z * .5f);
                    finalLocation = Position;
                }
                else
                {
                    finalLocation = Position + (_yAxis * volume.y * .5f);
                }
                willFitVolume.transform.position = finalLocation;
                willNotFitVolume.transform.position = finalLocation;
                willFitVolume.transform.rotation = Rotation;
                willNotFitVolume.transform.rotation = Rotation;
            }

            //broadcast status:
            if (Fit != _fitStatus)
            {
                Fit = _fitStatus;
                if (OnPlacementEvent != null) OnPlacementEvent.Invoke(_fitStatus);
            }
        }

        //----------- Public Methods -----------
        public void Confirm()
        {
            if (Running && Fit == FitType.Fits)
            {
                Running = false;
                willFitVolume.SetActive(false);
                willNotFitVolume.SetActive(false);
                _callback(Position, Rotation);
            }
        }

        /// <summary>
        /// Begins placement session. Callback provides position and volume rotation.
        /// </summary>
        public void Place(Transform placementSource, System.Action<Vector3, Quaternion> callback, Predicate<Vector3> customRule = null)
        {
            if (Running) return;

            //cache:
            _source = placementSource;
            _callback = callback;
            _customRule = customRule;

            //start placement loop:
            Running = true;
        }

        public void Cancel()
        {
            Running = false;

            //hide volumes:
            willFitVolume.SetActive(false);
            willNotFitVolume.SetActive(false);

        }

        //----------- Private Mathods -----------
        void ScaleVolumeByDistance()
        {
            // Get Distance from Headpose to placement point.
            // Scale Placement Volume based on formula.
            float headposeToPlacement = Vector3.Distance(Position, _source.position);
            float desiredScale = GetDesiredScaleByDistance(headposeToPlacement);
            volume = new Vector3(desiredScale, desiredScale, desiredScale);
        }

        float GetDesiredScaleByDistance(float distance)
        {
            float normalizedDistance = Mathf.InverseLerp(minAutoScaleDistance, maxAutoScaleDistance, distance);
            return Mathf.Lerp(minAutoScaleSize, maxAutoScaleSize, normalizedDistance);
        }

        float EvenSurfaceCheck(Vector3 rootPoint, Vector3 cornerPoint, Vector3 normalA, Vector3 normalB)
        {
            Vector3 gapVector = rootPoint - cornerPoint;

            //collapse vector:
            gapVector = Vector3.ProjectOnPlane(gapVector, normalA);

            //collapse vector again:
            gapVector = Vector3.ProjectOnPlane(gapVector, normalB);

            //visually reveal surface gap:
            Debug.DrawRay(cornerPoint, gapVector, Color.red);

            return gapVector.magnitude;
        }

        RaycastHit CornerCast(Vector3 normal, Vector3 corner, float volumeAxis)
        {
            RaycastHit hit;
            //translate corner to the top of our volume to cast "down":
            corner = corner + (normal * volumeAxis);

            Debug.DrawRay(corner, -normal * volumeAxis);

            //over cast to ensure we can hit a surface:
            float castDistance = volumeAxis * 1.5f;

            //cast:
            Physics.Raycast(corner, -normal, out hit, castDistance);

            return hit;
        }

        Vector3 GetCrossAxis(SurfaceType surfaceType, Vector3 normal)
        {
            if (surfaceType == SurfaceType.Horizontal)
            {
                if (normal == Vector3.up)
                {
                    //table or floor:
                    return Vector3.Cross(normal, _source.forward).normalized;
                }
                else
                {
                    //ceiling:
                    return Vector3.Cross(normal, _source.forward).normalized;
                }
            }

            if (surfaceType == SurfaceType.Vertical)
            {
                return Vector3.Cross(normal, Vector3.up);
            }

            return Vector3.zero;
        }

        Vector3 GetPerfectNormal(SurfaceType surfaceType, Vector3 normal)
        {
            if (surfaceType == SurfaceType.Horizontal)
            {
                //collapse the normal to be straight:
                Vector3 phase1 = Vector3.ProjectOnPlane(normal, Vector3.right).normalized;
                Vector3 phase2 = Vector3.ProjectOnPlane(phase1, Vector3.forward).normalized;

                return phase2;
            }

            if (surfaceType == SurfaceType.Vertical)
            {
                return Vector3.ProjectOnPlane(normal, Vector3.up).normalized;
            }

            return Vector3.zero;
        }

        FitType SurfaceTypeCheck(SurfaceType surfaceType)
        {
            if (surfaceType == SurfaceType.Slanted)
            {
                return FitType.Uneven;
            }

            if (surfaceType == SurfaceType.Horizontal && !allowHorizontal)
            {
                return FitType.WrongOrientation;
            }

            if (surfaceType == SurfaceType.Vertical && !allowVertical)
            {
                return FitType.WrongOrientation;
            }

            if (surfaceType == SurfaceType.Horizontal && allowHorizontal)
            {
                return FitType.Fits;
            }

            if (surfaceType == SurfaceType.Vertical && allowHorizontal)
            {
                return FitType.Fits;
            }

            return FitType.Unknown;
        }

        SurfaceType GetSurfaceType(Vector3 surfaceNormal)
        {
            float dot = Vector3.Dot(surfaceNormal, Vector3.up);

            //determine surface orientation:
            if (dot >= .97f || dot <= -.97f)
            {
                return SurfaceType.Horizontal;
            }

            if (dot >= -.3f && dot <= .3f)
            {
                return SurfaceType.Vertical;
            }

            return SurfaceType.Slanted;
        }
    }
}