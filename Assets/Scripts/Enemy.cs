using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public EntityState current_state = EntityState.NORMAL;
    public int damage = 1;
    public int movementFramesPerSecond = 4;
    public float timeToCrossTile = 0.0f;
    public float turnProbability = 0.02f;
    public int heartCount = 1;
    public float damageFlashLength = 1.0f;
    public Sprite[] spriteAnimation;
    public Color enemyDamageColor = Color.red;
    public float damageCooldown = 2.0f;
    public float damageDistancePushback = 4.0f;
    public bool stunable = false;

    public delegate void onEnemyDestroyed(GameObject enemy);
    public onEnemyDestroyed OnEnemyDestroyed;

    protected Color normalColor;
    protected float damageStartTime = 0.0f;
    protected SpriteRenderer spriteRenderer;
    protected StateMachine animation_statemachine;
    protected StateMachine control_statemachine;
    protected float lastDamageFlashTime = 0.0f;
    public Direction currDirection = Direction.SOUTH;

	public int kill_type = 0; //0, 1, or 3
	public float ItemDropFrequency = 0.3f;

    public Enemy() {
        return;
    }
    public Enemy(int damage) {
        this.damage = damage;
    }

    void Awake() {
        animation_statemachine = new StateMachine();
        control_statemachine = new StateMachine();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	// Use this for initialization
	protected virtual void Start () {
        StartEnemyAnimation(currDirection);
        StartEnemyMovement(false);
    }
	
	// Update is called once per frame
	void Update () {
        animation_statemachine.Update();
        control_statemachine.Update();
        if(current_state == EntityState.DAMAGED) {
            handleDamaged();
        } else if(current_state == EntityState.ATTACKING) {
            handleAttack();
        }
	}

    protected virtual void handleAttack() {}


    protected virtual void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Threshold" || other.gameObject.tag == "LockedDoor") {
            StartEnemyMovement(true);
        }
        OnTriggerDamageHandling(other);
    }

    protected virtual void OnTriggerDamageHandling(Collider other) {
        if ((other.gameObject.tag == "BombReleased") ||
            (other.gameObject.tag == "BoomerangReleased") ||
            (other.gameObject.tag == "Arrow") ||
            (other.gameObject.tag == "Sword")) {
            // then we have a weapon
            EnemyDamaged(other);
        }
    }

    protected virtual void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Tile" || other.gameObject.tag == "LockedDoor" || other.gameObject.tag == "Pushable") {
            StartEnemyMovement(true);
        } 
    }

    protected Vector3 adjustBackToGrid(Direction d, Vector3 pos) {
        Vector3 currPos = transform.position;
        switch (d) {
            case Direction.NORTH:
                currPos = new Vector3(currPos.x, Mathf.Floor(currPos.y), currPos.z);
                break;
            case Direction.EAST:
                currPos = new Vector3(Mathf.Floor(currPos.x), currPos.y, currPos.z);
                break;
            case Direction.SOUTH:
                currPos = new Vector3(currPos.x, Mathf.Ceil(currPos.y), currPos.z);
                break;
            case Direction.WEST:
                currPos = new Vector3(Mathf.Ceil(currPos.x), currPos.y, currPos.z);
                break;
        }
        return currPos;
    }

    public virtual void StartEnemyMovement(bool disallowCurrentDirection) {
        Direction turnDirection;
        // used for wall collisions
        if (disallowCurrentDirection) {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            turnDirection = UtilityFunctions.randomDirection(currDirection);
        } else {
            turnDirection = UtilityFunctions.randomDirection();
        }
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, turnDirection, turnProbability));
        OnEnemyTurned(turnDirection);
    }

    // start movement in a given direction
    public virtual void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, d, turnProbability));
    }

    public virtual void StartEnemyAnimation(Direction d) {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    public virtual void OnEnemyTurned(Direction d) {
        currDirection = d;
    }

    public virtual void OnEnemyAttack() {

    }

    public virtual void EnemyDamaged(Collider other) {
        Weapon w = other.GetComponent<Weapon>();
        int damageHalfHearts = w.damage;
        float stunCoolDown = w.stunCoolDown;
        int stunCooldown = 0;
        if (stunCooldown > 0) {
            control_statemachine.ChangeState(new StateEnemyStunned(this, currDirection, turnProbability, stunCooldown));
        }
        if(damageHalfHearts > 0) {
            normalColor = spriteRenderer.color;
            current_state = EntityState.DAMAGED;
            damageStartTime = Time.time;
            heartCount -= damageHalfHearts;
            if (heartCount <= 0) {
                // update room state (enemy destroyed)
				//PlayerControl. count up kills;
                OnEnemyDestroyed(this.gameObject);
				PlayerControl.instance.KillCount (this);
				PlayerControl.instance.EnemyDestroyed (this);
                Destroy(this.gameObject);
                return;
            }
            else {
                // find the nearest axis for pushback
                Vector3 pushback = UtilityFunctions.roundToNearestAxis((this.transform.position - other.transform.position).normalized);
                pushback.Set(Mathf.Round(pushback.x), Mathf.Round(pushback.y), Mathf.Round(pushback.z));
                currDirection = UtilityFunctions.DirectionFromNormal(pushback);
                control_statemachine.ChangeState(new StateEnemyDamaged(this, currDirection, turnProbability, damageCooldown / 2,  damageDistancePushback, pushback));
            }
        }
    }

    private void handleDamaged() {
        // then we should show damaged color in the flash
        if ((Time.time - lastDamageFlashTime) < (damageFlashLength)) {
            spriteRenderer.color = enemyDamageColor;
            // else if the amount of time passed is less than 2 times the flash rate, stay normal
        } else if ((Time.time - lastDamageFlashTime) < (2 * damageFlashLength)) {
            spriteRenderer.color = normalColor;
            // else start the cycle over
        } else {
            lastDamageFlashTime = Time.time;
            spriteRenderer.color = enemyDamageColor;
        }
        if ((Time.time - damageStartTime) > damageCooldown) {
            current_state = EntityState.NORMAL;
            spriteRenderer.color = normalColor;
        }

    }



    void DestroyEnemy() {
        Destroy(gameObject);
    }

}
