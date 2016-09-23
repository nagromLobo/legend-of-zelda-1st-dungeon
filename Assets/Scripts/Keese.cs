using UnityEngine;
using System.Collections;

public class Keese : Enemy {
    public float pauseProbability = 0.2f;
    public float pauseSlowdownTime = 0.2f;
    public float maxFlyRadius = 4.0f;
    public float timeToPause = 1.0f;

    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius));
    }

    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius));
    }

    public override void CameraMoved(Direction d, float transitionTime) {
       
    }

    void OnCollisionEnter(Collision other) {
        StartEnemyMovement(true);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
