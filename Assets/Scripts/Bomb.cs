using UnityEngine;
using System.Collections;
using UnityEditor;

public class Bomb: Weapon {

	public float timer = 3.0f;
	PlayerControl pc;
	public GameObject weapon_prefab;
	bool bomb_dropped = false;
	//private bool onUpdate = false;
	public GameObject weapon_instance;


	//	PlayerControl pc;

	//	public Bomb(GameObject weapon_prefab, PlayerControl pc) {
	//		this.weapon_prefab = weapon_prefab;
	//		MonoBehaviour.print ("hello??");
	//		this.pc = pc;
	//	}

	//	private void OnBombDropped(GameObject bomb){
	//		
	//	}



	//	public GameObject GetInstance() {
	////		GameObject;
	////
	////		print (weapon_instance.gameObject.tag);
	////		return weapon_instance;
	//	}

	// Update is called once per frame

	void Awake() { this.weapon_instance = weapon_prefab; }

	public void Initiate() {
		weapon_instance = Instantiate( PlayerControl.instance.selected_weapon_prefab, PlayerControl.instance.transform.position, Quaternion.identity) as GameObject;
	}

	void FixedUpdate() {
		///print ("anything"); 
		if (timer <= 0) {
			Invoke ("ReleaseBomb", 1);
			//onUpdate = false;
		} else if (bomb_dropped) {
			print ("bomb dropped"); 
			timer -= Time.fixedDeltaTime;
		}
		//print (timer); 
	}

	public void ReleaseBomb () {
		//print (this.transform.tag);
		//print ("hello"); 
		bomb_dropped = true;

		//onUpdate = true;

		//if (weapon_instance != null) {
		//if (weapon_instance.gameObject.tag == "BombReleased") { 
		print ("released"); 
		if(timer <= 0) {

			//print ("timer " + timer);
			SphereCollider myCollider = weapon_instance.GetComponent<SphereCollider> ();
			myCollider.radius = 2.0f; // or whatever radius you want.
			//print ("hello");
			Destroy (weapon_instance, 3);
			//Destroy (weapon_instance, sound clip length);
		}
		//}
		//}
	}
	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Enemy") {
			SphereCollider myCollider = weapon_instance.GetComponent<SphereCollider> ();
			myCollider.radius = 2.0f; // or whatever radius you want.
			//print ("hello"); 
			Destroy (weapon_instance, 3);
			//Destroy (weapon_instance, sound clip length);
		}
	}
} 
