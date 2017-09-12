using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndSceneButton : MonoBehaviour {

	void OnClick() {
		GameObject gameControllerObj = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObj != null)
			Destroy (gameControllerObj);
		GameObject canvas = GameObject.FindGameObjectWithTag ("Canvas");
		if (canvas != null)
			Destroy (canvas);
		SceneManager.LoadScene ("menu");
	}
}
