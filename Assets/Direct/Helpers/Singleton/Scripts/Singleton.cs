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
    /// Generic singleton for developer assistance.
    ///</summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //----------- Public Members -----------
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        GameObject singletonCreation = new GameObject(typeof(T).Name);
                        _instance = singletonCreation.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        //----------- Private Members -----------
        private static T _instance;
    }
}