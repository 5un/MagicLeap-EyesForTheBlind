// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

namespace MagicKit.Gestures
{
    /// <summary>
    /// Follows one of the keypoints from a hand gesture.
    /// </summary>
    public class GestureKeypointFollower : MonoBehaviour
    {
        //----------- Private Members -----------

        private class HandPointTuple
        {
            public MLHand Hand;
            public int KeyPoint;
        }

        private HandPointTuple _pointToFollow = new HandPointTuple();

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            if (_pointToFollow.Hand != null)
            {
                FollowKeypoint();
            }
        }

        //----------- Public Methods -----------
        
        public void SetPointToFollow(MLHand hand, int keyPoint)
        {
            _pointToFollow.Hand = hand;
            _pointToFollow.KeyPoint = keyPoint;
        }

        //----------- Private Methods -----------

        private void FollowKeypoint()
        {
           List<MLKeyPoint> indexKeyPoints = _pointToFollow.Hand.Index.KeyPoints;
            if (indexKeyPoints.Count  > _pointToFollow.KeyPoint)
            {
                transform.position = indexKeyPoints[_pointToFollow.KeyPoint].Position;
            }
        }
    }
}
