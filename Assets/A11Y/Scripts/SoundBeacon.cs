﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBeacon : MonoBehaviour {

    public float minPulseFrequency = 1f / 5f;
    public float maxPulseFrequency = 8f;
    public PulseGenerator pulseGenerator;
    public AudioSource audioSource;
    public float cameraBoxSize = 2f;
    public float maxPitch = 2.0f;
    public float minPitch = 1.0f;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //TODO: find the realtive height with cam
        //TODO: distance

        double dist = Vector3.Distance(transform.position, _camera.gameObject.transform.position);
        float newPitch = 0f;
        float heightDifference = transform.position.y - _camera.transform.position.y;
        //Debug.Log("Height difference for " + beacon.name + ": " + heightDifference);

        if (heightDifference >= cameraBoxSize * 0.25)
        {
            newPitch = maxPitch;
            // Debug.Log(beacon.name + "Maximum pitch reached");
        }

        else if (heightDifference <= cameraBoxSize * (-0.75))
        {
            newPitch = minPitch;
            // Debug.Log(beacon.name + "Minimum pitch reached");
        }

        else
        {
            newPitch = (minPitch + (maxPitch - minPitch) * (heightDifference + 0.75f * cameraBoxSize) / cameraBoxSize);
            // Debug.Log(beacon.name + " New pitch: " + newPitch);
        }

        //beacon.GetComponent<AudioSource>().pitch = newPitch;

        if(pulseGenerator != null)
        {
            pulseGenerator.pulseFrequency = Mathf.Lerp(minPulseFrequency, maxPulseFrequency, 1f - (float)(dist / 5.0f));
            pulseGenerator.frequency = newPitch;
        }
        
        if(audioSource != null)
        {
            // audioSource.pitch = newPitch;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        if(rigidBody != null)
        {
           
            //TODO: Do something with the oscillator
            bool hitTheMesh = true;
            /*
            foreach (ContactPoint contact in collision.contacts)
            {
                if(contact.otherCollider.gameObject.name == "Original")
                {
                    hitTheMesh = true;
                }
            }
            */

            if(hitTheMesh)
            {
                rigidBody.isKinematic = true;
                rigidBody.detectCollisions = false;

                //TODO:
            }
        }

        /*
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
        */
    }
}
