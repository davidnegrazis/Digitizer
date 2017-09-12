using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIPlayer : MonoBehaviour {
	public int playerNumb;
	public float playerHealth;
	public float wait;
	public int hits;

	private GameObject enemy;
	private GameObject[] spawnPoints;
	public GameObject follower;

	private Rigidbody rb;
	public string fire;

	public float movementSpeed;
	public float fireRate;
	public float nextShot;
	public bool OverKill = false;
	public bool Slayer = false;


	public GameObject shot;
	public Transform shotSpawn;
	public Text playerHealthText;

	MoveShot instantiatedShot;

	private GameController gameController;



	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody> ();

		playerHealth = 100.0f;

		//refer gamecontroller
		enemy = GameObject.FindGameObjectWithTag ("Enemy");
		spawnPoints = GameObject.FindGameObjectsWithTag ("EnemySpawnPoint");

		StartCoroutine (spawnWaves ());


	}

	// Update is called once per frame
	void Update () {
		enemy = GameObject.FindGameObjectWithTag ("Enemy");

		if (enemy != null) {

			//look at enemy
			Quaternion toRotate = Quaternion.LookRotation (enemy.transform.position - transform.position);
			rb.transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 10.0f));


			Shoot ();
		}

		if (hits >= 10) {
			Destroy (gameObject);
		}
	}

	//"shooooooot!" -arnold schwarzenegger
	void Shoot () {
		if (Time.time > nextShot) {
			nextShot = Time.time + fireRate;

			GameObject instantiatedGameObjectShot = Instantiate (shot, shotSpawn.position, shotSpawn.rotation) as GameObject;
			instantiatedShot = instantiatedGameObjectShot.GetComponent<MoveShot> ();
			instantiatedShot.playerNumb = playerNumb;
			if (OverKill == true) {
				instantiatedShot.overkill = true;
			} else {
				instantiatedShot.overkill = false;
			}

			if (Slayer == true) {
				instantiatedShot.shotSpeed = 15.0f;
				instantiatedShot.damageToDeal *= 2.0f;
			}

			instantiatedShot.shotSpeed = 20.0f;

		}
	}

	//spawn waves of menu followers
	IEnumerator spawnWaves () {
		yield return new WaitForSeconds (wait);
		float spawnRate = 2.5f;
		while (true) {
			GameObject spawnPoint = spawnPoints[Random.Range(0, (spawnPoints.Length))];
			Instantiate (follower, spawnPoint.transform.position, spawnPoint.transform.rotation);
			yield return new WaitForSeconds (spawnRate);
		}
	}
}