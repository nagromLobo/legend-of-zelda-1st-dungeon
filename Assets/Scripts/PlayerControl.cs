using UnityEngine;
using System.Collections;
using System;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING, DAMAGED, DOOR_TRANSITION, ENTERING_DOOR, GAME_OVER};

public class PlayerControl : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public Color LinkDamageColor;
    public float damageCooldown = 1;
    public float damageFlashLength = 0.2f;

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

    public int threshold_width = 2;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    public Sprite northDoorLeft;
    public Sprite northDoorRight;
    public Sprite eastDoor;



    private bool link_moving_through_doorway = false;
    private Direction link_doorway_direction;
    private float timeStartCrossThreshold;
    private float timeToCrossThreshold;
    private Vector3 linkPosDoorwayThreshold;
    private float damageStartTime;
    private float lastDamageFlashTime;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;


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
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        half_heart_count = max_half_heart_count;
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update() {
        switch (current_state) {
            case EntityState.DOOR_TRANSITION:
                handleDoorTransition();
                break;
            case EntityState.ENTERING_DOOR:
                handleDoorTransition();
                break;
            case EntityState.DAMAGED:
                handleDamaged();
                break;
        }
        
		animation_state_machine.Update ();
        control_state_machine.Update();

        if (control_state_machine.IsFinished()) {
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        }

    }

    private void handleDoorTransition() {
        // Linearly interpolate 
        float u = (Time.time - timeStartCrossThreshold) / timeToCrossThreshold;
        Vector3 currPos = gameObject.transform.position;
        // vertical
        if (link_doorway_direction == Direction.NORTH || link_doorway_direction == Direction.SOUTH) {
            float newPosY = Mathf.Lerp(linkPosDoorwayThreshold.y, linkPosDoorwayThreshold.y + 11 + 4, u);
            gameObject.transform.position.Set(currPos.x, newPosY, currPos.z);
            // horizontial
        } else {
            float newPosX = Mathf.Lerp(linkPosDoorwayThreshold.x, linkPosDoorwayThreshold.x + threshold_width - 11, u);
            transform.position.Set(newPosX, currPos.y, currPos.z);
        }
        if (u > 1) {
            if (current_state == EntityState.DOOR_TRANSITION) {
                timeStartCrossThreshold = Time.time;
                linkPosDoorwayThreshold = transform.position;
                current_state = EntityState.ENTERING_DOOR;
            } else if (current_state == EntityState.ENTERING_DOOR) {
                current_state = EntityState.NORMAL;
            }
        }
        current_state = EntityState.NORMAL;
    }

    private void handleDamaged() {
        // then we should show damaged color in the flash
        if((Time.time - lastDamageFlashTime) < (damageFlashLength)) {
            spriteRenderer.color = LinkDamageColor;
          // else if the amount of time passed is less than 2 times the flash rate, stay normal
        } else if((Time.time - lastDamageFlashTime) < (2* damageFlashLength)) {
            spriteRenderer.color = normalColor;
        // else start the cycle over
        } else {
            lastDamageFlashTime = Time.time;
            spriteRenderer.color = LinkDamageColor;
        }
        if((Time.time - damageStartTime) > damageCooldown) {
            current_state = EntityState.NORMAL;
            spriteRenderer.color = normalColor;
        }
        
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
            case "Door":
                if (current_state == EntityState.NORMAL) {
                    CameraControl.S.MoveCamera(current_direction);
                }
                break;
                // dont let link get damaged into a door
            case "DoorThreshold":
                //FIXME --> SET UP DOOR THRESHOLD
                if(current_state == EntityState.DAMAGED) {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
            default:
                break;
        }
    }

    public void linkDamaged(int damage, Vector3 normal) {
        normal = Vector3.Normalize(normal);
        // turns off player control
        control_state_machine.ChangeState(new StateLinkStunnedMovement(this, damageCooldown / 2, normal));
        Sprite[] animation = new Sprite[2];
        switch (current_direction) {
            case Direction.NORTH:
                animation = link_run_up;
                break;
            case Direction.EAST:
                animation = link_run_right;
                break;
            case Direction.SOUTH:
                animation = link_run_down;
                break;
            case Direction.WEST:
                animation = link_run_left;
                break;
        }
        animation_state_machine.ChangeState(new StateLinkDoorMovementAnimation(this, spriteRenderer, animation, 6, damageCooldown / 2));

        current_state = EntityState.DAMAGED;
        half_heart_count -= damage;
        if(half_heart_count <= 0) {
            current_state = EntityState.GAME_OVER;
        }

        damageStartTime = Time.time;
        lastDamageFlashTime = damageStartTime;
        normalColor = GetComponent<SpriteRenderer>().color;
        
    }

    public void CameraMoved(Direction d, float transitionTime) {
        current_state = EntityState.DOOR_TRANSITION;
        timeStartCrossThreshold = Time.time;
        timeToCrossThreshold = transitionTime;
        linkPosDoorwayThreshold = gameObject.transform.position;
        link_doorway_direction = d;
        Sprite[] animationSprites;
        control_state_machine.ChangeState(new StateLinkStunnedMovement(this, transitionTime * 2, Vector3.zero));
        switch (d) {
            case Direction.SOUTH:
                animationSprites = link_run_down;
                break;
            case Direction.EAST:
                animationSprites = link_run_right;
                break;
            case Direction.NORTH:
                animationSprites = link_run_up;
                break;
            case Direction.WEST:
                animationSprites = link_run_left;
                break;
            default:
                animationSprites = link_run_up;
                break;
        }
        animation_state_machine.ChangeState(new StateLinkDoorMovementAnimation(this, GetComponent<SpriteRenderer>(), animationSprites, 6, transitionTime * 2));
        //kanimation_state_machine.ChangeState(new StateLinkStunnedSprite(this, gameObject.GetComponent<SpriteRenderer>(), sprite, transitionTime + time_to_cross_threshold));
    }

    void OnCollisionEnter(Collision other) {
        switch (other.gameObject.tag) {
            case "LockedDoor":
                if(small_key_count > 0) {
                    Tile tile = other.gameObject.GetComponent<Tile>();
                    tile.openDoor(northDoorLeft, northDoorRight, eastDoor);
                    small_key_count--;
                }
                break;
            // Enemys
            case "Enemy":
                if (current_state != EntityState.DAMAGED) {
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    linkDamaged(enemy.damage, other.contacts[0].normal);
                }
                break;
                // Other game actions
        }
    }
}

//    void OnCollisionEnter(Collision coll) { }
//}
