using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBeacon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        if(rigidBody != null)
        {
            rigidBody.isKinematic = true;
            rigidBody.detectCollisions = false;
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
