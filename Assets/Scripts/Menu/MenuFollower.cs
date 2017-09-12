using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuFollower : MonoBehaviour {
	//vars
	private Rigidbody rb;
	public float enemyFollowerSpeed;
	public float lookAtSpeed;
	public float health;
	public float damageToDeal;
	private GameObject[] player;
	public GameObject explosion;
	private GameObject playerToFollow;

	public float findPlayerInterval = 3.0f;
	private float findPlayerTime;



	private GameController gameController;



	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		//refer gamecontroller
		player = GameObject.FindGameObjectsWithTag("Player");

		playerToFollow = player [Random.Range (0, player.Length)];
	}

	// Update is called once per frame
	void FixedUpdate () {
		/*
		if (Time.time > findPlayerTime) {
			findPlayerTime = Time.time + findPlayerInterval;
			player = gameController.FindClosestPlayer (transform.position);
		}
		*/

		Quaternion toRotate = Quaternion.LookRotation (playerToFollow.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 2.0f));
		//transform.LookAt (player.transform.position * lookAtSpeed);
		rb.velocity = transform.forward * enemyFollowerSpeed;




	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("PlayerShot")) {
			Destroy (gameObject);
		}
		if (other.gameObject.CompareTag ("Explosion")) {
			Destroy (gameObject);

		}

	}

	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.CompareTag ("Player")) {
			Destroy (gameObject);
			AIPlayer aiplayer;
			aiplayer = collision.gameObject.GetComponent<AIPlayer> ();
			aiplayer.hits++;

			//Destroy (collision.gameObject);
		}
	}

	IEnumerator JuicedPowerup () {
		enemyFollowerSpeed = 8.0f;
		yield return new WaitForSeconds (15.0f);
		enemyFollowerSpeed = 4.0f;
	}

	void OnDestroy() {
		GameObject explode = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;
		Destroy (explode, 0.1f);
	}

}
