using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour {
	//vars
	private Rigidbody rb;
	public float enemyFollowerSpeed;
	public float lookAtSpeed;
	public float health;
	public float damageToDeal = 50.0f;
	public GameObject nuke;
	float nextShot = Time.time + 5.0f;
	public GameObject music;




	private int playerNu;



	private GameObject player;

	private GameController gameController;



	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();

		Instantiate (music, transform.position, Quaternion.identity);

		//refer gamecontroller
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

		gameController.Boss.Add (gameObject);

		player = gameController.FindClosestPlayer (transform.position);

	
	}

	// Update is called once per frame
	void FixedUpdate () {
		/*
		if (Time.time > findPlayerTime) {
			findPlayerTime = Time.time + findPlayerInterval;
			player = gameController.FindClosestPlayer (transform.position);
		}
		*/
		player = gameController.FindClosestPlayer (transform.position);

		if (player != null && player.activeSelf) {
			
			Quaternion toRotate = Quaternion.LookRotation (player.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * lookAtSpeed));

			//transform.LookAt (player.transform.position * lookAtSpeed);
			rb.velocity = transform.forward * enemyFollowerSpeed;
		} else {
			transform.Rotate (new Vector3 (0.0f, -20.0f, 0.0f) * Time.deltaTime);
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		}

		if (health <= 0) {
			Destroy (gameObject);
		}

		NukeFire ();


	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("PlayerShot")) {
			MoveShot moveShot;
			moveShot = other.gameObject.GetComponent<MoveShot> ();
			playerNu = moveShot.playerNumb;
			health -= moveShot.damageToDeal;
			Destroy (other.gameObject);
		} else if (other.gameObject.CompareTag ("Explosion")) {
			Explosion explosion;
			explosion = other.gameObject.GetComponent<Explosion> ();
			playerNu = explosion.playerNumb;
			if (playerNu != -1) {
				health -= explosion.damageToDeal;
			}
		} else if (other.gameObject.CompareTag ("Player Shield")) {
			PlayerShield playerShield;
			playerShield = other.gameObject.GetComponent<PlayerShield> ();
			health -= playerShield.damageToDeal;
		} else if (other.gameObject.CompareTag ("Player")) {
			PlayerController playa = other.gameObject.GetComponent<PlayerController> ();
			playa.playerHealth -= playa.maxHealth;
		} else if (other.gameObject.CompareTag ("Player Turret")) {
			Destroy (other.gameObject);
		}
		if (health <= 0) {
			PlayerController playerC;
			foreach (GameObject queen in gameController.players) {
				playerC = queen.GetComponent<PlayerController> ();
				if (playerC.playerNumb == playerNu) {
					playerC.score += 2000.0f;
					break;
				}
				//if (playerC.playerNumb
			}
			Destroy (gameObject);
		}
		if (other.gameObject.CompareTag ("Powerup")) {
			Powerup powerup;
			powerup = other.GetComponent<Powerup> ();

			if (powerup.powerup == "Juiced") {
				StartCoroutine (JuicedPowerup ());
				player = gameController.FindClosestPlayer (transform.position);

				Destroy (other.gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision collision) {

	}

	IEnumerator JuicedPowerup () {
		enemyFollowerSpeed = 5.0f;
		yield return new WaitForSeconds (15.0f);
		enemyFollowerSpeed = 2.0f;
	}



	void NukeFire () {
		if (Time.time > nextShot) {
			nextShot = 20.0f + Time.time;
			Instantiate (nuke, new Vector3 (transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
			Debug.Log ("nuke");
		}
			
	}
		

	void OnDestroy () {
		gameController.Boss.Remove (gameObject); //delete me from boss list
	}

}
