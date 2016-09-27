using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyFabrication : MonoBehaviour {
    public Vector3[] roomCameraPositions = new Vector3[15]; // an array containing the set camera positions for rooms
    public int[] numEnemiesInRooms = new int[15]; // an array of the number of enemies in a given room
    public GameObject[] enemy_prefabs; // refers to what enemy to show in thier respective room
    public List<Vector3>[] spawnGrid;
    private List<GameObject> enemy_instances = new List<GameObject>();// instances in a give room
    private int currentRoom = 0;


    // Use this for initialization
    void Start() {
        spawnGrid = new List<Vector3>[15];

        for(int i = 0; i < spawnGrid.Length; ++i) {
            spawnGrid[i] = new List<Vector3>();
        }
        spawnGrid[2] = new List<Vector3> { new Vector3(52.0f, 7.0f, 0.0f),
                                            new Vector3(54.0f, 3.0f, 0.0f),
                                            new Vector3(57.0f, 5.0f, 0.0f),
                                            new Vector3(61.0f, 5.0f, 0.0f),
                                            new Vector3(61.0f, 2.0f, 0.0f)
        };
        CameraControl.S.cameraMoveCompleteDelegate += CameraMoveComplete;
        CameraControl.S.cameraMovedDelegate += OnCameraMoved;
    }

    // Update is called once per frame
    //	void Update () {
    //	
    //	}

    void CameraMoveComplete(Vector3 pos) {
        // figure out what room we are in
        for(int i = 0; i < roomCameraPositions.Length; ++i) {
            if((Mathf.FloorToInt(pos.x) == Mathf.FloorToInt(roomCameraPositions[i].x)) &&
                (Mathf.FloorToInt(pos.y) == Mathf.FloorToInt(roomCameraPositions[i].y))) {
                currentRoom = i;
                break;
            }
        }
        List<Vector3> currSpawnGrid = spawnGrid[currentRoom];
        GameObject currEnemy = enemy_prefabs[currentRoom];
        // if there are stil enemies in the room to spawn, spawn them
        for(int i = 0; (i < currSpawnGrid.Count) && (i < numEnemiesInRooms[currentRoom]); ++i) {
            enemy_instances.Add(Instantiate(currEnemy, currSpawnGrid[i], transform.rotation) as GameObject);
            enemy_instances[i].GetComponent<Enemy>().OnEnemyDestroyed += OnEnemyDestroyed;
        }
    }

    private void OnEnemyDestroyed(GameObject enemy) {
        // reduce the amount of enemies in the current room
        --numEnemiesInRooms[currentRoom];
    }

    private void OnCameraMoved(Direction d, float transitionTime) {
        // destroy all of the enemy instances when offscreen
        foreach (GameObject enemy in enemy_instances) {
            Destroy(enemy);
        }
        enemy_instances.Clear();
     }
}
