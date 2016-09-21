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
    public float half_heart_count = 6;
    public int small_key_count = 0;
    public bool map_retrieved = false;
    public bool compass_retrieved = false;
    public bool triforce_retrieved = false;
    public bool bow_retrieved = false;
    public bool boomerang_retrieved = false;
    public float time_to_cross_threshold = 2.0f;
    public int threshold_width = 2;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    private bool link_moving_through_doorway = false;
    private Direction link_doorway_direction;
    private float timeStartCrossThreshold;
    private Vector3 linkPosDoorwayThreshold;


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
        print("in start");

        CameraControl.S.cameraMovedDelegate += CameraMoved;

        // Launch Idle State
        animation_state_machine = new StateMachine();
        print("hello");
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        half_heart_count = max_half_heart_count;

    }

    // Update is called once per frame
    void Update() {

        ////print ("horizontal "+ Input.GetAxis("Horizontal")); 
        //      float horizontal_input = Input.GetAxis("Horizontal");
        ////print ("vertical "+ Input.GetAxis("Vertical")); 
        //      float vertical_input = Input.GetAxis("Vertical");
        //      if (horizontal_input != 0.0f) {
        //          vertical_input = 0.0f;
        //      }
        //if (link_moving_through_doorway) {
        //    // Linearly interpolate 
        //    float u = Time.time - timeStartCrossThreshold / time_to_cross_threshold;
        //    Vector3 currPos = gameObject.transform.position;
        //    // vertical
        //    if(link_doorway_direction == Direction.NORTH || link_doorway_direction == Direction.SOUTH) {
        //        float newPosY = Mathf.Lerp(linkPosDoorwayThreshold.y, linkPosDoorwayThreshold.y + threshold_width, u);
        //        gameObject.transform.position.Set(currPos.x, newPosY, currPos.z);
        //      // horizontial
        //    } else {
        //        float newPosX = Mathf.Lerp(linkPosDoorwayThreshold.x, linkPosDoorwayThreshold.x + threshold_width, u);
        //        gameObject.transform.position.Set(newPosX, currPos.y, currPos.z);
        //    }
        //}
        animation_state_machine.Update();
        control_state_machine.Update();

        //GetComponent<Rigidbody> ().velocity = new Vector3 (horizontal_input, -vertical_input, 0) * walkingVelocity;;

    }

    void OnTriggerEnter(Collider coll) {

        switch (coll.gameObject.tag) {
            // General Collectables
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
                break;
            case "Heart":
                Destroy(coll.gameObject);
                if (half_heart_count < max_half_heart_count) {
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
            // Other game actions
            case "Door":
                CameraControl.S.MoveCamera(current_direction);
                break;
            default:
                break;
        }
    }

    public void CameraMoved(Direction d, float transitionTime) {
        Invoke("MoveLinkThroughDoorway", transitionTime);
        link_doorway_direction = d;
        Sprite[] animationSprites;
        control_state_machine.ChangeState(new StateLinkStunnedMovement(this, time_to_cross_threshold + transitionTime));
        switch (d) {
            case Direction.SOUTH:
                animationSprites = link_run_up;
                break;
            case Direction.EAST:
                animationSprites = link_run_right;
                break;
            case Direction.NORTH:
                animationSprites = link_run_down;
                break;
            case Direction.WEST:
                animationSprites = link_run_left;
                break;
            default:
                animationSprites = link_run_up;
                break;
        }
        animation_state_machine.ChangeState(new StateLinkDoorMovementAnimation(this, GetComponent<SpriteRenderer>(), animationSprites, 6, time_to_cross_threshold + transitionTime));
        //kanimation_state_machine.ChangeState(new StateLinkStunnedSprite(this, gameObject.GetComponent<SpriteRenderer>(), sprite, transitionTime + time_to_cross_threshold));
    }

    private void MoveLinkThroughDoorway() {
        link_moving_through_doorway = true;
        timeStartCrossThreshold = Time.time;
        linkPosDoorwayThreshold = gameObject.transform.position;
    }
}

//    void OnCollisionEnter(Collision coll) { }
//}
