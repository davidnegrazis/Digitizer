using UnityEngine;
using System.Collections;

public class EnemyRocket : MonoBehaviour {
	public float speed;
	public float damageToDeal;
	private GameController gameController;
	public GameObject playerToFollow;
	private Rigidbody rb;
	private float lookAtSpeed = 4.0f;
	public GameObject explosion;
	public float lifetime;
	// Use this for initialization
	void Start () {
		if (lifetime == 0.0f) {
			lifetime = 12.0f;
		}
		rb = GetComponent<Rigidbody> ();
		GameObject gameControllerObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObj.GetComponent<GameController> ();
		Destroy (gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		playerToFollow = gameController.FindClosestPlayer (transform.position);
		if (playerToFollow != null && playerToFollow.activeSelf) {
			Quaternion toRotate = Quaternion.LookRotation (playerToFollow.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * lookAtSpeed));
			//transform.LookAt (player.transform.position * lookAtSpeed);
			rb.velocity = transform.forward * speed;
		} else {
			Destroy (gameObject);
		}
	}

	//destroys when it "hits" these objects
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Player")) {
			Destroy (gameObject);
		} else if (other.gameObject.CompareTag ("Wall")) {
			Destroy (gameObject);
		} else if (other.gameObject.CompareTag ("Player Turret")) {
			Destroy (gameObject);
		}
	}

	//spawn explosion
	void OnDestroy() {
		Explosion explode;
		GameObject explosionObject = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
		explode = explosionObject.GetComponent<Explosion> ();
		explode.playerNumb = -1;
		explode.damageToDeal = damageToDeal * 2.0f;
		Destroy (explosionObject, 0.5f);

	}
}
