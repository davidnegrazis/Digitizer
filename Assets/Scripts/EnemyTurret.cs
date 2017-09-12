using UnityEngine;
using System.Collections;

public class EnemyTurret : MonoBehaviour {
	private float minPos;
	private int index;
	private int x;

	private GameObject enemy;
	private GameController gameController;
	public GameObject bullet;
	public GameObject playerToFollow;
	public Transform shotspawn;

	private float nextShot;
	public float fireRate;
	public float damageToDeal;

	private float distance;


	public bool upgrade = false;
	// Use this for initialization
	void Start () {

		//Destroy (gameObject, lifetime);

		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();


	}

	// Update is called once per frame
	void Update () {
		enemy = gameController.FindClosestPlayer (transform.position);
		if (enemy != null && enemy.activeSelf) {
			Quaternion toRotate = Quaternion.LookRotation (enemy.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 20.0f));
			Shoot ();
		} else {
			//rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
			transform.Rotate (new Vector3 (0.0f, 40.0f, 0.0f) * Time.deltaTime);
		}

	}

	void Shoot () {
		if (Time.time > nextShot) {
			nextShot = Time.time + fireRate;
			GameObject instantiatedGameObjectShot = Instantiate (bullet, shotspawn.position, shotspawn.rotation) as GameObject;

			if (bullet.gameObject.CompareTag ("Enemy Rocket")) {
				EnemyRocket instantiatedShot;
				instantiatedShot = instantiatedGameObjectShot.GetComponent<EnemyRocket> ();
				instantiatedShot.damageToDeal = damageToDeal;
				instantiatedShot.lifetime = 7.0f;
				instantiatedShot.speed = 4.0f;

			} else if (bullet.gameObject.CompareTag ("EnemyShot")) {
				EnemyBullet instantiatedShot;
				instantiatedShot = instantiatedGameObjectShot.GetComponent<EnemyBullet> ();
				instantiatedShot.damageToDeal = damageToDeal;
			}
		}
	}
}
