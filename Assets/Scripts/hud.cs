using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Hud : MonoBehaviour {


	public GameObject heartPrefab;
	public static List<GameObject> heartImages = new List<GameObject>();
    public Text rupee_text;

	private static Hud instance;

	void Awake () {
		instance = this;
	}

	public static void RefreshDisplay() {
		int diff = 2*PlayerControl.instance.half_heart_count - heartImages.Count;
		int absVal = Mathf.Abs(diff);

		int num_player_rupees = PlayerControl.instance.rupee_count;
		instance.rupee_text.text = "Rupees: " + num_player_rupees.ToString();

		for(int i = 0; i < absVal; i++)
		{
			// Add hearts.
			if(diff > 0)
			{
				GameObject newHeart = Instantiate(instance.heartPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				newHeart.transform.SetParent(instance.gameObject.transform);
				newHeart.GetComponent<RectTransform>().anchoredPosition = new Vector3(heartImages.Count * 30, 0, 0);
				heartImages.Add(newHeart);
			}

			// Remove hearts.
			else if (diff < 0)
			{
				GameObject heartToRemove = heartImages[heartImages.Count-1];
				heartImages.Remove(heartToRemove);
				Destroy(heartToRemove);
			}
		}
			
	}
}
