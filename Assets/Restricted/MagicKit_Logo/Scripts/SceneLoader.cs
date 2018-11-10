// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MagicKit
{
    ///<summary>
    /// Loads the next scene after x seconds.
    ///</summary>
    public class SceneLoader : MonoBehaviour 
    {

        //----------- Private Members -----------

        [SerializeField]
        private float _time = 10.0f;
        [SerializeField]
        private string _nextLevel = "NextScene";

        private float _timer = 0;

        //----------- MonoBehaviour Methods -----------

        private void Update()
        {
            _timer += Time.deltaTime;
            if(_timer > _time)
            {
                SceneManager.LoadScene(_nextLevel);
            }
        }

    }
}
