// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

///<summary>
/// Displays a version number on the HUD.
///</summary>
public class Version : MonoBehaviour
{
    //----------- Private Members -----------
    private string _buildVersionFile = "BuildVersion";
    [SerializeField] private Text _versionText;
    [SerializeField] private float _fadeOutTime = 12f;

    //----------- MonoBehaviour Methods -----------
    private void Start()
    {
        _versionText.text = string.Format("{0} v{1}\nmlsdk: {2}\nUnity: {3}", Application.productName,
            Application.version, MLVersion.MLSDK_VERSION_BUILD_ID, Application.unityVersion, GetVersionText());
        Destroy(gameObject, _fadeOutTime);
    }
    
    private string GetVersionText()
    {
        TextAsset buildVersion = Resources.Load (_buildVersionFile) as TextAsset;
        return buildVersion ? buildVersion.text.Trim() : "0";
    }
}
