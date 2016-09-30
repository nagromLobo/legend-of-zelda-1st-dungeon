//using UnityEngine;
//using System.Collections;
//
//public class GameManager : MonoBehaviour {
//
//	public UIManager UI;
//
//	// Use this for initialization
//	/*void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}*/
//
//	public void TogglePauseMenu()
//	{
//		// not the optimal way but for the sake of readability
//		if (UI.GetComponentsInChildren<Canvas>().enabled)
//		{
//			UI.GetComponentsInChildren<Canvas>().enabled = false;
//			Time.timeScale = 1.0f;
//		}
//		else
//		{
//			UI.GetComponentsInChildren<Canvas>().enabled = true;
//			Time.timeScale = 0f;
//		}
//
//		Debug.Log("GAMEMANAGER:: TimeScale: " + Time.timeScale);
//	}
//}
