// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------


using MagicKit;
using MagicLeap.Utilities;
using UnityEngine;

/// <summary>
/// Handles events triggered by the logo animation.
/// </summary>
public class LogoEvents : MonoBehaviour
{

    //----------- Private Members -----------

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _explosionFX;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Billboard _billboard;
    [SerializeField] private Tether _tether;
    [SerializeField] private OnTimeEvent _onTime;
    [SerializeField] private float _floatTime = 2.0f;
    [SerializeField] private float _explosionTime = 1.8f;
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Vector3 _explosionOffset;
    [SerializeField] private ParticleSystem[] splashEmitter;

    private Camera _mainCam;

    //----------- MonoBehaviour Methods -----------

    private void OnEnable()
    {
        _rigidBody.useGravity = false;
        _rigidBody.angularVelocity = Vector3.zero;
        _rigidBody.velocity = Vector3.zero;
        _billboard.enabled = true;
        _tether.enabled = true;
        _onTime.enabled = false;
    }

    private void Start()
    {
        _mainCam = Camera.main;
        //this must be called from start so we can access headpose
        ResetPosition();
    }

    //----------- Private Methods -----------

    private void ResetPosition()
    {
        transform.parent.position = _mainCam.transform.TransformPoint(_cameraOffset);
        transform.parent.LookAt(_mainCam.transform);
    }

    private void PlayAudio()
    {
        _audioSource.Play();
    }

    private void Emit()
    {
        foreach(ParticleSystem emitter in splashEmitter)
        {
            emitter.Simulate(0.0f, false, true);
            emitter.Play();
        }
    }

    private void Explode()
    {
        Invoke("Disable", _floatTime);
        Invoke("PlayExplosionFX", _explosionTime);
    }

    private void PlayExplosionFX()
    {
        _explosionFX.transform.position = transform.parent.TransformPoint(_explosionOffset); ;
        _explosionFX.transform.rotation = Quaternion.LookRotation(transform.up, transform.forward);
        _explosionFX.SetActive(true);
    }

    private void Disable()
    {
        _billboard.enabled = false;
        _tether.enabled = false;
        transform.parent.gameObject.SetActive(false);
    }
}
