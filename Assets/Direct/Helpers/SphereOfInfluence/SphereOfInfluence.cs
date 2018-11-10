// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicKit
{

    ///<summary>
    /// Resizes the sphere based on influence transform's position deviation.
    ///</summary>
    public class SphereOfInfluence : MonoBehaviour
    {

        //----------- Public Members -----------

        public Vector3 AvgPosition
        {
            get;
            private set;
        }

        public Vector3 StdDeviation
        {
            get;
            private set;
        }

        //----------- Private Members -----------

        [SerializeField] private Transform _influenceTransform;
        [SerializeField] private int _noOfFrames = 10;
        private List<Vector3> _frameData;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            if (_influenceTransform == null)
            {
                enabled = false;
                return;
            }

            _frameData = new List<Vector3>();
        }

        private void Update()
        {
            AddFrameData(_influenceTransform.position);
            CalculateStdDeviation();
            CalculateAveragePosition();

            float radius = StdDeviation.magnitude *0.5f; // divide by 2
            transform.position = AvgPosition;
            transform.localScale = new Vector3(radius, radius, radius);
        }

        //----------- Private Methods -----------

        private void AddFrameData(Vector3 position)
        {
            if (_frameData.Count == _noOfFrames)
            {
                _frameData.RemoveAt(0);
            }
            _frameData.Add(position);
        }

        private void CalculateStdDeviation()
        {
            float xDeviation = MathUtils.StandardDeviation(_frameData.Select(pos => pos.x));
            float yDeviation = MathUtils.StandardDeviation(_frameData.Select(pos => pos.y));
            float zDeviation = MathUtils.StandardDeviation(_frameData.Select(pos => pos.z));
            StdDeviation = new Vector3(xDeviation, yDeviation, zDeviation);
        }

        private void CalculateAveragePosition()
        {
            AvgPosition = new Vector3(_frameData.Average(pos => pos.x), _frameData.Average(pos => pos.y), _frameData.Average(pos => pos.z));
        }
    }
}