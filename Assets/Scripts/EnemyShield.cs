using UnityEngine;
using System.Collections;

public class EnemyShield : MonoBehaviour {
	public float shieldHealth;
	public GameObject enemyToFollow;
	public float damageToDeal;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = enemyToFollow.transform.position;
		if (shieldHealth <= 0.0f) {
			Destroy (gameObject);
		}
		if (!enemyToFollow.activeSelf) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Player")) {
			shieldHealth -= 20.0f;
		} else if (other.gameObject.CompareTag ("PlayerShot")) {
			Destroy (other.gameObject);
			MoveShot moveShot;
			moveShot = other.gameObject.GetComponent<MoveShot> ();
			shieldHealth -= moveShot.damageToDeal;
		}
	}
}
