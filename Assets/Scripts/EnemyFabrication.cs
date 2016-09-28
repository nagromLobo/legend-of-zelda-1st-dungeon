using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyFabrication : MonoBehaviour {
    public Vector3[] roomCameraPositions = new Vector3[14]; // an array containing the set camera positions for rooms
    public int[] numEnemiesInRooms = new int[14]; // an array of the number of enemies in a given room
    public GameObject[] enemy_prefabs; // refers to what enemy to show in thier respective room
    public List<Vector3>[] spawnGrid;
    public float timeBetweenWallMasterSpawn = 1.0f;
    public float timeBetweenWallMasterSpawnDoor = 0.5f;
    private List<GameObject> enemy_instances = new List<GameObject>();// instances in a give room
    private int currentRoom = 0;
    private static int WALL_MASTER_ROOM;
    private float timeLastWallMasterSpawn = 0.0f;


    // Use this for initialization
    void Start() {
        spawnGrid = new List<Vector3>[15];

        for(int i = 0; i < spawnGrid.Length; ++i) {
            spawnGrid[i] = new List<Vector3>();
        }
        // for testing blade traps
        spawnGrid[1] = new List<Vector3> { new Vector3(19.0f, 7.0f, 0.0f),
                                            new Vector3(18.0f, 5.0f, 0.0f),
                                            new Vector3(26.0f, 2.0f, 0.0f)
        };
        spawnGrid[2] = new List<Vector3> { new Vector3(52.0f, 7.0f, 0.0f),
                                            new Vector3(54.0f, 3.0f, 0.0f),
                                            new Vector3(57.0f, 5.0f, 0.0f),
                                            new Vector3(61.0f, 5.0f, 0.0f),
                                            new Vector3(61.0f, 2.0f, 0.0f)
        };
        spawnGrid[3] = new List<Vector3> { new Vector3(41.0f, 19.0f, 0.0f),
                                            new Vector3(43.0f, 17.0f, 0.0f),
                                            new Vector3(36.0f, 17.0f, 0.0f)
        };
        spawnGrid[4] = new List<Vector3> { new Vector3(34.0f, 29.0f, 0.0f),
                                            new Vector3(35.0f, 28.0f, 0.0f),
                                            new Vector3(35.0f, 27.0f, 0.0f),
                                            new Vector3(37.0f, 29.0f, 0.0f),
                                            new Vector3(37.0f, 27.0f, 0.0f)
        };
        spawnGrid[5] = new List<Vector3> { new Vector3(26.0f, 30.0f, 0.0f),
                                            new Vector3(18.5f, 29.0f, 0.0f),
                                            new Vector3(18.5f, 27.0f, 0.0f),
                                            new Vector3(20.5f, 28.0f, 0.0f),
                                            new Vector3(21.0f, 26.5f, 0.0f),
                                            new Vector3(19.0f, 24.5f, 0.0f)
        };
        spawnGrid[6] = new List<Vector3> { new Vector3(53.0f, 30.0f, 0.0f),
                                            new Vector3(53.0f, 24.0f, 0.0f),
                                            new Vector3(56.0f, 28.0f, 0.0f),
                                            new Vector3(56.0f, 26.0f, 0.0f),
                                            new Vector3(58.0f, 26.0f, 0.0f),
                                            new Vector3(58.0f, 28.0f, 0.0f),
                                            new Vector3(60.0f, 27.0f, 0.0f),
                                            new Vector3(60.0f, 23.0f, 0.0f)
        };
        spawnGrid[7] = new List<Vector3> { new Vector3(24.0f, 36.0f, 0.0f),
                                            new Vector3(25.0f, 40.0f, 0.0f),
                                            new Vector3(23.0f, 40.0f, 0.0f)
        };
        spawnGrid[8] = new List<Vector3> { new Vector3(35.0f, 41.0f, 0.0f),
                                            new Vector3(45.0f, 41.0f, 0.0f),
                                            new Vector3(45.0f, 35.0f, 0.0f),
                                            new Vector3(42.0f, 36.0f, 0.0f)
        };
        spawnGrid[9] = new List<Vector3> { new Vector3(35.0f, 51.0f, 0.0f),
                                            new Vector3(42.0f, 50.0f, 0.0f),
                                            new Vector3(44.0f, 49.0f, 0.0f)
        };
        spawnGrid[10] = new List<Vector3> { new Vector3(41.0f, 63.0f, 0.0f),
                                            new Vector3(40.0f, 61.0f, 0.0f),
                                            new Vector3(38.0f, 62.0f, 0.0f)
        };
        spawnGrid[11] = new List<Vector3> { new Vector3(29.0f, 63.0f, 0.0f),
                                            new Vector3(18.0f, 63.0f, 0.0f),
                                            new Vector3(29.0f, 57.0f, 0.0f),
                                            new Vector3(18.0f, 57.0f, 0.0f)
        };
        spawnGrid[12] = new List<Vector3> { new Vector3(51.0f, 37.0f, 0.0f),
                                            new Vector3(52.0f, 40.0f, 0.0f),
                                            new Vector3(53.0f, 40.0f, 0.0f)
        };
        // wall masters are a special case?
        spawnGrid[13] = new List<Vector3>();
        spawnGrid[14] = new List<Vector3> { new Vector3(75.0f, 49.0f, 0.0f)
        };
        CameraControl.S.cameraMoveCompleteDelegate += CameraMoveComplete;
        CameraControl.S.cameraMovedDelegate += OnCameraMoved;
    }

    // Update is called once per frame
    void Update() {
        // in the wall master room we want to fabricate wallmasters on a timer
        // speed it up when the player is close to the boss doorS
        if(currentRoom == WALL_MASTER_ROOM) {
            if((Time.time - timeLastWallMasterSpawn) > timeBetweenWallMasterSpawn){
                // then spawn wallmaster
            }
            
        }

    }

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
        if(currentRoom == WALL_MASTER_ROOM) {
            timeLastWallMasterSpawn = Time.time;
        }
    }

    private void OnEnemyDestroyed(GameObject enemy) {
        // reduce the amount of enemies in the current room
        --numEnemiesInRooms[currentRoom];
        switch (currentRoom) {
            // handle room specific enemy killing events
            case 1:
                // first keese room
                if(numEnemiesInRooms[currentRoom] == 0) {
                    // FIXME --> make key appear
                }
                break;
            case 5:
                // (3rd) trap keese room
                if(numEnemiesInRooms[currentRoom] == 0) {
                    // FIXME --> Unlock door
                    // (keese room)
                }
                break;
            case 14:
                // Aquamentus
                if(numEnemiesInRooms[currentRoom] == 0) {
                    // FIXME --> Unlock door
                }
                break;
        }
    }

    private void OnCameraMoved(Direction d, float transitionTime) {
        // destroy all of the enemy instances when offscreen
        foreach (GameObject enemy in enemy_instances) {
            Destroy(enemy);
        }
        enemy_instances.Clear();
     }
}
