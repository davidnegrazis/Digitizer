using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour {
	private Rigidbody rb;
	public float shotSpeed = 5.0f;
	public bool overkill;
	public float damageToDeal = 10.0f;

	public float timeToDestroy;
	public float destroyTime = 5.0f;

	public int playerNumb;
	public GameObject explosion;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		timeToDestroy = Time.time + destroyTime;

		rb.velocity = transform.forward * shotSpeed;



	}

	// Update is called once per frame
	void Update () {
		if (Time.time >= timeToDestroy) {
			Destroy (gameObject);
		}

	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Wall")) {
			Destroy (gameObject);
		} else if (other.gameObject.CompareTag ("Player Shield")) {
			Destroy (gameObject);
		}
	}

	void OnDestroy() {
		if (overkill == true) {
			GameObject explosionObject = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
			Destroy (explosionObject, 0.5f);
		}


	}


}
