using UnityEngine;
using System.Collections;

public class UtilityFunctions : MonoBehaviour {
   public static float FromUnitsToGamePixels(float unityUnits) {
        return unityUnits * 16;
    }

    public static float FromGamePixelsToUnits(float gamePixels) {
        return gamePixels / 16;
    }

    public static Direction randomDirection() {
        // Find a random direction
        int random = Random.Range(0, 3);
        switch (random) {
            case 0:
                return Direction.NORTH;
            case 1:
                return Direction.EAST;
            case 2:
                return Direction.SOUTH;
            case 3:
                return Direction.WEST;
            default:
                // this line should never run
                return Direction.NORTH;
        }
    }
}
