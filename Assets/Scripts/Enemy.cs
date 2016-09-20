using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public int movementFramesPerSecond = 4;
    public float velocity = 0.0f;
    public float turnProbability = 0.02f;
    public Sprite[] spriteAnimation;
    private StateMachine animation_statemachine;
    private StateMachine control_statemachine;

    void Awake() {
        animation_statemachine = new StateMachine();
        control_statemachine = new StateMachine();
    }
	// Use this for initialization
	void Start () {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this, GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
        control_statemachine.ChangeState(new StateEnemyMovement(this, velocity, randomDirection(), turnProbability));
	}
	
	// Update is called once per frame
	void Update () {
        animation_statemachine.Update();
	}

    void OnTriggerEnter() {

    }

    public Direction randomDirection() {
        // Find a random direction
        int random = Random.Range(0, 3);
        switch (random) {
            case 0:
                return Direction.NORTH;
            case 1:
                return Direction.EAST;
            case 2:
                return Direction.SOUTH;
            case 3:
                return Direction.WEST;
            default:
                // this line should never run
                return Direction.NORTH;
        }
    }
}
