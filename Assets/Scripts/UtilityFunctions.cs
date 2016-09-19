using UnityEngine;
using System.Collections;

public class UtilityFunctions : MonoBehaviour {
   public static float FromUnitsToGamePixels(float unityUnits) {
        return unityUnits * 16;
    }

    public static float FromGamePixelsToUnits(float gamePixels) {
        return gamePixels / 16;
    }
	
}
