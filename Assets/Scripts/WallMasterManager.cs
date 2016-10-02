using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallMasterManager : MonoBehaviour {
    public float roomHeight = 0.0f;
    public float roomWidth = 0.0f;
    public GameObject triggerPrefab;
    public GameObject wallmasterPrefab;
    public float timeBetweenWallmasters;
    public float wallMasterWallOffset = 0.0f;
    public float wallMasterLinkOffset = 0.0f;

    private WallMasterTrigger[] triggers = new WallMasterTrigger[4];
    private float[] triggerStartTimes;
    private float startTime;
    private List<Wallmaster> instances = new List<Wallmaster>();
	// Use this for initialization
	void Start () {
        triggerStartTimes = new float[triggers.Length];
        float halfHeight = roomHeight / 2;
        float halfWidth = roomWidth / 2;
        Vector3 triggerPosition;
        // initalized triggers
        // North trigger
        triggerPosition = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);
        triggers[0] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<WallMasterTrigger>();
        triggers[0].SetTriggerType(RoomSizedTrigger.TriggerType.HORIZONTIAL);
        triggers[0].SetDirection(Direction.NORTH);
        // East trigger
        triggerPosition = new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z);
        triggers[1] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<WallMasterTrigger>();
        triggers[1].SetTriggerType(RoomSizedTrigger.TriggerType.VERTICAL);
        triggers[1].SetDirection(Direction.EAST);
        // South trigger
        triggerPosition = new Vector3(transform.position.x, transform.position.y - halfHeight, transform.position.z);
        triggers[2] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<WallMasterTrigger>();
        triggers[2].SetTriggerType(RoomSizedTrigger.TriggerType.HORIZONTIAL);
        triggers[2].SetDirection(Direction.SOUTH);
        // West trigger
        triggerPosition = new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z);
        triggers[3] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<WallMasterTrigger>();
        triggers[3].SetTriggerType(RoomSizedTrigger.TriggerType.VERTICAL);
        triggers[3].SetDirection(Direction.WEST);

        for (int i = 0; i < triggers.Length; ++i) {
            triggers[i].OnTriggeredWithDirection += OnTriggered;
            triggerStartTimes[i] = Time.time;
        }
    }

    void OnTriggered(Direction dir, Collider other) {
        Vector3 linkPosition = other.transform.position;
        Vector3 startPosition = transform.position;
        Direction wmDir = UtilityFunctions.reverseDirection(dir);
        Direction wmTurnDir = Direction.NORTH;
        float lengthTravel1 = wallMasterWallOffset;
        float lengthTravel2 = wallMasterLinkOffset;
        

        if ((Time.time - startTime) >= timeBetweenWallmasters) {
            startTime = Time.time;
            // then generate a new wallmaster
            switch (dir) {
                case Direction.NORTH:
                    // then we have to move south first
                    // equal chance of comming from right of door or left
                    startPosition.y += (roomHeight / 2);
                    if (Random.value > 0.5) {
                        wmTurnDir = Direction.WEST;
                        startPosition.Set(linkPosition.x + wallMasterLinkOffset, startPosition.y + wallMasterWallOffset, startPosition.z);
                    } else {
                        wmTurnDir = Direction.EAST;
                        startPosition.Set(linkPosition.x - wallMasterLinkOffset, startPosition.y + wallMasterWallOffset, startPosition.z);
                    }
                    
                        break;
                case Direction.EAST:
                    startPosition.x += (roomWidth / 2);
                    if (Random.value > 0.5) {
                        wmTurnDir = Direction.NORTH;
                        startPosition.Set(startPosition.x + wallMasterWallOffset, linkPosition.y - wallMasterLinkOffset, startPosition.z);
                    } else {
                        wmTurnDir = Direction.SOUTH;
                        startPosition.Set(startPosition.x + wallMasterWallOffset, linkPosition.y + wallMasterLinkOffset, startPosition.z);
                    }
                    break;
                case Direction.SOUTH:
                    startPosition.y -= (roomHeight / 2);
                    if (Random.value > 0.5) {
                        wmTurnDir = Direction.WEST;
                        startPosition.Set(linkPosition.x + wallMasterLinkOffset, startPosition.y - wallMasterWallOffset, startPosition.z);
                    } else {
                        wmTurnDir = Direction.EAST;
                        startPosition.Set(linkPosition.x - wallMasterLinkOffset, startPosition.y - wallMasterWallOffset, startPosition.z);
                    }
                    break;
                case Direction.WEST:
                    startPosition.x -= (roomWidth / 2);
                    if (Random.value > 0.5) {
                        wmTurnDir = Direction.NORTH;
                        startPosition.Set(startPosition.x - wallMasterWallOffset, linkPosition.y - wallMasterLinkOffset, startPosition.z);
                    } else {
                        wmTurnDir = Direction.SOUTH;
                        startPosition.Set(startPosition.x - wallMasterWallOffset, linkPosition.y + wallMasterLinkOffset, startPosition.z);
                    }
                    break;
            }
            Wallmaster wm = (Instantiate(wallmasterPrefab, startPosition, Quaternion.identity) as GameObject).GetComponent<Wallmaster>();
            instances.Add(wm);
            wm.setUpPositions(wmDir, wmTurnDir, startPosition, wallMasterWallOffset, wallMasterLinkOffset);
        }
    }

    void OnDestroy() {
        for(int i = 0; i < triggers.Length; ++i) {
            Destroy(triggers[i].gameObject);
        }
        foreach(var instance in instances) {
            if(instance != null) {
                Destroy(instance.gameObject);
            }
        }
    }
}
