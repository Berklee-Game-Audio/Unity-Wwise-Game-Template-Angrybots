using UnityEngine;
using System.Collections;

public class WwiseWrapper : MonoBehaviour {
	private AssetManager assetManager;

	private int numberOfActiveSpiders = 0;
	private int numberOfActiveMechs = 0;
	private int numberOfActiveBuzzers = 0;
	private int numberOfActiveEnemies = 0;
	private int numberOfHacksComplete = 0;
	private float playerHealth = 75.0f;

	// Use this for initialization
	void Start () {
		WwiseEvent ("level_start");
		assetManager = this.gameObject.GetComponent ("AssetManager") as AssetManager;
	}

	void Update() {
		//Debug.Log (assetManager.numberOfActiveEnemies);
		if (numberOfActiveEnemies != assetManager.numberOfActiveEnemies) {
			numberOfActiveEnemies = assetManager.numberOfActiveEnemies;
			WwiseRPTC("number_of_active_enemies" , (float)numberOfActiveEnemies);
			if (numberOfActiveEnemies > 0) {
				WwiseEvent ("enemies_active");
			}
			if (numberOfActiveEnemies == 0) {
				WwiseEvent ("enemies_clear");
			}
		}

		if (numberOfActiveMechs != assetManager.numberOfActiveMechs) {
			numberOfActiveMechs = assetManager.numberOfActiveMechs;
			if (numberOfActiveMechs > 0) {
				WwiseEvent ("boss_active");
			}
			if (numberOfActiveMechs == 0) {
				WwiseEvent ("boss_clear");
			}
		}

		if (numberOfHacksComplete != assetManager.numberOfHacksComplete) {
			numberOfHacksComplete = assetManager.numberOfHacksComplete;
			WwiseEvent ("hack_" + numberOfHacksComplete);
		}

		if (playerHealth != assetManager.playerHealth) {
			playerHealth = assetManager.playerHealth;
			WwiseRPTC ("player_health", playerHealth);
		}

		if (assetManager.playerDead) {
			WwiseEvent ("player_die");
			assetManager.playerDead = false;

		}
			

	}

	public void WwiseEvent(string whichEvent){
		AkSoundEngine.PostEvent (whichEvent, this.gameObject);
	}

	public void WwiseRPTC(string whichRPTC, float whatValue){
		AkSoundEngine.SetRTPCValue (whichRPTC, whatValue, this.gameObject);

	}


	// enemies active, all clear, boss active, boss killed √
	// switches (hacks) √
	// enemy rtpc √
	// health rtpc √

	// parts of the level through each key door....
	// level_part_1
	// level_part_2
	// level_part_3
	// level_part_4
	// level_part_5

	// player death (currently restarts)
	// player_die


	// not finished
	// end levels (escape, 2001)
	// player movement



}
