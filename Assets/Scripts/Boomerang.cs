using UnityEngine;
using System.Collections;
using System.Security.AccessControl;

public class Boomerang : MonoBehaviour {

	public GameObject weapon_prefab;
	public GameObject weapon_instance;
	public bool released;
	private bool startAgain = false;
	private Vector3 FinalDest;
	private float StartTime;
	private Vector3 beginPoint;
	//private Vector3 farPoint = new Vector3(0, 0, 0);
	public float duration;
	//public Vector3 pos;
	public float speed = 5.0f;
	float half_duration;

	// Use this for initialization
	void Awake () {
		weapon_instance = weapon_prefab;
		released = false;
	}
	void Start() {
		StartTime = Time.time;
		beginPoint = PlayerControl.instance.transform.position;
	}

	public void Instantiate() {
		weapon_instance = MonoBehaviour.Instantiate(weapon_prefab, PlayerControl.instance.transform.position, Quaternion.identity) as GameObject;
	}

	// Update is called once per frame
	void Update () {
		if (released) {

			//if (startAgain) Start();

			float u = (Time.time - StartTime) / half_duration;
			//weapon_instance.transform.position = Vector3.MoveTowards(beginPoint, FinalDest, step);
			if (u >= 1) {

				weapon_instance.transform.position = FinalDest;

			} else {
				
				weapon_instance.transform.position = Vector3.Lerp(beginPoint, FinalDest, u);

			}	
			
			//Start ();
			//weapon_instance.transform.position = Vector3.Lerp(FinalDest, beginPoint - offset, (Time.time/* - StartTime*/) / half_duration);

			//Destroy(weapon_instance);
		}
	}

	public void ReleaseBoomerang(float duration) {
		released = true;
		this.duration = duration;
		startAgain = true;
		half_duration = duration / 2.0f;
		//float step = speed * Time.deltaTime;

		if (startAgain) Start();

		Vector3 offset = Vector3.zero;
		Vector3 weapon_pos = Vector3.zero;
		if (PlayerControl.instance.current_direction == Direction.NORTH) {
			offset = new Vector3(0, 1, 0);
			weapon_pos = new Vector3(0, 3, 0);
		} else if (PlayerControl.instance.current_direction == Direction.EAST) {
			offset = new Vector3(1, 0, 0);
			weapon_pos = new Vector3(3, 0, 0);
		} else if (PlayerControl.instance.current_direction == Direction.SOUTH) {
			offset = new Vector3(0, -1, 0);
			weapon_pos = new Vector3(0, -3, 0);
		} else if (PlayerControl.instance.current_direction == Direction.WEST) {
			offset = new Vector3(-1, 0, 0);
			weapon_pos = new Vector3(-3, 0, 0);
		}
		beginPoint += offset;
		FinalDest = beginPoint + weapon_pos + offset;
	}

//	void OnCollisionEnter()
//	{
//		//return
//	}

//	void OnCollisionEnter() {
//		print ("don't get here yet"); 
//		if (IsFullHealth () && released) {
//			print("destroy already"); 
//			Destroy (weapon_instance);
//		}
//	}
}
