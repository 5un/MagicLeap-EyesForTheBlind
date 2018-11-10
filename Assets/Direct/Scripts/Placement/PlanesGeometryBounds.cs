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
    /// <summary>
    /// Handles the Planar extraction bounds and gives it the same volume as WorldGeometryBounds.
    /// </summary>
    public class PlanesGeometryBounds : MonoBehaviour
    {

        //----------- Private Members -----------

        [SerializeField] WorldGeometryBounds _worldGeometryBounds;

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            if (_worldGeometryBounds != null)
            {
                transform.position = _worldGeometryBounds.transform.position;
                transform.localScale = _worldGeometryBounds.transform.localScale;
            }
        }
    }
}