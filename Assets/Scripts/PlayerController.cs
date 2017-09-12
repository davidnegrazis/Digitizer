using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public int playerNumb;
	public float playerHealth;
	public float maxHealth = 100.0f;
	public float score;
	public float damageToDealWithShot = 10.0f;
	public List <GameObject> children;

	private Rigidbody rb;
	public string fire;

	//materials
	public Material player1;
	public Material player2;

	public float movementSpeed;
	public float fireRate;
	public float nextShot;
	public bool OverKill = false;
	public bool Slayer = false;
	public bool checkedDeath = false;
	public bool Respawned = false;
	private bool shotgun = false;
	public bool blackMagic = false;
	public bool waitingForRescue = false;

	public float rotateSpeed;
	public float horizRotate;

	public GameObject blackMagicPortal;
	public GameObject ambulance;
	public GameObject companion;
	public GameObject shield;
	public GameObject shot;
	public GameObject turret;
	public Transform shotSpawn;
	public Text playerHealthText;
	public Text roundWords;

	MoveShot instantiatedShot;

	private GameController gameController;
	private PlayerHealthFollow phealthfol;

	private int u;

	private Material mat;

	//preserve across scenes
	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}

	// Use this for initialization
	void Start () {

		//choose material
		if (playerNumb == 1) {
			mat = player1;
		} else if (playerNumb == 2) {
			mat = player2;
		}

		GetComponent<Renderer> ().material = mat;
		
		rb = GetComponent<Rigidbody> ();

		fire = "Fire" + playerNumb.ToString ();

		playerHealth = maxHealth;

		//refer gamecontroller
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

		movementSpeed = 4.0f;
		fireRate = 0.3f;

		phealthfol = playerHealthText.GetComponent<PlayerHealthFollow> ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!waitingForRescue) {
			Movement ();
			Shoot ();
			SpawnTurret ();
			Ambulance ();
		}

		//activate upgrade children
		if (playerNumb == 1) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				UpdrageChildren ();
			}
		} else if (playerNumb == 2) {
			if (Input.GetButtonDown("Fire4")) {
				UpdrageChildren ();
			}
		}

		if (playerHealth < 0.0f) {
			playerHealth = 0.0f;
		}
		gameController.UpdatePlayerHealth (playerNumb, playerHealthText, playerHealth, score);

		//on death
		if (playerHealth <= 0 && checkedDeath == false) {
			if (blackMagic) {
				blackMagic = false;
				GameObject blackPortal = Instantiate (blackMagicPortal, transform.position, transform.rotation) as GameObject;
				BlackMagicPortal blackScript = blackPortal.GetComponent<BlackMagicPortal> ();
				blackScript.player = gameObject;
				blackScript.numb = playerNumb;
				waitingForRescue = true;
				StartCoroutine (resetBlackMagic ());
				phealthfol.playerActive = false;
				//gameObject.SetActive (false);
				//Debug.Log ("LOLOLOL");
			}
			if (!waitingForRescue) {
				if (gameController.numbPlayerLives > 0) {
					phealthfol.playerActive = false;
				}

				//Destroy (gameObject);
				JudgeDeath ();

				checkedDeath = true;
			}
				
		}
		//reset player health text
		if (Respawned) {
			phealthfol.playerActive = true;
			playerHealthText.gameObject.SetActive (true);
			SpawnShield (3.0f);
			Respawned = false;
			waitingForRescue = false;
		}
	}

	IEnumerator resetBlackMagic () { //wait a few seconds before dying with black magic
		yield return new WaitForSeconds (10.0f);
		Debug.Log ("ienumerator doing shit");
		waitingForRescue = false;
	}

	void Shoot () {
		if (Input.GetButton(fire) && Time.time > nextShot) {
			nextShot = Time.time + fireRate;

			SpawnTheBullet (shotSpawn.rotation);

			//buckshot spread
			if (shotgun) {
				SpawnTheBullet (Quaternion.Euler (shotSpawn.rotation.x, shotSpawn.rotation.y + 3.0f, shotSpawn.rotation.z));
				SpawnTheBullet (Quaternion.Euler (shotSpawn.rotation.x, shotSpawn.rotation.y - 6.0f, shotSpawn.rotation.z));
			}
			
		}
	}

	//stuff for spawning shot
	void SpawnTheBullet (Quaternion ang) {
		GameObject instantiatedGameObjectShot = Instantiate (shot, shotSpawn.position, ang) as GameObject;
		instantiatedShot = instantiatedGameObjectShot.GetComponent<MoveShot> ();
		instantiatedShot.playerNumb = playerNumb;
		instantiatedShot.damageToDeal = damageToDealWithShot;
		if (OverKill == true) {
			instantiatedShot.overkill = true;
		} else {
			instantiatedShot.overkill = false;
		}

		if (Slayer == true) {
			instantiatedShot.shotSpeed = 15.0f;
			instantiatedShot.damageToDeal *= 2.0f;
		}
	}

	//for looking, moving
	void Movement () {
		string horizInput = "Horizontal" + playerNumb.ToString ();
		string vertInput = "Vertical" + playerNumb.ToString ();
		float moveHoriz = Input.GetAxis (horizInput);
		float moveDepth = Input.GetAxis (vertInput);

		Vector3 movement = new Vector3 (moveHoriz, 0.0f, moveDepth);

		rb.velocity = (movement * movementSpeed);

		if (playerNumb == 1) { //look at mouse
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast (ray, out hit);
			transform.LookAt (new Vector3 (hit.point.x, transform.position.y, hit.point.z));
		} else if (playerNumb == 2) { //rotate based on right stick
			float angle = Mathf.Atan2 (Input.GetAxis ("Rotate Horiz 2"), Input.GetAxis ("Rotate Vert 2")) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.up);
		}
	}

	//powerup functions to alter player abilities

	IEnumerator RamboPowerup () {
		fireRate = 0.15f;
		yield return new WaitForSeconds (15.0f);
		fireRate = 0.3f;
	}

	IEnumerator JuicedPowerup () {
		movementSpeed *= 2.0f;
		yield return new WaitForSeconds (15.0f);
		movementSpeed = 4.0f;
	}

	IEnumerator OverkillPowerup () {
		OverKill = true;
		yield return new WaitForSeconds (15.0f);
		OverKill = false;
	}

	IEnumerator SlayerPowerup () {
		Slayer = true;
		yield return new WaitForSeconds (15.0f);
		Slayer = false;
	}

	IEnumerator ShotgunPowerup () {
		shotgun = true;
		yield return new WaitForSeconds (15.0f);
		shotgun = false;
	}

	//spawn turret things on either side of you that help defend. they follow you
	void SpawnCompanion () {
		Companion compy;
		for (int w = 0; w < 2; w++) {
			GameObject spawnedComp = Instantiate (companion, transform.position, transform.rotation) as GameObject;
			children.Add (spawnedComp);
			compy = spawnedComp.GetComponent<Companion> ();
			compy.playerNumb = playerNumb;
			compy.playerToFollow = gameObject;
			compy.lifetime = 30.0f;
			if (w == 0) {
				compy.offset = 0.5f;
			} else {
				compy.offset = -0.5f;
			}
			compy.damageToDeal = damageToDealWithShot / 2.0f;
		}

	}

	//spawn a static turret. costs 250 points
	void SpawnTurret () {
		bool doIt = false;
		if (Input.GetKeyDown (KeyCode.Mouse1) && score >= 250.0f && playerNumb == 1) {
			doIt = true;
		} else if (Input.GetButtonDown("Fire3") && score >= 250.0f && playerNumb == 2) {
			doIt = true;
		}
		if (doIt) {
			score -= 250.0f;
			PlayerTurret playerTurret;
			GameObject spawnedTurret = Instantiate (turret, transform.position, transform.rotation) as GameObject;
			playerTurret = spawnedTurret.GetComponent<PlayerTurret> ();
			playerTurret.playerNumb = playerNumb;
			playerTurret.health = 250.0f;
			playerTurret.playerToFollow = gameObject;
			playerTurret.damageToDeal = damageToDealWithShot / 4.0f;
			
		}
	}

	//make children go rambo
	void UpdrageChildren () {
		if (score >= 100.0f && children.Count > 0) {
			score -= 100.0f;
			foreach (GameObject ch in children) {
	
				Companion comp;
				comp = ch.GetComponent<Companion> ();
				comp.lifetime = 40.0f;
				comp.speed = 4.0f;
				comp.upgrade = true;
				gameController.rowdyChildren.Add (ch);
			}
			children.Clear ();
		}
	}

	//spawn a shield that protects against enemy bullets and enemies hitting you
	public void SpawnShield (float life) {
		PlayerShield playaShield;
		GameObject spawnedShield = Instantiate (shield, transform.position, transform.rotation) as GameObject;
		playaShield = spawnedShield.GetComponent<PlayerShield> ();
		playaShield.playerNumb = playerNumb;
		playaShield.playerToFollow = gameObject;
		playaShield.lifetime = life;
		playaShield.damageToDeal = damageToDealWithShot;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Powerup")) {
			Powerup powerup;
			powerup = other.GetComponent<Powerup> ();
			
			Destroy (other.gameObject);

			if (powerup.powerup == "Rambo") {
				StartCoroutine (RamboPowerup ());
			} else if (powerup.powerup == "Juiced") {
				StartCoroutine (JuicedPowerup ());
			} else if (powerup.powerup == "Overkill") {
				StartCoroutine (OverkillPowerup ());
			} else if (powerup.powerup == "Slayer") {
				StartCoroutine (SlayerPowerup ());
			} else if (powerup.powerup == "Companion") {
				SpawnCompanion ();
			} else if (powerup.powerup == "Shield") {
				SpawnShield (30.0f);
			} else if (powerup.powerup == "Shotgun") {
				StartCoroutine (ShotgunPowerup ());
			} else if (powerup.powerup == "Black Magic") {
				blackMagic = true;
				roundText ("Player " + playerNumb.ToString () + " has Black Magic...");
			}
		} else if (other.gameObject.CompareTag ("EnemyShot")) {
			EnemyBullet enemyBullet;
			enemyBullet = other.gameObject.GetComponent<EnemyBullet> ();
			Destroy (other.gameObject);
			playerHealth -= enemyBullet.damageToDeal;
		} else if (other.gameObject.CompareTag ("Enemy Shield")) {
			EnemyShield enemyShield;
			enemyShield = other.gameObject.GetComponent<EnemyShield> ();
			playerHealth -= enemyShield.damageToDeal;
			rb.velocity = (transform.forward * -20.0f);
		} else if (other.gameObject.CompareTag ("Explosion")) {
			Explosion explode;
			explode = other.gameObject.GetComponent<Explosion> ();
			if (explode.playerNumb == -1) {
				playerHealth -= explode.damageToDeal;
				//Debug.Log ("HIT");
			}
		} else if (other.gameObject.CompareTag ("Ambulance")) { //regen health
			playerHealth = maxHealth;
		}
	}

	void OnCollisionEnter (Collision thing) {
		if (thing.gameObject.CompareTag ("Spinning Laser")) {
			LaserObstacle laser;
			laser = thing.gameObject.GetComponent<LaserObstacle> ();
			playerHealth -= laser.damageToDeal;
		}
	}

	//when health is 0 judge if player shall be destroyed based on remaining lives
	void JudgeDeath () {
		checkedDeath = true;
		score -= 100.0f;
		if (gameController.numbPlayerLives > 0) {
			gameObject.SetActive (false);
			gameController.PlayersToRespawn.Add (gameObject);
		} else {
			Destroy (gameObject);
			gameController.players.Remove (gameObject);
		}
	}

	//add info when dead to list
	void OnDestroy () {
		phealthfol.playerExists = false;
		gameController.players.Remove (gameObject);
		string info = "Player " + playerNumb.ToString () + " score: " + score.ToString ();
		gameController.deadPlayers.Add (info);
		Destroy (playerHealthText);
	}

	//spawn an ambulance that goes to you for ten seconds and then heals you before leaving
	void Ambulance () {
		bool ambul = false;
		if (playerNumb == 1 && Input.GetKeyDown ("h")) {
			ambul = true;
		} else if (playerNumb == 2 && Input.GetButtonDown ("Fire5")) {
			ambul = true;
		}

		if (ambul && score >= 200.0f) {
			score -= 200.0f;
			GameObject amb = Instantiate (ambulance, gameController.EnemySpawnPoints [Random.Range (0, gameController.EnemySpawnPoints.Length)].transform.position, Quaternion.identity) as GameObject; //spawn ambulance at random enemy spawn point
			Amberlance amberlance = amb.GetComponent<Amberlance>();
			amberlance.player = gameObject;
		}
	}

	//for displaying a message
	void roundText (string showText) {
		Text ok = Instantiate (roundWords, new Vector3 (0.0f, 100.0f, 0.0f), Quaternion.identity) as Text;
		ok.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
		ok.text = showText;
		Destroy (ok, 3.0f);
	}


}