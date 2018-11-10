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

    ///<summary>
    /// Initializes the position of assigned content to the camera position on Start.
    ///</summary>
    public class CoordinateFrameInitializer : MonoBehaviour
    {

        //----------- Public Members -----------

        public static Vector3 Origin
        {
            get
            {
                return _origin;
            }
        }

        public Transform[] contentToMove;

        //----------- Private Members -----------

        private static Vector3 _origin = Vector3.zero;
        private Camera _cam;
        private float _originalAngle;

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            foreach(Transform content in contentToMove)
            {
                content.gameObject.SetActive(false);
            }
            _cam = Camera.main;
            _originalAngle = Quaternion.Angle(transform.rotation, _cam.transform.rotation);
        }

        private void Start()
        {
            _origin = _cam.transform.position;
            if (contentToMove != null && contentToMove.Length > 0)
            {
                foreach (Transform content in contentToMove)
                {
                    content.position = Origin + content.position;
                    content.RotateAround(_origin, Vector3.up, _originalAngle);
                    content.gameObject.SetActive(true);
                }
            }
        }
    }
}