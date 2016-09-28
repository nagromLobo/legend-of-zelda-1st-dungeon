using UnityEngine;
using System.Collections;

public class RoomSizedTrigger : MonoBehaviour {
    public enum TriggerType { VERTICAL, HORIZONTIAL}
    public TriggerType type;
    public int horizontialSize = 15;
    public int verticalSize = 15;
    BoxCollider bc;

    public delegate void onTriggered(Collider other);
    public onTriggered OnTriggered;

	// Use this for initialization
	void Start () {
        this.bc = this.GetComponent<BoxCollider>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetTriggerType(TriggerType type) {
        this.type = type;
        switch (type) {
            case TriggerType.VERTICAL:
                transform.localScale = new Vector3(transform.localScale.x * verticalSize, transform.localScale.y, transform.localScale.z);
                break;
            case TriggerType.HORIZONTIAL:
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * horizontialSize, transform.localScale.z);
                break;
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.tag == "Link") {
            OnTriggered(other);
        }
    }
}
