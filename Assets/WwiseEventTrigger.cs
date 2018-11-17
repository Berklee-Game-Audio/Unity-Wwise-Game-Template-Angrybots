using UnityEngine;
using System.Collections;

public class WwiseEventTrigger : MonoBehaviour {

	public string wwiseEvent;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter () {
		if(wwiseEvent != null){
			AkSoundEngine.PostEvent (wwiseEvent, GameObject.Find("WwiseGlobal"));
			Debug.Log (wwiseEvent);
		}
	}
}
