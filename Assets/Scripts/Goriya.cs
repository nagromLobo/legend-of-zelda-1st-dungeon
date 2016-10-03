using UnityEngine;
using System.Collections;

public class Goriya : Enemy {
    public float throwBoomerangProbability = 0.1f;
    public float boomerangCoolDown = 3.0f;
    public GameObject boomerangPrefab;
    public Sprite[] goriya_up_prefabs;
    public Sprite[] goriya_down_prefabs;
    public Sprite[] goriya_left_prefabs;
    public Sprite[] goriya_right_prefabs;
    private Boomerang releasedBoomerang;

    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        Direction turnDirection;
        // used for wall collisions
        if (disallowCurrentDirection) {
            this.gameObject.transform.position = adjustBackToGrid(currDirection, transform.position);
            turnDirection = UtilityFunctions.randomDirection(currDirection);
        } else {
            turnDirection = UtilityFunctions.randomDirection();
        }
        OnEnemyTurned(turnDirection);
        control_statemachine.ChangeState(new StateGoriyaMovement(this, timeToCrossTile, turnDirection, turnProbability, throwBoomerangProbability, boomerangCoolDown));
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

    public override void OnEnemyAttack() {
//		releasedBoomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity) as GameObject;
//		releasedBoomerang.weapon_instance = releasedBoomerang;
//        releasedBoomerang.gameObject.tag = "BoomerangGoriya";
//        releasedBoomerang.ReleaseBoomerang();
//        releasedBoomerang.released = true;
        
    }


}
