using UnityEngine;
using System.Collections;
using System;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING};

public class PlayerControl : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public int rupee_count = 0;

    public Sprite[] link_run_down;
	public Sprite[] link_run_up;
	public Sprite[] link_run_right;
	public Sprite[] link_run_left;

	StateMachine animation_state_machine;
	StateMachine control_state_machine;
	
	public EntityState current_state = EntityState.NORMAL;
	public Direction current_direction = Direction.SOUTH;

	public GameObject selected_weapon_prefab;

    public static PlayerControl instance;

    // FIXME --> Connect all the sprites to this array (and the speed) in the inspector

    // Use this for initialization
    void Start() {
        if (instance != null) {
            Debug.LogError("Mutiple Link objects detected:");
        }
        instance = this;
		print ("in start"); 

        // Launch Idle State
        animation_state_machine = new StateMachine();
		print ("hello");
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));
    }

    // Update is called once per frame
    void Update() {
		
		//print ("horizontal "+ Input.GetAxis("Horizontal")); 
        float horizontal_input = Input.GetAxis("Horizontal");
		//print ("vertical "+ Input.GetAxis("Vertical")); 
        float vertical_input = Input.GetAxis("Vertical");
        if (horizontal_input != 0.0f) {
            vertical_input = 0.0f;
        }
		animation_state_machine.Update ();

		GetComponent<Rigidbody> ().velocity = new Vector3 (horizontal_input, -vertical_input, 0) * walkingVelocity;;

    }

    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.tag == "Rupee") {
			print ("I got one"); 
            Destroy(coll.gameObject);
            rupee_count++;
        } else if (coll.gameObject.tag == "heart") {
            // Whateva
        }
    }
}
