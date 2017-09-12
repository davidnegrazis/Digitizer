using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	//vars

	private GameObject player;
	public List<GameObject> players;
	public GameObject playerObject;
	public GameObject[] EnemySpawnPoints;
	private GameObject[] PowerupSpawnPoints;
	public GameObject[] PowerupsInScene;
	public GameObject[] EnemiesInScene;
	public GameObject BossEnemy;
	public GameObject BossBattleSpotlight;
	public List<GameObject> PlayersToRespawn;
	public List<GameObject> playersInPortal;
	public List<GameObject> Boss;
	public List<string> deadPlayers;
	public GameObject[] playerSpawnPoints;
	public GameObject enemy;
	//public GameObject enemyShooter;
	public Text playerHealth;
	public Text numbEnemies;
	public Text playerLivesText;
	public GameObject powerupObject;
	public GameObject endPortal;
	public List<GameObject> rowdyChildren;
	public Text roundWords; //round teller
	private bool checkedAllDead = false;
	private bool itsOver = false;
	private float curTime; //tracks when entered infinite room
	public float TotalTimeInChallengeRoom;




	private int cycles = 0;
	public int numbPlayerLives;

	//find closest player vars
	private float minPos;
	private int x;
	private int index;
	private int q = 0;

	//round spawn function vars
	public float spawnRate;
	public float startWait;
	public float waveWait;
	public int waveNumb = 0;
	public int round;
	public int numbWaves;
	public int NumbEnemiesInWave;
	public float otherWait;
	private float endRoundWait = 2.0f;
	private float health;
	private float damage;

	//instant round crap
	private float speed;
	private float fireRate;
	private string type;

	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}
	// Use this for initialization
	void Start () {
		for (int c = 0; c < MajorSettings.numbPlayers; c++) {
			Instantiate (playerObject, new Vector3 (0.0f, 0.5f, 0.0f), Quaternion.identity);
		}
		players.AddRange (GameObject.FindGameObjectsWithTag ("Player"));
			//GameObject.FindGameObjectsWithTag ("Player");
		EnemySpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawnPoint");
		PowerupSpawnPoints = GameObject.FindGameObjectsWithTag ("PowerupSpawnPoint");
		EnemiesInScene = GameObject.FindGameObjectsWithTag ("Enemy");
		playerSpawnPoints = GameObject.FindGameObjectsWithTag ("Player Spawn Point");

		round = 1;
		StartCoroutine (RoundController ());
		StartCoroutine (spawnPowerups ());



		foreach (GameObject playa in players) {
			PlayerController playerController;
			playerController = playa.GetComponent<PlayerController> ();

			//Vector3 pos = new Vector3 (-210.0f, (cycles * -20.0f + 115.0f), 0.0f);
			Vector3 pos = new Vector3 (0.0f, 0.0f, 0.0f);

			PlayerHealthFollow playerHealthFollow;
			Text spawnText = Instantiate (playerHealth, pos, Quaternion.identity) as Text;
			spawnText.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
			playerHealthFollow = spawnText.gameObject.GetComponent<PlayerHealthFollow> ();
			playerHealthFollow.playerNumb = cycles + 1;

			playerController.playerHealthText = spawnText;
			playerController.playerNumb = cycles + 1; //set player number

			cycles++;
			//Debug.Log (spawnText.transform.position);
		}

		numbPlayerLives = 0;

		UpdateNumbLives ();

	}

	// Update is called once per frame
	void Update () {
		//players = GameObject.FindGameObjectsWithTag ("Player");
		PowerupsInScene = GameObject.FindGameObjectsWithTag ("Powerup");
		EnemiesInScene = GameObject.FindGameObjectsWithTag ("Enemy");
		numbEnemies.text = "Enemies in scene: " + EnemiesInScene.Length.ToString ();

		respawnPlayers ();

		if (deadPlayers.Count == MajorSettings.numbPlayers && !checkedAllDead) {
			checkedAllDead = true;
			StartCoroutine (LoadDefeat ());
		}

		//in case of round change, these arrays change. BAD METHOD atm.
		//players = GameObject.FindGameObjectsWithTag ("Player");
			
	}

	IEnumerator spawnWaves () {
		spawnRate = 2.0f;
		while (true) {
			GameObject spawnPoint = EnemySpawnPoints[Random.Range(0, (EnemySpawnPoints.Length))];
			Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation);
			yield return new WaitForSeconds (spawnRate);
		}
	}

	//compares distance of all players in scene and selects nearest player as the one to follow
	public GameObject FindClosestPlayer (Vector3 curPos) {
		if (players.Count > 0) {
			minPos = Mathf.Infinity;
			x = 0;
			//Vector3 curPos = transform.position;
			foreach (GameObject p in players) {
				if (p.activeSelf != false) { //assure player is active
					float distance = Vector3.Distance (p.transform.position, curPos);
					if (distance < minPos) {
						minPos = distance;
						index = x;
					}
				}
				x++;
			}
				
			player = players [index];
			//Debug.Log (player);

			//get the juiced powerup
			if (PowerupsInScene.Length > 0) {
				Powerup powerupInScene;
				foreach (GameObject o in PowerupsInScene) {
					if (o != null) {
						powerupInScene = o.GetComponent<Powerup> ();
						if (powerupInScene.powerup == "Juiced") {
							float distance = Vector3.Distance (o.transform.position, curPos);
							if (distance < minPos) {
								minPos = distance;
								player = o;
							}
						}
					}
				}
			}

			if (rowdyChildren.Count > 0) {
				foreach (GameObject xc in rowdyChildren) {
					float distance = Vector3.Distance (xc.transform.position, curPos);
					if (distance < minPos) {
						minPos = distance;
						player = xc;
					}
				}
			}
			//Debug.Log (index);
		} else {
			player = null;
		}
		return player;
	}

	public void UpdatePlayerHealth (int playerNumb, Text playerHealthText, float playerHealth, float score) {
		playerHealthText.text =  playerHealth.ToString() + " ♥ | Score: " + score.ToString();

	}

	public IEnumerator RoundController () {
		if (round == 1) {
			//prep for first wave and beyond in first round
			spawnRate = 2.0f;
			startWait = 5.0f;
			waveWait = 5.0f;
			numbWaves = 3;
			NumbEnemiesInWave = 10;
			otherWait = 4.0f;
			health = 10.0f;
			damage = 10.0f;
			roundText ("Round " + round.ToString());
			yield return new WaitForSeconds (startWait);

			for (int y = 0; y < numbWaves; y++) {
				if (itsOver)
					break;
				for (int z = 0; z < NumbEnemiesInWave; z++) {
					if (itsOver)
						break;
					GameObject spawnPoint = EnemySpawnPoints [Random.Range (0, (EnemySpawnPoints.Length))];
					GameObject spawnedEnemy = Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
					Follower follower = spawnedEnemy.GetComponent<Follower> ();
					follower.health = health;
					follower.enemyFollowerSpeed = 3.0f;
					follower.damageToDeal = damage;
					//follower.typeOfEnemy = "Brute";
					yield return new WaitForSeconds (spawnRate);
				}
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (4.0f);
				instantWave ();
				spawnRate /= 1.25f;
				NumbEnemiesInWave += 5;
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (waveWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (otherWait);
			for (int x = 0; x < 3; x++) {
				if (itsOver)
					break;
				instantWave ();
				//yield return new WaitUntil (EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (otherWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (endRoundWait);
			EndRound ();
		} else if (round == 2) {
			yield return new WaitForSeconds (2.0f);
			SceneManager.LoadScene ("round2");
			roundText ("Round " + round.ToString());
			yield return new WaitForSeconds (3.0f);
			newSceneSpawn ();

			//prep for first wave and beyond
			spawnRate = 2.0f;
			startWait = 5.0f;
			waveWait = 5.0f;
			numbWaves = 4;
			NumbEnemiesInWave = 10;
			otherWait = 4.0f;
			health = 10.0f;
			damage = 20.0f;


			yield return new WaitForSeconds (startWait);

			for (int y = 0; y < numbWaves; y++) {
				if (itsOver)
					break;
				for (int z = 0; z < NumbEnemiesInWave; z++) {
					if (itsOver)
						break;
					GameObject spawnPoint = EnemySpawnPoints [Random.Range (0, (EnemySpawnPoints.Length))];
					GameObject spawnedEnemy = Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
					Follower followShooter = spawnedEnemy.GetComponent<Follower> ();
					followShooter.health = health;
					followShooter.enemyFollowerSpeed = 2.0f;
					followShooter.fireRate = 1.5f;
					followShooter.typeOfEnemy = "FollowShooter";
					followShooter.damageToDeal = damage;
					yield return new WaitForSeconds (spawnRate);
				}
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (4.0f);
				instantWave ();
				spawnRate /= 1.25f;
				NumbEnemiesInWave += 5;
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (waveWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (otherWait);
			for (int x = 0; x < 3; x++) {
				if (itsOver)
					break;
				instantWave ();
				//yield return new WaitUntil (EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (otherWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (endRoundWait);
			EndRound ();
		} else if (round == 3) {
			yield return new WaitForSeconds (2.0f);
			SceneManager.LoadScene ("round3");
			roundText ("Round " + round.ToString());
			yield return new WaitForSeconds (3.0f);
			newSceneSpawn ();

			//prep for first wave and beyond
			spawnRate = 0.75f;
			startWait = 5.0f;
			waveWait = 5.0f;
			numbWaves = 3;
			NumbEnemiesInWave = 10;
			otherWait = 4.0f;
			health = 20.0f;

			yield return new WaitForSeconds (startWait);

			for (int y = 0; y < numbWaves; y++) {
				if (itsOver)
					break;
				for (int z = 0; z < NumbEnemiesInWave; z++) {
					if (itsOver)
						break;
					GameObject spawnPoint = EnemySpawnPoints [Random.Range (0, (EnemySpawnPoints.Length))];
					GameObject spawnedEnemy = Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
					Follower followShooter = spawnedEnemy.GetComponent<Follower> ();
					followShooter.health = health;
					followShooter.typeOfEnemy = "FollowShooter";
					followShooter.enemyFollowerSpeed = 4.0f;
					followShooter.fireRate = 1.25f;
					followShooter.damageToDeal = 20.0f;
					yield return new WaitForSeconds (spawnRate);
				}
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (4.0f);
				instantWave ();
				health = 20.0f;
				spawnRate /= 1.5f;
				NumbEnemiesInWave += 5;
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (waveWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (otherWait);
			for (int x = 0; x < 3; x++) {
				if (itsOver)
					break;
				instantWave ();
				//yield return new WaitUntil (EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (otherWait);
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			yield return new WaitForSeconds (endRoundWait);
			EndRound ();
		} else if (round == 4) {
			yield return new WaitForSeconds (2.0f);
			Destroy (GameObject.FindGameObjectWithTag ("Jukebox"));
			SceneManager.LoadScene ("boss");
			roundText ("Something is coming...");
			yield return new WaitForSeconds (3.0f);
			newSceneSpawn ();
			Instantiate (BossBattleSpotlight, new Vector3 (0.0f, 9.0f, 0.0f), Quaternion.Euler (90.0f, 0.0f, 0.0f));
			yield return new WaitForSeconds (10.0f);
			Instantiate (BossEnemy, EnemySpawnPoints [Random.Range (0, EnemySpawnPoints.Length)].transform.position, Quaternion.identity);

			//prep for first wave and beyond
			spawnRate = 0.75f;
			startWait = 5.0f;
			waveWait = 5.0f;
			numbWaves = 5;
			NumbEnemiesInWave = 10;
			otherWait = 4.0f;
			health = 20.0f;

			yield return new WaitForSeconds (startWait);

			while (Boss.Count > 0) {
				if (itsOver)
					break;
				for (int y = 0; y < numbWaves; y++) {
					if (Boss.Count == 0 || itsOver) {
						break;
					}
					for (int z = 0; z < NumbEnemiesInWave; z++) {
						GameObject spawnPoint = EnemySpawnPoints [Random.Range (0, (EnemySpawnPoints.Length))];
						GameObject spawnedEnemy = Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
						Follower followShooter = spawnedEnemy.GetComponent<Follower> ();
						followShooter.health = health;
						followShooter.typeOfEnemy = "FollowShooter";
						followShooter.enemyFollowerSpeed = 4.0f;
						followShooter.fireRate = 1.25f;
						followShooter.damageToDeal = 20.0f;
						yield return new WaitForSeconds (spawnRate);
						if (Boss.Count == 0 || itsOver) {
							break;
						}
					}
					yield return new WaitUntil (() => EnemiesInScene.Length == 0);
					yield return new WaitForSeconds (4.0f);
					instantWave ();
					health = 20.0f;
					spawnRate /= 1.5f;
					NumbEnemiesInWave += 5;
					yield return new WaitUntil (() => EnemiesInScene.Length == 0);
					yield return new WaitForSeconds (waveWait);
				}
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (otherWait);
				for (int x = 0; x < 3; x++) {
					if (itsOver || Boss.Count == 0)
						break;
					instantWave ();
					//yield return new WaitUntil (EnemiesInScene.Length == 0);
					//yield return new WaitForSeconds (otherWait);
				}
			}
			yield return new WaitUntil (() => EnemiesInScene.Length == 0);
			roundText ("You've defeated the boss!");
			yield return new WaitForSeconds (5.0f);
			roundText ("Go here for infinite rounds...");
			yield return new WaitForSeconds (3.0f);
			EndRound ();
		} else if (round == 5) {
			yield return new WaitForSeconds (2.0f);
			SceneManager.LoadScene ("infiniteround");
			roundText ("Challenge room");
			yield return new WaitForSeconds (3.0f);
			newSceneSpawn ();

			spawnRate = 0.75f;
			startWait = 5.0f;
			waveWait = 5.0f;
			numbWaves = 3;
			NumbEnemiesInWave = 10;
			otherWait = 4.0f;
			health = 20.0f;
			curTime = Time.time;

			while (true) {
				if (itsOver)
					break;
				health += 5.0f;

				yield return new WaitForSeconds (startWait);

				for (int y = 0; y < numbWaves; y++) {
					if (itsOver)
						break;
					for (int z = 0; z < NumbEnemiesInWave; z++) {
						if (itsOver)
							break;
						GameObject spawnPoint = EnemySpawnPoints [Random.Range (0, (EnemySpawnPoints.Length))];
						GameObject spawnedEnemy = Instantiate (enemy, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
						Follower followShooter = spawnedEnemy.GetComponent<Follower> ();
						followShooter.health = health;
						followShooter.typeOfEnemy = "FollowShooter";
						followShooter.enemyFollowerSpeed = 4.0f;
						followShooter.fireRate = 1.25f;
						followShooter.damageToDeal = 20.0f;
						yield return new WaitForSeconds (spawnRate);
					}
					yield return new WaitUntil (() => EnemiesInScene.Length == 0);
					yield return new WaitForSeconds (4.0f);
					instantWave ();
					health = 20.0f;
					spawnRate /= 1.5f;
					NumbEnemiesInWave += 5;
					yield return new WaitUntil (() => EnemiesInScene.Length == 0);
					yield return new WaitForSeconds (waveWait);
				}
				yield return new WaitUntil (() => EnemiesInScene.Length == 0);
				yield return new WaitForSeconds (otherWait);
				for (int x = 0; x < 3; x++) {
					if (itsOver)
						break;
					instantWave ();
					//yield return new WaitUntil (EnemiesInScene.Length == 0);
					yield return new WaitForSeconds (otherWait);
				}
			}

		}
	}

	//spawns enemies from all enemy spawn points at once
	void instantWave () {
		
		foreach (GameObject s in EnemySpawnPoints) {
			if (itsOver)
				break;
			GameObject spawnedEnemy = Instantiate (enemy, s.transform.position, s.transform.rotation) as GameObject;
			Follower follower = spawnedEnemy.GetComponent<Follower> ();
			follower.health = health;
			if (round == 1) {
				speed = 4.0f;
			} else if (round == 2) {
				speed = 6.0f;
				fireRate = 0.3f;
				type = "FollowShooter";
			} else if (round == 3) {
				type = "Brute";
				speed = 1.0f;
				fireRate = 3.0f;
				health = 100.0f;
			} else if (round == 4) {
				type = "Brute";
				speed = 1.0f;
				fireRate = 5.0f;
				health = 50.0f;
			}
			follower.enemyFollowerSpeed = speed;
			follower.fireRate = fireRate;
			follower.typeOfEnemy = type;
			follower.health = health;
		}

	}

	//spawn powerups at intervals
	IEnumerator spawnPowerups () {
		while (!itsOver) {
			if (players.Count > 0) {
				int[] times = new int[11] { 5, 10, 15, 15, 15, 30, 40, 10, 35, 40, 30 };
				yield return new WaitForSeconds (times[Random.Range(0, times.Length)]);

				GameObject randomSpawnPoint = PowerupSpawnPoints [Random.Range (0, (PowerupSpawnPoints.Length))]; //spawm at random powerup spawnpoint
				if (randomSpawnPoint != null) {
					Instantiate (powerupObject, randomSpawnPoint.transform.position, Quaternion.identity);
				}
				//Debug.Log (randomSpawnPoint.transform.position);
			}
		}
	}

	//alter text displaying remaining number of lives
	public void UpdateNumbLives () {
		playerLivesText.text = "Lives: " + numbPlayerLives.ToString ();
	}

	//the player spawns after a number of seconds following death
	IEnumerator respawnPlayer (GameObject j) {
		yield return new WaitForSeconds (5.0f);
		//Debug.Log ("hapenning");

		//alter player gameobject properties
		j.transform.position = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].transform.position; //respawn at random spawn pos
		j.SetActive (true);
		PlayerController jScript;
		jScript = j.GetComponent<PlayerController> ();
		jScript.playerHealth = jScript.maxHealth;
		jScript.Respawned = true;
		jScript.checkedDeath = false;
	}

	void respawnPlayers () {
		//respawn players in spawn queue
		if (PlayersToRespawn.Count > 0 && numbPlayerLives > 0) {
			int a = 0;
			foreach (GameObject j in PlayersToRespawn) {
				StartCoroutine (respawnPlayer (j));
				PlayersToRespawn.RemoveAt (a);
				if (numbPlayerLives > 0) {
					numbPlayerLives--;
					UpdateNumbLives ();
				}
				a++;
			}
				
		}
	}

	//refresh arrays tracking stage info, spawn the players
	void newSceneSpawn () {
		q = 0;
		EnemySpawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawnPoint");
		PowerupSpawnPoints = GameObject.FindGameObjectsWithTag ("PowerupSpawnPoint");
		playerSpawnPoints = GameObject.FindGameObjectsWithTag ("Player Spawn Point");
		PlayerController playerController;
		foreach (GameObject dude in playersInPortal) {
			if (dude != null) {
				playerController = dude.GetComponent<PlayerController> ();
				playerController.Respawned = true;
				dude.SetActive (true);
				dude.transform.position = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)].transform.position; //respawn at random spawn pos
				//playersInPortal.RemoveAt(q);
			}
			q++;
		}

		players.Clear ();
		players.AddRange (GameObject.FindGameObjectsWithTag ("Player"));

	}

	//say what round it is cha feel?
	void roundText (string sayThis) {
		Text ok = Instantiate (roundWords, new Vector3 (0.0f, 200.0f, 0.0f), Quaternion.identity) as Text;
		ok.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
		ok.text = sayThis;
		Destroy (ok, 3.0f);
	}

	//do crap at the end of the round
	void EndRound () {
		GameObject portalSpawnPoint = GameObject.FindGameObjectWithTag ("EndPortalSpawnPoint");
		if (!itsOver)
			Instantiate (endPortal, portalSpawnPoint.transform.position, portalSpawnPoint.transform.rotation);

		Text ok = Instantiate (roundWords, new Vector3 (0.0f, 5.0f, 0.0f), Quaternion.identity) as Text;
		ok.transform.SetParent (GameObject.FindGameObjectWithTag ("Canvas").transform, false);
		ok.text = "Round completed!";
		Destroy (ok, 4.0f);
	}

	IEnumerator LoadDefeat () {
		itsOver = true;
		if (round == 5) {
			TotalTimeInChallengeRoom = Time.time - curTime;
		}
		yield return new WaitForSeconds (5.0f);
		GameObject jukebox = GameObject.FindGameObjectWithTag ("Jukebox");
		if (jukebox != null) {
			Destroy (jukebox);
		}
		SceneManager.LoadScene ("endscreen");
	}

}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    