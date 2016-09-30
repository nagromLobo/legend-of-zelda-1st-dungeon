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
    public float doorWaitDuration = 2.0f;

    public Vector3[] pushableTileCoords; // order of increasing room numbers
    public GameObject pushableTilePrefab;
    public GameObject smallKeyPrefab;
    public GameObject boomerangPrefab;
    public Vector3[] eventCoords; // index is roomnumber

    private PushableBlock[] pushableBlocks;


    private List<GameObject> enemy_instances = new List<GameObject>();// instances in a give room
    private int currentRoom = 0;
    private int prevRoom = 0;


    // Use this for initialization
    void Start() {
        spawnGrid = new List<Vector3>[enemy_prefabs.Length];
        PlayerControl.instance.playerInRoom += playerInRoom;
        for(int i = 0; i < spawnGrid.Length; ++i) {
            spawnGrid[i] = new List<Vector3>();
        }
        // for testing wall masters
        spawnGrid[0] = new List<Vector3> { new Vector3(39.5f, 6.0f, 0.0f)
        };
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
        spawnGrid[13] = new List<Vector3> { new Vector3(71.5f, 38.0f, 0.0f) };
        spawnGrid[14] = new List<Vector3> { new Vector3(75.0f, 49.0f, 0.0f)
        };
        CameraControl.S.cameraMoveCompleteDelegate += CameraMoveComplete;
        CameraControl.S.cameraMovedDelegate += OnCameraMoved;

        // pushable blocks
        pushableBlocks = new PushableBlock[pushableTileCoords.Length];
        for (int i = 0; i < pushableTileCoords.Length; ++i) {
            Vector3 coords = pushableTileCoords[i];
            pushableBlocks[i] = (Instantiate(pushableTilePrefab, coords, transform.rotation) as GameObject).GetComponent<PushableBlock>();
            pushableBlocks[i].SetUpPushableTile(true, true, true, true, coords, 0, true);
            pushableBlocks[i].onBlockPushed += OnBlockPushed;
        }
        // need to defeat all enemies in room to push tile
        pushableBlocks[0].SetUpPushableTile(true, true, true, true, pushableTileCoords[0], 7, false);
        // pushable in every direction besides from the west
        pushableBlocks[1].SetUpPushableTile(true, true, true, false, pushableTileCoords[1], 0, true);
    }

    void OnBlockPushed(PushableBlock pushedBlock) {
        // in the is case we want to trigger a room event (like unlocking a door)
        switch (currentRoom) {
            // Gel locked door room (with three gels)
            case 7:
                ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.WEST);
                break;
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
        if(currentRoom != prevRoom) {
            // special case for pushable block rooms
            switch (prevRoom) {
                case 7:
                    // gel pushable block room
                    bool pushable = false;
                    if (numEnemiesInRooms[prevRoom] == 0) {
                        pushable = true;
                    }
                    pushableBlocks[0].SetUpPushableTile(true, true, true, true, pushableTileCoords[0], 7, pushable);
                    // reset door
                    if(currentRoom != 15) {
                        ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                    }
                    break;
                case 11:
                    // blade trap pushable block room
                    pushableBlocks[1].SetUpPushableTile(true, true, true, false, pushableTileCoords[1], 11, true);
                    break;

            }
        }
    }

    private void playerInRoom() {
        // special case for second keese room
        switch (currentRoom) {
            case 5:
                if(numEnemiesInRooms[currentRoom] > 0) {
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                }
                break;
            case 7:
                ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                break;
        }
    }

    private void OnEnemyDestroyed(GameObject enemy) {
        // reduce the amount of enemies in the current room
        --numEnemiesInRooms[currentRoom];
        if (numEnemiesInRooms[currentRoom] == 0) {
            switch (currentRoom) {
                // handle room specific enemy killing events
                case 1:
                    // first keese room

                    // make key appear
                    Instantiate(smallKeyPrefab, eventCoords[currentRoom], transform.rotation);

                    break;
                case 4:
                    // Stalfo room (first branching room)                   
                        Instantiate(smallKeyPrefab, eventCoords[currentRoom], transform.rotation);
                        break;
                case 5:
                    // (3rd) trap keese room
                    //  Unlock door
                    // (keese room)
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.EAST);
                    break;
                case 7:
                    // (1st) pushable block room
                    // make block pushable
                    pushableBlocks[0].pushable = true;
                    break;
                case 10:
                    // Goryia water-room
                    Instantiate(smallKeyPrefab, eventCoords[currentRoom], transform.rotation);
                    break;
                case 12:
                    // (Right before wallmasters) goryia room
                    // drop boomerang
                    Instantiate(boomerangPrefab, eventCoords[currentRoom], transform.rotation);
                    break;
                case 14:
                    // Aquamentus
                    // Unlock door
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.EAST);
                    break;
            }
        }
    }

    private void OnCameraMoved(Direction d, float transitionTime) {
        // destroy all of the enemy instances when offscreen
        foreach (GameObject enemy in enemy_instances) {
            Destroy(enemy);
        }
        enemy_instances.Clear();
        prevRoom = currentRoom;
     }
}
