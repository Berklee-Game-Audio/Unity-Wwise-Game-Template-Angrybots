using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthBar : MonoBehaviour {

    private Health _Health;

    private Slider _Slider;

	// Use this for initialization
	void Start () {
        _Slider = this.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_Health) {
            _Slider.value = _Health.health / _Health.maxHealth;
        }
        else {
            if (PlayerController.LocalPlayer)
                _Health = PlayerController.LocalPlayer.GetComponent<Health>();
        }
    }
}
