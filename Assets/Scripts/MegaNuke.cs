using UnityEngine;
using System.Collections;

public class MegaNuke : MonoBehaviour {
	private Rigidbody rb;
	public GameObject explosion;
	private GameObject player;
	private GameController gameController;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.AddForce (new Vector3 (0.0f, 1000.0f, 0.0f));

		//refer gamecontroller
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {

		//look at velocity
		Quaternion toRotate = Quaternion.LookRotation(rb.velocity);
		transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 5.0f));

		if (rb.velocity.y < -2.0f && gameController.players.Count > 0) { //only start following player after launch has reached apex
			followPlaya ();
		}
	}

	void OnCollisionEnter (Collision other) {
		Destroy (gameObject);
	}

	//spawn explosion when destroyed
	void OnDestroy() {
		Explosion explode;
		GameObject explosionObject = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
		explode = explosionObject.GetComponent<Explosion> ();
		explode.playerNumb = -1;
		explode.damageToDeal = 500.0f;
		Destroy (explosionObject, 0.5f);

	}

	//direct rocket towards player
	void followPlaya () {
		player = gameController.FindClosestPlayer (transform.position);
		Quaternion toRotate = Quaternion.LookRotation (player.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, toRotate, (Time.deltaTime * 1.5f));
		rb.velocity = transform.forward * 6.0f;
	}
}
