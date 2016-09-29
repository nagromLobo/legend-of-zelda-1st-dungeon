using UnityEngine;
using System.Collections;

public class Goriya : Enemy {
    public float throwBoomerangProbability = 0.1f;
    public float boomerangCoolDown = 3.0f;
    public Sprite[] goriya_up_prefabs;
    public Sprite[] goriya_down_prefabs;
    public Sprite[] goriya_left_prefabs;
    public Sprite[] goriya_right_prefabs;

    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        if (disallowCurrentDirection) {
            control_statemachine.ChangeState(new StateGoriyaMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability, throwBoomerangProbability, boomerangCoolDown));
        } else {
            control_statemachine.ChangeState(new StateGoriyaMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(), turnProbability, throwBoomerangProbability, boomerangCoolDown));
        }
        
    }

    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateGoriyaMovement(this, timeToCrossTile, d, turnProbability, throwBoomerangProbability, boomerangCoolDown));
    }

    public override void StartEnemyAnimation(Direction d) {
        switch (d) {
            case Direction.NORTH:
                spriteAnimation = goriya_up_prefabs;
                break;
            case Direction.EAST:
                spriteAnimation = goriya_right_prefabs;
                break;
            case Direction.SOUTH:
                spriteAnimation = goriya_down_prefabs;
                break;
            case Direction.WEST:
                spriteAnimation = goriya_left_prefabs;
                break;
        }
        animation_statemachine.ChangeState(new StateEnemyMovementAnimation(GetComponent<SpriteRenderer>(), spriteAnimation, movementFramesPerSecond));
    }

    public override void OnEnemyTurned(Direction d) {
        base.OnEnemyTurned(d);
        StartEnemyAnimation(d);
    }
}
