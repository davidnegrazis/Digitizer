using UnityEngine;
using System.Collections;

public class PlayerTurret : MonoBehaviour {
	private float minPos;
	private int index;
	private int x;
	private int thompson;


	private GameObject enemy;
	private GameController gameController;
	public GameObject bullet;
	public GameObject playerToFollow;
	public Transform shotspawn;

	private float nextShot;
	private float fireRate = 0.15f;
	private MoveShot instantiatedShot;
	public int playerNumb;
	public float damageToDeal = 50.0f;
	public float offset;

	private float distance;

	//public float lifetime;
	public float health;
	public float speed;

	public bool upgrade = false;
	// Use this for initialization
	void Start () {

		//Destroy (gameObject, lifetime);

		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

		gameController.rowdyChildren.Add (gameObject);

	}

	// Update is called once per frame
	void Update () {
		enemy = FindClosestEnemy (transform.position);
		if (enemy != null) {
			Quaternion toRotate = Quaternion.LookRotation (enemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 20.0f));
			Shoot ();
		} else {
			//rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			transform.Rotate (new Vector3 (0.0f, 40.0f, 0.0f) * Time.deltaTime);
		}

		if (!playerToFollow.activeSelf) {
			Destroy (gameObject);
		}

		if (health <= 0.0f) {
			Destroy (gameObject);
		}
	}

	//compares distance of all enemeis in scene and selects nearest enemy as the one to look at
	public GameObject FindClosestEnemy (Vector3 curPos) {
		minPos = Mathf.Infinity;
		x = 0;
		//Vector3 curPos = transform.position;
		if (gameController.EnemiesInScene.Length > 0) {
			foreach (GameObject p in gameController.EnemiesInScene) {
				if (p.activeSelf) { //assure player is active
					distance = Vector3.Distance (p.transform.position, curPos);
					if (distance < minPos) {
						//Debug.Log (distance);
						minPos = distance;
						index = x;
					}
				}
				x++;
			}

			if (distance < 15.0f) {
				enemy = gameController.EnemiesInScene [index];
			} else {
				enemy = null;
			}

		} else {
			enemy = null;
		}
		//Debug.Log (player);

		//Debug.Log (index);
		return enemy;
	}

	void Shoot () {
		if (Time.time > nextShot) {
			nextShot = Time.time + fireRate;

			GameObject instantiatedGameObjectShot = Instantiate (bullet, shotspawn.position, shotspawn.rotation) as GameObject;
			instantiatedShot = instantiatedGameObjectShot.GetComponent<MoveShot> ();
			if (upgrade) {
				instantiatedShot.shotSpeed = 25.0f;
			}
			instantiatedShot.playerNumb = playerNumb;
			instantiatedShot.damageToDeal = damageToDeal;


		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("EnemyShot")) {
			EnemyBullet enemyBullet;
			enemyBullet = other.gameObject.GetComponent<EnemyBullet> ();
			Destroy (other.gameObject);
			health -= enemyBullet.damageToDeal;
		} if (other.gameObject.CompareTag ("Explosion")) {
			Explosion explode;
			explode = other.gameObject.GetComponent<Explosion> ();
			if (explode.playerNumb == -1) {
				health -= explode.damageToDeal;
			}
		}
	}

	//remove self from rowdychildren list over in game controller
	void OnDestroy () {
		GameController gameController;
		GameObject gameConObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameConObj.GetComponent<GameController> ();
		gameController.rowdyChildren.Remove (gameObject);
	}
}
