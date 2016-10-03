using UnityEngine;
using System.Collections;

public class Aquamentus : Enemy {

    public float attackProbability = 0.5f;
    public Sprite[] attackAnimation;
    public float attackWarningTime = 0.5f;
    public GameObject fireballPrefab;
    public float fireballDistance = 7;
    public float timeToThrowFireball = 2.0f;
    private float attackStartTime = 0.0f;


    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        Direction turnDirection;
        if(disallowCurrentDirection) {
            turnDirection = UtilityFunctions.reverseDirection(currDirection);
        } else {
            turnDirection = Direction.WEST;
        }
        control_statemachine.ChangeState(new StateAquamentusMovement(this, timeToCrossTile, turnDirection, turnProbability, attackProbability));
        OnEnemyTurned(turnDirection);
    }

    // start movement in a given direction
    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateAquamentusMovement(this, timeToCrossTile, d, turnProbability, attackProbability));
    }

    public override void StartEnemyAnimation(Direction d) {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    public override void OnEnemyAttack() {
        current_state = EntityState.ATTACKING;
        attackStartTime = Time.time;
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(GetComponent<SpriteRenderer>(), attackAnimation, movementFramesPerSecond));
        Fireball fireball = (Instantiate(fireballPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Fireball>();
        fireball.StartFireball(new Vector3(Mathf.Cos(Mathf.Deg2Rad * 135), Mathf.Sin(Mathf.Deg2Rad * 135), 0.0f), fireballDistance, timeToThrowFireball);
        fireball = (Instantiate(fireballPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Fireball>();
        fireball.StartFireball(new Vector3(Mathf.Cos(Mathf.Deg2Rad * 180), Mathf.Sin(Mathf.Deg2Rad * 180), 0.0f), fireballDistance, timeToThrowFireball);
        fireball = (Instantiate(fireballPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Fireball>();
        fireball.StartFireball(new Vector3(Mathf.Cos(Mathf.Deg2Rad * 225), Mathf.Sin(Mathf.Deg2Rad * 225), 0.0f), fireballDistance, timeToThrowFireball);
    }

    public override void OnEnemyTurned(Direction d) {
        currDirection = d;
    }

    protected override void handleAttack() {
        if((Time.time - attackStartTime) >= attackWarningTime) {
            current_state = EntityState.NORMAL;
            StartEnemyAnimation(currDirection);
            // release projectiles
        }
        
    }
}
