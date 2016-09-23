using UnityEngine;
using System.Collections;

public class Gel : Enemy {
    public float pauseProbability = 0.8f;
    public float timeToPause = 1.0f;

    public override void StartEnemyMovement(bool disallowCurrentDirection) {
        if (disallowCurrentDirection) {
            control_statemachine.ChangeState(new StateGelMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(currDirection), turnProbability, pauseProbability, timeToPause));
        } else {
            control_statemachine.ChangeState(new StateGelMovement(this, timeToCrossTile, UtilityFunctions.randomDirection(), turnProbability, pauseProbability, timeToPause));
        }  
    }

    public override void StartEnemyMovement(Direction d) {
        control_statemachine.ChangeState(new StateGelMovement(this, timeToCrossTile, currDirection, turnProbability, pauseProbability, timeToPause));
    }

    public override void CameraMoved(Direction d, float transitionTime) {
        base.CameraMoved(d, transitionTime);
    }
}
