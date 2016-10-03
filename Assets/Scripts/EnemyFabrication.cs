using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class EnemyFabrication : MonoBehaviour {
    public bool customLevel = false;
    public Vector3[] roomCameraPositions = new Vector3[14]; // an array containing the set camera positions for rooms
    public int[] numEnemiesInRooms = new int[14]; // an array of the number of enemies in a given room
    public GameObject[] enemy_prefabs; // refers to what enemy to show in thier respective room
    public List<Vector3>[] spawnGrid;
    public float timeBetweenWallMasterSpawn = 1.0f;
    public float timeBetweenWallMasterSpawnDoor = 0.5f;
    public float doorWaitDuration = 2.0f;

    public Vector3[] pushableTileCoords; // order of increasing room numbers
    public GameObject pushableTilePrefab;
    public GameObject slideableBlocksPrefab;
    public GameObject smallKeyPrefab;
    public GameObject boomerangPrefab;
    public GameObject room15TextEngine;
    public GameObject flamePrefab;
    public GameObject NPCprefab;
    public Vector3[] eventCoords; // index is roomnumber

    // room based audio events
    public AudioClip puzzleSolvedAudio;
    public AudioClip doorClosedAudio;
    public AudioClip keyAppearedAudio;
    public AudioClip dungeonMusic;
    private AudioSource roomEventAudioSrc;
    private AudioSource dungeonMusicSrc;


    private PushableBlock[] pushableBlocks;


    private List<GameObject> enemy_instances = new List<GameObject>();// instances in a give room
    private int currentRoom = 0;
    private int prevRoom = 0;
    private int numBlackTilesTrigered = 0;
    private SlideableBlocks[] lockedBlocks;


    // Use this for initialization
    void Start() {
        // set up audio
        AudioSource[] audioSources = GetComponents<AudioSource>();
        dungeonMusicSrc = audioSources[0];
        roomEventAudioSrc = audioSources[1];
        dungeonMusicSrc.clip = dungeonMusic;
        dungeonMusicSrc.playOnAwake = true;
        dungeonMusicSrc.Play();

        spawnGrid = new List<Vector3>[enemy_prefabs.Length];
        PlayerControl.instance.playerInRoom += playerInRoom;
        for(int i = 0; i < spawnGrid.Length; ++i) {
            spawnGrid[i] = new List<Vector3>();
        }
        // for testing wall masters
        //spawnGrid[0] = new List<Vector3> { new Vector3(39.5f, 6.0f, 0.0f)
        //};
        if (!customLevel) {
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
            spawnGrid[15] = new List<Vector3> { new Vector3(4.0f, 38.0f, 0.0f),
                                            new Vector3(10.0f, 38.0f, 0.0f)
            };
        } else {
            spawnGrid[1] = new List<Vector3> { new Vector3(38.0f, 15.0f, 0.0f),
                                            new Vector3(42.0f, 15.0f, 0.0f),
                                            new Vector3(42.0f, 17.0f, 0.0f),
                                            new Vector3(38.0f, 17.0f, 0.0f)
            };
            spawnGrid[2] = new List<Vector3> { new Vector3(39.0f, 24.0f, 0.0f),
                                            new Vector3(44.0f, 25.0f, 0.0f),
                                            new Vector3(44.0f, 30.0f, 0.0f),
                                            new Vector3(38.0f, 30.0f, 0.0f),
                                            new Vector3(36.0f, 29.0f, 0.0f)
            };
            spawnGrid[3] = new List<Vector3> { new Vector3(27.0f, 27.0f, 0.0f),
                                            new Vector3(28.0f, 30.0f, 0.0f),
                                            new Vector3(20.0f, 30.0f, 0.0f),
                                            new Vector3(20.0f, 27.0f, 0.0f),
                                            new Vector3(20.0f, 24.0f, 0.0f)
            };
            spawnGrid[4] = new List<Vector3> { new Vector3(13.0f, 30.0f, 0.0f),
                                            new Vector3(8.0f, 25.0f, 0.0f),
                                            new Vector3(4.0f, 28.0f, 0.0f)
            };
            spawnGrid[5] = new List<Vector3> { new Vector3(51.0f, 25.0f, 0.0f),
                                            new Vector3(52.0f, 25.0f, 0.0f),
                                            new Vector3(54.0f, 29.0f, 0.0f),
                                            new Vector3(57.0f, 29.0f, 0.0f),
                                            new Vector3(60.0f, 25.0f, 0.0f)
            };
            spawnGrid[6] = new List<Vector3> { new Vector3(69.0f, 28.0f, 0.0f),
                                            new Vector3(74.0f, 28.0f, 0.0f),
                                            new Vector3(71.0f, 24.0f, 0.0f)
            };
            spawnGrid[7] = new List<Vector3> { new Vector3(4.0f, 35.0f, 0.0f),
                                            new Vector3(11.0f, 35.0f, 0.0f)
            };
            spawnGrid[8] = new List<Vector3> { new Vector3(4.0f, 46.0f, 0.0f)
            };
            spawnGrid[9] = new List<Vector3> { new Vector3(19.0f, 50.0f, 0.0f),
                                            new Vector3(23.0f, 51.0f, 0.0f),
                                            new Vector3(22.0f, 47.0f, 0.0f),
                                            new Vector3(28.0f, 46.0f, 0.0f)
            };
            spawnGrid[10] = new List<Vector3> { new Vector3(41.0f, 63.0f, 0.0f),
                                            new Vector3(40.0f, 61.0f, 0.0f),
                                            new Vector3(38.0f, 62.0f, 0.0f)
            };
            spawnGrid[11] = new List<Vector3> { new Vector3(76.0f, 51.0f, 0.0f),
                                            new Vector3(68.0f, 47.0f, 0.0f)
            };
            spawnGrid[12] = new List<Vector3> { new Vector3(51.0f, 51.0f, 0.0f),
                                            new Vector3(51.0f, 46.0f, 0.0f),
                                            new Vector3(54.0f, 48.0f, 0.0f),
                                            new Vector3(60.0f, 51.0f, 0.0f),
                                            new Vector3(59.0f, 46.0f, 0.0f)
            };
            spawnGrid[13] = new List<Vector3> { new Vector3(39.5f, 49.0f, 0.0f) };
            spawnGrid[14] = new List<Vector3> { new Vector3(42.0f, 37.0f, 0.0f),
                                                new Vector3(36.0f, 38.0f, 0.0f)
            };
        }
        
        CameraControl.S.cameraMoveCompleteDelegate += CameraMoveComplete;
        CameraControl.S.cameraMovedDelegate += OnStartCameraMovement;

        // pushable blocks
        pushableBlocks = new PushableBlock[pushableTileCoords.Length];
        for (int i = 0; i < pushableTileCoords.Length; ++i) {
            Vector3 coords = pushableTileCoords[i];
            pushableBlocks[i] = (Instantiate(pushableTilePrefab, coords, transform.rotation) as GameObject).GetComponent<PushableBlock>();
            pushableBlocks[i].SetUpPushableTile(true, true, true, true, coords, 0, true);
            pushableBlocks[i].onBlockPushed += OnBlockPushed;
            pushableBlocks[i].onBlackTileTrigered += OnBlackTileTrigered;
        }
        // need to defeat all enemies in room to push tile
        pushableBlocks[0].SetUpPushableTile(true, true, true, true, pushableTileCoords[0], 7, false);
        pushableBlocks[1].SetUpPushableTile(true, true, true, true, pushableTileCoords[1], 0, false);


        // set up locked blocks in rooms
        lockedBlocks = new SlideableBlocks[numEnemiesInRooms.Length];
        // room 1
        lockedBlocks[1] = (Instantiate(slideableBlocksPrefab, eventCoords[1], Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        lockedBlocks[1].InitBlocks(eventCoords[1]);
        lockedBlocks[1].RestBlocksToOpen();
        // room 7
        lockedBlocks[7] = (Instantiate(slideableBlocksPrefab, eventCoords[7], Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        lockedBlocks[7].InitBlocks(eventCoords[7]);
        lockedBlocks[7].RestBlocksToOpen();
        // room 13
        lockedBlocks[13] = (Instantiate(slideableBlocksPrefab, eventCoords[13], Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        lockedBlocks[13].InitBlocks(eventCoords[13]);
        lockedBlocks[13].RestBlocksToOpen();
        // room 14
        lockedBlocks[14] = (Instantiate(slideableBlocksPrefab, eventCoords[14], Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        lockedBlocks[14].InitBlocks(eventCoords[14]);
        lockedBlocks[14].RestBlocksToOpen();
        // room 10
        lockedBlocks[10] = (Instantiate(slideableBlocksPrefab, eventCoords[10], Quaternion.identity) as GameObject).GetComponent<SlideableBlocks>();
        lockedBlocks[10].InitBlocks(eventCoords[10]);
        lockedBlocks[10].RestBlocksToOpen();
    }

    void Update() {
        if (Input.GetKeyDown("f5")) {
            SceneManager.LoadScene("Dungeon");
        } else if (Input.GetKeyDown("f6")) {
            SceneManager.LoadScene("Custom");
        }
    }

    void OnBlockPushed(PushableBlock pushedBlock) {
        // in the is case we want to trigger a room event (like unlocking a door)
        if (!customLevel) {
            switch (currentRoom) {
                // Gel locked door room (with three gels)
                case 7:
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.WEST);
                    PuzzleSolved();
                    break;
                case 11:
                    // blade trap room
                    PuzzleSolved();
                    break;
            }
        }
    }

    void OnBlackTileTrigered(GameObject pushedBlock) {
        if (customLevel) {
            numBlackTilesTrigered++;
        }
        switch (currentRoom) {
            case 1:
                // then we are in the water/gel room
                if(numBlackTilesTrigered == 2) {
                    // unlock slideable blocks
                    lockedBlocks[currentRoom].MoveBlocks();
                    PuzzleSolved();
                }
                break;
            case 7:
                lockedBlocks[currentRoom].MoveBlocks();
                PuzzleSolved();
                break;
            case 8:
                ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.EAST);
                PuzzleSolved();
                break;
            case 10:
                // then we are in the test block puzzle room
                if(numBlackTilesTrigered == 2) {
                    lockedBlocks[currentRoom].MoveBlocks();
                    PuzzleSolved();
                }
                break;
            case 11:
                // diamond puzzle room
                if(numBlackTilesTrigered == 8) {
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.WEST);
                    PuzzleSolved();
                }
                break;
            case 13:
                // wallmaster room
                PuzzleSolved();
                lockedBlocks[currentRoom].MoveBlocks();
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
        if (!customLevel) {
            if (currentRoom != prevRoom) {
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
                        if (currentRoom != 15) {
                            ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                        }
                        break;
                    case 11:
                        // blade trap pushable block room
                        pushableBlocks[1].SetUpPushableTile(true, true, true, false, pushableTileCoords[1], 11, true);
                        break;
                }
                switch (currentRoom) {
                    case 15:
                        // then we have to instantiate the oldman
                        enemy_instances.Add((Instantiate(NPCprefab, eventCoords[15], transform.rotation)) as GameObject);
                        break;
                }
            }
        } else {
            if(currentRoom != prevRoom) {
                numBlackTilesTrigered = 0;
            }
            switch (prevRoom) {
                case 1:
                    // then in room with gels, rest locked door
                    lockedBlocks[prevRoom].RestBlocksToOpen();
                    bool pushable = false;
                    if (numEnemiesInRooms[prevRoom] == 0) {
                        pushable = true;
                    }
                    pushableBlocks[0].SetUpPushableTile(true, true, true, true, pushableTileCoords[0], 1, pushable);
                    pushableBlocks[1].SetUpPushableTile(true, true, true, true, pushableTileCoords[1], 1, pushable);
                    break;
                case 7:
                    lockedBlocks[prevRoom].RestBlocksToOpen();
                    break;
                case 8:
                    pushableBlocks[12].SetUpPushableTile(true, true, true, true, pushableTileCoords[12], 8, true);
                    pushableBlocks[13].SetUpPushableTile(true, true, true, true, pushableTileCoords[13], 8, true);
                    break;
                case 10:
                    lockedBlocks[prevRoom].RestBlocksToOpen();
                    pushableBlocks[2].SetUpPushableTile(true, true, true, true, pushableTileCoords[2], 10, true);
                    pushableBlocks[3].SetUpPushableTile(true, true, true, true, pushableTileCoords[3], 10, true);
                    break;
                case 11:
                    for(int i = 0; i <= 11; ++i) {
                        pushableBlocks[i].SetUpPushableTile(true, true, true, true, pushableTileCoords[i], 11, true);
                    }
                    break;
                case 13:
                    pushableBlocks[14].SetUpPushableTile(true, true, true, true, pushableTileCoords[14], 8, true);
                    lockedBlocks[prevRoom].RestBlocksToOpen();
                    break;
                case 14:
                    break;
            }
        }
    }

    private void playerInRoom() {
        List<Vector3> currSpawnGrid = spawnGrid[currentRoom];
        GameObject currEnemy = enemy_prefabs[currentRoom];
        print(currentRoom);
        // if there are stil enemies in the room to spawn, spawn them
        for (int i = 0; (i < currSpawnGrid.Count) && (i < numEnemiesInRooms[currentRoom]); ++i) {
            enemy_instances.Add(Instantiate(currEnemy, currSpawnGrid[i], transform.rotation) as GameObject);
            if (enemy_instances[i].GetComponent<Enemy>() != null) {
                enemy_instances[i].GetComponent<Enemy>().OnEnemyDestroyed += OnEnemyDestroyed;
                if(enemy_instances[i].GetComponent<BladeTrap>() != null) {
                    enemy_instances[i].GetComponent<BladeTrap>().onBlackTileTriger += OnBlackTileTrigered;
                }
            }
        }
        if (!customLevel) {
            // special case for second keese room
            switch (currentRoom) {
                case 5:
                    if (numEnemiesInRooms[currentRoom] > 0) {
                        ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                        DoorStateChanged();
                    }
                    break;
                case 7:
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                    break;
                case 8:
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                    break;
                case 15:
                    // then we entered the room with the text engine. have to start it
                    room15TextEngine.GetComponent<TextEngine>().AnimateText();
                    break;
            }
        } else {
            // then we are in the custom level
            switch (currentRoom) {
                case 1:
                    // gel and water room
                    lockedBlocks[currentRoom].CloseBlocks();
                    DoorStateChanged();
                    break;
                case 2:
                    // lockedBlocks[currentRoom].CloseBlocks();
                   // DoorStateChanged();
                    break;
                case 7:
                    lockedBlocks[currentRoom].CloseBlocks();
                    DoorStateChanged();
                    break;
                case 8:
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                    DoorStateChanged();
                    break;
                case 10:
                    lockedBlocks[currentRoom].CloseBlocks();
                    DoorStateChanged();
                    break;
                case 13:
                    lockedBlocks[currentRoom].CloseBlocks();
                    DoorStateChanged();
                    break;
                case 11:
                    ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].closeEventDoor();
                    DoorStateChanged();
                    break;
                case 14:
                    lockedBlocks[currentRoom].CloseBlocks();
                    DoorStateChanged();
                    break;
            }
        }
    }

    private void OnEnemyDestroyed(GameObject enemy) {
        // reduce the amount of enemies in the current room
        --numEnemiesInRooms[currentRoom];
        if (!customLevel) {
            // then we are in the 1st dungeon
            if (numEnemiesInRooms[currentRoom] == 0) {
                switch (currentRoom) {
                    // handle room specific enemy killing events
                    case 1:
                        // first keese room
                        KeyAppeared();
                        break;
                    case 4:
                        // Stalfo room (first branching room)                   
                        KeyAppeared();
                        break;
                    case 5:
                        // (3rd) trap keese room
                        //  Unlock door
                        // (keese room)
                        ShowMapOnCamera.MAP_TILES[Mathf.RoundToInt(eventCoords[currentRoom].x), Mathf.RoundToInt(eventCoords[currentRoom].y)].openEventDoor(Direction.EAST);
                        DoorStateChanged();
                        break;
                    case 7:
                        // (1st) pushable block room
                        // make block pushable
                        pushableBlocks[0].pushable = true;
                        break;
                    case 10:
                        // Goryia water-room
                        KeyAppeared();
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
                        DoorStateChanged();
                        break;
                }
            }
        } else {
            // then we are in the custom level
            if(numEnemiesInRooms[currentRoom] == 0) {
                switch (currentRoom) {
                    case 1:
                        // then we are in the first gel room with pushable blocks
                        pushableBlocks[0].pushable = true;
                        pushableBlocks[1].pushable = true;
                        break;
                    case 2:
                        // then we are in triforce room
                        Instantiate(smallKeyPrefab, eventCoords[currentRoom], Quaternion.identity);
                        KeyAppeared();
                        break;
                    case 4:
                        // then we are in the left bow room
                        Instantiate(smallKeyPrefab, eventCoords[currentRoom], Quaternion.identity);
                        KeyAppeared();
                        break;
                    case 6:
                        // then we are in the right bow room
                        Instantiate(smallKeyPrefab, eventCoords[currentRoom], Quaternion.identity);
                        KeyAppeared();
                        break;
                    case 12:
                        // then we are in the right goriya room
                        Instantiate(smallKeyPrefab, eventCoords[currentRoom], Quaternion.identity);
                        KeyAppeared();
                        break;
                }
            }
        }
       
    }

    private void PuzzleSolved() {
        roomEventAudioSrc.clip = puzzleSolvedAudio;
        roomEventAudioSrc.Play();
    }

    private void KeyAppeared() {
        roomEventAudioSrc.clip = keyAppearedAudio;
        roomEventAudioSrc.Play();
        Instantiate(smallKeyPrefab, eventCoords[currentRoom], transform.rotation);
    }

    private void DoorStateChanged() {
        roomEventAudioSrc.clip = doorClosedAudio;
        roomEventAudioSrc.Play();
    }

    private void OnStartCameraMovement(Direction d, float transitionTime) {
        // destroy all of the enemy instances when offscreen
        foreach (GameObject enemy in enemy_instances) {
            Destroy(enemy);
        }
        enemy_instances.Clear();
        prevRoom = currentRoom;
        if (!customLevel) {
            switch (prevRoom) {
                case 15:
                    room15TextEngine.GetComponent<TextEngine>().ClearText();
                    // then we have to clear the text from the room
                    break;
            }
        }
     }
}
