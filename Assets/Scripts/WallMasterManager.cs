using UnityEngine;
using System.Collections;

public class WallMasterManager : MonoBehaviour {
    public float roomHeight = 0.0f;
    public float roomWidth = 0.0f;
    public GameObject triggerPrefab;

    private RoomSizedTrigger[] triggers = new RoomSizedTrigger[4];
	// Use this for initialization
	void Start () {
        float halfHeight = roomHeight / 2;
        float halfWidth = roomWidth / 2;
        Vector3 triggerPosition;
        // initalized triggers
        // North trigger
        triggerPosition = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);
        triggers[0] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[0].SetTriggerType(RoomSizedTrigger.TriggerType.VERTICAL);
        // East trigger
        triggerPosition = new Vector3(transform.position.x + halfWidth, transform.position.y, transform.position.z);
        triggers[1] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[1].SetTriggerType(RoomSizedTrigger.TriggerType.HORIZONTIAL);
        // West trigger
        triggerPosition = new Vector3(transform.position.x, transform.position.y - halfHeight, transform.position.z);
        triggers[2] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[2].SetTriggerType(RoomSizedTrigger.TriggerType.VERTICAL);
        // South trigger
        triggerPosition = new Vector3(transform.position.x - halfWidth, transform.position.y, transform.position.z);
        triggers[3] = (Instantiate(triggerPrefab, triggerPosition, Quaternion.identity) as GameObject).GetComponent<RoomSizedTrigger>();
        triggers[3].SetTriggerType(RoomSizedTrigger.TriggerType.HORIZONTIAL);

        for(int i = 0; i < triggers.Length; ++i) {
            triggers[i].OnTriggered += OnTriggered;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggered(Collider other) {

    }

    void OnDestroy() {
        for(int i = 0; i < triggers.Length; ++i) {
            Destroy(triggers[i].gameObject);
        }
    }
}
