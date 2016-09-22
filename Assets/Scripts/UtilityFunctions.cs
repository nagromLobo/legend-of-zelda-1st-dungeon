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

    public static Vector3 fixToGrid(Vector3 position, Direction direction, Direction prevDir) {
        // if the direction has changed, have to snap him to the grid
        if (prevDir != direction) {
            if ((prevDir == Direction.NORTH || prevDir == Direction.SOUTH)
                && (direction == Direction.EAST || direction == Direction.WEST)) {
                // if axis is changed from vertical to horizontial
                // we have to fix his postion on the vertical axis
                float posY = position.y;
                float nonFractionY = Mathf.Floor(posY);
                float fractionY = posY - nonFractionY;
                if (fractionY < 0.25) {
                    posY = nonFractionY;
                } else if (fractionY >= 0.25 && fractionY < 0.75) {
                    posY = nonFractionY + 0.5f;
                } else {
                    posY = nonFractionY + 1.0f;
                }
                position.Set(position.x, posY, position.z);

            } else if ((prevDir == Direction.EAST || prevDir == Direction.WEST)
                && (direction == Direction.NORTH || direction == Direction.SOUTH)) {
                // If axis is changed from horizontial to 
                // we have to fix his postion on the horizontial axis
                float posX = position.x;
                float nonFractionX = Mathf.Floor(posX);
                float fractionX = posX - nonFractionX;
                if (fractionX < 0.25) {
                    posX = nonFractionX;
                } else if (fractionX >= 0.25 && fractionX < 0.75) {
                    posX = nonFractionX + 0.5f;
                } else {
                    posX = nonFractionX + 1.0f;
                }
                position.Set(posX, position.y, position.z);
            }
        }
        return position;
    }
}
