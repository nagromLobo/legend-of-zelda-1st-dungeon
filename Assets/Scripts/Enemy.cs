using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public int damage;
    public int movementFramesPerSecond = 4;
    public float timeToCrossTile = 0.0f;
    public float turnProbability = 0.02f;
    public int heartCount = 1;
    public Sprite[] spriteAnimation;
    protected StateMachine animation_statemachine;
    protected StateMachine control_statemachine;
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
	void Start () {
        StartEnemyAnimation(currDirection);
        StartEnemyMovement(false);
        CameraControl.S.cameraMovedDelegate += CameraMoved;
    }
	
	// Update is called once per frame
	void Update () {
        animation_statemachine.Update();
        control_statemachine.Update();
	}

    void OnTriggerStay(Collider other) {
        //MonoBehaviour.print("OnTrigger Skeleton");
        //if (other.gameObject.tag == "Tile") {
        //    // adjust enemy position back to the subgrid
        //    Vector3 currPos = this.gameObject.transform.position;
        //    this.gameObject.transform.position.Set(Mathf.Round(currPos.x), Mathf.Round(currPos.y), currPos.z);
        //    control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability));
        //}
      }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Threshold" || other.gameObject.tag == "LockedDoor") {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            StartEnemyMovement(true);
        } else if (other.gameObject.tag == "Weapon") {
            // int damageHalfHearts = other.gameObject.getComponent<Weapon>().damage;
            int damage = 0;
            heartCount -= damage;
            if(heartCount <= 0) {
                // update room state (enemy destroyed)
                Destroy(other.gameObject);
            } 
        }
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Tile" || other.gameObject.tag == "LockedDoor") {
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
   

    public virtual void CameraMoved(Direction d, float transitionTime) {
        //Invoke("DestroyEnemy", transitionTime);
    }

    public virtual void StartEnemyMovement(bool disallowCurrentDirection) {
        if (disallowCurrentDirection) {
            control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability));
        } else {
            control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(), turnProbability));
        }
    }

    // start movement in a given direction
    public virtual void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, d, turnProbability));
    }

    public virtual void StartEnemyAnimation(Direction d) {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this, GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    void DestroyEnemy() {
        Destroy(gameObject);
    }

}
