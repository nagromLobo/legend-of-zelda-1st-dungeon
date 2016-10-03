using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Net;
using System.Security.Policy;
using System.Net.Sockets;
using UnityEditorInternal.VersionControl;
using System.Threading;

public class Hud : MonoBehaviour {


	public GameObject heartPrefab;
	public List<GameObject> heartImages = new List<GameObject>();
    public Text rupee_text;
	public Text heart_text;
	public Text key_text;
	public Text bomb_text;
	public List<GameObject> weapons = new List<GameObject> (); //these are prefabs
	static List<GameObject> canSelect = new List<GameObject> ();
	//public static List<GameObject> weapon_instances = new List<GameObject> ();
	public static List<Sprite> weapon_sprites = new List<Sprite> ();
	public static Image [] slots = new Image[3];
	//private static List<Vector3> weapon_positions = new List<Vector3>();
	private bool selectionEnabled = false;
	public static bool DisableWeapons = false;
	private Canvas canvas;
	public RectTransform panel;
	float new_pos;
	float begin;
	private bool _isLerping;
	private float timeTakenDuringLerp = 2.0f;
	private int spot;
	PlayerControl pc;
	//float timer;


	private static Hud instance;
	//private bool startTimeGot = false;
	private float StartTime;

	void Awake () {
		instance = this;
		//weapon_positions.Add (Vector3());
		instance.canvas = GetComponent<Canvas> ();
		//set Slots with canvas components
		new_pos = instance.panel.transform.localPosition.y;
		//slots = instance.panel.GetComponentsInChildren<Image> ();
		Image[] comps = GetComponentsInChildren<Image>();
		int i = 0;
		foreach (Image comp in comps)
		{
			if ( comp.gameObject.name != "Panel" && i < 3)
			{
				slots [i] = comp;
				++i;
			}
		}
		print (slots [0].name); 
		//panel = instance.canvas.GetComponentInChildren <RectTransform> (); 
		begin =  instance.panel.transform.localPosition.y;
		//timer = 0.0f;
		spot = 0;
	}

	void GetStartTime() {
		//startTimeGot = true;
		StartTime = Time.unscaledTime;
	}
	float GetNextPos(){
		if (!instance.selectionEnabled) {
			print ("instance not enabled");
//			print (panel.localPosition.y);
//			print (panel.localPosition.y - Screen.height); 
			return panel.localPosition.y + (Screen.height - 50.0f);
		} else { 
			//print ("instance enabled");
			return panel.localPosition.y - (Screen.height - 50.0f);
		}
			
	}

//	static void Start() {
//		int half_heart_count = PlayerControl.instance.half_heart_count;
//		double full_heart_num = Math.Floor(PlayerControl.instance.half_heart_count/2.0f);
//		Vector3 heart_pos = PlayerControl.instance.transform.position;
//		heart_pos.x += 2;
//		heart_pos.y += 4;
//		GameObject newHeart = Instantiate(instance.heartPrefab, heart_pos, Quaternion.identity) as GameObject;
//	}

	void StartLerping() {
		_isLerping = true;
		//print ("is lerping changed " + _isLerping);
		//_timeStartedLerping = Time.time;
		begin = instance.panel.transform.localPosition.y;
		//print ("begin in lerping " + begin);
		new_pos = instance.GetNextPos (); 
		//print ("new in lerping " + new_pos);
		instance.GetStartTime ();
	}

	void Update() {
		//print ("updating"); 
		//print (panel.gameObject.name);
		if (Input.GetKeyDown (KeyCode.Return) && !_isLerping) {
//			print ("enter pressed");
//			print (instance.selectionEnabled);
			instance.selectionEnabled = !instance.selectionEnabled;
			DisableWeapons = instance.selectionEnabled;
//			print (instance.selectionEnabled);
			instance.StartLerping ();
			Time.timeScale = Time.timeScale == 0.0f ? 1.0f : 0.0f;
			//print ("begin " + begin);
			//print ("new " + new_pos);
			//un = 0;
			RenderWeaponSelection ();

		}
		if (instance.selectionEnabled) {
			//print(instance.selectionEnabled);
			slots [spot].color = Color.red;
			//print ("spot " + spot);
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				print ("no here yet");
				slots [spot].color = Color.white;
				spot = (spot + 1) % slots.Length;
				//slots [spot].color = Color.red;
			} else if (Input.GetKeyDown (KeyCode.LeftArrow) && spot > 0) {
				print ("no not here either");
				slots [spot].color = Color.white;
				spot = (spot - 1) % slots.Length;
				//slots [spot].color = Color.red;
			} else if (Input.GetKeyDown (KeyCode.A)) {
				print (spot);
				print ("how many " + canSelect.Count);
				//print (weapons [0].name);
				print(canSelect [spot].name);
				pc.Select( canSelect [spot].name);
				print (pc.selected_weapon_prefab.name);
			}
		}
		// partially sourced from: http://www.blueraja.com/blog/404/how-to-use-unity-3ds-linear-interpolation-vector3-lerp-correctly

