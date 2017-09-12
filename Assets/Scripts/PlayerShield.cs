using UnityEngine;
using System.Collections;

public class PlayerShield : MonoBehaviour {
	public float damageToDeal;
	public GameObject playerToFollow;
	public int playerNumb;
	public float lifetime;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerToFollow.activeSelf) {
			Destroy (gameObject);
		}
		transform.position = playerToFollow.transform.position;
	}
}
