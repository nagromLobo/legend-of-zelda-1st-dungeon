using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Direction {NORTH, EAST, SOUTH, WEST};
public enum EntityState {NORMAL, ATTACKING, DAMAGED, CAMERA_TRANSITION, ENTERING_ROOM, GAME_OVER, GRABBED};
public enum AttackType { SWORD, MAGIC_SWORD, PROJECTILE, BOMB}

public class PlayerControl : MonoBehaviour {

    public float walkingVelocity = 1.0f;
    public Color LinkDamageColor;
    public float damageCooldown = 1;
    public float flashLength = 0.2f;
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
    public bool invincibleCheat = false;
    public bool invincible = false;
    public float invincablityCooldown = 2.0f;
    public Color invincablityColor;

    public int threshold_width = 2;

    public Sprite[] link_run_down;
    public Sprite[] link_run_up;
    public Sprite[] link_run_right;
    public Sprite[] link_run_left;

    StateMachine animation_state_machine;
    StateMachine control_state_machine;

    public EntityState current_state = EntityState.NORMAL;
    public Direction current_direction = Direction.SOUTH;

    public GameObject[] Inventory; // 0 Boomerang, 2 Bomb, 3 Bow

    public GameObject Sword_prefab;

    public GameObject selected_weapon_prefab;

    public bool ________________________;
    public GameObject rupee_prefab; // set prefab
    public GameObject bomb_prefab; //set
    public GameObject heart_prefab;
    public GameObject clock_prefab;
    public bool ___________________;

    //public GameObject BowPrefab;
    public delegate void PlayerInRoom();
    public PlayerInRoom playerInRoom;

    public bool _____________________;

    public AudioClip heartRetrievedAudio;
    public AudioClip itemRetrievedAudio;
    public AudioClip ruppeeRetrievedAudio;
    public AudioClip linkHurtAudio;
    public AudioClip projectileAudio;
    public AudioClip dropingBombAudio;
    public AudioClip projectileSwordAudio;
    public AudioClip swordSlashAudio;

    private AudioSource playerAudio;
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
    private float lastInvincibilityStartTime;
    private float invincibilityStartTime;

	public GameObject BIGHEART;

    public int enemy_kill_count = 0; //mod 10
    public GameObject[,] ItemDrops; //10 by 3 array
                                    //Item drop chart from here: http://www.zeldaspeedruns.com/loz/generalknowledge/item-drops-chart
    public static PlayerControl instance;

    public delegate void AquamentusIsDead();
    public AquamentusIsDead aquamentusIsDead;

    // runs before any start function gets called
    void Awake() {
        playerAudio = GetComponent<AudioSource>();
        ItemDrops = new GameObject[10, 3];
        if (instance != null) {
            Debug.LogError("Mutiple Link objects detected:");
        }
        instance = this;
        //print("in start");
        for (int i = 0; i < 10; i++) {
            for (int j = 1; j < 3; j++) {
                ItemDrops[i, j] = rupee_prefab;
            }
        }
        ItemDrops[1, 1] = bomb_prefab;
        //ItemDrops [1] [0] = heart_prefab;
        ItemDrops[2, 2] = heart_prefab;
        ItemDrops[3, 1] = clock_prefab;
        ItemDrops[5, 1] = heart_prefab;
        ItemDrops[5, 2] = heart_prefab;
        ItemDrops[6, 1] = bomb_prefab;
        ItemDrops[6, 2] = clock_prefab;
        ItemDrops[8, 1] = bomb_prefab;
        ItemDrops[9, 1] = heart_prefab;
        ItemDrops[0, 1] = heart_prefab;

    }

