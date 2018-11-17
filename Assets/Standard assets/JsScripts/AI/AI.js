#pragma strict

// Public member data
public var behaviourOnSpotted : MonoBehaviour;
public var soundOnSpotted : AudioClip;
public var behaviourOnLostTrack : MonoBehaviour;

// Private member data
private var character : Transform;
private var player : Transform;
private var insideInterestArea : boolean = true;
private var activeFlag: boolean = false;

function Awake () {
	character = transform;
	player = GameObject.FindWithTag ("Player").transform;
	behaviourOnSpotted.enabled = false;
}

function OnEnable () {
	behaviourOnLostTrack.enabled = true;
	behaviourOnSpotted.enabled = false;
}

function OnTriggerEnter (other : Collider) {
	if (other.transform == player && CanSeePlayer ()) {
		OnSpotted ();
	}
}

function OnEnterInterestArea () {
	insideInterestArea = true;
}

function OnExitInterestArea () {
	insideInterestArea = false;
	OnLostTrack ();
}

function OnSpotted () {
	if (!insideInterestArea)
		return;
	if (!behaviourOnSpotted.enabled) {
		behaviourOnSpotted.enabled = true;
		behaviourOnLostTrack.enabled = false;
		
		if (GetComponent.<AudioSource>() && soundOnSpotted) {
			GetComponent.<AudioSource>().clip = soundOnSpotted;
			GetComponent.<AudioSource>().Play ();
		}

		if(!activeFlag){
			Debug.Log("enemy active:" + transform.parent.gameObject.name);
			var go = GameObject.Find("AssetManager");
			var script: AssetManager;
			script = go.GetComponent("AssetManager");
    		script.Alive(transform.parent.gameObject.name);
    		activeFlag = true;
    	}
        
	}
}

function OnLostTrack () {
	if (!behaviourOnLostTrack.enabled) {
		behaviourOnLostTrack.enabled = true;
		behaviourOnSpotted.enabled = false;
	}
	Debug.Log("enemy lost track:" + transform.parent.gameObject.name);
	var go = GameObject.Find("AssetManager");
	var script: AssetManager;
	script = go.GetComponent("AssetManager");
    script.Dead(transform.parent.gameObject.name);
}

function CanSeePlayer () : boolean {
	var playerDirection : Vector3 = (player.position - character.position);
	var hit : RaycastHit;
	Physics.Raycast (character.position, playerDirection, hit, playerDirection.magnitude);
	if (hit.collider && hit.collider.transform == player) {
		return true;
	}

	return false;
}
