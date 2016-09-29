using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING, DAMAGED, CAMERA_TRANSITION, ENTERING_ROOM, GAME_OVER, GRABBED};

public class PlayerControl : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public Color LinkDamageColor;
    public float damageCooldown = 1;
    public float damageFlashLength = 0.2f;
    public float timeToEnterThreshold = 1.0f;
    public float delayInThreshold = 1.0f;
    public float timeToLeaveThreshold = 1.0f;
    public float distanceToLeaveThreshold = 2.0f;
    public float distanceToEnterThreshold = 2.0f;

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

    public int threshold_width = 2;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    public Sprite northDoorLeft;
    public Sprite northDoorRight;
    public Sprite eastDoor;
    public Sprite westDoor;

    StateMachine animation_state_machine;
	StateMachine control_state_machine;

	public EntityState current_state = EntityState.NORMAL;
	public Direction current_direction = Direction.SOUTH;

	public GameObject[] Inventory; // 0 Boomerang, 2 Bomb, 3 Bow

	public GameObject Sword_prefab;

	public GameObject selected_weapon_prefab;

    private Direction link_doorway_direction;
    private float timeStartDelay = 0.0f;
    private float timeStartTransition;
    private float transitionTime;
    private Vector3 posStartTransition;
    private Vector3 posEndTransition;
    private float damageStartTime;
    private float lastDamageFlashTime;
    private SpriteRenderer spriteRenderer;
    private Color normalColor;
    private Vector3 startPosition;
    private Wallmaster linkGrabber;

    public static PlayerControl instance;

    // runs before any start function gets called
    void Awake() {
        if (instance != null) {
            Debug.LogError("Mutiple Link objects detected:");
        }
        instance = this;
        //print("in start"); 
    }

    // Use this for initialization
    void Start() {

        // Launch Idle State
        startPosition = transform.position;
        animation_state_machine = new StateMachine();
        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));

        CameraControl.S.cameraMovedDelegate += CameraMoved;

        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        half_heart_count = max_half_heart_count;
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update() {
        switch (current_state) {
            case EntityState.CAMERA_TRANSITION:
                handleTransitionMovement();
                break;
            case EntityState.ENTERING_ROOM:
                handleTransitionMovement();
                break;
            case EntityState.DAMAGED:
                handleDamaged();
                break;
            case EntityState.GRABBED:
                handleGrabbed();
                break;
        }
        
		animation_state_machine.Update ();
        control_state_machine.Update();

        if (control_state_machine.IsFinished()) {
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        }
    }

    private void handleTransitionMovement() {
        float u = (Time.time - timeStartTransition) / transitionTime;
        if (u > 1) {
            if(timeStartDelay == 0.0f) {
                // then we start delaying now
                timeStartDelay = Time.time;
            }
            if(((Time.time - timeStartDelay) / delayInThreshold) < 1) {
                // then we have to wait
                return;
            }
            if (current_state == EntityState.CAMERA_TRANSITION) {
                // then we have to start entering room transition
                timeStartTransition = Time.time;
                transitionTime = timeToLeaveThreshold;
                posStartTransition = transform.position;
                switch (current_direction) {
                    case Direction.NORTH:
                        posEndTransition = new Vector3(posStartTransition.x, posStartTransition.y + distanceToLeaveThreshold, posStartTransition.z);
                        break;
                    case Direction.EAST:
                        posEndTransition = new Vector3(posStartTransition.x + distanceToLeaveThreshold, posStartTransition.y, posStartTransition.z);
                        break;
                    case Direction.SOUTH:
                        posEndTransition = new Vector3(posStartTransition.x, posStartTransition.y - distanceToLeaveThreshold, posStartTransition.z);
                        break;
                    case Direction.WEST:
                        posEndTransition = new Vector3(posStartTransition.x - distanceToLeaveThreshold, posStartTransition.y, posStartTransition.z);
                        break;
                }
                current_state = EntityState.ENTERING_ROOM;
            } else {
                current_state = EntityState.NORMAL;
                // have to set this back to zero so we know if we've started waiting the next time
                timeStartDelay = 0.0f;
                return;
            }
        }
        Vector3 currPos = gameObject.transform.position;
        // vertical
        if (link_doorway_direction == Direction.NORTH || link_doorway_direction == Direction.SOUTH) {
            float newPosY = Mathf.Lerp(posStartTransition.y, posEndTransition.y, u);
            currPos.Set(posStartTransition.x, newPosY, currPos.z);
            // horizontial
        } else {
            float newPosX = Mathf.Lerp(posStartTransition.x, posEndTransition.x, u);
            currPos.Set(newPosX, currPos.y, currPos.z);
        }
        transform.position = currPos;
    }

    private void handleGrabbed() {
        this.transform.position = linkGrabber.transform.position;
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

		//add arrows to this list

        switch (coll.gameObject.tag) {
            // General Collectables
            case "Rupee":
                Destroy(coll.gameObject);
                rupee_count++;
				//print ("rupees: "+ rupee_count); 
				Hud.UpdateRupees();
                break;
            case "Heart":
                Destroy(coll.gameObject);
                if (half_heart_count < max_half_heart_count) {
                    half_heart_count += 2;
					if (half_heart_count > max_half_heart_count)
						half_heart_count = max_half_heart_count;
                }
				Hud.UpdateLives ();
                break;
            case "Fairy":
                Destroy(coll.gameObject);
                half_heart_count = max_half_heart_count;
				Hud.UpdateLives ();
                break;
			case "SmallKey":
				Destroy (coll.gameObject);
				small_key_count++;
				Hud.UpdateKeys();
				break;
            // Weapon Collectables
            case "Bow":
				//cannot use bow unless you have arrows 
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
				//update weapon selection
				//if(bomb_count>=1) ; //add to weapons list
				Hud.UpdateBombs ();
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
        Hud.UpdateLives();
        if(half_heart_count <= 0) {
            current_state = EntityState.GAME_OVER;
        }

        damageStartTime = Time.time;
        lastDamageFlashTime = damageStartTime;
        normalColor = GetComponent<SpriteRenderer>().color;
        
    }

    public void CameraMoved(Direction d, float cameraTransitionTime) {
        current_state = EntityState.CAMERA_TRANSITION;
        timeStartTransition = Time.time;
        transitionTime = timeToEnterThreshold;
        posStartTransition = gameObject.transform.position;

        link_doorway_direction = d;
        Sprite[] animationSprites;
        // turn off movement control
        control_state_machine.ChangeState(new StateLinkStunnedMovement(this, timeToEnterThreshold + timeToLeaveThreshold + delayInThreshold, Vector3.zero));
        switch (d) {
            case Direction.SOUTH:
                animationSprites = link_run_down;
                posEndTransition = new Vector3(posStartTransition.x, posStartTransition.y - distanceToEnterThreshold, posStartTransition.z);
                break;
            case Direction.EAST:
                animationSprites = link_run_right;
                posEndTransition = new Vector3(posStartTransition.x + distanceToEnterThreshold, posStartTransition.y, posStartTransition.z);
                break;
            case Direction.NORTH:
                animationSprites = link_run_up;
                posEndTransition = new Vector3(posStartTransition.x, posStartTransition.y + distanceToEnterThreshold, posStartTransition.z);
                break;
            case Direction.WEST:
                animationSprites = link_run_left;
                posEndTransition = new Vector3(posStartTransition.x - distanceToEnterThreshold, posStartTransition.y, posStartTransition.z);
                break;
            default:
                animationSprites = link_run_up;
                break;
        }
        // keep animation of link walking in the current direction
        animation_state_machine.ChangeState(new StateLinkDoorMovementAnimation(this, GetComponent<SpriteRenderer>(), animationSprites, 6, timeToEnterThreshold + timeToLeaveThreshold + delayInThreshold));
    }

    void OnCollisionEnter(Collision other) {
        switch (other.gameObject.tag) {
            case "LockedDoor":
                if(small_key_count > 0) {
                    Tile tile = other.gameObject.GetComponent<Tile>();
                    tile.openDoor();
                    small_key_count--;
                    Hud.UpdateKeys();
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

    public void GrabLink(Wallmaster grabber) {
        if (current_state == EntityState.NORMAL) {
            this.linkGrabber = grabber;
            Sprite[] animation = new Sprite[1];
            animation[0] = link_run_down[0];

            current_state = EntityState.GRABBED;

            control_state_machine.ChangeState(new StateLinkStunnedMovement(this, float.MaxValue, Vector3.zero));
            animation_state_machine.ChangeState(new StateLinkDoorMovementAnimation(this, spriteRenderer, animation, 6, float.MaxValue));
        }
    }

    public void OnReturnToStart() {
        transform.position = startPosition;
        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_up[0]));
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        current_state = EntityState.NORMAL;
    }
}
