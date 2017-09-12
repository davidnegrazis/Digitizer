using UnityEngine;
using System.Collections;

public class BlackMagicPortal : MonoBehaviour {
	public GameObject player;
	public int numb;
	private PlayerController playerController;
	// Use this for initialization
	void Start () {
		playerController = player.GetComponent<PlayerController> ();
		Destroy (gameObject, 10.0f); //destroy after ten seconds
	}


	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Player")) {
			Debug.Log ("I've been hit!");
			PlayerController guyWhoHitMe;
			guyWhoHitMe = other.gameObject.GetComponent<PlayerController> ();
			if (guyWhoHitMe.playerNumb != numb) {
				player.SetActive (true);
				playerController.playerHealth = playerController.maxHealth;
				playerController.waitingForRescue = false;
				Destroy (gameObject);
			}
		}
	}

}
