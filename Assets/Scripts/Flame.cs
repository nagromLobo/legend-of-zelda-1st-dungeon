using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {
    public Sprite[] FlameAnimation;
    private StateMachine animation_statemachine;
    public int animationFps;

	// Use this for initialization
	void Start () {
        animation_statemachine = new StateMachine();
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(this.gameObject.GetComponent<SpriteRenderer>(), FlameAnimation, animationFps));
	}

    void Update() {
        animation_statemachine.Update();
    }
}
