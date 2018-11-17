#pragma strict
#pragma downcast
import System.Collections.Generic;

public var affected : List.<GameObject> = new List.<GameObject> ();

ActivateAffected (false);

function OnTriggerEnter (other : Collider) {
	if (other.tag == "Player"){
		ActivateAffected (true);
		//Debug.Log("enemy area - on trigger enter: " + gameObject.name);
		}
}

function OnTriggerExit (other : Collider) {
	if (other.tag == "Player"){
		ActivateAffected (false);
		//Debug.Log("enemy area - on trigger exit: " + gameObject.name);
		}
}

function ActivateAffected (state : boolean) {
	for (var go : GameObject in affected) {
		if (go == null)
			continue;
		go.SetActive (state);
		//Debug.Log("enemy area - activateaffected 1: " + gameObject.name);
		yield;
	}
	for (var tr : Transform in transform) {
		tr.gameObject.SetActive (state);
		//Debug.Log("enemy area - activatedaffected 2:" + gameObject.name);
		yield;
	}
}
