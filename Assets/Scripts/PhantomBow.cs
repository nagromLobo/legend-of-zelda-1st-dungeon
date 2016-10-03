using UnityEngine;
using System.Collections;

public class PhantomBow : Enemy {

    public virtual void StartEnemyMovement(bool disallowCurrentDirection) {
        Direction turnDirection;
        // used for wall collisions
        if (disallowCurrentDirection) {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            turnDirection = UtilityFunctions.randomDirection(currDirection);
        } else {
            turnDirection = UtilityFunctions.randomDirection();
        }
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, turnDirection, turnProbability));
        OnEnemyTurned(turnDirection);
    }

    // start movement in a given direction
    public virtual void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateEnemyMovement(this, timeToCrossTile, d, turnProbability));
    }

    public virtual void StartEnemyAnimation(Direction d) {
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    public virtual void OnEnemyTurned(Direction d) {
        currDirection = d;
    }

    public virtual void OnEnemyAttack() {}
}
