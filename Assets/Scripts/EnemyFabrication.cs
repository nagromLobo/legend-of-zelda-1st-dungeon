using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyFabrication : MonoBehaviour {

	public GameObject enemy_prefab; //in reality this is a list

	GameObject enemy_instance; // in reality this is a larger container for all enemies in that camera position

	Vector3 previousCameraPosition = Vector3.zero;

	//for first room only. wil figure this out later

	private List<Vector3>[] spawnGrid;
	//then make something to see which rooms spawn which enemies

	// Use this for initialization
	void Start () {
		spawnGrid = new List<Vector3> [15];

		for (int i = 0; i < spawnGrid.Length; ++i) {
				if (i == 1)
					spawnGrid [1] = new List<Vector3> { new Vector3 (52.0f, 7.0f, 0.0f) };
				else
					spawnGrid [i] = new List<Vector3> ();
		}
		CameraControl.S.cameraMoveCompleteDelegate += CameraMoveComplete;

	
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}

	void CameraMoveComplete(Vector3 pos) {
		if (previousCameraPosition != pos) {
			Destroy(enemy_instance);
		}
		if (Math.Floor (pos.x) == 55) {
			previousCameraPosition = pos;
			for (int i = 0; i < spawnGrid.Length; ++i) {
				foreach(Vector3 vec in spawnGrid[i])  {
					enemy_instance  = Instantiate(enemy_prefab, vec, transform.rotation) as GameObject;
				}
			}
		}
	}
}