    // Use this for initialization
    void Start() {
        // Launch Idle State
        startPosition = transform.position;
        animation_state_machine = new StateMachine();
        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        normalColor = GetComponent<SpriteRenderer>().color;

        CameraControl.S.cameraMovedDelegate += CameraMoved;

        animation_state_machine.ChangeState(new StateIdleWithSprite(this, GetComponent<SpriteRenderer>(), link_run_down[0]));

        control_state_machine = new StateMachine();
        control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        half_heart_count = max_half_heart_count;
        spriteRenderer = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update() {
        // handle cheat code 
        if (Input.GetKeyDown("f1")) {
            if (!invincibleCheat) {
                invincibleCheat = true;
            } else {
                invincibleCheat = false;
            }
        }
        switch (current_state) {
            case EntityState.NORMAL:
                spriteRenderer.color = normalColor;
                break;
            case EntityState.CAMERA_TRANSITION:
                handleTransitionMovement();
                break;
            case EntityState.ENTERING_ROOM:
                handleTransitionMovement();
                break;
            case EntityState.DAMAGED:
                if (half_heart_count <= 0) {
                    //animate death sequence
                    SceneManager.LoadScene("Dungeon");
                }
                handleDamaged();
                break;
            case EntityState.GRABBED:
                handleGrabbed();
                break;
        }

        if (invincible) {
            handleLinkInvincable();
        }

        animation_state_machine.Update();
        control_state_machine.Update();

        if (control_state_machine.IsFinished()) {
            control_state_machine.ChangeState(new StateLinkNormalMovement(this));
        }
    }

    private void handleTransitionMovement() {
        float u = (Time.time - timeStartTransition) / transitionTime;
        if (u > 1) {
            if (timeStartDelay == 0.0f) {
                // then we start delaying now
                timeStartDelay = Time.time;
            }
            if (((Time.time - timeStartDelay) / delayInThreshold) < 1) {
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
                if (playerInRoom != null) {
                    playerInRoom();
                }
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
        if ((Time.time - lastDamageFlashTime) < (flashLength)) {
            spriteRenderer.color = LinkDamageColor;
            // else if the amount of time passed is less than 2 times the flash rate, stay normal
        } else if ((Time.time - lastDamageFlashTime) < (2 * flashLength)) {
            spriteRenderer.color = normalColor;
            // else start the cycle over
        } else {
            lastDamageFlashTime = Time.time;
            spriteRenderer.color = LinkDamageColor;
        }
        if ((Time.time - damageStartTime) > damageCooldown) {
            current_state = EntityState.NORMAL;
            spriteRenderer.color = normalColor;
        }

    }

    private void handleLinkInvincable() {
        if ((Time.time - lastInvincibilityStartTime) < flashLength) {
            spriteRenderer.color = invincablityColor;
        } else if ((Time.time - lastInvincibilityStartTime) < (2 * flashLength)) {
            spriteRenderer.color = normalColor;
        } else {
            lastInvincibilityStartTime = Time.time;
            spriteRenderer.color = invincablityColor;
        }
        if ((Time.time - invincibilityStartTime) > invincablityCooldown) {
            invincible = false;
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
                playerAudio.clip = ruppeeRetrievedAudio;
                playerAudio.Play();
                break;
            case "Heart":
                Destroy(coll.gameObject);
                if (half_heart_count < max_half_heart_count) {
                    half_heart_count += 2;
                    if (half_heart_count > max_half_heart_count)
                        half_heart_count = max_half_heart_count;
                }
                playerAudio.clip = heartRetrievedAudio;
                playerAudio.Play();
                Hud.UpdateLives();
                break;
            case "Fairy":
                Destroy(coll.gameObject);
                half_heart_count = max_half_heart_count;
                Hud.UpdateLives();
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                break;
            case "SmallKey":
                Destroy(coll.gameObject);
                small_key_count++;
                Hud.UpdateKeys();
                playerAudio.clip = heartRetrievedAudio;
                playerAudio.Play();
                break;
            case "Clock":
                MakePlayerInvincible();
                Destroy(coll.gameObject);
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                break;
            // Weapon Collectables
            case "Bow":
                //cannot use bow unless you have arrows 
                Destroy(coll.gameObject);
                bow_retrieved = true;
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                break;
            case "Boomerang":
                Destroy(coll.gameObject);
                MonoBehaviour.print("Detroy collect Boomerang");
                boomerang_retrieved = true;
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                break;
            case "Bomb":
                Destroy(coll.gameObject);
                bomb_count++;
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                //update weapon selection
                //if(bomb_count>=1) ; //add to weapons list
                Hud.UpdateBombs();
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
                playerAudio.clip = itemRetrievedAudio;
                playerAudio.Play();
                break;
            case "HeartContainer":
                Destroy(coll.gameObject);
                max_half_heart_count += 2;
                half_heart_count = max_half_heart_count;
                playerAudio.clip = heartRetrievedAudio;
                playerAudio.Play();
                Hud.UpdateLives();
                break;
            case "Door":
                if ((current_state == EntityState.NORMAL) || (current_state == EntityState.DAMAGED)) {
                    CameraControl.S.MoveCamera(current_direction);
                }
                break;
            // dont let link get damaged into a door
            case "Threshold":
                if (current_state == EntityState.DAMAGED) {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
            case "EnemyProjectiles":
                //TO DO: Test me!!
                Vector3 EnemyProjectilePos = coll.gameObject.transform.position.normalized;
                if (current_direction == UtilityFunctions.reverseDirection(UtilityFunctions.DirectionFromNormal(EnemyProjectilePos))) //LOL!!
                    Destroy(coll.gameObject);
                else {
                    linkDamaged(1, EnemyProjectilePos);
                }
                break;
            default:
                break;
        }
    }

    void OnTriggerStay(Collider coll) {
        switch (coll.gameObject.tag) {
            case "Threshold":
                if (current_state == EntityState.DAMAGED) {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
        }
    }

    public void MakePlayerInvincible() {
        invincible = true;
        lastInvincibilityStartTime = Time.time;
        invincibilityStartTime = lastInvincibilityStartTime;
        spriteRenderer.color = invincablityColor;
    }


    public void linkDamaged(int damage, Vector3 normal) {
        if (!invincibleCheat && !invincible) {
            playerAudio.clip = linkHurtAudio;
            playerAudio.Play();
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
            if (half_heart_count <= 0) {
                current_state = EntityState.GAME_OVER;
            }

            damageStartTime = Time.time;
            lastDamageFlashTime = damageStartTime;
        }
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
                if (small_key_count > 0) {
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

    public void KillCount(Enemy e) {
        enemy_kill_count = (enemy_kill_count + 1) % 10;

    }

    public void EnemyDestroyed(Enemy e) {
        //enemy 0 doesn't drop any items
        if (e.kill_type != 0) {
            if (UnityEngine.Random.value <= e.ItemDropFrequency) {
                GameObject prefab = ItemDrops[enemy_kill_count, e.kill_type];
                GameObject new_item_drop = Instantiate(prefab, e.gameObject.transform.position, Quaternion.identity) as GameObject;
                //could do this properly but I won't
            }
        }
		if (e.name == "Aquamentus(Clone)") {
			GameObject new_item_drop = Instantiate(BIGHEART, e.gameObject.transform.position, Quaternion.identity) as GameObject;
            if(aquamentusIsDead != null) {
                aquamentusIsDead();
            }
		}
    }

    public void OnAttack(AttackType attack) {
        switch (attack) {
            case AttackType.SWORD:
                playerAudio.clip = swordSlashAudio;
                playerAudio.Play();
                break;
            case AttackType.MAGIC_SWORD:
                playerAudio.clip = projectileSwordAudio;
                playerAudio.Play();
                break;
            case AttackType.BOMB:
                playerAudio.clip = dropingBombAudio;
                playerAudio.Play();
                break;
            case AttackType.PROJECTILE:
                playerAudio.clip = projectileAudio;
                playerAudio.Play();
                break;
        }
    }
}
