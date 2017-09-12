using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndThing : MonoBehaviour {
	private GameController gameController;
	public Text scores;
	public Text time;
	private string str = null;
	// Use this for initialization
	void Start () {
		//refer gamecontroller
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
			if (gameController.TotalTimeInChallengeRoom > 0.0f) {
				time.text = "You survived " + Mathf.Round (gameController.TotalTimeInChallengeRoom).ToString () + " seconds in the challenge room";
			} else {
				time.gameObject.SetActive (false);
			}
		}
		


		foreach (string v in gameController.deadPlayers) {
			str = str + "\n" + v;
		}

		scores.text = str;


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
