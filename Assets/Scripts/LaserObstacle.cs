using UnityEngine;
using System.Collections;

public class LaserObstacle : MonoBehaviour {
	public float damageToDeal = 10.0f;
	public float speed; //rotate speed

	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up, Time.deltaTime * speed, Space.World); //weeeee
	}
}
