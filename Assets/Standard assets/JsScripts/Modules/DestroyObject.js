#pragma strict

var objectToDestroy : GameObject;

function OnSignal () {
	//Debug.Log("object destroyed: " + gameObject.name);
	Spawner.Destroy (objectToDestroy);

}

function OnDestroy() {
	//Debug.Log("object destroyed: " + gameObject.name);
	var go = GameObject.Find("AssetManager");
	var script: AssetManager;
	if(go != null){
		script = go.GetComponent("AssetManager");
	}

    if(script != null){
    	script.Dead(gameObject.name);
    }
}

