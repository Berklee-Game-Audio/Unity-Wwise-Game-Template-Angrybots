using UnityEngine;
using System.Collections;

public class ProximityDetect : MonoBehaviour {

	private GameObject player;
	private float proximityDetectDistance = 300.0f;
	private bool alreadyTriggered = false;
	private int timeOut = 10;


	// Use this for initialization
	void Start () {
		
	}


	void OnEnable () {
		alreadyTriggered = false;
	}
	
	// Update is called once per frame
	void Update () {

		//where am i? - not the player
		Vector3 position = transform.position;

		//find the player
		player = GameObject.FindWithTag("Player");

		//find the distance between the player and this
		Vector3 distance = player.transform.position - position;

		//calculate distance as a number
		float currentDistance = distance.sqrMagnitude;

		//Debug.Log ("the distance from the player is: " + currentDistance);

		if (currentDistance < proximityDetectDistance && !alreadyTriggered) {

			Debug.Log ("enemyPoximity stinger should play now!");

			AkSoundEngine.PostEvent ("boss_proximity_warning", GameObject.Find ("WwiseGlobal"));

			alreadyTriggered = true;
		}


	}
}
