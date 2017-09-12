using UnityEngine;
using System.Collections;

public class Amberlance : MonoBehaviour {
	public GameObject player;
	public GameObject engine;
	private Rigidbody rb;
	private bool touched = false;
	// Use this for initialization
	void Start () {
		engine.SetActive (false);
		rb = GetComponent<Rigidbody> ();
		StartCoroutine (Wait ());
	}
	
	// Update is called once per frame
	void Update () {
		if (!touched) { //follow player unless player is dead. if so, destroy self
			if (player != null && player.activeSelf) {
				Quaternion toRotate = Quaternion.LookRotation (transform.position - player.transform.position);
				transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 3.0f));
				//transform.LookAt (player.transform.position * lookAtSpeed);
				rb.velocity = transform.forward * -2.0f;
			} else {
				Destroy (gameObject);
			}
		} else {
			//fly away
			rb.velocity = new Vector3 (0.0f, 1.0f, 0.0f) * 5.0f;
			Quaternion toRotate = Quaternion.LookRotation(-rb.velocity);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 5.0f));
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Player")  && !touched) {
			touched = true;
			engine.SetActive (true);
			Destroy (gameObject, 10.0f);
		}
	}

	//if the player doesn't touch in time then leave
	IEnumerator Wait() {
		yield return new WaitForSeconds (10.0f);
		if (!touched) {
			touched = true;
			Destroy (gameObject, 10.0f);
		}
	}
}
