using UnityEngine;
using System.Collections;

public class Companion : MonoBehaviour {
	private float minPos;
	private int index;
	private int x;
	private int thompson;

	private Rigidbody rb;

	private GameObject enemy;
	private GameController gameController;
	public GameObject bullet;
	public GameObject playerToFollow;

	private float nextShot;
	private float fireRate = 0.25f;
	private MoveShot instantiatedShot;
	public int playerNumb;
	public float damageToDeal;
	public float offset;

	public float lifetime;
	public float health = 100.0f;
	public float speed;

	public bool upgrade = false;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

		Destroy (gameObject, lifetime);

		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		enemy = FindClosestEnemy (transform.position);
		if (enemy != null) {
			Quaternion toRotate = Quaternion.LookRotation (enemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 3.0f));
			if (upgrade) {
				if (Vector3.Distance (enemy.transform.position, transform.position) > 5.0f) {
					rb.velocity = transform.forward * speed;
				} else {
					rb.velocity = transform.forward * -speed;
				}
			}
			Shoot ();
		} else {
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			transform.Rotate (new Vector3 (0.0f, 40.0f, 0.0f) * Time.deltaTime);
		}

		if (playerToFollow.activeSelf && !upgrade) {
			transform.position = new Vector3 (playerToFollow.transform.position.x + offset, playerToFollow.transform.position.y + 0.5f, playerToFollow.transform.localPosition.z + 0.5f);
		} else if (!playerToFollow.activeSelf) {
			Destroy (gameObject);
		}

		if (upgrade && health <= 0.0f) {
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
					float distance = Vector3.Distance (p.transform.position, curPos);
					if (distance < minPos) {
						minPos = distance;
						index = x;
					}
				}
				x++;
			}

			enemy = gameController.EnemiesInScene [index];
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

			GameObject instantiatedGameObjectShot = Instantiate (bullet, transform.position, transform.rotation) as GameObject;
			instantiatedShot = instantiatedGameObjectShot.GetComponent<MoveShot> ();
			if (upgrade) {
				instantiatedShot.shotSpeed = 15.0f;
			}
			instantiatedShot.playerNumb = playerNumb;
			instantiatedShot.damageToDeal = damageToDeal;


		}
	}

	void OnTriggerEnter (Collider other) {
		if (upgrade) {
			if (other.gameObject.CompareTag ("EnemyShot")) {
				EnemyBullet enemyBullet;
				enemyBullet = other.gameObject.GetComponent<EnemyBullet> ();
				Destroy (other.gameObject);
				health -= enemyBullet.damageToDeal;
			} else if (other.gameObject.CompareTag ("Enemy Shield")) {
				EnemyShield enemyShield;
				enemyShield = other.gameObject.GetComponent<EnemyShield> ();
				health -= enemyShield.damageToDeal;
				rb.velocity = (transform.forward * -20.0f);
			} else if (other.gameObject.CompareTag ("Explosion")) {
				Explosion explode;
				explode = other.gameObject.GetComponent<Explosion> ();
				if (explode.playerNumb == -1) {
					health -= explode.damageToDeal;
				}
			}
		}
	}

	//remove self from rowdychildren list over in game controller
	//remove self from rowdychildren list over in game controller
	void OnDestroy () {
		GameController gameController;
		GameObject gameConObj = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameConObj.GetComponent<GameController> ();
		gameController.rowdyChildren.Remove (gameObject);
	}
}
