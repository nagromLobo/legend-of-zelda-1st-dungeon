using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class hud : MonoBehaviour {

    public Text rupee_text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int num_player_rupees = PlayerControl.instance.rupee_count;
        rupee_text.text = "rupees: " + num_player_rupees.ToString();
	}
}
