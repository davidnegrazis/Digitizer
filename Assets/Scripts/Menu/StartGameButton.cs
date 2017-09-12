using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour {
	public Dropdown dropdown;
	// Use this for initialization


	void OnClick() {
		MajorSettings.numbPlayers = dropdown.value + 1;
		//MajorSettings.numbPlayers = 100;
		SceneManager.LoadScene ("scene1");
	}
}
