using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class Grounded : MonoBehaviour {
	Rigidbody body;
	public float floor_height = 0.0f;
	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < floor_height) {
			transform.position = new Vector3(transform.position.x, floor_height, transform.position.z);
			body.velocity = new Vector3(body.velocity.x, 0.0f, body.velocity.z);
		}
	}
}
