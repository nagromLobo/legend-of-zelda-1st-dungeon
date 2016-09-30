using UnityEngine;
using System.Collections;

public class Keese : Enemy {
    public float pauseProbability = 0.2f;
    public float pauseSlowdownTime = 0.2f;
    public float maxFlyRadius = 4.0f;
    public float timeToPause = 1.0f;

    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius, Vector3.zero));
    }

    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius, Vector3.zero));
    }

    protected override void OnCollisionEnter(Collision other) {
        MonoBehaviour.print(other.gameObject.tag);
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        //Vector3 newDirection = other.contacts[0].normal;
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius, Vector3.zero));
    }

    protected override void OnTriggerEnter(Collider other) {
        OnTriggerDamageHandling(other);
        MonoBehaviour.print(other.gameObject.tag);
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        //Vector3 newDirection = other.contacts[0].normal;
        control_statemachine.ChangeState(new StateKeeseMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause, pauseSlowdownTime, maxFlyRadius, Vector3.zero));
    }
}
