using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Preserve : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}
		
}
