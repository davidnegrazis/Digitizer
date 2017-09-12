using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthFollow : MonoBehaviour {
	public int playerNumb;
	private GameObject[] players;
	private GameObject playerToFollow;
	public bool playerExists;
	public bool playerActive;
	// Use this for initialization

	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject lol in players) {
			PlayerController playerController;
			playerController = lol.GetComponent<PlayerController> ();
			if (playerController.playerNumb == playerNumb) {
				playerToFollow = lol;
				playerExists = true;
				playerActive = true;
		
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//set self active according to player's state
		if (playerExists && !playerToFollow.activeSelf && gameObject.activeSelf) {
			gameObject.SetActive (false);
		}
			//destroy if player is dead for good
		if (!playerExists) {
			Destroy (this.gameObject);
		}

		if (playerToFollow == null) {
			Destroy (gameObject);
		}
			
		if (playerExists)
			transform.position = Camera.main.WorldToScreenPoint(playerToFollow.transform.position); //follow player
		//Debug.Log (transform.position);
	}



}