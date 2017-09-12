using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {
	//private string[] powerupNames = new string[1] {"Shotgun"};
	//all possible powerups
	private string[] powerupNames = new string[8] {"Rambo", "Juiced", "Overkill", "Slayer", "Companion", "Shield", "Shotgun", "Black Magic"};
	public string powerup;
	// Use this for initialization
	void Start () {
		powerup = powerupNames [Random.Range (0, powerupNames.Length)]; //choose random powerup to become
		Destroy (gameObject, 20.0f);

		//Debug.Log (powerup);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (30.0f, 20.0f, 10.0f) * Time.deltaTime);
	}
}
