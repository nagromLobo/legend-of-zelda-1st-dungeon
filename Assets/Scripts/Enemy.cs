using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public int damage { get; private set;}
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
        MonoBehaviour.print("OnTrigger Skeleton");
        if (other.gameObject.tag == "Tile") {
            // adjust enemy position back to the subgrid
            Vector3 currPos = this.gameObject.transform.position;
            this.gameObject.transform.position.Set(Mathf.Round(currPos.x), Mathf.Round(currPos.y), currPos.z);
            control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability));
        }
      }

    void CameraMoved(Direction d, float transitionTime) {
        //Invoke("DestroyEnemy", transitionTime);
    }

    void DestroyEnemy() {
        Destroy(gameObject);
    }

}