		//print("is lerping fixed update " + _isLerping);
		if(_isLerping)
		{

			//We want percentage = 0.0 when Time.time = _timeStartedLerping
			//and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
			//In other words, we want to know what percentage of "timeTakenDuringLerp" the value
			//"Time.time - _timeStartedLerping" is.
			//print("I can lerp!!");
			float timeSinceStarted = Time.unscaledTime - StartTime;
			//timer += 1;
			//			float timeSinceStarted = 50.0f - timer;
			float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
			//float percentageComplete = timer / timeTakenDuringLerp;

			//Perform the actual lerping.  Notice that the first two parameters will always be the same
			//throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
			//to start another lerp)
			Vector3 pos = instance.panel.transform.localPosition;
			pos.y = Mathf.Lerp (begin, new_pos, percentageComplete);
			instance.panel.transform.localPosition = pos;
			//When we've completed the lerp, we set _isLerping to false
			if(percentageComplete >= 1.0f)
			{
				//timer = 0;
				//Time.timeScale = Time.timeScale == 0 ? 1 : 0;
				_isLerping = false;
			}
		}

	}
	/*void FixedUpdate() {
		//print ("hello");
		print("is lerping fixed update " + _isLerping);
		if(_isLerping)
		{
			
			//We want percentage = 0.0 when Time.time = _timeStartedLerping
			//and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
			//In other words, we want to know what percentage of "timeTakenDuringLerp" the value
			//"Time.time - _timeStartedLerping" is.
			print("I can lerp!!");
			//float timeSinceStarted = Time.time - StartTime;
			timer += 1;
//			float timeSinceStarted = 50.0f - timer;
			//float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
			float percentageComplete = timer / timeTakenDuringLerp;

			//Perform the actual lerping.  Notice that the first two parameters will always be the same
			//throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
			//to start another lerp)
			Vector3 pos = instance.panel.transform.localPosition;
			pos.y = Mathf.Lerp (begin, new_pos, percentageComplete);
			instance.panel.transform.localPosition = pos;
			//When we've completed the lerp, we set _isLerping to false
			if(percentageComplete >= 1.0f)
			{
				timer = 0;
				//Time.timeScale = Time.timeScale == 0 ? 1 : 0;
				_isLerping = false;
			}
		}
	}*/

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

	public static void AddWeapon(GameObject weapon_prefab) {
				
		if (!instance.weapons.Contains (weapon_prefab)) {
			print ("added");
			//GameObject tempingot = Instantiate(weapon_prefab) as GameObject;
			//weapons.Add (weapon_prefab);
			foreach (GameObject weapon in instance.weapons) {
				if (weapon.name == weapon_prefab.name) {
					print ("I was added " + weapon.name);
					canSelect.Add (weapon);
				}
			}
			weapon_sprites.Add (weapon_prefab.GetComponent<SpriteRenderer> ().sprite);
			//print (weapon_prefab.name); 
			print ("actually though " + instance.weapons [0]);
		}
	}

	public static void RenderWeaponSelection(){
		//for
		int i = 0;
		foreach(var weapon in weapon_sprites) {
			if (i >= slots.Length)
				break;
			else {
				Color color = slots [i].color;
				color.a = 255.0f;
				slots [i].color = color;
				slots [i].overrideSprite = weapon_sprites [i];
				print (slots [i].color);
				print (slots [i].name);
				//weapon_instances.Add (Instantiate (weapon, weapon_positions [i], Quaternion.identity) as GameObject);
				++i;
			}
		}
			
	}

//	public static void UpdateAddWeapon() {
//		
//	}
}
