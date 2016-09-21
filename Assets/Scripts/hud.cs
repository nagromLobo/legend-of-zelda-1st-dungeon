using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor;

public class Hud : MonoBehaviour {


	public GameObject heartPrefab;
	public List<GameObject> heartImages = new List<GameObject>();
    public Text rupee_text;
	public Text heart_text;
	public Text key_text;
	public Text bomb_text;

	private static Hud instance;

	void Awake () {
		instance = this;
	}

//	static void Start() {
//		int half_heart_count = PlayerControl.instance.half_heart_count;
//		double full_heart_num = Math.Floor(PlayerControl.instance.half_heart_count/2.0f);
//		Vector3 heart_pos = PlayerControl.instance.transform.position;
//		heart_pos.x += 2;
//		heart_pos.y += 4;
//		GameObject newHeart = Instantiate(instance.heartPrefab, heart_pos, Quaternion.identity) as GameObject;
//	}

	public static void UpdateRupees() {
		int num_player_rupees = PlayerControl.instance.rupee_count;
		//print ("rupees: " + num_player_rupees);

		instance.rupee_text.text = "Rupees: " + num_player_rupees.ToString();
	}
	public static void UpdateLives() {
		int half_heart_count = PlayerControl.instance.half_heart_count;
		double full_heart_num = half_heart_count / 2.0f;
		instance.heart_text.text = "Lives: " + full_heart_num.ToString ();
	}
	public static void UpdateKeys() {
		int num_keys = PlayerControl.instance.small_key_count;
		instance.key_text.text = "Keys: " + num_keys.ToString ();
	}

	public static void UpdateBombs() {
		int num_bombs = PlayerControl.instance.bomb_count;
		instance.bomb_text.text = "Bombs: " + num_bombs.ToString ();
	}

//	public static void UpdateAddWeapon() {
//		
//	}
}
