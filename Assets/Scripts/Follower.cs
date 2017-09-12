using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Follower : MonoBehaviour {
	//vars
	private Rigidbody rb;
	public float enemyFollowerSpeed;
	public float lookAtSpeed;
	public float health;
	public float damageToDeal;
	public string typeOfEnemy;

	private int playerNu;

	//public float findPlayerInterval = 3.0f;
	private float findPlayerTime;

	public GameObject shotSpawn;
	public GameObject bullet;
	private EnemyBullet enemyBullet;
	public GameObject rocket;
	private float nextShot;
	public float fireRate;
	public GameObject shield;
	private GameObject myShield;


	private GameObject player;

	private GameController gameController;



	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		//refer gamecontroller
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

		player = gameController.FindClosestPlayer (transform.position);
		if (typeOfEnemy == null) {
			typeOfEnemy = "NormalFollower";
		} else if (typeOfEnemy == "Brute") {
			if (fireRate == 0.0f) {
				fireRate = 4.0f;
			}
			if (damageToDeal == 0.0f) {
				damageToDeal = 10.0f;
			}
			nextShot = Time.time + fireRate;
			EnemyShield theShield;
			GameObject shieldObj = Instantiate (shield, transform.position, transform.rotation) as GameObject;
			myShield = shieldObj;
			theShield = shieldObj.GetComponent<EnemyShield> ();
			theShield.shieldHealth = 20.0f;
			theShield.enemyToFollow = gameObject;
			theShield.damageToDeal = 10.0f;
		}

		if (typeOfEnemy == "NormalFollower") {
			damageToDeal = 10.0f;
		}
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
			if (typeOfEnemy == "FollowShooter") {
				Shoot ();
			} else if (typeOfEnemy == "Brute") {
				ShootRockets ();
			}
		} else {
			transform.Rotate (new Vector3 (20.0f, 20.0f, 20.0f) * Time.deltaTime);
		}

		if (health <= 0) {
			Destroy (gameObject);
		}


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
		} else if (other.gameObject.CompareTag ("Boss")) {
			transform.position = gameController.EnemySpawnPoints [Random.Range (0, gameController.EnemiesInScene.Length)].transform.position; //teleport to a spawn
		}
		if (health <= 0) {
			PlayerController playerC;
			foreach (GameObject queen in gameController.players) {
				playerC = queen.GetComponent<PlayerController> ();
				if (playerC.playerNumb == playerNu) {
					playerC.score += 10.0f;
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
		if (collision.gameObject.CompareTag ("Player")) {
			PlayerController playerController;
			playerController = collision.gameObject.GetComponent<PlayerController> ();
			playerController.playerHealth -= damageToDeal;
			Destroy (gameObject);
		} else if (collision.gameObject.CompareTag ("Player Turret")) {
			PlayerTurret playerTurret;
			playerTurret = collision.gameObject.GetComponent<PlayerTurret> ();
			playerTurret.health -= damageToDeal;
			Destroy (gameObject);
		}

		if (collision.gameObject.CompareTag ("Spinning Laser")) {
			LaserObstacle laser;
			laser = collision.gameObject.GetComponent<LaserObstacle> ();
			health -= laser.damageToDeal;
		}
	}

	IEnumerator JuicedPowerup () {
		enemyFollowerSpeed = 8.0f;
		yield return new WaitForSeconds (15.0f);
		enemyFollowerSpeed = 4.0f;
	}

	void Shoot () {
		if (Time.time > nextShot) {
			nextShot = Time.time + fireRate;

			GameObject instantiatedGameObjectShot = Instantiate (bullet, shotSpawn.transform.position, shotSpawn.transform.rotation) as GameObject;
			enemyBullet = instantiatedGameObjectShot.GetComponent<EnemyBullet> ();

		}
	}

	void ShootRockets () {
		if (Time.time > nextShot) {
			EnemyRocket enemyRocket;
			nextShot = Time.time + fireRate * 2.0f;

			GameObject instantiatedGameObjectShot = Instantiate (rocket, shotSpawn.transform.position, shotSpawn.transform.rotation) as GameObject;
			enemyRocket = instantiatedGameObjectShot.GetComponent<EnemyRocket> ();
			enemyRocket.speed = 2.0f;
			enemyRocket.damageToDeal = damageToDeal;
			

		}
	}



	void OnDestroy () {
		if (myShield != null) {
			Destroy (myShield);
		}
	}
		
}
