using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    public float speed = 0.0f;
    public Sprite[] animation;
    private StateMachine animation_statemachine;
    private StateMachine control_statemachine;

    void Awake() {
        animation_statemachine = new StateMachine();
        control_statemachine = new StateMachine();
    }
	// Use this for initialization
	void Start () {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this, GetComponent<SpriteRenderer>(), animation, 6));


	
	}
	
	// Update is called once per frame
	void Update () {
        animation_statemachine.Update();
	
	}
}
