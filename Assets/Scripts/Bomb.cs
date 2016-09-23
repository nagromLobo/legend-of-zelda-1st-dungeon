using UnityEngine;
using System.Collections;

public class Bomb: MonoBehaviour {

	private float timer;
	public GameObject weapon_instance;
	//public GameObject weapon_prefab;


//	PlayerControl pc;

//	public Bomb(GameObject weapon_prefab, PlayerControl pc) {
//		this.weapon_prefab = weapon_prefab;
//		MonoBehaviour.print ("hello??");
//		this.pc = pc;
//	}

//	private void OnBombDropped(GameObject bomb){
//		
//	}


	// Use this for initialization
	public void Start () {
		//print ("I am a bomb"); 
		timer = 30.0f;
		//print (this.gameObject.tag);
	}

//	public GameObject GetInstance() {
////		GameObject;
////
////		print (weapon_instance.gameObject.tag);
////		return weapon_instance;
//	}

	// Update is called once per frame

	void FixedUpdate() {
		///print ("anything"); 
		timer -= Time.deltaTime;
	}

	public void ReleaseBomb () {
		//print (this.transform.tag);

		if (weapon_instance != null) {
			if (weapon_instance.gameObject.tag == "BombReleased") { 
				//print ("released"); 
				if (timer > 0) { 
					float time_delta = Time.deltaTime / (1.0f / Application.targetFrameRate);
					timer -= time_delta;
					print (timer);
				} else {
					SphereCollider myCollider = weapon_instance.transform.GetComponent<SphereCollider> ();
					myCollider.radius = 1f; // or whatever radius you want.
					//print ("hello"); 
					Destroy (weapon_instance);
				}
			}
		}
	}
} 
