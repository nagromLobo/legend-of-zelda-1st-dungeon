using UnityEngine;
using System.Collections;
using System;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING};

public class PlayerControl : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public int rupee_count = 0;
    public int bomb_count = 0;
    public int max_half_heart_count = 6;
    public int half_heart_count = 6;
    public int small_key_count = 0;
    public bool map_retrieved = false;
    public bool compass_retrieved = false;
    public bool triforce_retrieved = false;
    public bool bow_retrieved = false;
    public bool boomerang_retrieved = false;

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
		//print("in start"); 

        // Launch Idle State
        animation_state_machine = new StateMachine();
        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));

		//print ("hello");
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));
        half_heart_count = max_half_heart_count;

    }

    // Update is called once per frame
    void Update() {
		
//		//print ("horizontal "+ Input.GetAxis("Horizontal")); 
//        float horizontal_input = Input.GetAxis("Horizontal");
//		//print ("vertical "+ Input.GetAxis("Vertical")); 
//        float vertical_input = Input.GetAxis("Vertical");
//        if (horizontal_input != 0.0f) {
//            vertical_input = 0.0f;
//        }
		animation_state_machine.Update ();
        control_state_machine.Update();

        if (control_state_machine.IsFinished()) {
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        }
		//GetComponent<Rigidbody> ().velocity = new Vector3 (horizontal_input, -vertical_input, 0) * walkingVelocity;

    }

    void OnTriggerEnter(Collider coll) {

        switch (coll.gameObject.tag) {
            // General Collectables
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
				Hud.RefreshDisplay ();
                break;
            case "Heart":
                Destroy(coll.gameObject);
                if(half_heart_count < max_half_heart_count) {
                    half_heart_count += 2;
                }
                break;
            case "Fairy":
                Destroy(coll.gameObject);
                half_heart_count = max_half_heart_count;
                break;
            case "SmallKey":
                Destroy(coll.gameObject);
                small_key_count++;
                break;
            // Weapon Collectables
            case "Bow":
                Destroy(coll.gameObject);
                bow_retrieved = true;
                break;
            case "Boomerang":
                Destroy(coll.gameObject);
                boomerang_retrieved = true;
                break;
            case "Bomb":
                Destroy(coll.gameObject);
                bomb_count++;
                break;
            // Dungeon State Collectables
            case "Map":
                Destroy(coll.gameObject);
                map_retrieved = true;
                break;
            case "Compass":
                Destroy(coll.gameObject);
                compass_retrieved = true;
                break;
            // HUD State Collectables
            case "Triforce":
                // FIXME need animation here
                Destroy(coll.gameObject);
                triforce_retrieved = true;
                break;
            case "HeartContainer":
                Destroy(coll.gameObject);
                max_half_heart_count += 2;
                half_heart_count = max_half_heart_count;
                break;
            default:
                break;
        }
    }
}
