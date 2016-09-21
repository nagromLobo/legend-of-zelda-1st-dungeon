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

    // takes in a direction to not be counted for in the random decision
    public static Direction randomDirection(Direction d) {
        int random = Random.Range(0, 2);
        if(d == Direction.NORTH) {
            switch (random) {
                case 0:
                    return Direction.EAST;
                case 1:
                    return Direction.SOUTH;
                case 2:
                    return Direction.WEST;
            }
        } else if(d == Direction.EAST) {
            switch (random) {
                case 0:
                    return Direction.SOUTH;
                case 1:
                    return Direction.WEST;
                case 2:
                    return Direction.NORTH;
            }
        } else if(d == Direction.SOUTH) {
            switch (random) {
                case 0:
                    return Direction.WEST;
                case 1:
                    return Direction.NORTH;
                case 2:
                    return Direction.EAST;
            }
        } else {
            switch (random) {
                case 0:
                    return Direction.NORTH;
                case 1:
                    return Direction.EAST;
                case 2:
                    return Direction.SOUTH;
            }
        }
        // Default --> Shouldn't be reached
        return Direction.NORTH;
    }
}
