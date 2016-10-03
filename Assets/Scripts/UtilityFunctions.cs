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

    public static Direction DirectionFromNormal(Vector3 normal) {
        Vector3 north = new Vector3(0, 1, 0);
        Vector3 east = new Vector3(1, 0, 0);
        Vector3 south = new Vector3(0, -1, 0);
        if(normal == north) {
            return Direction.NORTH;
        }
        if(normal == east) {
            return Direction.EAST;
        }
        if(normal == south) {
            return Direction.SOUTH;
        }
        return Direction.WEST;
    }

    public static Direction reverseDirection(Direction d) {
        switch (d) {
            case Direction.NORTH:
                return Direction.SOUTH;
            case Direction.EAST:
                return Direction.WEST;
            case Direction.SOUTH:
                return Direction.NORTH;
            default:
                return Direction.EAST;
        }
    }
    Vector3 SnapTo(Vector3 v3, float snapAngle) {
        float angle = Vector3.Angle(v3, Vector3.up);
        if (angle < 45.0f)      
            return Vector3.up * v3.magnitude; 
        if (angle > 180.0f - 45.05)
            return Vector3.down * v3.magnitude;

        float t = Mathf.Round(angle / snapAngle);
        float deltaAngle = (t * snapAngle) - angle;

        Vector3 axis = Vector3.Cross(Vector3.up, v3);
        Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);
        return q * v3;
    }

    public static Vector3 roundToNearestAxis(Vector3 vec) {
        float posXAngle = Vector3.Angle(vec, Vector3.right);
        float negXAngle = Vector3.Angle(vec, Vector3.left);
        float posYAngle = Vector3.Angle(vec, Vector3.up);
        // then we are close to the pos x axis
        if (posXAngle <= 45.0f) {
            return Vector3.right;
        } else if(posYAngle <= 45.0f) {
            return Vector3.up;
        } else if(negXAngle <= 45.0f) {
            return Vector3.left;
        } else {
            return Vector3.down;
        }
    }
}
