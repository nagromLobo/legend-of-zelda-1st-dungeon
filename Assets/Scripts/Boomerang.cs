using UnityEngine;
using System.Collections;
using System.Security.AccessControl;
using System;

enum BoomerangState {RELEASED, COMEBACK};

public class Boomerang : Weapon {

	public GameObject weapon_prefab;
	public GameObject weapon_instance;
	public bool released;

	//private bool startAgain = false;
	private Vector3 FinalDest;
	public Vector3 beginPoint;

	private float StartTime;
	float duration;
	float half_duration;
	float u = 0.0f;
	private bool _isLerping;
	//private float _timeStartedLerping;
	//float timer;

	//float second_timer;

	BoomerangState state;
	// Use this for initialization
	void Awake () {
		print ("I am awake");
		weapon_instance = weapon_prefab;
		released = false;
		duration = 1.0f;
		half_duration = duration / 2.0f;
		//timer = half_duration; 
		//second_timer = -1.0f;
	}

	Vector3 Position () {
		return PlayerControl.instance.transform.position;
	}

	Direction DirectionGo () {
		return PlayerControl.instance.current_direction;
	}

	void StartLerp () {
		//print ("start was called");
		_isLerping = true;
		StartTime = Time.time;
		beginPoint = Position();
	}

	public void Instantiate () {
		weapon_instance = MonoBehaviour.Instantiate(weapon_prefab, Position(), Quaternion.identity) as GameObject;
	}

	// Update is called once per frame
	void Update () {

		u = (Time.time - StartTime) / half_duration;

		//weapon_instance.transform.position = Vector3.Lerp (beginPoint, FinalDest, u);
		GameObject[] rupees = GameObject.FindGameObjectsWithTag ("Rupee");
		if (rupees.Length >= 1) {
			for(int i = 0; i < rupees.Length; ++i){
				if (Vector3.Distance (rupees [i].transform.position, weapon_instance.transform.position) <= 0.5) {
					PlayerControl.instance.rupee_count += 1;
					Hud.UpdateRupees ();
					Destroy (rupees [i]);
				}
			}
		}

		if(_isLerping)
		{
			if(state == BoomerangState.RELEASED) {

				float timeSinceStarted = Time.time - StartTime;
				float percentageComplete = timeSinceStarted / half_duration;

				transform.position = Vector3.Lerp (beginPoint, FinalDest, percentageComplete);

				if(percentageComplete >= 1.0f)
				{
					//_isLerping = false;
					StartTime = Time.time;
					state = BoomerangState.COMEBACK;
				}
			}
			if (state == BoomerangState.COMEBACK) {
				
				float timeSinceStarted = Time.time - StartTime;
				float percentageComplete = timeSinceStarted / half_duration;

				transform.position = Vector3.Lerp (FinalDest, PlayerControl.instance.transform.position, percentageComplete);

				if(percentageComplete >= 1.0f)
				{
					_isLerping = false;
					state = BoomerangState.COMEBACK;
					Destroy (weapon_instance);
				}
			}
		}
			


		//		if (released) {
		//
		//			//if (startAgain) Start();
		//			//			if (second_timer >= 0.0f) {
		//			//				second_timer -= Time.deltaTime;
		//			//			}
		//			//print ("timer " + timer); 
		//			if (state == BoomerangState.RELEASED) {
		//				if (startAgain) Start();
		//				//timer -= Time.deltaTime; 
		//				u = (Time.time - StartTime) / half_duration;
		//				print ("half_duration " + half_duration);
		//				//weapon_instance.transform.position = Vector3.MoveTowards(beginPoint, FinalDest, step);
		//				if (u >= 1) {
		//					print ("why you no get here??");
		//					state = BoomerangState.COMEBACK;
		//					weapon_instance.transform.position = FinalDest;
		//
		//				} else {
		//					print ("first final dest " + FinalDest);
		//					print ("begin at " + beginPoint);
		//					weapon_instance.transform.position = Vector3.Lerp (beginPoint, FinalDest, u);
		//
		//				}
		//			} else if (state == BoomerangState.COMEBACK) {
		//				print ("not yet god damnit"); 
		//				//second_timer = half_duration;
		//				Start ();
		//				//half_duration = duration / 2.0f;
		//				u = (Time.time - StartTime) / half_duration;
		//
		//				if (u >= 1) {
		//					print ("don't be in here this is for u is 1");
		//					weapon_instance.transform.position = Position ();
		//
		//				} else {
		//					FinalDest = weapon_instance.transform.position;
		//					//MonoBehaviour.print ("final d " + FinalDest);
		//					//MonoBehaviour.print ("weapon pos " + weapon_instance.transform.position);
		//					Vector3 newpos = Position ();
		//					//print ("go to " + newpos); 
		//					//print (StartTime + " " + Time.time + " " + half_duration); 
		//					//print (Vector3.Lerp (FinalDest, newpos, u));
		//					weapon_instance.transform.position = Vector3.Lerp (FinalDest, newpos, u);
		//				}
		//
		//				if (weapon_instance.transform.position == Position ()) {
		//					print ("destroyed");
		//					//Destroy (weapon_instance);
		//				}
		//			}
		//		}
	}

	public void ReleaseBoomerang () {
		released = true;
		//startAgain = true;
		half_duration = duration / 2.0f;
		state = BoomerangState.RELEASED;

		print ("released");
		StartLerp ();
		//if (startAgain) Start ();

		//Vector3 offset = Vector3.zero;
		Vector3 weapon_pos = Vector3.zero;
		if (DirectionGo() == Direction.NORTH) {
			//offset = new Vector3(0, 1, 0);
			weapon_pos = new Vector3(0, 4, 0);
		} else if (DirectionGo() == Direction.EAST) {
			//offset = new Vector3(1, 0, 0);
			weapon_pos = new Vector3(4, 0, 0);
		} else if (DirectionGo() == Direction.SOUTH) {
			//offset = new Vector3(0, -1, 0);
			weapon_pos = new Vector3(0, -4, 0);
		} else if (DirectionGo() == Direction.WEST) {
			//offset = new Vector3(-1, 0, 0);
			weapon_pos = new Vector3(-4, 0, 0);
		}
		//beginPoint += offset;
		FinalDest = beginPoint + weapon_pos;
		//weapon_instance.transform.position = beginPoint;
	}

		void OnTriggernEnter(Collider coll)
		{
			//if(coll.gameObject.tag == "Enemy") 
				//call damage?? IDK
			//does it bounce back if it is an enemy
			//making it 
			if (this.state == BoomerangState.COMEBACK && coll.gameObject.tag != "Enemy") { //Right??
				print("enemy hit");
				//Destroy (weapon_instance);
			} else if (this.state == BoomerangState.RELEASED && coll.gameObject.tag != "Enemy") {
				FinalDest = weapon_instance.transform.position;
				u = 1.0f;
				//timer = 0;
			}
		}
//		void OnCollisionEnter(Collision coll) {
//		print ("ive collided"); 
//			if (coll.gameObject.layer == 6 || coll.gameObject.tag == "Bomb") {
//				print ("item pick up");
//			}
//		}

}