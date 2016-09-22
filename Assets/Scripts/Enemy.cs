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
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this, GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(), turnProbability));
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
            control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability));
        }
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Tile") {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability));
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
   

    void CameraMoved(Direction d, float transitionTime) {
        //Invoke("DestroyEnemy", transitionTime);
    }

    void DestroyEnemy() {
        Destroy(gameObject);
    }

}
