using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalController : MonoBehaviour {
	private int EnteredPlayers;
	private GameController gameController;
	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		gameController = gameControllerObject.GetComponent<GameController> ();

	}
	
	// Update is called once per frame
	void Update () {
		//transform.GetComponent<Light>().intensity = Mathf.Abs(Mathf.Sin(Time.time));
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Player")) {
			EnteredPlayers++;
			//other.gameObject.transform.position = new Vector3 (2000.0f, 2000.0f, 2000.0f);
			gameController.playersInPortal.Add (other.gameObject);
			other.gameObject.SetActive(false);
		}

		if (gameController.players.Count == EnteredPlayers) {
			gameController.round++;
			gameController.StartCoroutine (gameController.RoundController ());
		}
			
	}
}
