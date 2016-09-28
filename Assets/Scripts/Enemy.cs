using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public EntityState current_state = EntityState.NORMAL;
    public int damage;
    public int movementFramesPerSecond = 4;
    public float timeToCrossTile = 0.0f;
    public float turnProbability = 0.02f;
    public int heartCount = 1;
    public float damageFlashLength = 1.0f;
    public Sprite[] spriteAnimation;
    public Color enemyDamageColor = Color.red;
    public float damageCooldown = 2.0f;

    public delegate void onEnemyDestroyed(GameObject enemy);
    public onEnemyDestroyed OnEnemyDestroyed;

    protected Color normalColor;
    protected float damageStartTime = 0.0f;
    protected SpriteRenderer spriteRenderer;
    protected StateMachine animation_statemachine;
    protected StateMachine control_statemachine;
    protected float lastDamageFlashTime = 0.0f;
    public Direction currDirection = Direction.SOUTH;

    public Enemy() {
        return;
    }
    public Enemy(int damage) {
        this.damage = damage;
    }

    void Awake() {
        animation_statemachine = new StateMachine();
        control_statemachine = new StateMachine();
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
        }
	}


    protected virtual void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Threshold" || other.gameObject.tag == "LockedDoor") {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            StartEnemyMovement(true);
        } else if (other.gameObject.tag == "Weapon") {
            EnemyDamaged(other.GetComponent<Weapon>());
        }
    }

    protected virtual void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Tile" || other.gameObject.tag == "LockedDoor" || other.gameObject.tag == "Pushable") {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
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
        if (disallowCurrentDirection) {
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
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this, GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    public virtual void OnEnemyTurned(Direction d) {
        currDirection = d;
    }

    public virtual void EnemyDamaged(Weapon w) {
        //int damageHalfHearts = w.damage;
        int damage = 1;
        normalColor = spriteRenderer.color;
        current_state = EntityState.DAMAGED;
        damageStartTime = Time.time;
        heartCount -= damage;
        if (heartCount <= 0) {
            // update room state (enemy destroyed)
            OnEnemyDestroyed(this.gameObject);
            Destroy(this.gameObject);
            return;
        }
        // if not destroyed animate enemy
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
