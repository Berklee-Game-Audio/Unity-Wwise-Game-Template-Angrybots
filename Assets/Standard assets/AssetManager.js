#pragma strict

public var numberOfActiveSpiders:int = 0;
public var numberOfActiveMechs:int = 0;
public var numberOfActiveBuzzers:int = 0;
public var numberOfActiveEnemies:int = 0;
public var numberOfHacksComplete:int = 0;
public var playerHealth:float = 75.0f;
public var player: GameObject;
public var playerDead:boolean = false;
var script: Health;

	// Use this for initialization
function Start () {
	player = GameObject.FindGameObjectWithTag("Player");
	script = player.GetComponent("Health");
	Debug.Log("player health = " + script.health);
}
// Update is called once per frame
function Update () {
	numberOfActiveEnemies = numberOfActiveSpiders + numberOfActiveMechs + numberOfActiveBuzzers;
	if(playerHealth != script.health){
		playerHealth = script.health;
	}
}

function Alive(objectName:String){
	if (objectName == "EnemySpider") {
		SpiderActive ();
	}

	if (objectName == "EnemyMech" || objectName == "ConfusedEnemyMech") {
		MechActive ();
	}

	if (objectName == "KamikazeBuzzer") {
		BuzzerActive ();
	}

}

function Dead(objectName:String){
	if (objectName == "EnemySpider") {
		SpiderDestroy ();
	}

	if (objectName == "EnemyMech" || objectName == "ConfusedEnemyMech") {
		MechDestroy ();
	}

	if (objectName == "KamikazeBuzzer") {
		BuzzerDestroy ();
	}


}

function SpiderActive(){
	numberOfActiveSpiders++;
}

function SpiderDestroy(){
	if (numberOfActiveSpiders > 0) {
		numberOfActiveSpiders--;
	}
}


function MechActive(){
	numberOfActiveMechs++;
}

function MechDestroy(){
	if (numberOfActiveMechs > 0) {
		numberOfActiveMechs--;
	}
}

function BuzzerActive(){
	numberOfActiveBuzzers++;
}

function BuzzerDestroy(){
	if (numberOfActiveBuzzers > 0) {
		numberOfActiveBuzzers--;
	}
}

function HackCompleted(){
	numberOfHacksComplete++;
}

function PlayerDead(){
	playerDead = true;
}

