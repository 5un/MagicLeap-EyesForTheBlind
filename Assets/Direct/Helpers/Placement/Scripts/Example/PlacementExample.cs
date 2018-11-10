// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicLeap.Utilities
{
    ///<summary>
    /// Simple example for placing content with the placement system.  Does not currently operate on device and is only meant to explain the operation of this system.
    ///</summary>
    public class PlacementExample : MonoBehaviour
    {
        //----------- Public Members -----------
        public Placement placement;
        public Transform contentToPlace;

        //----------- MonoBehaviour Methods -----------
        private void OnGUI()
        {
            if (!placement.Running)
            {
                if (GUILayout.Button("Start Placement"))
                {
                    contentToPlace.gameObject.SetActive(false);
                    placement.Place(Camera.main.transform, HandlePlacementComplete);
                }
            }
            else
            {
                if (GUILayout.Button("Confirm Placement"))
                {
                    placement.Confirm();
                }
            }
        }

        //----------- Event Handlers -----------
        private void HandlePlacementComplete(Vector3 position, Quaternion rotation)
        {
            contentToPlace.position = position;
            contentToPlace.rotation = rotation;
            contentToPlace.localScale = placement.volume;
            contentToPlace.gameObject.SetActive(true);
        }
    }
}