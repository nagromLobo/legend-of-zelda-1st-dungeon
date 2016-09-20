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
        control_statemachine.ChangeState(new StateEnemyMovement(this, velocity, UtilityFunctions.randomDirection(), turnProbability));
	}
	
	// Update is called once per frame
	void Update () {
        animation_statemachine.Update();
        control_statemachine.Update();
	}

    void OnTriggerEnter() {

    }

}
